using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for pMC estimation of reflectance as a function of Fx.
    /// </summary>
    public class pMCROfFxDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for pMC reflectance as a function of Fx detector input
        /// </summary>
        public pMCROfFxDetectorInput()
        {
            TallyType = "pMCROfFx";
            Name = "pMCROfFx";
            Fx = new DoubleRange(0.0, 10, 101);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IspMCReflectanceTally = true;
        }
        /// <summary>
        /// detector Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public OpticalProperties[] PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public int[] PerturbedRegionsIndices { get; set; }

        public IDetector CreateDetector()
        {
            return new pMCROfFxDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Fx = this.Fx,
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of Fx.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class pMCROfFxDetector : Detector, IDetector
    {
        private double[] _fxArray;
        private OpticalProperties[] _referenceOps;
        private OpticalProperties[] _perturbedOps;
        private int[] _perturbedRegionsIndices;
        private Func<long[], double[], OpticalProperties[], OpticalProperties[], int[], double> _absorbAction;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public OpticalProperties[] PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public int[] PerturbedRegionsIndices { get; set; }

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

        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new Complex[Fx.Count];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Fx.Count] : null);

            // intialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToArray();
            TallyCount = 0;
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(tissue, this);
            _fxArray = Fx.AsEnumerable().ToArray();
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var dp = photon.DP;
            var x = dp.Position.X;

            double weightFactor = _absorbAction(
                photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToArray(),
                photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray(),
                _perturbedOps, _referenceOps, _perturbedRegionsIndices);


            for (int ifx = 0; ifx < _fxArray.Length; ++ifx)
            {
                double freq = _fxArray[ifx];

                var sinNegativeTwoPiFX = Math.Sin(-2 * Math.PI * freq * x);
                var cosNegativeTwoPiFX = Math.Cos(-2 * Math.PI * freq * x);

                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1

                var deltaWeight = (weightFactor * dp.Weight) * (cosNegativeTwoPiFX + Complex.ImaginaryOne * sinNegativeTwoPiFX);

                Mean[ifx] += deltaWeight;
                if (TallySecondMoment)  // 2nd moment is E[xx*]=E[xreal^2]+E[ximag^2]
                {
                    var deltaWeight2 =
                        weightFactor * weightFactor * dp.Weight * dp.Weight * cosNegativeTwoPiFX * cosNegativeTwoPiFX +
                        weightFactor * weightFactor * dp.Weight * dp.Weight * sinNegativeTwoPiFX * sinNegativeTwoPiFX;
                    // second moment of complex tally is square of real and imag separately
                    SecondMoment[ifx] += deltaWeight2;
                }
            }
            TallyCount++;
            
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            for (int ifx = 0; ifx < Fx.Count; ifx++)
            {
                Mean[ifx] /= numPhotons;
                // the above is pi(rmax*rmax-rmin*rmin) * FxDelta * N
                if (TallySecondMoment)
                {
                    SecondMoment[ifx] /= numPhotons;
                }
            }
        }

        // this is to allow saving of large arrays separately as a binary file
        public BinaryArraySerializer[] GetBinarySerializers() // NEED TO ASK DC: about complex array implementation
        {
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (int i = 0; i < Fx.Count; i++) {
                                binaryWriter.Write(Mean[i].Real);
                                binaryWriter.Write(Mean[i].Imaginary);
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new Complex[ Fx.Count ];
                        for (int i = 0; i <  Fx.Count; i++) {
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                Mean[i] = new Complex(real, imag);
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
                        for (int i = 0; i < Fx.Count; i++) {
                            {
                                binaryWriter.Write(SecondMoment[i].Real);
                                binaryWriter.Write(SecondMoment[i].Imaginary);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new Complex[ Fx.Count ];
                        for (int i = 0; i < Fx.Count; i++) {
                            {
                                var real = binaryReader.ReadDouble();
                                var imag = binaryReader.ReadDouble();
                                SecondMoment[i] = new Complex(real, imag);
                            }                       
			            }
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            // return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
