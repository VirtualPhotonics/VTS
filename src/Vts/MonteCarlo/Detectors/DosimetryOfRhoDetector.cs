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
    /// Implements ISurfaceTally<double[]>.  Tally for dosimetry as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    /// 
    
    public class DosimetryOfRhoDetector : ISurfaceDetector<double[]>
    {
        private Func<PhotonDataPoint, double> _absorbAction;
        
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
        public void Tally(PhotonDataPoint dp)
        {
            // update weight
            var weight = _absorbAction(
                dp);

            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
                
            // update tally
            Mean[ir] += weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir] += weight * weight;
            }
            TallyCount++;
        }
         
        // need to check following Absorb actions
        private double AbsorbAnalog(PhotonDataPoint dp)
        {
            return dp.Weight;
        }

        private double AbsorbDiscrete(PhotonDataPoint dp)
        {
            return dp.Weight;
        }

        private double AbsorbContinuous(PhotonDataPoint dp)
        {
            return dp.Weight;
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
