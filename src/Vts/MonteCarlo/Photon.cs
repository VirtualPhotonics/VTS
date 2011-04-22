using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Factories;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Handles all data and methods necessary to photon biography generation.
    /// </summary>
    public class Photon
    {
        // reducing any of the following values might result in unit tests not passing
        private const int MAX_HISTORY_PTS = 300000; // moved this from MCSimulation
        private const double CHANCE = 0.1;
        private const double MAX_PHOTON_PATHLENGTH = 2000; // mm

        // could add layer of indirection to not expose Absorb;
        private ITissue _tissue;
        private Random _rng;
        private bool _inTissue;

        public Photon(
            Position p,
            Direction d,
            ITissue tissue,
            PhotonStateType pst,
            Random generator)
        {
            DP = new PhotonDataPoint(
                    p,
                    d,
                    1.0,
                    0.0,
                    pst);
            DP.StateFlag = DP.StateFlag.Add(PhotonStateType.Alive);
            History = new PhotonHistory(tissue.Regions.Count);
            S = 0.0;
            SLeft = 0.0;
            // case of photon on boundary of tissue and considered already in tissue
            if (DP.StateFlag.Has(PhotonStateType.OnBoundary)) // if state has OnBoundary on
            {
                // check if Photon currently not on boundary of tissue
                if (!tissue.OnDomainBoundary(this))
                {
                    throw new ArgumentException(
                        "PhotonStateType set to OnBoundary but Position indicates otherwise");
                }
                else
                {
                    // this assumes if on boundary and pointed into a region, 
                    // then index is of entering tissue region.  Have to make
                    // sure GetRegionIndex abides by this law
                    CurrentRegionIndex = tissue.GetRegionIndex(DP.Position);
                    DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.OnBoundary);
                    _inTissue = true;
                }
            }
            else // case of photon sitting on tissue surface but not inside tissue yet
            {
                // check if Photon currently on boundary
                if (tissue.OnDomainBoundary(this))
                {
                    // get index of regions "behind" photon
                    DP.Direction = new Direction(-DP.Direction.Ux, -DP.Direction.Uy, -DP.Direction.Uz);
                    CurrentRegionIndex = tissue.GetNeighborRegionIndex(this);
                    // reset back
                    DP.Direction = new Direction(-DP.Direction.Ux, -DP.Direction.Uy, -DP.Direction.Uz); 
                    // add this temporarily for now to help debug
                    DP.Weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);
                    _inTissue = false;
                }
                else // photon not on any boundary
                {
                    CurrentRegionIndex = tissue.GetRegionIndex(DP.Position);
                    // need to set _inTissue here but not sure how to tell
                }
            }
            CurrentTrackIndex = 0;
            _tissue = tissue;
            SetAbsorbAction(_tissue.AbsorptionWeightingType);
            SetScatterAction(_tissue.PhaseFunctionType);
            _rng = generator;
        }

        public Photon()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                new MultiLayerTissue(),
                PhotonStateType.Alive,
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(RandomNumberGeneratorType.MersenneTwister)
                ) { }

        public PhotonDataPoint DP { get; set; }
        // PhotonHistory has SubRegionCollisionInfo
        public PhotonHistory History { get; set; }
        public double S { get; set; }
        public double SLeft { get; set; }

        public int CurrentRegionIndex { get; private set; }
        public int CurrentTrackIndex { get; private set; }
        public Action Absorb { get; private set; }
        public Action Scatter { get; private set; }

        private void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                case AbsorptionWeightingType.Analog:
                    Absorb = AbsorbAnalog;
                    break;
                case AbsorptionWeightingType.Continuous:
                    Absorb = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    Absorb = AbsorbDiscrete;
                    break;
            }
        }
        private void SetScatterAction(PhaseFunctionType st)
        {
            switch (st)
            {
                case PhaseFunctionType.HenyeyGreenstein:
                    Scatter = ScatterHenyeyGreenstein;
                    break;
                case PhaseFunctionType.Bidirectional:
                    Scatter = Scatter1D;
                    break;
            }
        }

        public void SetStepSize(Random rng)
        {
            if (SLeft == 0.0)
            {
                S = -Math.Log(rng.NextDouble()) / _tissue.RegionScatterLengths[CurrentRegionIndex];
            }
            else
            {
                S = SLeft / _tissue.RegionScatterLengths[CurrentRegionIndex];
                SLeft = 0.0;
            }
        }

        public bool Move(double distance)
        {
            bool willHitBoundary = WillHitBoundary(distance);

            if (willHitBoundary)
            {
                AdjustTrackLength(distance);
            }

            if (History.HistoryData.Count() == 0) // add initial data point
            {
                History.AddDPToHistory(DP);
            }

            DP.Position.X += S * DP.Direction.Ux;
            DP.Position.Y += S * DP.Direction.Uy;
            DP.Position.Z += S * DP.Direction.Uz;

            DP.TotalTime += DetectorBinning.GetTimeDelay(S, _tissue.Regions[CurrentRegionIndex].RegionOP.N);

            CurrentTrackIndex++;

            History.SubRegionInfoList[CurrentRegionIndex].PathLength += S;

            // need to add: only increment number of collision counter if NOT pseudo-collision
            // i.e. collision at boundary but need to change while check in main
            // MC loop
            if (!willHitBoundary)
            {
                History.SubRegionInfoList[CurrentRegionIndex].NumberOfCollisions++;
            }

            History.AddDPToHistory(DP);

            //DP.StateFlag = PhotonStateType.NotSet; // reset state back to not set

            return willHitBoundary;
        }

        private bool WillHitBoundary(double distanceToBoundary)
        {
            return S >= distanceToBoundary;
        }

        private void AdjustTrackLength(double distanceToBoundary)
        {
            // if crossing boundary, modify photon track-length, S, by pro-rating
            // the remaining distance by the optical properties in the next region
            SLeft = (S - distanceToBoundary) * _tissue.RegionScatterLengths[CurrentRegionIndex];

            // reassign S to be distance that takes track to boundary
            S = distanceToBoundary;
        }

        public void CrossRegionOrReflect()
        {
            double cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(this);
            double nCurrent = _tissue.Regions[CurrentRegionIndex].RegionOP.N;
            int neighborIndex = _tissue.GetNeighborRegionIndex(this);
            double nNext = _tissue.Regions[neighborIndex].RegionOP.N;

            double coscrit;
            if (nCurrent > nNext)
                coscrit = Math.Sqrt(1.0 - (nNext / nCurrent) * (nNext / nCurrent));
            else
                coscrit = 0.0;

            double probOfCrossing;
            double cosThetaSnell;
            // call Fresnel be default to have uZSnell set, used to be within else
            probOfCrossing = Optics.Fresnel(nCurrent, nNext, cosTheta, out cosThetaSnell);
            if (cosTheta <= coscrit)
                probOfCrossing = 1.0;
            //else
            //    probOfCrossing = Optics.Fresnel(nCurrent, nNext, cosTheta, out cosThetaSnell);

            /* Decide whether or not photon goes to next region */
            if (_rng.NextDouble() > probOfCrossing)
            {
                // if at border of system but not first time entering, then exit
                if (_tissue.OnDomainBoundary(this) && _inTissue) 
                //if (_tissue.OnDomainBoundary(this))
                {
                    DP.StateFlag = DP.StateFlag.Add(_tissue.GetPhotonDataPointStateOnExit(DP.Position));
                    DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                    // add updated final DP to History
                    History.AddDPToHistory(DP);
                    // adjust CAW weight for portion of track prior to exit
                    if (Absorb == AbsorbContinuous)
                    {
                        AbsorbContinuous();
                    }
                    //don't need to update these unless photon not dead upon exiting tissue
                    //DP.Direction.Ux *= nCurrent / nNext;
                    //DP.Direction.Uy *= nCurrent / nNext;
                    //DP.Direction.Uz = uZSnell;
                }
                else // not on domain boundary, at internal interface, pass to next
                {
                    CurrentRegionIndex = neighborIndex;
                    DP.Direction = _tissue.GetRefractedDirection(DP.Position, DP.Direction,
                        nCurrent, nNext, cosThetaSnell);
                    DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.OnBoundary);
                    _inTissue = true;
                }
            }
            else  // don't cross, reflect
            {
                DP.Direction = _tissue.GetReflectedDirection(DP.Position, DP.Direction);
            }
        }

        /*****************************************************************/
        public void AbsorbAnalog()
        {
            if (_rng.NextDouble() > _tissue.Regions[CurrentRegionIndex].RegionOP.Mus /
                (_tissue.Regions[CurrentRegionIndex].RegionOP.Mus +
                 _tissue.Regions[CurrentRegionIndex].RegionOP.Mua))
            {
                DP.StateFlag = DP.StateFlag.Add(PhotonStateType.Absorbed);
                DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive); 
                History.AddDPToHistory(DP);
            }
        }

        /*****************************************************************/
        public void ScatterHenyeyGreenstein()
        {
            // readability eased with local copies of following
            double ux = DP.Direction.Ux;
            double uy = DP.Direction.Uy;
            double uz = DP.Direction.Uz;
            PhotonDataPoint p = DP;
            Direction dir = p.Direction;

            int currentRegionIndex = this.CurrentRegionIndex;
            double g = this._tissue.Regions[currentRegionIndex].RegionOP.G;
            double cost, sint;    /* cosine and sine of theta */
            double cosp, sinp;    /* cosine and sine of phi */
            double psi;

            if (g == 0.0)
                cost = 2 * _rng.NextDouble() - 1;
            else
            {
                double temp = (1 - g * g) / (1 - g + 2 * g * _rng.NextDouble());
                cost = (1 + g * g - temp * temp) / (2 * g);
                if (cost < -1) cost = -1;
                else if (cost > 1) cost = 1;
            }
            sint = Math.Sqrt(1.0 - cost * cost);

            psi = 2.0 * Math.PI * _rng.NextDouble();
            cosp = Math.Cos(psi);
            sinp = Math.Sin(psi);

            if (Math.Abs(dir.Uz) > (1 - 1e-10))
            {   /* normal incident. */
                dir.Ux = sint * cosp;
                dir.Uy = sint * sinp;
                dir.Uz = cost * dir.Uz / Math.Abs(dir.Uz);
            }
            else
            {
                double temp = Math.Sqrt(1.0 - uz * uz);
                dir.Ux = sint * (ux * uz * cosp - uy * sinp) / temp + ux * cost;
                dir.Uy = sint * (uy * uz * cosp + ux * sinp) / temp + uy * cost;
                dir.Uz = -sint * cosp * temp + uz * cost;
            }

            DP.Direction = dir;
        }
        /*********************************************************/
        public void Scatter1D()
        {
            int currentRegion = this.CurrentRegionIndex;
            double g = this._tissue.Regions[currentRegion].RegionOP.G;

            // comment for compile
            if (_rng.NextDouble() < ((1 + g) / 2.0))
                this.DP.Direction.Uz *= 1.0;
            else
                this.DP.Direction.Uz *= -1.0;
        }
        /*****************************************************************/
        public void AbsorbDiscrete()
        {
            double dw;
            int currentRegion = CurrentRegionIndex;
            double mua = _tissue.Regions[currentRegion].RegionOP.Mua;
            double mus = _tissue.Regions[currentRegion].RegionOP.Mus;
            double w = DP.Weight;
            double x = DP.Position.X;
            double y = DP.Position.Y;
            int index = History.HistoryData.Count() - 1;

            if (this.SLeft == 0.0)  // only deweight if at real collision
            {
                dw = w * mua / (mua + mus);
                DP.Weight -= dw;
                // fluence tallying used to be done here 

                // update weight for current DP in History 
                History.HistoryData[index].Weight = DP.Weight;
            }
        }

        /*****************************************************************/
        public void AbsorbContinuous()
        {
            double dw;
            double mua = _tissue.Regions[CurrentRegionIndex].RegionOP.Mua;
            double mus = _tissue.Regions[CurrentRegionIndex].RegionOP.Mus;
            int index = History.HistoryData.Count() - 1;
            // the following deweights at pseudo (sleft>0) and real collisions (sleft=0) as it should
            dw = DP.Weight * (1 - Math.Exp(-mua * S));
            DP.Weight -= dw;

            // update weight for current DP in History 
            History.HistoryData[index].Weight = DP.Weight;
        }

        public void TestWeightAndDistance()
        {
            //   if (photptr.w < Weight_Limit) 
            //     Roulette();  
            // kill photon if it has had too many collisions
            if (History.HistoryData.Count >= MAX_HISTORY_PTS)
            {
                DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledOverMaximumCollisions);
                DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                History.AddDPToHistory(DP);
            }
            /* kill photon if it has gone too far */
            var totalPathLength = History.SubRegionInfoList.Select((pl, c) => pl.PathLength).Sum();
            if (totalPathLength >= MAX_PHOTON_PATHLENGTH)
            {
                DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledOverMaximumPathLength);
                DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
                History.AddDPToHistory(DP);
            }
        }

        /*****************************************************************/
        void Roulette()
        {
            if (DP.Weight == 0.0)
            {
                DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
                DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
            }
            else if (_rng.NextDouble() < CHANCE)
            {
                DP.Weight = DP.Weight / CHANCE;
            }
            else
            {
                DP.StateFlag = DP.StateFlag.Add(PhotonStateType.KilledRussianRoulette);
                DP.StateFlag = DP.StateFlag.Remove(PhotonStateType.Alive);
            } 
            if (DP.StateFlag.Has(PhotonStateType.KilledRussianRoulette))
            {
                History.AddDPToHistory(DP);
            }
        }

    }
}
