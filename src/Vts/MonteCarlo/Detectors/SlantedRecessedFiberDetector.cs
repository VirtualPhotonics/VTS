using System;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;

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
            ZPlane = 0.0;
            Center = new Position(2.0, 0.0, 0.0);
            N = 1.4;
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0;   // recessed detector is always above the tissue

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
        }

        /// <summary>
        /// radius of the detector
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// slanted angle (clockwise 0-89.999999deg)
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// the lowest z coordinate of the fiber after rotation. If fiber is just touching the tissue, ZPlane=0.
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// detector center location (before rotation)
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// detector fiber refractive index
        /// </summary>
        public double N { get; set; }
        /// <summary>
        /// detector region index
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
                ZPlane = this.ZPlane,
                Center = this.Center,
                N=this.N,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for fiber flat surface detection.
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
        /// slanted angle (clockwise 0-89.999999deg)
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// the lowest z coordinate of the fiber after rotation. If fiber is just touching the tissue, ZPlane=0. 
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// detector center location (before rotation)
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// detector fiber refractive index
        /// </summary>
        public double N { get; set; }
        /// <summary>
        /// detector region index
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
            //find the center point of fiber, when it is rotated around the right edge (x = Center.X + Radius)
            var cosAngle = Math.Cos(Angle);
            var sinAngle = Math.Sqrt(1.0 - cosAngle * cosAngle);

            //compute acceptance angle of fiber
            var regionRefIndex = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
            var acceptanceAngle = Math.Asin(NA / regionRefIndex);

            //find the normal vector to the detector plane (inward)
            var normDir = new Direction(sinAngle, 0.0, -cosAngle);

            // determine if photon direction and detector normal
            var cosTheta = Direction.GetDotProduct(photon.DP.Direction, normDir);

            if (cosTheta < Math.Cos(acceptanceAngle)) return;
            //regardless of the z coordinate of the 'Center', replace it with "zPlane + tiny air layer"
            Center.Z = ZPlane - 1e-10;
                
            // center after rotation
            var xShift = Radius * sinAngle * sinAngle / cosAngle;
            var zShift = -Radius * sinAngle;
            var rotatedCenterPos = new Position(Center.X + xShift, Center.Y, Center.Z + zShift );

            // apply the method described in https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
            //compute the difference between a point on the plane and photon exit point
            var planeDir = new Direction(rotatedCenterPos.X - photon.DP.Position.X,
                rotatedCenterPos.Y - photon.DP.Position.Y, rotatedCenterPos.Z - photon.DP.Position.Z);

            //compute "t" parameter
            var t = Direction.GetDotProduct(planeDir, normDir) / cosTheta;
            if (t <= 0.0) return;
            //compute the location on the plane
            var planePos = new Position(photon.DP.Position.X + photon.DP.Direction.Ux * t, photon.DP.Position.Y + 
                photon.DP.Direction.Uy * t, photon.DP.Position.Z + photon.DP.Direction.Uz * t);
                    
            //check that photon entry location is within fiber radius
            var dx = planePos.X - rotatedCenterPos.X;
            var dy = planePos.Y - rotatedCenterPos.Y;
            var dz = planePos.Z - rotatedCenterPos.Z;
            var d = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (d > Radius) return;  //when entry location is NOT within fiber radius

            //Compute fresnel reflectance and modify photon weight
            var photonWeight = photon.DP.Weight * (1.0 - Optics.Fresnel(regionRefIndex, N, cosTheta, out double uz_snell));

            Mean += photonWeight;
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment += photonWeight * photonWeight;
        }            

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            // Normalize the results by area of fiber surface. 
            var areaNorm = Math.PI * Radius * Radius; 
            Mean /= areaNorm * numPhotons;
            if (!TallySecondMoment) return;
            SecondMoment /= areaNorm * areaNorm * numPhotons;
        }

        /// <summary>
        /// this scalar tally is saved to json
        /// </summary>
        /// <returns>empty array of BinaryArraySerializer</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return Array.Empty<BinaryArraySerializer>();
        }
    }
}
