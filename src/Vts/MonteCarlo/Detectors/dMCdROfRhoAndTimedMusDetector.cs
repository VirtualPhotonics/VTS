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
    /// Tally for dMC estimation of the derivative or reflectance with respect to Mus as a function of Rho.
    /// </summary>
    public class dMCdROfRhoAndTimedMusDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for pMC reflectance as a function of rho detector input
        /// </summary>
        public dMCdROfRhoAndTimedMusDetectorInput()
        {
            TallyType = "dMCdROfRhoAndTimedMus";
            Name = "dMCdROfRhoAndTimedMus";
            Rho = new DoubleRange(0.0, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsCylindricalTally = true;
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
            return new dMCdROfRhoAndTimedMusDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = TallyType,
                Name = Name,
                TallySecondMoment = TallySecondMoment,
                TallyDetails = TallyDetails,

                // optional/custom detector-specific properties
                Rho = Rho,
                Time = Time,
                PerturbedOps = PerturbedOps,
                PerturbedRegionsIndices = PerturbedRegionsIndices,
                NA = NA,
                FinalTissueRegionIndex = FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of Rho.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class dMCdROfRhoAndTimedMusDetector : Detector, IDetector
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices; 
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
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
        public double[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// Method to initialize detector
        /// </summary>
        /// <param name="tissue">ITissue implementation</param>
        /// <param name="rng">random number generator</param>
        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean ??= new double[Rho.Count - 1, Time.Count - 1];
            SecondMoment ??= TallySecondMoment ? new double[Rho.Count - 1, Time.Count - 1] : null;

            // initialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _tissue = tissue;
            _absorbAction = AbsorptionWeightingMethods.GetdMCTerminationAbsorptionWeightingMethod(
                tissue.AbsorptionWeightingType, this, DifferentialMonteCarloType.DMus);
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon)) return;

            // WhichBin to match pMCROfRho which matches ROfRho
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var it = DetectorBinning.WhichBin(photon.DP.TotalTime, Time.Count - 1, Time.Delta, Time.Start);

            if (ir == -1 || it == -1) return;
            var weightFactor = _absorbAction(
                photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                _perturbedOps, _referenceOps, _perturbedRegionsIndices);

            Mean[ir, it] += photon.DP.Weight * weightFactor;
            TallyCount++;
            if (!TallySecondMoment) return;
            SecondMoment[ir, it] += photon.DP.Weight * weightFactor * photon.DP.Weight * weightFactor;
        }

        /// <summary>
        /// The following method works for both discrete or continuous absorption weighting
        /// </summary>
        /// <param name="numberOfCollisions">photon number of collisions in perturbed region</param>
        /// <param name="pathLength">photon path length in perturbed region</param>
        /// <param name="perturbedOps">perturbed optical properties of perturbed region</param>
        /// <returns>derivative with respect to mua</returns>
        private double AbsorbContinuousOrDiscrete(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            // NOTE: following code only works for single perturbed region because derivative of
            // Radon-Nikodym product needs d(AB)=dA B + A dB and this does not produce that
            // Check for only one perturbedRegionIndices specified by user performed in DataStructuresValidation
            var i = _perturbedRegionsIndices[0];
            // rearranged to be more numerically stable
            return (numberOfCollisions[i] / _perturbedOps[i].Mus - pathLength[i]) *
                    Math.Pow(_perturbedOps[i].Mus / _referenceOps[i].Mus *
                        Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua -
                                   _referenceOps[i].Mus - _referenceOps[i].Mua) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
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
                for (var it = 0; it < Time.Count - 1; it++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, it] /= areaNorm * numPhotons;
                    // the above is pi(rmax*rmax-rmin*rmin) * rhoDelta * N
                    if (!TallySecondMoment) continue;
                    SecondMoment[ir, it] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            Mean ??= new double[Rho.Count - 1, Time.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[Rho.Count - 1, Time.Count - 1];
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
