using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for Radiance(r) for internal surface detector at depth z.
    /// Detector captures radiance in upward or downward direction through plane at depth z,
    /// as a function of rho, time and maximum depth
    /// Note: this replies on the tissue definition to have a layer interface at
    /// ZDepth so that a pseudo-collision can be created there and a tally can
    /// be made.  Another assumption ZDepth does not equal the top or bottom of
    /// the tissue definition (checked in input validation).
    /// </summary>
    public class RadianceOfRhoAndTimeAndMaxDepthAtZDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for radiance as a function of rho detector input
        /// </summary>
        public RadianceOfRhoAndTimeAndMaxDepthAtZDetectorInput()
        {
            TallyType = "RadianceOfRhoAndTimeAndMaxDepthAtZ";
            Name = "RadianceOfRhoAndTimeAndMaxDepthAtZ";
            Rho = new DoubleRange(0.0, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            MaxDepth = new DoubleRange(0.0, 1.0, 101);
            ZDepth = 3;
            ZDirection = 1;
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsInternalSurfaceTally = true;
            TallyDetails.IsCylindricalTally = true;
        }

        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// MaxDepth binning
        /// </summary>
        public DoubleRange MaxDepth { get; set; }
        /// <summary>
        /// constant defining surface of tally
        /// </summary>
        public double ZDepth { get; set; }
        /// <summary>
        /// int defining direction of radiance to be detected 1=downward, -1=upward
        /// </summary>
        public int ZDirection { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }

        /// <summary>
        /// detector numerical aperture
        /// </summary>
        public double NA { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new RadianceOfRhoAndTimeAndMaxDepthAtZDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = TallyType,
                Name = Name,
                TallySecondMoment = TallySecondMoment,
                TallyDetails = TallyDetails,

                // optional/custom detector-specific properties
                Rho = Rho,
                Time = Time,
                MaxDepth = MaxDepth,
                ZDepth = ZDepth,
                ZDirection = ZDirection,
                NA = NA,
                FinalTissueRegionIndex = FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for reflectance as a function  of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class RadianceOfRhoAndTimeAndMaxDepthAtZDetector : Detector, IHistoryDetector
    {
        private ITissue _tissue;
        private double[,,] _tallyForOnePhoton;
        private double _maxDepth; // max depth to current position in history

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// Time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// MaxDepth binning
        /// </summary>
        public DoubleRange MaxDepth { get; set; }
        /// <summary>
        /// constant defining surface of tally
        /// </summary>
        public double ZDepth { get; set; }        
        /// <summary>
        /// int defining direction of radiance to be detected 1=downward, -1=upward
        /// </summary>
        public int ZDirection { get; set; }
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
        public double[,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,] SecondMoment { get; set; }

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
            Mean ??= new double[Rho.Count - 1, Time.Count - 1, MaxDepth.Count - 1];
            SecondMoment ??= TallySecondMoment ? new double[Rho.Count - 1, Time.Count - 1, MaxDepth.Count - 1] : null;

            // initialize any other necessary class fields here
            _tissue = tissue;
            _tallyForOnePhoton ??= TallySecondMoment ? new double[Rho.Count - 1, Time.Count - 1, MaxDepth.Count - 1] : null;
        }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            if (!IsWithinDetectorAperture(previousDP, dp)) return;

            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1,
                Rho.Delta, Rho.Start);
            var id = DetectorBinning.WhichBin(_maxDepth, MaxDepth.Count - 1, MaxDepth.Delta, MaxDepth.Start);
            var it = DetectorBinning.WhichBin(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);

            if (dp.Weight == 0.0) return;
 
            Mean[ir, it, id] += dp.Weight; // FIX: do I divide by Uz here?
            TallyCount++;
            if (!TallySecondMoment) return;
            _tallyForOnePhoton[ir, it, id] += dp.Weight;
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
            // reinitialize max depth for each photon history
            _maxDepth = 0.0;
            // go though history with a moving window of 3 data points: start at i=1 so that there is a previous data point
            for (var i = 1; i < photon.History.HistoryData.Count - 3; i++)
            {
                var previousDp = photon.History.HistoryData[i - 1];
                var dp = photon.History.HistoryData[i];
                var nextDp = photon.History.HistoryData.ElementAtOrDefault(i + 1);

                // keep track of max depth reached in photon history to this point
                if (dp.Position.Z > _maxDepth) _maxDepth = dp.Position.Z;

                // check if dp at pseudo-collision at ZDepth and previous and next straddle ZDepth in right direction
                if (Math.Abs(dp.Position.Z - ZDepth) < 1E-10 &&
                    nextDp != null &&
                    ((previousDp.Position.Z < ZDepth && nextDp.Position.Z > ZDepth && ZDirection > 0) ||
                     (previousDp.Position.Z > ZDepth && nextDp.Position.Z < ZDepth && ZDirection < 0)))
                {
                    TallySingle(previousDp, dp, _tissue.GetRegionIndex(dp.Position));
                }
            }

            // second moment determined after all tallies to each detector bin for ONE photon has been complete
            if (!TallySecondMoment) return;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var it = 0; it < Time.Count - 1; it++)
                {
                    for (var id = 0; id < MaxDepth.Count - 1; id++)
                    {
                        SecondMoment[ir, it, id] += _tallyForOnePhoton[ir, it, id] * _tallyForOnePhoton[ir, it, id];
                    }
                }
            }
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            // normalization accounts for Rho.Start != 0
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Time.Delta;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                for (var it = 0; it < Time.Count - 1; it++)
                {
                    for (var id = 0; id < MaxDepth.Count - 1; id++)
                    {
                        Mean[ir, it, id] /= areaNorm * numPhotons;
                        if (!TallySecondMoment) continue;
                        SecondMoment[ir, it, id] /= areaNorm * areaNorm * numPhotons;
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
            Mean ??= new double[Rho.Count - 1, Time.Count - 1, MaxDepth.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[Rho.Count - 1, Time.Count - 1, MaxDepth.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),
                TallySecondMoment
                    ? BinaryArraySerializerFactory.GetSerializer(
                        SecondMoment, "SecondMoment", "_2")
                    : null
            };
            return allSerializers.Where(s => s is not null).ToArray();
        }

        /// <summary>
        /// Method to determine if photon is within detector NA.  This calling signature breaks with
        /// other detectors because this is unique internal *surface* tally with NA.
        /// </summary>
        /// <param name="previousDataPoint">photon previous data point</param>
        /// <param name="dataPoint">photon current data point</param>
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(PhotonDataPoint previousDataPoint, PhotonDataPoint dataPoint)
        {
            // determine current region index for dataPoint
            var currentRegionIndex = _tissue.GetRegionIndex(dataPoint.Position);
            // determine if capturing downward 
            if (ZDirection > 0) 
            {
                if (currentRegionIndex == FinalTissueRegionIndex)
                {
                    var detectorRegionN = _tissue.Regions[currentRegionIndex].RegionOP.N;
                    return dataPoint.IsWithinNA(NA, Direction.AlongPositiveZAxis, detectorRegionN);
                }
                else // determine n of prior tissue region
                {
                    var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                    return previousDataPoint.IsWithinNA(NA, Direction.AlongPositiveZAxis, detectorRegionN);
                }
            }
            // upward radiance
            if (currentRegionIndex == FinalTissueRegionIndex)
            {
                var detectorRegionN = _tissue.Regions[currentRegionIndex].RegionOP.N;
                return dataPoint.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
            else // determine n of prior tissue region
            {
                var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                return previousDataPoint.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
        }

    }
}
