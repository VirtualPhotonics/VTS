using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IHistoryDetector&lt;double[,,]&gt;.  Tally for Fluence(rho,z,t).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetector))]
    public class FluenceOfRhoAndZAndTimeDetector : IHistoryDetector<double[, ,]>
    {
        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        /// <summary>
        /// constructor for fluence as a function of rho, z and time detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="time">time binning</param>
        /// <param name="tissue">tissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
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
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);

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
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }

        /// <summary>
        /// method to tally to detector given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous photon data point</param>
        /// <param name="dp">current photon data point</param>
        /// <param name="currentRegionIndex">index of region photon is currently in</param>
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

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }
        }
        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
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
        /// <summary>
        /// method to determine if photon within detector, i.e. in NA, etc.
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}