using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for pMC estimation of reflectance as a function of X, Y, Time and Subregion
    /// recessed in air.
    /// Method tallies photon weight to time bin associated with pathlength in each region.
    /// Integrated R(rho,t,subregion) will not integrate to R(x,y), independent array
    /// ROfRho used to determine this. Reference: Hiraoka93, Phys.Med.Biol.38 and
    /// Okada96, Appl. Opt. 35(19) -> the sum of the partial path lengths over all the
    /// medium is equivalent to the mean total path length (CH found this to be true)
    /// </summary>
    public class pMCROfRhoAndTimeAndSubregionRecessedDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of x,y,time,tissue region detector input
        /// </summary>
        public pMCROfRhoAndTimeAndSubregionRecessedDetectorInput()
        {
            TallyType = "pMCROfRhoAndTimeAndSubregionRecessed";
            Name = "pMCROfRhoAndTimeAndSubregionRecessed";
            Rho = new DoubleRange(0, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            ZPlane = -1.0;
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IspMCReflectanceTally = true;
        }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// z-plane above tissue in air
        /// </summary>
        public double ZPlane { get; set; }
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
            return new pMCROfRhoAndTimeAndSubregionRecessedDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                Time = this.Time,
                ZPlane = this.ZPlane,
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of Rho and Time.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class pMCROfRhoAndTimeAndSubregionRecessedDetector : Detector, IDetector
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private ITissue _tissue;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// z-plane above tissue in air
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// total reflectance, needed to normalize partial differential path length
        /// </summary>
        public double[] ROfRho { get; set; }
        /// <summary>
        /// total reflectance 2nd moment, needed to normalize partial differential path length
        /// </summary>
        public double[] ROfRhoSecondMoment { get; set; }
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

            NumberOfRegions = tissue.Regions.Count;
            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[Rho.Count - 1, Time.Count - 1, NumberOfRegions];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1, Time.Count - 1, NumberOfRegions] : null);
            ROfRho = ROfRho ?? new double[Rho.Count - 1];
            ROfRhoSecondMoment = ROfRhoSecondMoment ?? new double[Rho.Count - 1];

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

            // WhichBin to match ROfRhoAndTimeDetector
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            // determine path length in each tissue region
            var pathLengthInRegion = photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray();

            var weightFactor = _absorbAction(
                    photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps, _referenceOps, _perturbedRegionsIndices);

            ROfRho[ir] += photon.DP.Weight * weightFactor;
            if (TallySecondMoment)
            {
                ROfRhoSecondMoment[ir] += photon.DP.Weight * weightFactor *
                                                photon.DP.Weight * weightFactor;
            }

            for (var ig = 0; ig < NumberOfRegions; ig++)
            {
                var timeInRegion = pathLengthInRegion[ig] / (GlobalConstants.C / _tissue.Regions[ig].RegionOP.N);
                // determine time bin based on individual region
                var it = DetectorBinning.WhichBin(timeInRegion, Time.Count - 1, Time.Delta, Time.Start);
                if (timeInRegion <= 0.0) continue; // only tally if path length in region
                Mean[ir, it, ig] += photon.DP.Weight * weightFactor;
                if (!TallySecondMoment) continue;
                SecondMoment[ir, it, ig] += photon.DP.Weight * weightFactor *
                                                photon.DP.Weight * weightFactor;
            }

            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Time.Delta;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                ROfRho[ir] /= areaNorm * numPhotons;
                ROfRhoSecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                for (var it = 0; it < Time.Count - 1; it++)
                {
                    for (var ig = 0; ig < NumberOfRegions; ig++)
                    {
                        Mean[ir, it, ig] /= areaNorm * numPhotons;
                        if (TallySecondMoment)
                        {
                            SecondMoment[ir, it, ig] /= areaNorm * areaNorm * numPhotons;
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
            Mean ??= new double[Rho.Count - 1, Time.Count - 1, NumberOfRegions];
            ROfRho ??= new double[Rho.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[Rho.Count - 1, Time.Count - 1, NumberOfRegions];
                ROfRhoSecondMoment ??= new double[Rho.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),
                BinaryArraySerializerFactory.GetSerializer(
                    ROfRho, "ROfRho", "_ROfRho"),
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    SecondMoment, "SecondMoment", "_2") : null,
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    ROfRhoSecondMoment, "ROfRhoSecondMoment", "_ROfRho_2") : null
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
