using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for fluence as a function of spatial frequency fx and z
    /// </summary>
    public class FluenceOfFxAndZDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of Fx and Z detector input
        /// </summary>
        public FluenceOfFxAndZDetectorInput()
        {
            TallyType = "FluenceOfFxAndZ";
            Name = "FluenceOfFxAndZ";
            Fx = new DoubleRange(0.0, 0.5, 51);
            Z = new DoubleRange(0.0, 10, 101);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
            TallyDetails.IsNotImplementedForCAW = true;
        }

        /// <summary>
        /// detector Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Z { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new FluenceOfFxAndZDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Fx = this.Fx,
                Z = this.Z
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Fx.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class FluenceOfFxAndZDetector : Detector, IHistoryDetector
    {
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */

        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Z { get; set; }

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

        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private Complex[,] _tallyForOnePhoton;

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
            Mean = Mean ?? new Complex[Fx.Count, Z.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Fx.Count, Z.Count - 1] : null);

            // initialize any other necessary class fields here
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();
            _tallyForOnePhoton = _tallyForOnePhoton ?? (TallySecondMoment ? new Complex[Fx.Count, Z.Count - 1] : null);

        }
        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var x = dp.Position.X;
            var fxArray = Fx.ToArray();
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);
            // Note: GetVolumeAbsorptionWeightingMethod in Initialize method determines the *absorbed* weight
            //  so for fluence this weight is divided by Mua
            var regionIndex = currentRegionIndex;
            if (weight != 0)
            {
                for (int ifx = 0; ifx < fxArray.Length; ifx++)
                {
                    double freq = fxArray[ifx];
                    var sinNegativeTwoPiFX = Math.Sin(-2 * Math.PI * freq * x);
                    var cosNegativeTwoPiFX = Math.Cos(-2 * Math.PI * freq * x);
                    // convert to Hz-sec from GHz-ns 1e-9*1e9=1
                    var deltaWeight = weight * (cosNegativeTwoPiFX + Complex.ImaginaryOne * sinNegativeTwoPiFX);

                    Mean[ifx, iz] += (deltaWeight / _ops[regionIndex].Mua);
                    if (TallySecondMoment) // set tallyForOnePhoton to be mean tally, 2nd moment taken at photon end
                    {
                        _tallyForOnePhoton[ifx, iz] += (deltaWeight / _ops[regionIndex].Mua);
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
            for (var ifx = 0; ifx < Fx.Count; ifx++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {                       
                    SecondMoment[ifx, iz] += _tallyForOnePhoton[ifx, iz].Real * _tallyForOnePhoton[ifx, iz].Real +
                                             _tallyForOnePhoton[ifx, iz].Imaginary * _tallyForOnePhoton[ifx, iz].Imaginary;                        
                }
            }
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            for (var ifx = 0; ifx < Fx.Count; ifx++)
            {
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    Mean[ifx, iz] /= numPhotons * Z.Delta;
                    if (!TallySecondMoment) continue;
                    SecondMoment[ifx, iz] /= numPhotons * Z.Delta * Z.Delta;
                }
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            Mean ??= new Complex[Fx.Count,Z.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new Complex[Fx.Count, Z.Count - 1];
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
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
        }

    }
}
