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
    /// <summary>
    /// Implements IDetector&lt;double[,,]&gt;.  Tally for Fluence(rho,z,t).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetector))]
    public class FluenceOfRhoAndZAndTimeDetector : IHistoryDetector<double[, ,]>
    {
        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        ///<summary>
        /// Returns an instance of FluenceOfRhoAndZAndTimeDetector
        ///</summary>
        ///<param name="rho"></param>
        ///<param name="z"></param>
        ///<param name="time"></param>
        ///<param name="tissue"></param>
        public FluenceOfRhoAndZAndTimeDetector(
            DoubleRange rho,
            DoubleRange z,
            DoubleRange time,
            ITissue tissue,
            bool tallySecondMoment,
            String name
            )
        {
            Rho = rho;
            Z = z;
            Time = time;
            Mean = new double[Rho.Count - 1, Z.Count - 1, Time.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1, Time.Count - 1];
            }
            TallyType = TallyType.FluenceOfRhoAndZAndTime;
            Name = name;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetAbsorptionWeightingMethod(tissue, this);

            TallyCount = 0;
            _tissue = tissue;
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        /// Returns an instance of FluenceOfRhoAndZAndTimeDetector (for serialization purposes only)
        /// </summary>
        public FluenceOfRhoAndZAndTimeDetector()
            : this(
            new DoubleRange(),
            new DoubleRange(),
            new DoubleRange(),
            new MultiLayerTissue(),
            true,
            TallyType.FluenceOfRhoAndZAndTime.ToString())
        {
        }

        [IgnoreDataMember]
        public double[, ,] Mean { get; set; }

        [IgnoreDataMember]
        public double[, ,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Z { get; set; }

        public DoubleRange Time { get; set; }

        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            var it = DetectorBinning.WhichBin(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);

            var weight = _absorptionWeightingMethod(dp, previousDP, currentRegionIndex);

            var regionIndex = currentRegionIndex;

            if (weight != 0.0)
            {
                Mean[ir, iz, it] += weight / _ops[regionIndex].Mua;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir, iz, it] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
                }
                TallyCount++;
            }
        }

        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta * Time.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (int it = 0; it < Time.Count - 1; it++)
                    {
                        var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                        Mean[ir, iz, it] /= areaNorm * numPhotons;
                        if (_tallySecondMoment)
                        {
                            SecondMoment[ir, iz, it] /= areaNorm * areaNorm * numPhotons;
                        }
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