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
    /// Implements IDetector&lt;double[,,]&gt;.  Tally for Radiance(rho,z,angle).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    [KnownType(typeof(RadianceOfRhoAndZAndAngleDetector))]
    public class RadianceOfRhoAndZAndAngleDetector : IHistoryDetector<double[,,]>
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;

        ///<summary>
        /// Returns an instance of RadianceOfRhoAndZAndAngleDetector
        ///</summary>
        ///<param name="rho"></param>
        ///<param name="z"></param>
        ///<param name="angle"></param>
        ///<param name="tissue"></param>
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
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetAbsorptionWeightingMethod(tissue, this);
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

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Z { get; set; }

        public DoubleRange Angle { get; set; }

        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }
        }

        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            // using Acos, -1<Uz<1 goes to pi<theta<0, so first bin is most forward directed angle
            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Angle.Count - 1, Angle.Delta, Angle.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

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