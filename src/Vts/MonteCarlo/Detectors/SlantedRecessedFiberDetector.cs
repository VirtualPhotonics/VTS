using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for R(r) recessed in air at a specified z plane
    /// </summary>
    public class SlantedRecessedFiberDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of slanted flat detector input
        /// </summary>
        public SlantedRecessedFiberDetectorInput()
        {
            TallyType = "SlantedRecessedFiber";
            Name = "SlantedRecessedFiber";
            Radius = 1.0;
            Angle = Math.PI / 6.0;
            ContactPosition = new Position(5.0, 0.0, 0.0);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // detector is always in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = true;
        }

        /// <summary>
        /// radius of the detector
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Slanted angle (clockwise 0-90)
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Contact Position of the tissue
        /// </summary>
        public Position ContactPosition { get; set; }

        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }

        /// <summary>
        /// detector numerical aperture
        /// </summary>
        public double NA { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new SlantedRecessedFiberDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // custom detector-specific properties
                Radius = this.Radius,
                Angle = this.Angle,
                ContactPosition = this.ContactPosition,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class SlantedRecessedFiberDetector : Detector, IDetector
    {
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// radius of the detector
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Slanted angle (clockwise 0-90)
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// Contact Position of the tissue
        /// </summary>
        public Position ContactPosition { get; set; }
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
        [IgnoreDataMember] public double Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember] public double SecondMoment { get; set; }

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
            Mean = new double();
            if (TallySecondMoment)
            {
                SecondMoment = new double();
            }

            // initialize any other necessary class fields here
            _tissue = tissue;
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            //Find the center point of fiber, when it is normal the surface
            Position centerPos = new Position(ContactPosition.X - Radius, ContactPosition.Y, ContactPosition.Z);
            //Find the center point of fiber, when it is rotated
            Position rotatedCenterPos = new Position(ContactPosition.X - Radius * Math.Cos(Angle), 
                ContactPosition.Y, ContactPosition.Z - Radius * Math.Sin(Angle));

            //find the inward normal vector to the detector plane
            Direction normDir = new Direction(0.0, 0.0, -1.0);  //Default direction (0, 0, 1
            var dx = rotatedCenterPos.X - centerPos.X;
            var dy = rotatedCenterPos.Y - centerPos.Y;
            var dz = rotatedCenterPos.Z - centerPos.Z;
            var d = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (d > 0.0)             
                normDir = new Direction(dx / d, dy / d, dz / d);

            // ray trace exit location and direction to location at ZPlane
            var positionAtSlantedPlane = LayerTissueRegionToolbox.RayExtendToInfiniteSlantedPlane
                (photon.DP.Position, photon.DP.Direction, ContactPosition, normDir);

            //check that exit location is within fiber radius
            var dx1 = positionAtSlantedPlane.X - rotatedCenterPos.X;
            var dy1 = positionAtSlantedPlane.Y - rotatedCenterPos.Y;
            var dz1 = positionAtSlantedPlane.Z - rotatedCenterPos.Z;
            var d1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1 + dz1 * dz1);
            if (d1 >= Radius) return;

            if (!IsWithinDetectorAperture(photon, normDir)) return;

            Mean += photon.DP.Weight;
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment += photon.DP.Weight * photon.DP.Weight;
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var areaNorm = Math.PI * Radius * Radius; // do we normalize fiber detector results by area of fiber end?
            Mean /= areaNorm * numPhotons;
            if (!TallySecondMoment) return;
            SecondMoment /= areaNorm * areaNorm * numPhotons;
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers() => null;

        /// <summary>
        /// Method to determine if photon is within detector NA
        /// </summary>
        /// <param name="photon">photon</param>
        /// <param name="detectorNormal">normal Direction of detector</param> 
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(Photon photon, Direction detectorNormal)
        {
            var detectorRegionN = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
            return photon.DP.IsWithinNA(NA, detectorNormal, detectorRegionN);
        }
    }
}
