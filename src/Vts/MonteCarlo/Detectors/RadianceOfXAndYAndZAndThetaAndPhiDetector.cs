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
    /// <summary>
    /// Implements IHistoryDetector&lt;double[,,,,]&gt;.  Tally for Radiance(x,y,z,theta,phi).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    [KnownType(typeof(RadianceOfXAndYAndZAndThetaAndPhiDetector))]
    public class RadianceOfXAndYAndZAndThetaAndPhiDetector : IHistoryDetector<double[,,,,]>
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;

        /// <summary>
        /// constructor for radiance as a function of x, y, z, theta and phi detector input
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="z">z binning</param>
        /// <param name="theta">polar angle binning</param>
        /// <param name="phi">azimuthal angle binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public RadianceOfXAndYAndZAndThetaAndPhiDetector(
            DoubleRange x,
            DoubleRange y,
            DoubleRange z, 
            DoubleRange theta, 
            DoubleRange phi,
            ITissue tissue,
            bool tallySecondMoment,
            String name
            )
        {
            X = x;
            Y = y;
            Z = z;
            Theta = theta;
            Phi = phi;
            Mean = new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count - 1, Phi.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count - 1, Phi.Count - 1];
            }
            TallyType = TallyType.RadianceOfXAndYAndZAndThetaAndPhi;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// Returns an instance of RadianceOfRhoAndZAndAngleDetector (for serialization purposes only)
        /// </summary>
        public RadianceOfXAndYAndZAndThetaAndPhiDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            new DoubleRange(), 
            new DoubleRange(),
            new DoubleRange(), 
            new MultiLayerTissue(), 
            true,
            TallyType.RadianceOfRhoAndZAndAngle.ToString())
        {
        }

        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,,,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,,,] SecondMoment { get; set; }

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
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// theta (polar angle) binning
        /// </summary>
        public DoubleRange Theta { get; set; }
        /// <summary>
        /// phi (azimuthal angle) binning
        /// </summary>
        public DoubleRange Phi { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }
        }

        /// <summary>
        /// method to tally to detector given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous photon data point</param>
        /// <param name="dp">current photon data point</param>
        /// <param name="currentRegionIndex">index of region photon is currently in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ix = DetectorBinning.WhichBin(dp.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(dp.Position.Y, Y.Count - 1, Y.Delta, Y.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
            // using Acos, -1<Uz<1 goes to pi<theta<0, so first bin is most forward directed angle
            var it = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Theta.Count - 1, Theta.Delta, Theta.Start);
            var ip = DetectorBinning.WhichBin(Math.Atan2(dp.Direction.Uy, dp.Direction.Ux), Phi.Count - 1, Phi.Delta, Phi.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            var regionIndex = _tissue.GetRegionIndex(dp.Position);

            if (weight != 0.0) // if weight = 0.0, then pseudo-collision and no tally
            {
                Mean[ix, iy, iz, it, ip] += weight / _ops[regionIndex].Mua;
                if (_tallySecondMoment)
                {
                    SecondMoment[ix, iy, iz, it, ip] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
                }
                TallyCount++;
            }
        }

        /// <summary>
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">Number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Z.Delta * Theta.Delta * Phi.Delta;
            for (int ix = 0; ix < X.Count - 1; ix++)
            {
                for (int iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (int iz = 0; iz < Z.Count - 1; iz++)
                    {
                        for (int it = 0; it < Theta.Count - 1; it++)
                        {
                            for (int ip = 0; ip < Phi.Count - 1; ip++)
                            {
                                var areaNorm = Math.Sin((it + 0.5)  * Theta.Delta) * normalizationFactor;
                                Mean[ix, iy, iz, it, ip] /= areaNorm*numPhotons;
                                if (_tallySecondMoment)
                                {
                                    SecondMoment[ix, iy, iz, it, ip] /= areaNorm*areaNorm*numPhotons;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }

    }
}