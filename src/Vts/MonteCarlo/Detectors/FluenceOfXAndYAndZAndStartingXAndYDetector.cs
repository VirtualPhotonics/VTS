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
    /// DetectorInput for Fluence(x,y,z) and starting (x,y).  This detector
    /// captures Fluence(x,y,z) with respect to the initial tissue starting 
    /// location (x,y).  It was written for adjoint fluorescence simulations.
    /// </summary>
    public class FluenceOfXAndYAndZAndStartingXAndYDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of x, y and z detector input
        /// </summary>
        public FluenceOfXAndYAndZAndStartingXAndYDetectorInput()
        {
            TallyType = "FluenceOfXAndYAndZAndStartingXAndY";
            Name = "FluenceOfXAndYAndZAndStartingXAndY";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            Z = new DoubleRange(0.0, 10.0, 101);
            StartingX = new DoubleRange(-10.0, 10.0, 11);
            StartingY = new DoubleRange(-10.0, 10.0, 11);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
            TallyDetails.IsNotImplementedForCAW = true;
        }

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
        /// x binning
        /// </summary>
        public DoubleRange StartingX { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange StartingY { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new FluenceOfXAndYAndZAndStartingXAndYDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                Z = this.Z,
                StartingX = this.StartingX,
                StartingY = this.StartingY
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for Fluence(x,y,z) and starting (x,y).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    public class FluenceOfXAndYAndZAndStartingXAndYDetector : Detector, IHistoryDetector
    {
        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
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
        /// x binning
        /// </summary>
        public DoubleRange StartingX { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange StartingY { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember] public double[,,,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember] public double[,,,,] SecondMoment { get; set; }
        /// <summary>
        /// detector count of photons starting in each X-Y bin
        /// </summary>
        [IgnoreDataMember] public double[,] StartingXYCount { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private double[,,,,] _tallyForOnePhoton;
        private int _startingXBin, _startingYBin;

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
            Mean = Mean ?? new double[StartingX.Count - 1, StartingY.Count - 1,X.Count - 1, Y.Count - 1, Z.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[
                StartingX.Count - 1, StartingY.Count - 1, X.Count - 1, Y.Count - 1, Z.Count - 1] : null);
            StartingXYCount = StartingXYCount ?? new double[StartingX.Count - 1, StartingY.Count - 1];
            
            // initialize any other necessary class fields here
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();
            _tallyForOnePhoton = _tallyForOnePhoton ?? (TallySecondMoment ? new double[
                StartingX.Count - 1, StartingY.Count - 1, X.Count - 1, Y.Count - 1, Z.Count - 1] : null);
        }

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
            // Note: GetVolumeAbsorptionWeightingMethod in Initialize method determines the *absorbed* weight
            //  so for fluence this weight is divided by Mua

            var regionIndex = currentRegionIndex;

            if (weight != 0.0)
            {
                Mean[_startingXBin, _startingYBin, ix, iy, iz] += weight / _ops[regionIndex].Mua;
                if (TallySecondMoment)
                {
                    _tallyForOnePhoton[_startingXBin, _startingYBin, ix, iy, iz] += weight / _ops[regionIndex].Mua;
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
            // second moment is calculated AFTER the entire photon biography has been processed
            if (TallySecondMoment)
            {
                Array.Clear(_tallyForOnePhoton, 0, _tallyForOnePhoton.Length);
            }
            var previousDp = photon.History.HistoryData.First();
            var firstLocation = photon.History.HistoryData.First();

            _startingXBin = DetectorBinning.WhichBin(firstLocation.Position.X, StartingX.Count - 1, StartingX.Delta, StartingX.Start);
            _startingYBin = DetectorBinning.WhichBin(firstLocation.Position.Y, StartingY.Count - 1, StartingY.Delta, StartingY.Start);
            StartingXYCount[_startingXBin, _startingYBin] += 1;

            foreach (var dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDp, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDp = dp;
            }
            // second moment determined after all tallies to each detector bin for ONE photon has been complete
            if (!TallySecondMoment) return;
            for (var ix = 0; ix < X.Count - 1; ix++)
            {
                for (var iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (var iz = 0; iz < Z.Count - 1; iz++)
                    {
                        SecondMoment[_startingXBin, _startingYBin, ix, iy, iz] += 
                            _tallyForOnePhoton[_startingXBin, _startingYBin, ix, iy, iz]*
                            _tallyForOnePhoton[_startingXBin, _startingYBin, ix, iy, iz];
                    }
                }
            }
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// for this detector normalization is based on number launched in starting
        /// x,y bin
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Z.Delta;
            for (var isx = 0; isx < StartingX.Count - 1; isx++)
            {
                for (var isy = 0; isy < StartingY.Count - 1; isy++)
                {
                    for (var ix = 0; ix < X.Count - 1; ix++)
                    {
                        for (var iy = 0; iy < Y.Count - 1; iy++)
                        {
                            for (var iz = 0; iz < Z.Count - 1; iz++)
                            {
                                // check so no divide by 0
                                if (StartingXYCount[isx, isy] == 0)
                                {
                                    Mean[isx, isy, ix, iy, iz] = 0.0;
                                }
                                else
                                {
                                    Mean[isx, isy, ix, iy, iz] /=
                                        normalizationFactor * StartingXYCount[isx, isy];

                                    if (!TallySecondMoment) continue;
                                    SecondMoment[isx, isy, ix, iy, iz] /=
                                        normalizationFactor * normalizationFactor * StartingXYCount[isx, isy];
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            Mean ??= new double[X.Count - 1, Y.Count - 1, Z.Count - 1, StartingX.Count - 1, StartingY.Count - 1];
            StartingXYCount ??= new double[StartingX.Count - 1, StartingY.Count - 1];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[X.Count - 1, Y.Count - 1, Z.Count - 1, StartingX.Count - 1, StartingY.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),                
                BinaryArraySerializerFactory.GetSerializer(
                    StartingXYCount, "StartingXYCount", "_StartingXYCount"),
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    SecondMoment, "SecondMoment", "_2") : null
            };
            return allSerializers.Where(s => s is not null).ToArray();
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