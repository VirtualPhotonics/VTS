using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// The <see cref="Detectors"/> namespace contains the Monte Carlo detector classes 
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Tally for absorbed energy as a function of Rho and Z.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class AOfRhoAndZDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for absorbed energy as a function of rho and Z detector input
        /// </summary>
        public AOfRhoAndZDetectorInput()
        {
            TallyType = "AOfRhoAndZ";
            Name = "AOfRhoAndZ";
            Rho = new DoubleRange(0.0, 10, 101);
            Z = new DoubleRange(0.0, 10, 101);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
            TallyDetails.IsCylindricalTally = true;
            TallyDetails.IsNotImplementedForCAW = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public DoubleRange Z { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new AOfRhoAndZDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                Z = this.Z
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Rho and Z.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class AOfRhoAndZDetector : Detector, IHistoryDetector
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private double[,] _tallyForOnePhoton;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public DoubleRange Z { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Zs detector gets tallied to
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
            Mean = Mean ?? new double[Rho.Count - 1, Z.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1, Z.Count - 1] : null);

            // initialize any other necessary class fields here
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();
            _tallyForOnePhoton = _tallyForOnePhoton ?? (TallySecondMoment ? new double[Rho.Count - 1, Z.Count - 1] : null);
        }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);
            // Note: GetVolumeAbsorptionWeightingMethod in Initialize method determines the *absorbed* weight

            if (weight == 0.0) return;
            Mean[ir, iz] += weight;
            TallyCount++;
            if (!TallySecondMoment) return;
            _tallyForOnePhoton[ir, iz] += weight;
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
            if (!TallySecondMoment) return;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    SecondMoment[ir, iz] += _tallyForOnePhoton[ir, iz] * _tallyForOnePhoton[ir, iz];
                }
            }
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, iz] /= areaNorm * numPhotons;
                    if (!TallySecondMoment) continue;
                    SecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
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
                    WriteData = MeanWriter,
                    ReadData = MeanReader
                },
                // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null :  new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = SecondMomentWriter,
                    ReadData = SecondMomentReader,
                },
            };
        }
        private void MeanWriter(BinaryWriter binaryWriter)
        {
            for (var i = 0; i < Rho.Count - 1; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    binaryWriter.Write(Mean[i, j]);
                }
            }
        }

        private void MeanReader(BinaryReader binaryReader)
        {
            Mean = Mean ?? new double[Rho.Count - 1, Z.Count - 1];
            for (var i = 0; i < Rho.Count - 1; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    Mean[i, j] = binaryReader.ReadDouble();
                }
            }
        }

        private void SecondMomentWriter(BinaryWriter binaryWriter)
        {
            if (!TallySecondMoment || SecondMoment == null) return;
            for (var i = 0; i < Rho.Count - 1; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    binaryWriter.Write(SecondMoment[i, j]);
                }
            }
        }

        private void SecondMomentReader(BinaryReader binaryReader)
        {
            if (!TallySecondMoment || SecondMoment == null) return;
            SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            for (var i = 0; i < Rho.Count - 1; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    SecondMoment[i, j] = binaryReader.ReadDouble();
                }
            }
        }

        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            return true; // or, possibly test for NA or confined position, etc
        }

    }
}
