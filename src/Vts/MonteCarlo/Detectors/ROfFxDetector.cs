using System;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for reflectance as a function of spatial frequency fx
    /// </summary>
    public class ROfFxDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of Fx detector input
        /// </summary>
        public ROfFxDetectorInput()
        {
            TallyType = "ROfFx";
            Name = "ROfFx";
            Fx = new DoubleRange(0.0, 0.5, 51);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
        }

        /// <summary>
        /// detector Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
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
            return new ROfFxDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Fx = this.Fx,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Fx.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfFxDetector : Detector, IDetector
    {
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
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
        public Complex[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public Complex[] SecondMoment { get; set; }

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
            Mean = Mean ?? new Complex[Fx.Count];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Fx.Count] : null);

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
            
            var dp = photon.DP;
            var x = dp.Position.X;
            var fxArray = Fx.ToArray();
            for (var i = 0; i < fxArray.Length; i++)
            {
                var freq = fxArray[i];
                var sinNegativeTwoPiFx = Math.Sin(-2*Math.PI*freq*x);
                var cosNegativeTwoPiFx = Math.Cos(-2*Math.PI*freq*x);
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1
                var deltaWeight = dp.Weight*(cosNegativeTwoPiFx + Complex.ImaginaryOne*sinNegativeTwoPiFx);

                Mean[i] += deltaWeight;
                // 2nd moment is E[xx*]=E[xReal^2]+E[xImag^2] and with cos^2+sin^2=1 => weight^2
                if (!TallySecondMoment) continue;
                SecondMoment[i] += dp.Weight * dp.Weight;
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            for (var i = 0; i < Fx.Count; i++)
            {
                Mean[i] /= numPhotons;
                if (!TallySecondMoment) continue;
                SecondMoment[i] /= numPhotons;
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers() 
        {
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (var i = 0; i < Fx.Count; i++) 
                        {
                            binaryWriter.Write(Mean[i].Real);
                            binaryWriter.Write(Mean[i].Imaginary);
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new Complex[ Fx.Count ];
                        for (var i = 0; i <  Fx.Count; i++) 
                        {
                            var real = binaryReader.ReadDouble();
                            var imag = binaryReader.ReadDouble();
                            Mean[i] = new Complex(real, imag);
                        }
                    }
                },
                // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null :  new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                    for (var i = 0; i < Fx.Count; i++)
                        {
                            binaryWriter.Write(SecondMoment[i].Real);
                            binaryWriter.Write(SecondMoment[i].Imaginary);
                        }          
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new Complex[ Fx.Count ];
                        for (var i = 0; i < Fx.Count; i++) 
                        {
                            var real = binaryReader.ReadDouble();
                            var imag = binaryReader.ReadDouble();
                            SecondMoment[i] = new Complex(real, imag);
                        }   
                    },
                },
            };
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
