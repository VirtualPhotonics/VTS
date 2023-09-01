using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for pMC estimation of reflectance as a function of Fx.
    /// </summary>
    public class pMCROfFxAndTimeDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for pMC reflectance as a function of Fx detector input
        /// </summary>
        public pMCROfFxAndTimeDetectorInput()
        {
            TallyType = "pMCROfFxAndTime";
            Name = "pMCROfFxAndTime";
            Fx = new DoubleRange(0.0, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IspMCReflectanceTally = true;
        }
        /// <summary>
        /// detector Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }
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
            return new pMCROfFxAndTimeDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Fx = this.Fx,
                Time = this.Time,
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of Fx.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class pMCROfFxAndTimeDetector : Detector, IDetector
    {
        private double[] _fxArray;
        private ITissue _tissue;
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }
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
        public Complex[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }

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
            Mean = Mean ?? new Complex[Fx.Count, Time.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Fx.Count, Time.Count - 1] : null);

            // initialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToArray();
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(tissue, this);
            _fxArray = Fx.ToArray();
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
            // WhichBin to match ROfFxAndTime
            var it = DetectorBinning.WhichBin(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);

            if (it == -1) return;
            var weightFactor = _absorbAction(
                photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToArray(),
                photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray(),
                _perturbedOps, _referenceOps, _perturbedRegionsIndices);

            for (var ifx = 0; ifx < _fxArray.Length; ++ifx)
            {
                var freq = _fxArray[ifx];

                var sinNegativeTwoPiFx = Math.Sin(-2*Math.PI*freq*x);
                var cosNegativeTwoPiFx = Math.Cos(-2*Math.PI*freq*x);

                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1

                var deltaWeight = dp.Weight * weightFactor *
                                  (cosNegativeTwoPiFx + Complex.ImaginaryOne*sinNegativeTwoPiFx);

                Mean[ifx, it] += deltaWeight;
                // 2nd moment is E[xx*]=E[xReal^2]+E[xImag^2] and with cos^2+sin^2=1 => weight^2
                if (!TallySecondMoment) continue;
                // second moment of complex tally is square of real and imag separately
                SecondMoment[ifx, it] += dp.Weight * weightFactor *
                                         dp.Weight * weightFactor;
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            for (var ifx = 0; ifx < Fx.Count; ifx++)
            {
                for (var it = 0; it < Time.Count - 1; it++)
                {
                    Mean[ifx, it] /= Time.Delta * numPhotons;
                    if (!TallySecondMoment) continue;
                    SecondMoment[ifx, it] /= Time.Delta * Time.Delta * numPhotons;
                }
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
                        for (var i = 0; i < Fx.Count; i++) {
                            for (var j = 0; j < Time.Count - 1; j++)
                            {
                                binaryWriter.Write(Mean[i, j].Real);
                                binaryWriter.Write(Mean[i, j].Imaginary);
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new Complex[ Fx.Count, Time.Count - 1];
                        for (var i = 0; i <  Fx.Count - 1; i++) {
                            for (var j = 0; j < Time.Count - 1; j++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                Mean[i, j] = new Complex(real, imag);
                            }
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
                        for (var i = 0; i < Fx.Count; i++) {
                            for (var j = 0; j < Time.Count - 1; j++)
                            {
                                binaryWriter.Write(SecondMoment[i, j].Real);
                                binaryWriter.Write(SecondMoment[i, j].Imaginary);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new Complex[ Fx.Count, Time.Count - 1];
                        for (var i = 0; i < Fx.Count - 1; i++) {
                            for (var j = 0; j < Time.Count - 1; j++)
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                SecondMoment[i, j] = new Complex(real, imag);
                            }                       
			            }
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector NA
        /// pMC does not have access to PreviousDP so logic based on DP and 
        /// n1 sin(theta1) = n2 sin(theta2) 
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            var detectorRegionN = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
            return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
        }
    }
}
