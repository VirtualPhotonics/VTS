using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IVolumeDetector&lt;double[,,]&gt;.  Tally for Radiance(rho,z,angle).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    [KnownType(typeof(RadianceOfRhoAndZAndAngleDetector))]
    public class RadianceOfRhoAndZAndAngleDetector : IVolumeDetector<double[, ,]>
    {
        private Func<double, double, double, double, PhotonStateType, double> _absorbAction;

        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;
        /// <summary>
        /// constructor for radiance as a function of rho, z and angle detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="angle">angle binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public RadianceOfRhoAndZAndAngleDetector(
            DoubleRange rho, 
            DoubleRange z, 
            DoubleRange angle, // this is binned with respect to theta 
            ITissue tissue,
            bool tallySecondMoment,
            String name
            )
        {
            Rho = rho;
            Z = z;
            Angle = angle;
            Mean = new double[Rho.Count - 1, Z.Count - 1, Angle.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1, Angle.Count - 1];
            }
            TallyType = TallyType.RadianceOfRhoAndZAndAngle;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            SetAbsorbAction(_tissue.AbsorptionWeightingType);
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        /// Returns an instance of RadianceOfRhoAndZAndAngleDetector (for serialization purposes only)
        /// </summary>
        public RadianceOfRhoAndZAndAngleDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            new DoubleRange(), 
            new MultiLayerTissue(), 
            true,
            TallyType.RadianceOfRhoAndZAndAngle.ToString())
        {
        }

        [IgnoreDataMember]
        public double[, ,] Mean { get; set; }

        [IgnoreDataMember]
        public double[, ,] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }

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
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            // using Acos, -1<Uz<1 goes to pi<theta<0, so first bin is most forward directed angle
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            var weight = _absorbAction(
                _ops[_tissue.GetRegionIndex(dp.Position)].Mua,
                _ops[_tissue.GetRegionIndex(dp.Position)].Mus,
                previousDP.Weight,
                dp.Weight,
                dp.StateFlag);

            var regionIndex = _tissue.GetRegionIndex(dp.Position);

            if (weight != 0.0) // if weight = 0.0, then pseudo-collision and no tally
            {
                Mean[ir, iz, ia] += weight / _ops[regionIndex].Mua;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir, iz, ia] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
                }
                TallyCount++;
            }
        }

        private double AbsorbAnalog(double mua, double mus, double previousWeight, double weight, PhotonStateType photonStateType)
        {
            if (photonStateType.HasFlag(PhotonStateType.Absorbed))
            {
                weight = previousWeight; 
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

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta * 2.0 * Math.PI * Angle.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    for (int ia = 0; ia < Angle.Count - 1; ia++)
                    {
                        var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * Math.Sin((ia + 0.5) * Angle.Delta) * normalizationFactor;
                        Mean[ir, iz, ia] /= areaNorm * numPhotons;
                        if (_tallySecondMoment)
                        {
                            SecondMoment[ir, iz, ia] /= areaNorm * areaNorm * numPhotons;
                        }
                    }
                }
            }

        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }

    }
}