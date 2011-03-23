using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetector))]
    /// <summary>
    /// Implements IHistoryDetector<double[,,]>.  Tally for Fluence(rho,z,t).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    public class FluenceOfRhoAndZAndTimeDetector : HistoryTallyBase, IHistoryDetector<double[, ,]>
    {
        public Func<double, double, double, double, PhotonStateType, double> _absorbAction;

        ///<summary>
        /// Returns an instance of FluenceOfRhoAndZAndTimeDetector
        ///</summary>
        ///<param name="rho"></param>
        ///<param name="z"></param>
        ///<param name="time"></param>
        ///<param name="tissue"></param>
        public FluenceOfRhoAndZAndTimeDetector(DoubleRange rho, DoubleRange z, DoubleRange time, ITissue tissue)
            : base(tissue)
        {
            Rho = rho;
            Z = z;
            Time = time;

            Mean = new double[Rho.Count - 1, Z.Count - 1, Time.Count - 1];
            SecondMoment = new double[Rho.Count - 1, Z.Count - 1, Time.Count - 1];
            TallyType = TallyType.FluenceOfRhoAndZAndTime;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns an instance of FluenceOfRhoAndZAndTimeDetector (for serialization purposes only)
        /// </summary>
        public FluenceOfRhoAndZAndTimeDetector()
            : this(new DoubleRange(), new DoubleRange(), new DoubleRange(), new MultiLayerTissue())
        {
        }

        [IgnoreDataMember]
        public double[, ,] Mean { get; set; }

        [IgnoreDataMember]
        public double[, ,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Z { get; set; }

        public DoubleRange Time { get; set; }

        protected override void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                case AbsorptionWeightingType.Analog:
                    _absorbAction = AbsorbAnalog;
                    break;
                case AbsorptionWeightingType.Continuous:
                    _absorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                    _absorbAction = AbsorbDiscrete;
                    break;
                default:
                    throw new ArgumentException("AbsorptionWeightingType not set");
            }
        }

        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            var it = DetectorBinning.WhichBin(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);

            var weight = _absorbAction(
                _ops[_tissue.GetRegionIndex(dp.Position)].Mua,
                _ops[_tissue.GetRegionIndex(dp.Position)].Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            var regionIndex = _tissue.GetRegionIndex(dp.Position);

            Mean[ir, iz, it] += weight / _ops[regionIndex].Mua;
            SecondMoment[ir, iz, it] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
            TallyCount++;
        }

        private double AbsorbAnalog(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (photonStateType != PhotonStateType.Absorbed)
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight * mua / (mua + mus);
            }
            return weight;
        }

        private double AbsorbDiscrete(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (previousWeight == weight) // pseudo collision, so no tally
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight * mua / (mua + mus);
            }
            return weight;
        }

        private double AbsorbContinuous(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            throw new NotImplementedException();
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta * Z.Delta * Time.Delta * numPhotons;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (int it = 0; it < Time.Count - 1; it++)
                    {
                        Mean[ir, iz, it] /= (ir + 0.5) * normalizationFactor;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }

    }
}