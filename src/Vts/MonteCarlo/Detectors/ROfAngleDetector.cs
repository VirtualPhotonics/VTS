using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for reflectance as a function of angle
    /// </summary>
    public class ROfAngleDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of Angle detector input
        /// </summary>
        public ROfAngleDetectorInput()
        {
            TallyType = "ROfAngle";
            Name = "ROfAngle";
            Angle = new DoubleRange(Math.PI/2, Math.PI, 2);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
        }

        /// <summary>
        /// detector Angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// numerical aperture
        /// </summary>
        public double NA { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new ROfAngleDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Angle = this.Angle,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Angle.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfAngleDetector : Detector, IDetector
    {
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// Angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// numerical aperture
        /// </summary>
        public double NA { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

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

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[Angle.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Angle.Count - 1] : null);

            // initialize any other necessary class fields here
            _tissue = tissue;
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon)) return;
            
            var ia = DetectorBinning.WhichBin(Math.Acos(photon.DP.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            Mean[ia] += photon.DP.Weight;
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment[ia] += photon.DP.Weight * photon.DP.Weight;
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            // normalization accounts for Angle.Start != 0
            var normalizationFactor = 2.0 * Math.PI * Angle.Delta;
            for (var ir = 0; ir < Angle.Count - 1; ir++)
            {
                var areaNorm = Math.Sin(Angle.Start + (ir + 0.5) * Angle.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                if (!TallySecondMoment) continue;
                SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            Mean ??= new double[Angle.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[Angle.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    SecondMoment, "SecondMoment", "_2") : null
            };
            return allSerializers.Where(s => s is not null).ToArray();
        }

        /// <summary>
        /// Method to determine if photon is within detector NA
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            if (photon.CurrentRegionIndex == FinalTissueRegionIndex)
            {
                var detectorRegionN = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
                return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
            else // determine n of prior tissue region
            {
                var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                return photon.History.PreviousDP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
        }

    }
}
