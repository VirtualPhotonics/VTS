using System;
using Vts.IO;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for total absorption in bounding volume.  This works like a reflectance
    /// tally rather than a IHistoryDetector because it tallies once the photon enters
    /// the bounding volume.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    public class ATotalBoundingVolumeDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for absorption at bounding volume detector input
        /// </summary>
        public ATotalBoundingVolumeDetectorInput()
        {
            TallyType = "ATotalBoundingVolume";
            Name = "ATotalBoundingVolume";

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsLateralBoundingVolumeTally = true;
        }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new ATotalBoundingVolumeDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for total absorbed energy.
    /// This implementation works for Analog, DAW processing.
    /// </summary>
    public class ATotalBoundingVolumeDetector : Detector, IDetector
    {
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        public double SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        /// <summary>
        /// Method to initialize detector
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="rng">random number generator</param>
        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            Mean = new double();
            if (TallySecondMoment)
            {
                SecondMoment = new double();
            }

            // initialize any other necessary class fields here
            AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            Mean += photon.DP.Weight;  // no mua factor needed because absorbed energy
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment += photon.DP.Weight * photon.DP.Weight;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons are launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
            if (!TallySecondMoment) return;
            SecondMoment /= numPhotons;
        }

        /// <summary>
        /// this scalar tally is saved to json
        /// </summary>
        /// <returns>array of BinaryArraySerializer</returns>
        public BinaryArraySerializer[] GetBinarySerializers() => null;

    }
}
