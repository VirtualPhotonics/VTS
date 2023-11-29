using System;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Extensions;
using System.Collections.Generic;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for transmittance as a function of X and Y and Time and Subregion.
    /// This works for Analog, DAW and CAW processing.
    /// Method tallies photon weight to time bin associated with pathlength in each region.
    /// Integrated T(x,y,t,subregion) will not integrate to T(x,y), independent array
    /// TOfXAndY used to determine this. Reference: Hiraoka93, Phys.Med.Biol.38
    /// </summary>
    public class TOfXAndYAndTimeAndSubregionDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for transmittance as a function of X and Y detector input
        /// </summary>
        public TOfXAndYAndTimeAndSubregionDetectorInput()
        {
            TallyType = "TOfXAndYAndTimeAndSubregion";
            Name = "TOfXAndYAndTimeAndSubregion";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 2; // assume detector is in air below tissue

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsTransmittanceTally = true;
        }

        /// <summary>
        /// X binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// Y binning
        /// </summary>
        public DoubleRange Y { get; set; }      
        /// <summary>
        /// Time binning
        /// </summary>
        public DoubleRange Time { get; set; }
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
            return new TOfXAndYAndTimeAndSubregionDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                Time = this.Time,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for transmittance as a function  of X and Y and TimeAndSubregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TOfXAndYAndTimeAndSubregionDetector : Detector, IDetector
    {
        private ITissue _tissue; // need _tissue for region OPs

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// X binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// Y binning
        /// </summary>
        public DoubleRange Y { get; set; }        
        /// <summary>
        /// Time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// Number of tissue regions for serial/deserialization
        /// </summary>
        public int NumberOfRegions { get; set; }
        /// <summary>
        /// total transmittance, needed to normalize partial differential path length
        /// </summary>
        public double[,] TOfXAndY { get; set; }
        /// <summary>
        /// total transmittance 2nd moment, needed to normalize partial differential path length
        /// </summary>
        public double[,] TOfXAndYSecondMoment { get; set; }
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
        public double[,,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Ys detector gets tallied to
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

            NumberOfRegions = tissue.Regions.Count;
            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1,Time.Count - 1,NumberOfRegions];
            SecondMoment = SecondMoment ?? (
                TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, Time.Count - 1, tissue.Regions.Count] : null);
            TOfXAndY = TOfXAndY ?? new double[X.Count - 1, Y.Count - 1];
            TOfXAndYSecondMoment = TOfXAndYSecondMoment ?? new double[X.Count - 1, Y.Count - 1];

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

            var ix = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(photon.DP.Position.Y, Y.Count - 1, Y.Delta, Y.Start);
            TOfXAndY[ix, iy] += photon.DP.Weight;
            TOfXAndYSecondMoment[ix, iy] += photon.DP.Weight * photon.DP.Weight;

            // determine path length in each tissue region
            var pathLengthInRegion = photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray();

            for (var ir = 0; ir < NumberOfRegions; ir++)
            {
                var timeInRegion = pathLengthInRegion[ir] / (GlobalConstants.C / _tissue.Regions[ir].RegionOP.N);
                if (timeInRegion <= 0.0) continue; // only tally if path length in region

                // determine time bin based on individual region
                var it = DetectorBinning.WhichBin(timeInRegion, Time.Count - 1, Time.Delta, Time.Start);

                Mean[ix, iy, it, ir] += photon.DP.Weight;
                if (!TallySecondMoment) continue;
                SecondMoment[ix, iy, it, ir] += photon.DP.Weight * photon.DP.Weight;
            }

            TallyCount++;
        }
        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Time.Delta;
            for (var ix = 0; ix < X.Count - 1; ix++)
            {
                for (var iy = 0; iy < Y.Count - 1; iy++)
                {
                    TOfXAndY[ix, iy] /= X.Delta * Y.Delta * numPhotons;
                    TOfXAndYSecondMoment[ix, iy] /= X.Delta * Y.Delta * X.Delta * Y.Delta * numPhotons;
                    for (var it = 0; it < Time.Count - 1; it++)
                    {
                        for (var ir = 0; ir < NumberOfRegions; ir++)
                        {
                            Mean[ix, iy, it, ir] /= normalizationFactor * numPhotons;
                            if (TallySecondMoment)
                            {
                                SecondMoment[ix, iy, it, ir] /= normalizationFactor * normalizationFactor * numPhotons;
                            }
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
            Mean ??= new double[X.Count - 1, Y.Count - 1, Time.Count - 1, NumberOfRegions];
            TOfXAndY ??= new double[X.Count - 1, Y.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[X.Count - 1, Y.Count - 1, Time.Count - 1, NumberOfRegions];
                TOfXAndYSecondMoment ??= new double[X.Count - 1, Y.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),
                BinaryArraySerializerFactory.GetSerializer(
                    TOfXAndY, "TOfXAndY", "_TOfXAndY"),
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    SecondMoment, "SecondMoment", "_2") : null,
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    TOfXAndYSecondMoment, "TOfXAndYSecondMoment", "_TOfXAndY_2") : null
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
                return photon.DP.IsWithinNA(NA, Direction.AlongPositiveZAxis, detectorRegionN);
            }
            else // determine n of prior tissue region
            {
                var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                return photon.History.PreviousDP.IsWithinNA(NA, Direction.AlongPositiveZAxis, detectorRegionN);
            }
        }

    }
}
