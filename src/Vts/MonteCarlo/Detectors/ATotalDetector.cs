using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(ATotalDetector))]
    /// <summary>
    /// Implements IHistoryDetector<double[,]>.  Tally for Absorption(rho,z).
    /// </summary>
    public class ATotalDetector : IHistoryDetector<double>
    {
        private Func<double, double, double, double, PhotonStateType, double> _absorbAction;

        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        /// <summary>
        /// Returns am instance of ATotalDetector
        /// </summary>
        /// <param name="tissue"></param>
        public ATotalDetector(ITissue tissue, String name)
        {
            TallyType = TallyType.ATotal;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            SetAbsorbAction(_tissue.AbsorptionWeightingType);
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        /// Returns a default instance of ATotalDetector (for serialization purposes only)
        /// </summary>
        public ATotalDetector()
            : this(new MultiLayerTissue(), TallyType.ATotal.ToString())
        {
        }

        public double Mean { get; set; }

        public double SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        private void SetAbsorbAction(AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                case AbsorptionWeightingType.Analog:
                    _absorbAction = AbsorbAnalog;
                    break;
                case AbsorptionWeightingType.Continuous:
                    _absorbAction = AbsorbContinuous;
                    break;
                case AbsorptionWeightingType.Discrete:
                default:
                    _absorbAction = AbsorbDiscrete;
                    break;
            }
        }
 
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            var weight = _absorbAction(
                _ops[_tissue.GetRegionIndex(dp.Position)].Mua, 
                _ops[_tissue.GetRegionIndex(dp.Position)].Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            Mean += weight; 
            SecondMoment += weight * weight;
            TallyCount++;
        }
        
        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }

        private double AbsorbAnalog(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (photonStateType.Has(PhotonStateType.Absorbed))
            {
                weight = previousWeight * mua / (mua + mus);
            }
            else
            {
                weight = 0.0;
            }
            return weight;
        }

        private double AbsorbDiscrete(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (previousWeight == weight) // pseudo collision, so no tally
            {
                weight = 0.0;
            }
            else
            {
                weight = previousWeight * mua / (mua + mus);
            }
            return weight;
        }
        
        private double AbsorbContinuous(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            throw new NotImplementedException();
        }
    }
}