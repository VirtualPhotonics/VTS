using System;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for Absorption(rho,z).
    /// </summary>
    [KnownType(typeof(AOfRhoAndZDetector))]
    public class AOfRhoAndZDetector : IHistoryDetector<double[,]>
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        private ITissue _tissue;
        private bool _tallySecondMoment;

        /// <summary>
        /// constructor for absorbed energy as a function of rho and z
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public AOfRhoAndZDetector(
            DoubleRange rho, 
            DoubleRange z, 
            ITissue tissue, 
            bool tallySecondMoment,
            String name 
            )
        {
            Rho = rho;
            Z = z;
            Mean = new double[Rho.Count - 1, Z.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            }
            TallyType = TallyType.AOfRhoAndZ;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// Returns a default instance of AOfRhoAndZDetector (for serialization purposes only)
        /// </summary>
        public AOfRhoAndZDetector()
            : this(new DoubleRange(), new DoubleRange(), new MultiLayerTissue(), true, TallyType.AOfRhoAndZ.ToString())
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
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of time detector gets tallied to
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
        /// method to tally to detector given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous photon data point</param>
        /// <param name="dp">current photon data point</param>
        /// <param name="currentRegionIndex">index of region where the photon is currently in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            if (weight != 0.0)
            {
                Mean[ir, iz] += weight;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir, iz] += weight * weight;
                }
                TallyCount++;
            }
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
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
        /// method to determine if photon within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>this method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}