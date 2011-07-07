using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(DosimetryOfRhoDetector))]
    /// <summary>
    /// Implements IHistoryTally<double[]>.  Tally for dosimetry as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    /// 
    
    public class DosimetryOfRhoDetector : IHistoryDetector<double[]>
    {
        private Func<double, PhotonDataPoint, double> _absorbAction;
        
        private bool _tallySecondMoment;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private double _xIntercept;
        private double _yIntercept;

        /// <summary>
        /// Returns an instance of DosimetryOfRhoDetector
        /// </summary>
        /// <param name="rho"></param>
        public DosimetryOfRhoDetector(
            double zDepth, 
            DoubleRange rho, 
            ITissue tissue,
            bool tallySecondMoment, 
            String name)
        {
            Rho = rho;
            ZDepth = zDepth;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = TallyType.DosimetryOfRho;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            SetAbsorbAction(_tissue.AbsorptionWeightingType);
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        ///  Returns a default instance of DosimetryOfRhoDetector (for serialization purposes only)
        /// </summary>
        public DosimetryOfRhoDetector()
            : this(10.0, new DoubleRange(), new MultiLayerTissue(), true, TallyType.DosimetryOfRho.ToString())
        {
        }

        [IgnoreDataMember]
        public double[] Mean { get; set; }

        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public double ZDepth { get; set; }

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
                    _absorbAction = AbsorbDiscrete;
                    break;
                default:
                    throw new ArgumentException("AbsorptionWeightingType not set");
            }
        }
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            // check if two DPs pass through ZDepth plane in downward direction
            if ((previousDP.Position.Z < ZDepth) && (dp.Position.Z > ZDepth))
            {
                // determine x,y intercept with plane
                 var s = (ZDepth - previousDP.Position.Z) / (dp.Position.Z - previousDP.Position.Z);
                _xIntercept = previousDP.Position.X + (dp.Position.X - previousDP.Position.X) * s;
                _yIntercept = previousDP.Position.Y = (dp.Position.Y - previousDP.Position.Y) * s;

                // update weight
                var weight = _absorbAction(
                    _ops[_tissue.GetRegionIndex(dp.Position)].Mua,
                    previousDP);

                var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(_xIntercept, _yIntercept), Rho.Count - 1, Rho.Delta, Rho.Start);
                
                // update tally
                Mean[ir] += weight;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] += weight * weight;
                }
                TallyCount++;
            }

        }

        private double AbsorbAnalog(double mua, PhotonDataPoint previousDP)
        {
            return previousDP.Weight;
        }

        private double AbsorbDiscrete(double mua, PhotonDataPoint previousDP)
        {
            return previousDP.Weight;
        }

        private double AbsorbContinuous(double mua, PhotonDataPoint previousDP)
        {
            // determine length of track portion from previousDP to intersection with ZDepth        
            var partialTrackLength = Math.Sqrt(
                (_xIntercept - previousDP.Position.X) * (_xIntercept - previousDP.Position.X) +
                (_yIntercept - previousDP.Position.Y) * (_yIntercept - previousDP.Position.Y) +
                (ZDepth - previousDP.Position.Z) * (ZDepth - previousDP.Position.Z));
            return previousDP.Weight * Math.Exp(-mua * partialTrackLength);
        }
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (ir + 0.5) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }

    }
}
