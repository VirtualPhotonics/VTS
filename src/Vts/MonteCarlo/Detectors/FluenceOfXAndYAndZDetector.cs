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
    /// Implements IHistoryDetector&lt;double[,,]&gt;.  Tally for Fluence(x,y,z).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    [KnownType(typeof(FluenceOfXAndYAndZDetector))]
    public class FluenceOfXAndYAndZDetector : IHistoryDetector<double[, ,]>
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<OpticalProperties> _ops;
        /// <summary>
        /// constructor for fluence as a function of x, y and z detector input
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="z">z binning</param>
        /// <param name="tissue">tissue definition</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public FluenceOfXAndYAndZDetector(
            DoubleRange x, 
            DoubleRange y, 
            DoubleRange z, 
            ITissue tissue,
            bool tallySecondMoment,
            String name
            )
        {
            X = x;
            Y = y;
            Z = z;
            Mean = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            }
            TallyType = TallyType.FluenceOfXAndYAndZ;
            Name = name;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);

            TallyCount = 0;
            _tissue = tissue;
            _ops = tissue.Regions.Select(r => r.RegionOP).ToArray();
        }

        /// <summary>
        /// Returns an instance of FluenceOfXAndYAndZDetector (for serialization purposes only)
        /// </summary>
        public FluenceOfXAndYAndZDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            new DoubleRange(), 
            new MultiLayerTissue(), 
            true,
            TallyType.FluenceOfXAndYAndZ.ToString())
        {
        }
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of zs detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ix = DetectorBinning.WhichBin(dp.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(dp.Position.Y, Y.Count - 1, Y.Delta, Y.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            var regionIndex = currentRegionIndex;

            if (weight != 0.0)
            {
                Mean[ix, iy, iz] += weight / _ops[regionIndex].Mua;
                if (_tallySecondMoment)
                {
                    SecondMoment[ix, iy, iz] += (weight / _ops[regionIndex].Mua) * (weight / _ops[regionIndex].Mua);
                }
                TallyCount++;
            }
        }
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
        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Z.Delta;
            for (int ix = 0; ix < X.Count - 1; ix++)
            {
                for (int iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (int iz = 0; iz < Z.Count - 1; iz++)
                    {
                        Mean[ix, iy, iz] /= normalizationFactor * numPhotons;
                        if (_tallySecondMoment)
                        {
                            SecondMoment[ix, iy, iz] /= normalizationFactor * normalizationFactor * numPhotons;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// method to determine if photon within detector, i.e. in NA, etc.
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }

    }
}