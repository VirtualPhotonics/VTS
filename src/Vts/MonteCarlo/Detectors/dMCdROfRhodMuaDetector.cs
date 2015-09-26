using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for dMC estimation of the derivative or reflectance with respect to mua as a function of Rho.
    /// </summary>
    public class dMCdROfRhodMuaDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for pMC reflectance as a function of rho detector input
        /// </summary>
        public dMCdROfRhodMuaDetectorInput()
        {
            TallyType = "dMCdROfRhodMua";
            Name = "dMCdROfRhodMua";
            Rho = new DoubleRange(0.0, 10, 101);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsCylindricalTally = true;
            TallyDetails.IspMCReflectanceTally = true;
        }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// list of perturbed OPs listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }

        public IDetector CreateDetector()
        {
            return new dMCdROfRhodMuaDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of Rho.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class dMCdROfRhodMuaDetector : Detector, IDetector
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private double _rhoDelta; // need to kep this because DoubleRange adjust deltas automatically
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, double> _absorbAction;
        private AbsorptionWeightingType _awt;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
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
        [IgnoreDataMember]
        public double[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[Rho.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1] : null);

            // intialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            SetAbsorbAction(tissue.AbsorptionWeightingType);
            TallyCount = 0;
            _awt = tissue.AbsorptionWeightingType;
        }

        /// <summary>
        /// Set the absorption to discrete or continuous
        /// </summary>
        /// <param name="awt">absorption weighting type</param>
        protected void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                // note: pMC is not applied to analog processing,
                // only DAW and CAW
                case AbsorptionWeightingType.Continuous:
                    _absorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    _absorbAction = AbsorbDiscrete;
                    break;
            }
        }
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var ir = DetectorBinning.WhichBinExclusive(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            if (ir != -1)
            {
                double weightFactor = _absorbAction(
                    photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps);

                Mean[ir] += photon.DP.Weight * weightFactor;
                if (TallySecondMoment)
                {
                    SecondMoment[ir] += photon.DP.Weight * weightFactor * photon.DP.Weight * weightFactor;
                }
                TallyCount++;
            }
        }

        private double AbsorbContinuous(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            // NOTE: following code only works for single perturbed region
            foreach (var i in _perturbedRegionsIndices)
            {
                weightFactor *=
                    -pathLength[i] * // dMua* factor
                    (Math.Exp(-perturbedOps[i].Mua * pathLength[i]) / Math.Exp(-_referenceOps[i].Mua * pathLength[i])); // mua pert
                if (numberOfCollisions[i] > 0)
                {
                    // the following is more numerically stable
                    Math.Pow(
                        (_perturbedOps[i].Mus / _referenceOps[i].Mus) * Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) *
                            pathLength[i] / numberOfCollisions[i]),
                        numberOfCollisions[i]);
                }
                else
                {
                    weightFactor *= Math.Exp(-(_perturbedOps[i].Mus - _referenceOps[i].Mus) * pathLength[i]);
                }
            }
            return weightFactor;
        }

        private double AbsorbDiscrete(IList<long> numberOfCollisions, IList<double> pathLength, IList<OpticalProperties> perturbedOps)
        {
            double weightFactor = 1.0;

            // NOTE: following code only works for single perturbed region
            foreach (var i in _perturbedRegionsIndices)
            {
                if (numberOfCollisions[i] > 0)
                {
                    weightFactor *=
                        -pathLength[i] * // dMua* factor
                        Math.Pow(
                            (_perturbedOps[i].Mus / _referenceOps[i].Mus) *
                                Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                                    pathLength[i] / numberOfCollisions[i]),
                            numberOfCollisions[i]);
                }
                else // numberOfCollisions[i] in pert region is 0
                {
                    weightFactor *=
                        -pathLength[i] * // dMua* factor
                                Math.Exp(-(_perturbedOps[i].Mus + _perturbedOps[i].Mua - _referenceOps[i].Mus - _referenceOps[i].Mua) *
                                    pathLength[i]);
                }
            }
            return weightFactor;
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                // the above is pi(rmax*rmax-rmin*rmin) * rhoDelta * N
                if (TallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        // this is to allow saving of large arrays separately as a binary file
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            binaryWriter.Write(Mean[i]);
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ Rho.Count - 1];
                        for (int i = 0; i <  Rho.Count - 1; i++) {
                            Mean[i] = binaryReader.ReadDouble();
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
                            binaryWriter.Write(SecondMoment[i]);
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ Rho.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            SecondMoment[i] = binaryReader.ReadDouble();
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
            // return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
