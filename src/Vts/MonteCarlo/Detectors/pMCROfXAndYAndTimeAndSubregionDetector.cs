using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for pMC estimation of reflectance as a function of X, Y, Time and Subregion.
    /// Method tallies photon weight to time bin associated with pathlength in each region.
    /// Integrated R(x,y,t,subregion) will not integrate to R(x,y), independent array
    /// ROfXAndY used to determine this. Reference: Hiraoka93, Phys.Med.Biol.38 and
    /// Okada96, Appl. Opt. 35(19) -> the sum of the partial path lengths over all the
    /// medium is equivalent to the mean total path length (CH found this to be true)
    /// </summary>
    public class pMCROfXAndYAndTimeAndSubregionDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of x,y,time,tissue region detector input
        /// </summary>
        public pMCROfXAndYAndTimeAndSubregionDetectorInput()
        {
            TallyType = "pMCROfXAndYAndTimeAndSubregion";
            Name = "pMCROfXAndYAndTimeAndSubregion";
            X = new DoubleRange(-10, 10, 101);
            Y = new DoubleRange(-10, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IspMCReflectanceTally = true;
        }
        /// <summary>
        /// detector x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// detector y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// perturbed optical properties listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// perturbed regions indices
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
            return new pMCROfXAndYAndTimeAndSubregionDetector
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
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of XAndY and Time.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class pMCROfXAndYAndTimeAndSubregionDetector : Detector, IDetector
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private ITissue _tissue;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// detector x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// detector y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// total reflectance, needed to normalize partial differential path length
        /// </summary>
        public double[,] ROfXAndY { get; set; }        
        /// <summary>
        /// total reflectance 2nd moment, needed to normalize partial differential path length
        /// </summary>
        public double[,] ROfXAndYSecondMoment { get; set; }
        /// <summary>
        /// Number of tissue regions for serial/deserialization
        /// </summary>
        public int NumberOfRegions { get; set; }
        /// <summary>
        /// perturbed optical properties listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// perturbed regions indices
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
        public double[,,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] SecondMoment { get; set; }

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

            NumberOfRegions = tissue.Regions.Count;
            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1,Time.Count - 1,NumberOfRegions];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1,Time.Count - 1,NumberOfRegions] : null);
            ROfXAndY = ROfXAndY ?? new double[X.Count - 1, Y.Count - 1];
            ROfXAndYSecondMoment = ROfXAndYSecondMoment ?? new double[X.Count - 1, Y.Count - 1];

            // initialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _tissue = tissue;
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon)) return;

            // WhichBin to match ROfXAndYAndTimeDetector
            var ix = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(photon.DP.Position.Y, Y.Count - 1, Y.Delta, Y.Start);

            // determine path length in each tissue region
            var pathLengthInRegion = photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray();

            if (ix == -1 || iy == -1) return;
            {
                var weightFactor = _absorbAction(
                    photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps, _referenceOps, _perturbedRegionsIndices);

                ROfXAndY[ix, iy] += photon.DP.Weight * weightFactor;
                if (TallySecondMoment)
                {
                    ROfXAndYSecondMoment[ix, iy] += photon.DP.Weight * weightFactor *
                                                    photon.DP.Weight * weightFactor;
                }

                for (var ir = 0; ir < NumberOfRegions; ir++)
                {
                    var timeInRegion = pathLengthInRegion[ir] / (GlobalConstants.C / _tissue.Regions[ir].RegionOP.N);
                    // determine time bin based on individual region
                    var it = DetectorBinning.WhichBin(timeInRegion, Time.Count - 1, Time.Delta, Time.Start);
                    if (timeInRegion <= 0.0) continue; // only tally if path length in region
                    Mean[ix, iy, it, ir] += photon.DP.Weight * weightFactor;
                    if (!TallySecondMoment) continue;
                    SecondMoment[ix, iy, it, ir] += photon.DP.Weight * weightFactor *
                                                    photon.DP.Weight * weightFactor;
                }

                TallyCount++;
            }
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Time.Delta;
            for (var ix = 0; ix < X.Count - 1; ix++)
            {
                for (var iy = 0; iy < Y.Count - 1; iy++)
                {
                    ROfXAndY[ix, iy] /= X.Delta * Y.Delta * numPhotons;
                    ROfXAndYSecondMoment[ix, iy] /= X.Delta * Y.Delta * X.Delta * Y.Delta * numPhotons;
                    for (var it = 0; it < Time.Count - 1; it++)
                    {
                        for (var ir = 0; ir < NumberOfRegions; ir++)
                        {
                            var areaNorm = normalizationFactor;
                            Mean[ix, iy, it, ir] /= areaNorm * numPhotons;
                            if (!TallySecondMoment) continue;
                            SecondMoment[ix, iy, it, ir] /= areaNorm * areaNorm * numPhotons;
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
            ROfXAndY ??= new double[X.Count - 1, Y.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[X.Count - 1, Y.Count - 1, Time.Count - 1, NumberOfRegions];
                ROfXAndYSecondMoment ??= new double[X.Count - 1, Y.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),
                BinaryArraySerializerFactory.GetSerializer(
                    ROfXAndY, "ROfXAndY", "_ROfXAndY"),
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    SecondMoment, "SecondMoment", "_2") : null,
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    ROfXAndYSecondMoment, "ROfXAndYSecondMoment", "_ROfXAndY_2") : null
            };
            return allSerializers.Where(s => s is not null).ToArray();
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
