using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for R(x,y,theta,phi)
    /// </summary>
    public class ROfXAndYAndThetaAndPhiDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for R as a function of x, y, theta and phi detector input
        /// </summary>
        public ROfXAndYAndThetaAndPhiDetectorInput()
        {
            TallyType = "ROfXAndYAndThetaAndPhi";
            Name = "ROfXAndYAndThetaAndPhi";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            Theta = new DoubleRange(Math.PI / 2, Math.PI, 5); // theta (polar angle)
            Phi = new DoubleRange(-Math.PI, Math.PI, 5); // phi (azimuthal angle)            
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
        }

        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }

        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }

        /// <summary>
        /// theta binning
        /// </summary>
        public DoubleRange Theta { get; set; }
        /// <summary>
        /// phi binning
        /// </summary>
        public DoubleRange Phi { get; set; }
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
            return new ROfXAndYAndThetaAndPhiDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                Theta = this.Theta,
                Phi = this.Phi,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for R(x,y,theta,phi).
    /// </summary>
    public class ROfXAndYAndThetaAndPhiDetector : Detector, IDetector
    {
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// theta binning
        /// </summary>
        public DoubleRange Theta { get; set; }
        /// <summary>
        /// phi binning
        /// </summary>
        public DoubleRange Phi { get; set; }        
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
        public double[, , ,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary> 
        [IgnoreDataMember]
        public double[, , ,] SecondMoment { get; set; }

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
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1, Theta.Count - 1, Phi.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, Theta.Count -1, Phi.Count - 1] : null);

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

            // if exiting tissue top surface, Uz < 0 => Acos in [pi/2, pi]
            var ix = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(photon.DP.Position.Y, Y.Count - 1, Y.Delta, Y.Start);
            // using Acos, -1<Uz<1 goes to pi<theta<0, so first bin is most forward directed angle
            var it = DetectorBinning.WhichBin(Math.Acos(photon.DP.Direction.Uz), Theta.Count - 1, Theta.Delta, Theta.Start);
            var ip = DetectorBinning.WhichBin(Math.Atan2(photon.DP.Direction.Uy, photon.DP.Direction.Ux), 
                Phi.Count - 1, Phi.Delta, Phi.Start);
            Mean[ix, iy, it, ip] += photon.DP.Weight;
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment[ix, iy, it, ip] += photon.DP.Weight * photon.DP.Weight;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Theta.Delta * Phi.Delta;
            for (var ix = 0; ix < X.Count - 1; ix++)
            {
                for (var iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (var it = 0; it < Theta.Count - 1; it++)
                    {
                        for (var ip = 0; ip < Phi.Count - 1; ip++)
                        {
                            var areaNorm = Math.Sin(Theta.Start + (it + 0.5)*Theta.Delta)*normalizationFactor;
                            Mean[ix, iy, it, ip] /= areaNorm * numPhotons;
                            if (!TallySecondMoment) continue;
                            SecondMoment[ix, iy, it, ip] /= areaNorm * areaNorm * numPhotons;
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            Mean ??= new double[X.Count - 1, Y.Count - 1, Theta.Count - 1, Phi.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[X.Count - 1, Y.Count - 1, Theta.Count - 1, Phi.Count - 1];
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