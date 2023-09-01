using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for Fluence(rho, z, ft). Fluence in cylindrical coordinates and temporal-frequency.
    /// </summary>
    public class FluenceOfRhoAndZAndOmegaDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of rho, z and omega detector input
        /// </summary>
        public FluenceOfRhoAndZAndOmegaDetectorInput()
        {
            TallyType = "FluenceOfRhoAndZAndOmega";
            Name = "FluenceOfRhoAndZAndOmega";
            Rho = new DoubleRange(0.0, 10.0, 101);
            Z = new DoubleRange(0.0, 10.0, 101);
            Omega = new DoubleRange(0.05, 1.0, 20);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
            TallyDetails.IsNotImplementedForCAW = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// Omega binning
        /// </summary>
        public DoubleRange Omega { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new FluenceOfRhoAndZAndOmegaDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                Z = this.Z,
                Omega = this.Omega,
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for Fluence(x,y,z,omega).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    public class FluenceOfRhoAndZAndOmegaDetector : Detector, IHistoryDetector
    {
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// Omega binning
        /// </summary>
        public DoubleRange Omega { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember] public Complex[, ,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember] public Complex[, ,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private Complex[,,] _tallyForOnePhoton;

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
            Mean = Mean ?? new Complex[Rho.Count - 1, Z.Count - 1, Omega.Count];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Rho.Count - 1, Z.Count - 1, Omega.Count] : null);

            // initialize any other necessary class fields here
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();
            _tallyForOnePhoton = _tallyForOnePhoton ?? (TallySecondMoment ? new Complex[Rho.Count - 1, Z.Count - 1, Omega.Count] : null);
        }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var totalTime = dp.TotalTime;
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);
            // Note: GetVolumeAbsorptionWeightingMethod in Initialize method determines the *absorbed* weight
            //  so for fluence this weight is divided by Mua

            var regionIndex = currentRegionIndex;

            if (weight != 0.0)
            {
                for (int iw = 0; iw < Omega.Count; iw++)
                {
                    double freq = Omega.ToArray()[iw];

                    Mean[ir, iz, iw] += (weight/_ops[regionIndex].Mua) *
                                               (Math.Cos(-2 * Math.PI * freq * totalTime) +
                         Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime));
                    if (TallySecondMoment)
                    {
                        // set tallyForOnePhoton to be mean tally, 2nd moment taken at photon end
                        _tallyForOnePhoton[ir, iz, iw] += (weight / _ops[regionIndex].Mua) *
                                               (Math.Cos(-2 * Math.PI * freq * totalTime) +
                         Complex.ImaginaryOne * Math.Sin(-2 * Math.PI * freq * totalTime));
                    }
                }
                TallyCount++;
            }
        }
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // second moment is calculated AFTER the entire photon biography has been processed
            if (TallySecondMoment)
            {
                Array.Clear(_tallyForOnePhoton, 0, _tallyForOnePhoton.Length);
            }
            var previousDp = photon.History.HistoryData.First();
            foreach (var dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDp, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDp = dp;
            }
            // second moment determined after all tallies to each detector bin for ONE photon has been complete
            if (!TallySecondMoment) return; // 2nd moment is E[xx*]=E[xReal^2]+E[xImag^2]
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (var iw = 0; iw < Omega.Count; iw++)
                    {
                        SecondMoment[ir, iz, iw] += _tallyForOnePhoton[ir, iz, iw].Real*_tallyForOnePhoton[ir, iz, iw].Real+
                                                    _tallyForOnePhoton[ir, iz, iw].Imaginary*_tallyForOnePhoton[ir, iz, iw].Imaginary;
                    }
                }
            }
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                var volumeNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (var iw = 0; iw < Omega.Count; iw++)
                    {
                        Mean[ir, iz, iw] /= volumeNorm * numPhotons;
                        if (!TallySecondMoment) continue;
                        SecondMoment[ir, iz, iw] /= volumeNorm * volumeNorm * numPhotons;
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
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var k = 0; k < Z.Count - 1; k++) {
                                for (var l = 0; l < Omega.Count; l++)
                                {
                                    binaryWriter.Write(Mean[i, k, l].Real);
                                    binaryWriter.Write(Mean[i, k, l].Imaginary);
                                }
                            } 
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new Complex[Rho.Count - 1, Z.Count -1, Omega.Count];
                        for (var i = 0; i <  Rho.Count - 1; i++) {
                            for (var k = 0; k < Z.Count - 1; k++) {
                                for (var l = 0; l < Omega.Count; l++)
                                {
                                    var real = binaryReader.ReadDouble();
                                    var imag = binaryReader.ReadDouble();
                                    Mean[i, k, l] = new Complex(real, imag);
                                }
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
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var k = 0; k < Z.Count - 1; k++){
                                for (var l = 0; l < Omega.Count; l++)
                                {
                                    binaryWriter.Write(SecondMoment[i, k, l].Real);
                                    binaryWriter.Write(SecondMoment[i, k, l].Imaginary);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new Complex[Rho.Count - 1, Z.Count - 1, Omega.Count];
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var k = 0; k < Z.Count - 1; k++) {
                                for (var l = 0; l < Omega.Count; l++)
                                {
                                    var real = binaryReader.ReadDouble();
                                    var imag = binaryReader.ReadDouble();
                                    SecondMoment[i, k, l] = new Complex(real, imag);
                                }
                            }  
			            }
                    },
                },
            };
        }

        /// <summary>
        /// method to determine if photon within detector, i.e. in NA, etc.
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }

    }
}