using System;
using System.Numerics;
using MathNet.Numerics;
using System.Runtime.Serialization;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for reflectance as a function of Rho and Omega.
    /// This works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoAndOmegaDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of rho and Omega detector input
        /// </summary>
        public ROfRhoAndOmegaDetectorInput()
        {
            TallyType = "ROfRhoAndOmega";
            Name = "ROfRhoAndOmega";
            Rho = new DoubleRange(0.0, 10, 101);
            Omega = new DoubleRange(0.05, 1.0, 20);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Omega binning
        /// </summary>
        public DoubleRange Omega { get; set; }

        public IDetector CreateDetector()
        {
            return new ROfRhoAndOmegaDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                Omega = this.Omega
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Rho and Omega.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoAndOmegaDetector : Detector, IDetector
    {
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Omega binning
        /// </summary>
        public DoubleRange Omega { get; set; }

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

        public void Initialize(ITissue tissue)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new Complex[Rho.Count - 1, Omega.Count];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Rho.Count - 1, Omega.Count] : null);

            // intialize any other necessary class fields here
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var totalTime = photon.DP.TotalTime;
            for (int iw = 0; iw < Omega.Count; iw++)
            {
                double freq = Omega.AsEnumerable().ToArray()[iw];
                Mean[ir, iw] += photon.DP.Weight*(Math.Cos(-2*Math.PI*freq*totalTime) +
                                                  Complex.ImaginaryOne * Math.Sin(-2*Math.PI*freq*totalTime));

                if (TallySecondMoment)
                {
                    SecondMoment[ir, iw] +=
                        photon.DP.Weight*(Math.Cos(-2*Math.PI*freq*totalTime))*
                        photon.DP.Weight*(Math.Cos(-2*Math.PI*freq*totalTime)) +
                        Complex.ImaginaryOne*
                        photon.DP.Weight*(Math.Sin(-2*Math.PI*freq*totalTime))*
                        photon.DP.Weight*(Math.Sin(-2*Math.PI*freq*totalTime));
                }
            }
            TallyCount++;
        }
        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iw = 0; iw < Omega.Count; iw++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, iw] /= areaNorm * numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ir, iw] /= areaNorm * areaNorm * numPhotons;
                    }
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
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Omega.Count; j++)
                            {
                                binaryWriter.Write(Mean[i, j].Real);
                                binaryWriter.Write(Mean[i, j].Imaginary);
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new Complex[ Rho.Count - 1, Omega.Count];
                        for (int i = 0; i <  Rho.Count - 1; i++) {
                            for (int j = 0; j < Omega.Count; j++)
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
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Omega.Count; j++)
                            {
                                binaryWriter.Write(SecondMoment[i, j].Real);
                                binaryWriter.Write(SecondMoment[i, j].Imaginary);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new Complex[ Rho.Count - 1, Omega.Count ];
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Omega.Count; j++)
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
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }

    }
}
