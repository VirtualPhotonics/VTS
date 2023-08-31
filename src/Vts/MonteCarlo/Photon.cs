using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Handles all data and methods necessary to photon biography generation.
    /// </summary>
    public class Photon
    {
        // reducing any of the following values might result in unit tests not passing
        // should we dynamically set MAX_HISTORY_PTS and MAX_PHOTON_TIME?  derive one from other?
        private const int MaxHistoryPts = 300000; // 300000 * [1/(5/mm)] = 60000 mm
        private const double Chance = 0.1;
        private const double MaxPhotonTime = 280; // ns = 60000 mm (path length) / (300 / 1.4)
    
        // could add layer of indirection to not expose Absorb
        private readonly ITissue _tissue;
        private readonly Random _rng;
        private bool _firstTimeEnteringDomain;
        private readonly double _russianRouletteWeightThreshold;
        /// <summary>
        /// Class that keeps a photon's data as it moves through the tissue
        /// </summary>
        /// <param name="p">Position</param>
        /// <param name="d">Direction</param>
        /// <param name="weight">Initial weight</param>
        /// <param name="tissue">Tissue></param>
        /// <param name="currentTissueRegionIndex">integer index within ITissue definition indicating photon's current position</param>
        /// <param name="generator">Random Number Generator</param>
        public Photon(
            Position p,
            Direction d,
            double weight,
            ITissue tissue,
            int currentTissueRegionIndex,
            Random generator)
        {
            DP = new PhotonDataPoint(
                    p,
                    d,
                    weight, // weight
                    0.0, // total time
                    PhotonStateType.Alive);

            History = new PhotonHistory(tissue.Regions.Count);
            History.AddDPToHistory(DP);  // add initial data point
            S = 0.0;
            SLeft = 0.0;        
            CurrentRegionIndex = currentTissueRegionIndex;
            // flag to determine whether passing through specular or not
            // the following assumes tissues considered are slabs, only ones we have coded to date
            _firstTimeEnteringDomain = true;
            if (CurrentRegionIndex >= 1) // photon does not go through specular
            {
                _firstTimeEnteringDomain = false;
            }
            CurrentTrackIndex = 0;
            _tissue = tissue;
            SetAbsorbAction(_tissue.AbsorptionWeightingType);
            SetScatterAction(_tissue.PhaseFunctionType);
            _rng = generator;
            _russianRouletteWeightThreshold = _tissue.RussianRouletteWeightThreshold;
        }
        /// <summary>
        /// default constructor for Photon
        /// </summary>
        public Photon()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0,
                new MultiLayerTissue(),
                0, 
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(RandomNumberGeneratorType.MersenneTwister)
                ) { }

        /// <summary>
        /// photon data point has position, direction etc. info
        /// </summary>
        public PhotonDataPoint DP { get; set; }        
        /// <summary>
        /// PhotonHistory has list of PhotonDataPoints and SubRegionCollisionInfo
        /// </summary>
        public PhotonHistory History { get; set; }
        /// <summary>
        /// path length of current track, gets updated when passing tissue boundary
        /// </summary>
        public double S { get; set; }
        /// <summary>
        /// path length left after crossing tissue boundary
        /// </summary>
        public double SLeft { get; set; }
        /// <summary>
        /// tissue region index where photon current is.  Set is public because MCPP
        /// needs to set in order to determine which surface normal to use for
        /// WithinNA method
        /// </summary>
        public int CurrentRegionIndex { get; set; }
        /// <summary>
        /// index of current track of photon
        /// </summary>
        public int CurrentTrackIndex { get; private set; }
        /// <summary>
        /// absorb action: analog, discrete, continuous
        /// </summary>
        public Action Absorb { get; private set; }
        /// <summary>
        /// scatter action: Henyey-Greenstein, bidirectional, Mie
        /// </summary>
        public Action Scatter { get; private set; }

        private void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            Absorb = awt switch
            {
                AbsorptionWeightingType.Analog => AbsorbAnalog,
                AbsorptionWeightingType.Continuous => AbsorbContinuous,
                AbsorptionWeightingType.Discrete => AbsorbDiscrete,
                _ => AbsorbDiscrete
            };
        }
        private void SetScatterAction(PhaseFunctionType st)
        {
            Scatter = st switch
            {
                PhaseFunctionType.HenyeyGreenstein => ScatterHenyeyGreenstein,
                PhaseFunctionType.Bidirectional => Scatter1D,
                _ => Scatter
            };
        }
        /// <summary>
        /// method that determines photon's step size based on the RegionScatterLength (1/mus or 1/mut depending on CAW or DAW)
        /// </summary>
        public void SetStepSize()
        {
            if (SLeft == 0.0)
            {
                S = -Math.Log(_rng.NextDouble()) / _tissue.RegionScatterLengths[CurrentRegionIndex];
            }
            else
            {
                S = SLeft / _tissue.RegionScatterLengths[CurrentRegionIndex];
                SLeft = 0.0;
            }
        }
        /// <summary>
        /// method to move the photon to its next location
        /// </summary>
        /// <param name="distance">distance to move photon</param>
        /// <returns>Boolean indicating whether photon will hit boundary or not</returns>
         public bool Move(double distance)
         {
            var willHitBoundary = S >= distance;

            if (willHitBoundary)
            {
                AdjustTrackLength(distance);
            }

            DP.Position.X += S * DP.Direction.Ux;
            DP.Position.Y += S * DP.Direction.Uy;
            DP.Position.Z += S * DP.Direction.Uz;

            DP.TotalTime += DetectorBinning.GetTimeDelay(S, _tissue.Regions[CurrentRegionIndex].RegionOP.N);

            CurrentTrackIndex++;

            History.SubRegionInfoList[CurrentRegionIndex].PathLength += S;

            // only increment number of collisions counter if NOT pseudo-collision
            if (!willHitBoundary)
            {
                History.SubRegionInfoList[CurrentRegionIndex].NumberOfCollisions++;
            }

            History.AddDPToHistory(DP);

            return willHitBoundary;
        }

        private void AdjustTrackLength(double distanceToBoundary)
        {
            // if crossing boundary, modify photon track-length, S, by pro-rating
            // the remaining distance by the optical properties in the next region
            // in preparation for this convert SLeft to non-dimensional units
            SLeft = (S - distanceToBoundary) * _tissue.RegionScatterLengths[CurrentRegionIndex];

            // reassign S to be distance that takes track to boundary
            S = distanceToBoundary;
        }
        /// <summary>
        /// Method that determines whether photon reflects or refracts across interface.  When this 
        /// method is called photon is sitting on boundary of region and CurrentRegionIndex is Index
        /// of region photon had been in.
        /// </summary>
        public void CrossRegionOrReflect()
        {
            var cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(this);
            var nCurrent = _tissue.Regions[CurrentRegionIndex].RegionOP.N;
            var neighborIndex = _tissue.GetNeighborRegionIndex(this);
            var nNext = _tissue.Regions[neighborIndex].RegionOP.N;

            var cosCriticalAngle = nCurrent > nNext 
                ? Math.Sqrt(1.0 - nNext / nCurrent * (nNext / nCurrent)) 
                : 0.0;

            double cosThetaSnell;
            // call Fresnel be default to have uZSnell set, used to be within else
            var probOfReflecting = Optics.Fresnel(nCurrent, nNext, cosTheta, out cosThetaSnell);
            if (cosTheta <= cosCriticalAngle)
            {
                probOfReflecting = 1.0;
            }

            /* Decide whether or not photon goes to next region */
            // perform first check so that rng not called on pseudo-collisions
            if (probOfReflecting == 0.0 || _rng.NextDouble() > probOfReflecting) // transmitted
            {
                // if at border of system  
                if (_tissue.OnDomainBoundary(DP.Position) && !_firstTimeEnteringDomain)
                {
                    DP.StateFlag = DP.StateFlag.Add(_tissue.GetPhotonDataPointStateOnExit(DP.Position));
                }

                // adjust CAW weight for portion of track to pseudo collision before CurrentRegionIndex updated
                if (Absorb == AbsorbContinuous) AbsorbContinuous();
                
                CurrentRegionIndex = neighborIndex;
                DP.Direction = _tissue.GetRefractedDirection(DP.Position, DP.Direction,
                    nCurrent, nNext, cosThetaSnell);

                if (!_firstTimeEnteringDomain) return;
                _firstTimeEnteringDomain = false;
            }
            else  // don't cross, reflect
            {
                DP.Direction = _tissue.GetReflectedDirection(DP.Position, DP.Direction);
                // check if specular reflection
                if (_firstTimeEnteringDomain)
                {
                    DP.StateFlag = DP.StateFlag.Add(PhotonStateType.PseudoSpecularTissueBoundary);
                }

                // adjust CAW weight for portion of track to pseudo collision
                if (Absorb == AbsorbContinuous)  AbsorbContinuous();
            }
        }

        /// <summary>
        /// Method to scatter according to Henyey-Greenstein scatter function
        /// </summary>
        public void ScatterHenyeyGreenstein()
        {
            // readability eased with local copies of following
            var ux = DP.Direction.Ux;
            var uy = DP.Direction.Uy;
            var uz = DP.Direction.Uz;

            var g = _tissue.Regions[CurrentRegionIndex].RegionOP.G;
            double cost, sint;    // cosine and sine of theta 
            double cosp, sinp;    // cosine and sine of phi 

            if (g == 0.0)
                cost = 2 * _rng.NextDouble() - 1;
            else
            {
                var temp = (1 - g * g) / (1 - g + 2 * g * _rng.NextDouble());
                cost = (1 + g * g - temp * temp) / (2 * g);
                if (cost < -1) cost = -1;
                else if (cost > 1) cost = 1;
            }
            sint = Math.Sqrt(1.0 - cost * cost);

            var phi = 2.0 * Math.PI * _rng.NextDouble();
            cosp = Math.Cos(phi);
            sinp = Math.Sin(phi);

            if (Math.Abs(DP.Direction.Uz) > 1 - 1e-10)
            {   /* normal incident. */
                DP.Direction.Ux = sint * cosp;
                DP.Direction.Uy = sint * sinp;
                DP.Direction.Uz = cost * DP.Direction.Uz / Math.Abs(DP.Direction.Uz);
            }
            else
            {
                var temp = Math.Sqrt(1.0 - uz * uz);
                DP.Direction.Ux = sint * (ux * uz * cosp - uy * sinp) / temp + ux * cost;
                DP.Direction.Uy = sint * (uy * uz * cosp + ux * sinp) / temp + uy * cost;
                DP.Direction.Uz = -sint * cosp * temp + uz * cost;
            }

        }
        /// <summary>
        /// Method to scatter bidirectionally
        /// </summary>
        public void Scatter1D()
        {
            var g = _tissue.Regions[CurrentRegionIndex].RegionOP.G;

            if (_rng.NextDouble() < (1 + g) / 2.0)
                DP.Direction.Uz *= 1.0;
            else
                DP.Direction.Uz *= -1.0;
        }
        /// <summary>
        /// Method to check for absorption according to analog random walk process
        /// </summary>
        public void AbsorbAnalog()
        {
            if (!(_rng.NextDouble() > _tissue.Regions[CurrentRegionIndex].RegionOP.Mus /
                    (_tissue.Regions[CurrentRegionIndex].RegionOP.Mus +
                     _tissue.Regions[CurrentRegionIndex].RegionOP.Mua))) return;
            
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.Absorbed);
            DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
            History.AddDPToHistory(DP);
        }
        /// <summary>
        /// Method to de-weight for absorption according to discrete absorption weighting (DAW)
        /// random walk process
        /// </summary>
        public void AbsorbDiscrete()
        {
            var mua = _tissue.Regions[CurrentRegionIndex].RegionOP.Mua;
            var mus = _tissue.Regions[CurrentRegionIndex].RegionOP.Mus;

            if (SLeft != 0.0) return; // only de-weight if at real collision
            var dw = DP.Weight * mua / (mua + mus);
            DP.Weight -= dw;
            // fluence tallying used to be done here 

            // update weight for current DP in History 
            History.HistoryData[History.HistoryData.Count - 1].Weight = DP.Weight;
        }
        /// <summary>
        /// Method to de-weight for absorption according to continuous absorption weighting (CAW)
        /// random walk process
        /// </summary>
        public void AbsorbContinuous()
        {
            // the following de-weights at pseudo (sleft>0) and real collisions (sleft=0) as it should
            // rather than use total path length in each layer to determine weight,
            // this method updates weight at pseudo collision and can be used for total absorption tallies
            // note: added call to AbsorbContinuous in CrossOrReflect to accomplish this
            var dw = DP.Weight * (1 - Math.Exp(-_tissue.Regions[CurrentRegionIndex].RegionOP.Mua * S));          
            DP.Weight -= dw;

            // update weight for current DP in History 
            History.HistoryData[History.HistoryData.Count() - 1].Weight = DP.Weight;
        }
        /// <summary>
        /// Method to test for death of the photon
        /// </summary>
        public void TestDeath()
        {
            TestWeightAndDistance();         
            // if VB crossing flagged
            if (!DP.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                !DP.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                !DP.StateFlag.HasFlag(PhotonStateType.PseudoSpecularReflectanceVirtualBoundary) &&
                !DP.StateFlag.HasFlag(PhotonStateType.PseudoLateralBoundingVirtualBoundary)) return;
            
            DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
            History.AddDPToHistory(DP);
        }
        /// <summary>
        /// Method that kills photon due to Russian Roulette, maximum path length, etc.
        /// </summary>
        public void TestWeightAndDistance()
        {
            // kill by RR if weight < user-input threshold (=0.0 then no RR)
            if (DP.Weight < _russianRouletteWeightThreshold)
            {
                if (DP.Weight == 0.0)
                {
                    DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
                    DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                }
                else
                {
                    if (_rng.NextDouble() < Chance)
                    {
                        DP.Weight /= Chance;
                    }
                    else
                    {
                        DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
                        DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                    }
                }

                if (DP.StateFlag.HasFlag(PhotonStateType.KilledRussianRoulette))
                {
                    History.AddDPToHistory(DP);
                }
            }
            else
            {
                // kill photon if it has had too many collisions
                if (History.HistoryData.Count >= MaxHistoryPts)
                {
                    DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledOverMaximumCollisions);
                    DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                    History.AddDPToHistory(DP);
                }
                // kill photon if it has gone too far 
                if (DP.TotalTime < MaxPhotonTime) return;
                DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledOverMaximumPathLength);
                DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                History.AddDPToHistory(DP);
            }
        }
    }
}
