using System;
using System.Linq;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double&gt;.  Tally for Total Absorption.
    /// </summary>
    [KnownType(typeof(ATotalDetector))]
    public class ATotalDetector : IHistoryDetector<double>
    {
        private ITissue _tissue;
        private bool _tallySecondMoment;
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        /// <summary>
        /// constructor for total absorption detector input
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public ATotalDetector(ITissue tissue, bool tallySecondMoment, String name)
        {
            TallyType = TallyType.ATotal;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            _tallySecondMoment = tallySecondMoment;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// Returns a default instance of ATotalDetector (for serialization purposes only)
        /// </summary>
        public ATotalDetector()
            : this(new MultiLayerTissue(), true, TallyType.ATotal.ToString())
        {
        }
        /// <summary>
        /// detector mean
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        public double SecondMoment { get; set; }
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
        /// <param name="currentRegionIndex">index of region photon is currently in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            if (weight != 0.0)
            {
                Mean += weight;
                if (_tallySecondMoment)
                {
                    SecondMoment += weight * weight;
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
            Mean /= numPhotons;
            if (_tallySecondMoment)
            {
                SecondMoment /= numPhotons;
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