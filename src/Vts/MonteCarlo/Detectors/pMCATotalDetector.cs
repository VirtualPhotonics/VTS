using System;
using System.Collections.Generic;
using System.Linq;
using Vts.IO;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for pMC estimation of total absorption currently *only for CAW*
    /// </summary>
    public class pMCATotalDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for pMC reflectance as a function of rho detector input
        /// </summary>
        public pMCATotalDetectorInput()
        {
            TallyType = "pMCATotal";
            Name = "pMCATotal";

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
        } 
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new pMCATotalDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
             };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC total absorption
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class pMCATotalDetector : Detector, IDetector
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;
           
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */ 
                                                                                                                      
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }
    
        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        public double SecondMoment { get; set; }

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
            Mean = new double();
            if (TallySecondMoment)
            {
                SecondMoment = new double();
            }

            // initialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            // perturb Ops of tissue then can determine weight as usual
            _absorbAction = AbsorptionWeightingMethods.GetpMCVolumeAbsorptionWeightingMethod(tissue, this);
         }
        /// <summary>
        /// method to tally to detector 
        /// </summary>
        /// <param name="photon">photon to be tallied</param>
        public void Tally(Photon photon)
        {
            // first determine perturbed *reflected* weight
            var weightFactor = _absorbAction(
                photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                _perturbedOps, _referenceOps, _perturbedRegionsIndices);
            // absorbed energy is 1 - perturbed reflected weight
            var weight = 1 - (photon.DP.Weight * weightFactor);

            Mean += weight;
            if (!TallySecondMoment) return;
            SecondMoment += weight * weight;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons are launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
            if (!TallySecondMoment) return;
            SecondMoment /= numPhotons;
        }

        /// <summary>
        /// this scalar tally is saved to json
        /// </summary>
        /// <returns>empty array of BinaryArraySerializer</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return Array.Empty<BinaryArraySerializer>();
        }

        /// <summary>
        /// Method to determine if photon is within detector NA not applicable for ATotal
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>method always returns true</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            return true; // or, possibly test for NA or confined position, etc
        }
    }
}
