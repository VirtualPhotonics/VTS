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
    /// Implements IHistoryDetector&lt;double[,]&gt;.  Tally for Fluence(rho,z).
    /// Note: this tally currently only works with discrete absorption weighting
    /// </summary>
    [KnownType(typeof(FluenceOfRhoAndZDetector))]
    public class FluenceOfRhoAndZDetector : IHistoryDetector<double[,]>
    {
        //private Func<double, double, double, double, PhotonStateType, double> _absorbAction;

        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        /// <summary>
        /// Returns an instance of FluenceOfRhoAndZDetector
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="tissue">tissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public FluenceOfRhoAndZDetector(
            DoubleRange rho,
            DoubleRange z,
            ITissue tissue,
            bool tallySecondMoment,
            String name
            )
        {
            Rho = rho;
            Z = z;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1, Z.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            }
            TallyType = TallyType.FluenceOfRhoAndZ;
            Name = name;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);

            TallyCount = 0;
            _tissue = tissue;
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        /// Returns an instance of FluenceOfRhoAndZDetector (for serialization purposes only)
        /// </summary>
        public FluenceOfRhoAndZDetector()
            : this(
            new DoubleRange(),
            new DoubleRange(),
            new MultiLayerTissue(),
            true, // tally SecondMoment
            TallyType.FluenceOfRhoAndZ.ToString())
        {
        }

        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name
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
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            var regionIndex = currentRegionIndex;

            if (weight != 0.0) // if weight = 0.0, then pseudo-collision and no tally
            {
                Mean[ir, iz] += weight / _ops[regionIndex].Mua;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir, iz] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
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
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">Number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, iz] /= areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }

        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}