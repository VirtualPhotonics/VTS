using System;
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
        private const int MAX_HISTORY_PTS = 300000; // moved this from MCSimulation
        private const double CHANCE = 0.1;
        private const double MAX_PHOTON_PATHLENGTH = 300; // mm
        private readonly bool _tallyMomentumTranfer;

        // could add layer of indirection to not expose AbsorbAction;
        public Action _AbsorbAction;  // sets correct abs. for DAW/CAW 
        private ITissue _tissue;
        private Random _rng;

        public Photon(
            Position p, 
            Direction d, 
            ITissue tissue, 
            AbsorptionWeightingType awt,
            Random generator, 
            bool tallyMomentumTransfer)
        {
            _tallyMomentumTranfer = tallyMomentumTransfer;

            DP = new PhotonDataPoint(
                    p,
                    d,
                    1.0,
                    PhotonStateType.NotSet,
                    Enumerable.Range(0, tissue.Regions.Count).Select(i =>
                        new SubRegionCollisionInfo(0.0, 0, tallyMomentumTransfer, 0D)).ToArray());

            History = new PhotonHistory();
            // can't set up initial point in constructor
            // set initial point in history
            //History.HistoryData.Add(
            //    new PhotonDataPoint(
            //        DP.Position, 
            //        DP.Direction,
            //        DP.Weight,
            //        DP.StateFlag,
            //        null)); // don't carry SubRegionCollisionInfo data in History
            S = 0.0;
            SLeft = 0.0;
            CurrentRegionIndex = tissue.GetRegionIndex(DP.Position);
            CurrentTrackIndex = 0;
            _tissue = tissue;
            SetAbsorbAction(awt);
            _rng = generator;
        }

        public Photon()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                new MultiLayerTissue(),
                AbsorptionWeightingType.Discrete,
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(RandomNumberGeneratorType.MersenneTwister),
                false) { }

        public PhotonDataPoint DP { get; set; }
        public PhotonHistory History { get; set; }
        public double S { get; set; }
        public double SLeft { get; set; }
        public int CurrentRegionIndex { get; set; }
        public int CurrentTrackIndex { get; set; }
        public Action AbsorbAction { get; set; }

        private void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                case AbsorptionWeightingType.Analog:
                    AbsorbAction = AbsorbAnalog;
                    break;
                case AbsorptionWeightingType.Continuous:
                    AbsorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    AbsorbAction = AbsorbDiscrete;
                    break;
            }
        }

        public void SetStepSize(Random rng)
        {
            if (SLeft == 0.0)
            {
                S = -Math.Log(rng.NextDouble()) / _tissue.Regions[CurrentRegionIndex].ScatterLength;
            }
            else
            {
                S = SLeft / _tissue.Regions[CurrentRegionIndex].ScatterLength;
                SLeft = 0.0;
            }
        }

        public void Move(bool hitBoundary)
        {
            if (History.HistoryData.Count() == 0) // add initial data point
            {
                History.HistoryData.Add(
                    new PhotonDataPoint(
                        new Position(DP.Position.X, DP.Position.Y, DP.Position.Z),
                        new Direction(DP.Direction.Ux, DP.Direction.Uy, DP.Direction.Uz),
                        DP.Weight,
                        DP.StateFlag,
                        null)); // don't carry SubRegionCollisionInfo data in History
            }
            DP.Position.X += S * DP.Direction.Ux;
            DP.Position.Y += S * DP.Direction.Uy;
            DP.Position.Z += S * DP.Direction.Uz;

            CurrentTrackIndex++;

            DP.SubRegionInfoList[CurrentRegionIndex].PathLength += S;

            // only increment number of collision counter if NOT pseudo-collision
            // i.e. collision at boundary
            if (!hitBoundary)
            {
                DP.SubRegionInfoList[CurrentRegionIndex].NumberOfCollisions++;
            }
            else
            {
                DP.StateFlag = PhotonStateType.PseudoCollision;
            }

            History.HistoryData.Add(
                new PhotonDataPoint(
                    new Position(DP.Position.X, DP.Position.Y, DP.Position.Z),
                    new Direction(DP.Direction.Ux, DP.Direction.Uz, DP.Direction.Uz),
                    DP.Weight,
                    DP.StateFlag,
                    null));
            DP.StateFlag = PhotonStateType.NotSet; // reset state back to not set
        }

        public void CrossRegionOrReflect()
        {
            double cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(this);
            //double uz = photptr.DP.Direction.Uz;
            double nCurrent = _tissue.Regions[CurrentRegionIndex].RegionOP.N;
            int neighborIndex = _tissue.GetNeighborRegionIndex(this);
            double nNext = _tissue.Regions[neighborIndex].RegionOP.N;

            double coscrit;
            if (nCurrent > nNext)
                coscrit = Math.Sqrt(1.0 - (nNext / nCurrent) * (nNext / nCurrent));
            else
                coscrit = 0.0;

            double probOfCrossing;
            double uZSnell;
            probOfCrossing = Optics.Fresnel(nCurrent, nNext, cosTheta, out uZSnell);
            if (cosTheta <= coscrit)
                probOfCrossing = 1.0;
            //else
            //    probOfCrossing = Optics.Fresnel(nCurrent, nNext, cosTheta, out uZSnell);

            /* Decide whether or not photon goes to next region */
            if (_rng.NextDouble() > probOfCrossing)
            {
                // check if at border of system
                if (_tissue.OnDomainBoundary(this))
                /* transmitted to next layer */
                //if (((_tissue.do_ellip_layer != 3) && (currentRegion == _tissue.num_layers)) ||
                //    ((_tissue.do_ellip_layer == 3) && (currentRegion == 1)))
                {
                    if (DP.Position.Z < 1e-10)
                        DP.StateFlag = PhotonStateType.ExitedOutTop;
                    else
                        DP.StateFlag = PhotonStateType.ExitedOutBottom;
                    DP.Direction.Ux *= nCurrent / nNext;
                    DP.Direction.Uy *= nCurrent / nNext;
                    DP.Direction.Uz = uZSnell;
                }
                else
                {
                    CurrentRegionIndex = neighborIndex;
                    DP.Direction.Ux *= nCurrent / nNext;
                    DP.Direction.Uy *= nCurrent / nNext;
                    DP.Direction.Uz = uZSnell;
                }
            }
            else  // don't cross, reflect
            {
                DP.Direction.Uz = -DP.Direction.Uz;
            }
        }
        /*****************************************************************/
        public void AbsorbAnalog()
        {
            if (_rng.NextDouble() > _tissue.Regions[CurrentRegionIndex].RegionOP.Mus /
                (_tissue.Regions[CurrentRegionIndex].RegionOP.Mus + _tissue.Regions[CurrentRegionIndex].RegionOP.Mua))
            {
                //Absorb_Discrete(rng);  // CKH don't think this call is needed
                DP.StateFlag = PhotonStateType.Absorbed;
            }
            // might need to add to History here
        }

        /*****************************************************************/
        public void Scatter()
        {
            // don't need local copy here, but readability not good if don't
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

            if (DP.SubRegionInfoList[CurrentRegionIndex].TallyMomentumTransfer)
            {
                DP.SubRegionInfoList[CurrentRegionIndex].MomentumTransfer += 1 - cost;
            }

            DP.Direction = dir;
        }
        /*********************************************************/
        //public void Scatter1D(Generator rng)
        //{
        //    int currentRegion = this.CurrentRegionIndex;
        //    double g = this._tissue.Regions[currentRegion].RegionOP.G;

        //    // comment for compile
        //    if (rng.NextDouble() < ((1 + g) / 2.0))
        //        this.DP.Direction.Uz *= 1.0;
        //    else
        //        this.DP.Direction.Uz *= -1.0;
        //}
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
                // CKH TODO: add tallies here
                /* Compute array indices from r and z */
                //iz = DetectorBinning.WhichBin(this.DP.Position.Z, Detector.Nz, Detector.Dz, 0.0);
                //ir = DetectorBinning.WhichBin(Math.Sqrt(x * x + y * y), Detector.Nr, Detector.Dr, 0.0);
                //outptr.A_layer[currentRegion] += dw;
                //outptr.A_rz[ir, iz] += dw;

                //if (MonteCarloSimulation.DO_TIME_RESOLVED_FLUENCE)
                //{
                //    double t_delay = this.Histptr.HistoryData.Select(
                //    double t_delay = this.History.HistoryData.Select(
                //        dp => dp.SubRegionInfoList.Select(sr => sr.PathLength).Sum() /
                //            (.03 / _tissue.Regions[currentRegion].RegionOP.N));  /* . ps */
                //    int it = (int)Math.Floor(t_delay / Detector.Dt); /* assumes tmin=0 */
                //    if ((it > Detector.Nt - 1) || (it < 0)) it = -1; /* if outside [tmin,tmax] */
                //    //if (it != -1) outptr.A_rzt[ir, iz, it] += dw;
                //}

                /* update weight for History */
                // ckh question: do I update current historydata or add?
                History.HistoryData[index].Weight = DP.Weight;
                //History.HistoryData.Add(
                //    new PhotonDataPoint(
                //        DP.Position,
                //        DP.Direction,
                //        DP.Weight,
                //        DP.StateFlag,
                //        null)); 
            }
        }

        /*****************************************************************/
        public void AbsorbContinuous()
        {
            double dw;
            double mua = _tissue.Regions[CurrentRegionIndex].RegionOP.Mua;
            double mus = _tissue.Regions[CurrentRegionIndex].RegionOP.Mus;
            double x = DP.Position.X;
            double y = DP.Position.Y;
            int index = History.HistoryData.Count() - 1;
            double d = DP.SubRegionInfoList[CurrentRegionIndex].PathLength; 
            // the following deweights at pseudo (sleft>0) and real collisions (sleft=0) as it should
            dw = DP.Weight * (1 - Math.Exp(-mua * d));
            DP.Weight -= dw;

            ///* Compute array indices from r and z */
            //iz = DetectorBinning.WhichBin(this.DP.Position.Z, Detector.Nz, Detector.Dz, 0.0);
            //ir = DetectorBinning.WhichBin(Math.Sqrt(x * x + y * y), Detector.Nr, Detector.Dr, 0.0);
            //outptr.A_layer[currentRegion] += dw;
            //outptr.A_rz[ir, iz] += dw;

            //TODO: DC add feature to unmanaged code
            //if (MonteCarloSimulation.DO_TIME_RESOLVED_FLUENCE)
            //{
            //    double t_delay = this.Histptr.HistoryData[currentRegion].SubRegionInfoList.Select(s => s.PathLength).Sum() /
            //    double t_delay = this.History.HistoryData[currentRegion].SubRegionInfoList.Select(s => s.PathLength).Sum() /
            //        (.03 / this._tissue.Regions[currentRegion].RegionOP.N);  /* . ps */
            //    int it = (int)Math.Floor(t_delay / Detector.Dt); /* assumes tmin=0 */
            //    if ((it > Detector.Nt - 1) || (it < 0)) it = -1; /* if outside [tmin,tmax] */
            //    if (it != -1) outptr.A_rzt[ir, iz, it] += dw;
            //}

            /* update weight for History */
            History.HistoryData[index].Weight = DP.Weight;
            //History.HistoryData.Add(
            //    new PhotonDataPoint(
            //        DP.Position,
            //        DP.Direction,
            //        DP.Weight,
            //        DP.StateFlag,
            //        null)); 
        }
        public void TestWeight()
        {
            /*   if (photptr.w < Weight_Limit) */
            /*     Roulette();  */
            if (History.HistoryData.Count >= MAX_HISTORY_PTS - 4)
            {
                DP.StateFlag = PhotonStateType.KilledOverMaximumCollisions;
            }
        }

        public bool WillHitBoundary(double distanceToBoundary)
        {
            return S >= distanceToBoundary;
        }

        public void AdjustTrackLength(double distanceToBoundary)
        {
            // if crossing boundary, modify photon track-length, S by pro-rating
            // the remaining distance by the optical properties in the next region
            SLeft = (S - distanceToBoundary) * _tissue.Regions[CurrentRegionIndex].ScatterLength;

            // reassign S to be distance that takes track to boundary
            S = distanceToBoundary;
        }

        /*****************************************************************/
        void Test_Distance()
        {
            /* kill photon if it has gone too far */
            if (History.HistoryData[CurrentTrackIndex].SubRegionInfoList.Select((pl, c) => pl.PathLength).Sum()
                >= MAX_PHOTON_PATHLENGTH)
                DP.StateFlag = PhotonStateType.KilledOverMaximumPathLength;
        }

        /*****************************************************************/
        void Roulette()
        {
            if (DP.Weight == 0.0)
                DP.StateFlag = PhotonStateType.KilledRussianRoulette;
            else if (_rng.NextDouble() < CHANCE)
                DP.Weight = DP.Weight / CHANCE;
            else
                DP.StateFlag = PhotonStateType.KilledRussianRoulette;
        }

    }
}
