using System;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for an actual fiber detector.  This is a special case of a volume detector since
    /// it tallies only after passing out the surface.  However, volume detectors go through each collision
    /// and process it.  This allows the checking of if a reflection at the surface should be tallied if
    /// it is under the fiber.
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
            TallyDetails.IsVolumeTally = true;
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
    /// This implements IHistoryDetector because need to tally at intermediate collisions
    /// </summary>
    public class SurfaceFiberDetectorDetector : Detector, IHistoryDetector
    {
        private ITissue _tissue;
        private Random _rng;
        private bool _dead;

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

            // initialize any other necessary class fields here
            _tissue = tissue;
            _rng = rng;
        }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var weight = dp.Weight; //* Math.Abs(dp.Direction.Uz);
            if (weight != 0.0)
            {
                // is this photon conflicting with photon passed into IsWithinAperture?
                var photon = new Photon(dp.Position, dp.Direction, _tissue, currentRegionIndex, _rng);

                // check if passes Fresnel Note!  this may only work if N tissue = N detector
                double cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(photon);
                var nCurrent = _tissue.Regions[currentRegionIndex].RegionOP.N;
                double coscrit;
                if (nCurrent > N)
                {
                    coscrit = Math.Sqrt(1.0 - (N / nCurrent) * (N / nCurrent));
                }
                else
                {
                    coscrit = 0.0;
                }

                double cosThetaSnell;
                
                double probOfReflecting = Optics.Fresnel(nCurrent, N, cosTheta, out cosThetaSnell);
                if (cosTheta <= coscrit)
                {
                    probOfReflecting = 1.0;
                }

                // perform first check so that rng not called on pseudo-collisions
                if ((probOfReflecting == 0.0) || (_rng.NextDouble() > probOfReflecting)) // transmitted
                {
                    // refract into detector
                    photon.DP.Direction = _tissue.GetRefractedDirection(dp.Position, dp.Direction,
                        nCurrent, N, cosThetaSnell);

                    // if transmitted check if within aperture
                    if (!IsWithinDetectorAperture(photon))
                    {
                        return;
                    }

                    if (NA == 0.22)
                    {
                        var count = 1;
                    }
                    Mean += weight;
                    if (TallySecondMoment)
                    {
                        SecondMoment += weight * weight;
                    }

                    TallyCount++;
                    _dead = true;
                }
            }
                
        }
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // concern: rng used to determine whether detected or not varies with detector
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            _dead = false;
            int index = 0;
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                if (_dead)
                {
                    return;
                }

                index = index + 1;
                // check if at pseudo-collision due to reflection at the surface and in detector
                if ((Math.Abs(dp.Position.Z) < 1E-6) &&
                    (Math.Sqrt((dp.Position.X - Center.X) * (dp.Position.X - Center.X) +
                               (dp.Position.Y - Center.Y) * (dp.Position.Y - Center.Y)) < Radius)) 
                {
                    // FIX! debug hard code index=1 
                    TallySingle(previousDP, dp, 1);
                }
                previousDP = dp;
                
            }
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            //var areaNorm = 2.0 * Math.PI * Radius; // do we normalize fiber detector results by area of fiber end?
            var areaNorm = 1;
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
            //var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
            return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, N);            

            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
