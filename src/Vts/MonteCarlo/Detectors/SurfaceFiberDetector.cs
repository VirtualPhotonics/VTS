using System;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for an actual fiber detector
    /// </summary>
    public class SurfaceFiberDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for cylindrical fiber detector input. The fiber only detects as photon
        /// crosses bottom cap in an upward direction (negative z direction)
        /// </summary>
        public SurfaceFiberDetectorInput()
        {
            TallyType = "SurfaceFiber";
            Center = new Position(0, 0, 0.5);
            Radius = 0.6;
            N = 1.4;
            Name = "SurfaceFiberDetector";
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 3; // assume detector is in cylinder region

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = false;
        }

        /// <summary>
        /// detector center location
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// detector Radius
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// detector fiber refractive index
        /// </summary>
        public double N { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }

        /// <summary>
        /// detector numerical aperture
        /// </summary>
        public double NA { get; set; }

        public IDetector CreateDetector()
        {
            return new SurfaceFiberDetectorDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Center = this.Center,
                Radius = this.Radius,
                N = this.N,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for fiber detection.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class SurfaceFiberDetectorDetector : Detector, IDetector
    {
        private ITissue _tissue;
        private Random _rng;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// detector center location
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// detector Radius
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// detector fiber refractive index
        /// </summary>
        public double N { get; set; }
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

        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            //Mean = Mean ?? new double();
            //SecondMoment = SecondMoment ?? (TallySecondMoment ? new double() : null);
            Mean = new double();
            if (TallySecondMoment)
            {
                SecondMoment = new double();
            }

            // intialize any other necessary class fields here
            _tissue = tissue;
            _rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(RandomNumberGeneratorType.MersenneTwister);
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // check if in detector
            if (Math.Sqrt((photon.DP.Position.X - Center.X) * (photon.DP.Position.X - Center.X) +
                          (photon.DP.Position.Y - Center.Y) * (photon.DP.Position.Y - Center.Y)) < Radius)
            {
                // check if passes Fresnel
                double cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(photon);
                var nCurrent = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
                double coscrit;
                if (nCurrent > N)
                    coscrit = Math.Sqrt(1.0 - (N / nCurrent) * (N / nCurrent));
                else
                    coscrit = 0.0;

                double cosThetaSnell;

                double probOfReflecting = Optics.Fresnel(nCurrent, N, cosTheta, out cosThetaSnell);
                if (cosTheta <= coscrit)
                    probOfReflecting = 1.0;
                // perform first check so that rng not called on pseudo-collisions
                if ((probOfReflecting == 0.0) || (_rng.NextDouble() > probOfReflecting)) // transmitted
                {
                    // if transmitted check if within aperture
                    if (!IsWithinDetectorAperture(photon))
                        return;

                    Mean += photon.DP.Weight;
                    if (TallySecondMoment)
                    {
                        SecondMoment += photon.DP.Weight * photon.DP.Weight;
                    }

                    TallyCount++;
                }
            }
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var areaNorm = 2.0 * Math.PI * Radius; // do we normalize fiber detector results by area of fiber end?
            Mean /= areaNorm * numPhotons;
            if (TallySecondMoment)
            {
                SecondMoment /= areaNorm * areaNorm * numPhotons;
            }           
        }

        // this scalar tally is saved to json
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return null;
        }
        /// <summary>
        /// Method to determine if photon is within detector NA
        /// </summary>
        /// <param name="photon">photon</param>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
            return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);            

            //return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
