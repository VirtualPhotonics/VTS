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
    /// <summary>
    /// Implements IDetector&lt;double&gt;.  Tally for Total Absorption.
    /// </summary>
    [KnownType(typeof(ATotalDetector))]
    public class ATotalDetector : IDetector<double> 
    {
        private Func<double, double, double, double, PhotonStateType, double> _absorbAction;

        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;
        private double _fullTrackLength;
        /// <summary>
        /// Returns am instance of ATotalDetector
        /// </summary>
        /// <param name="tissue"></param>
        public ATotalDetector(ITissue tissue, bool tallySecondMoment, String name)
        {
            TallyType = TallyType.ATotal;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            _tallySecondMoment = tallySecondMoment;
            SetAbsorbAction(_tissue.AbsorptionWeightingType);
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        /// Returns a default instance of ATotalDetector (for serialization purposes only)
        /// </summary>
        public ATotalDetector()
            : this(new MultiLayerTissue(), true, TallyType.ATotal.ToString())
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
        public void Tally(Photon photon)
        {
            //// todo: is this logically consistent at any place that could call Tally(photon)?
            Tally(photon.History.PreviousDP, photon.History.CurrentDP);
            //PhotonDataPoint previousDP = photon.History.HistoryData.First();
            //foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            //{
            //    Tally(previousDP, dp);
            //    previousDP = dp;
            //}
        }
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            var weight = _absorbAction(
                _ops[_tissue.GetRegionIndex(dp.Position)].Mua, 
                _ops[_tissue.GetRegionIndex(dp.Position)].Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            if (weight != 0.0)
            {
                Mean += weight;
                if (_tallySecondMoment)
                {
                    SecondMoment += weight * weight;
                }
                TallyCount++;
            }
        }
        
        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
            if (_tallySecondMoment)
            {
                SecondMoment /= numPhotons;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
         
        // to get variance correct, all of the following tallies have to tally at end of biography
        private double AbsorbAnalog(double mua, double mus, double previousWeight, double weight, 
            PhotonStateType photonStateType)
        {
            weight = 0.0; // if not absorbed, no weight tallied
            if (photonStateType.HasFlag(PhotonStateType.Absorbed)) // tally only at end of biography
            {
                weight = 1.0; // ref: my dissertation eq. (2.75)
            }
            return weight;
        }

        private double AbsorbDiscrete(double mua, double mus, double previousWeight, double weight, 
            PhotonStateType photonStateType)
        {
            // only tally if photon died
            if (photonStateType.HasFlag(PhotonStateType.Alive))
            {
                weight = 0.0;
            }
            else
            {
                weight = 1 - weight; // 1 - surviving weight = absorbed weight
            }
            return weight;
        }
        
        private double AbsorbContinuous(double mua, double mus, double previousWeight, double weight,
            PhotonStateType photonStateType)
        {
            // only tally if photon died
            if (photonStateType.HasFlag(PhotonStateType.Alive))
            {
                weight = 0.0;
            }
            else
            {
                weight = 1.0 - weight; // this is the absorbed weight for entire biography
            }
            return weight;
        }
    }
}