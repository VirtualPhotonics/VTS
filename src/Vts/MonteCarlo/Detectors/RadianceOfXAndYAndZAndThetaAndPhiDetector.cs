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
    /// DetectorInput for Radiance(x,y,z,theta,phi)
    /// </summary>
    public class RadianceOfXAndYAndZAndThetaAndPhiDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for Radiance as a function of x, y, z, theta and phi detector input
        /// </summary>
        public RadianceOfXAndYAndZAndThetaAndPhiDetectorInput()
        {
            TallyType = "RadianceOfXAndYAndZAndThetaAndPhi";
            Name = "RadianceOfXAndYAndZAndThetaAndPhi";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            Z = new DoubleRange(0.0, 10.0, 101);
            Theta = new DoubleRange(0.0, Math.PI, 5); // theta (polar angle)
            Phi = new DoubleRange(-Math.PI, Math.PI, 5); // phi (azimuthal angle)

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsVolumeTally = true;
            TallyDetails.IsNotImplementedForCAW = true;
        }

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }

        /// <summary>
        /// detector name
        /// </summary>
        public string Name { get; set; }

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
        /// theta binning
        /// </summary>
        public DoubleRange Theta { get; set; }
        /// <summary>
        /// phi binning
        /// </summary>
        public DoubleRange Phi { get; set; }

        public IDetector CreateDetector()
        {
            return new RadianceOfXAndYAndZAndThetaAndPhiDetector
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
                Theta = this.Theta,
                Phi = this.Phi
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for Radiance(x,y,z).
    /// Note: this tally currently only works with discrete absorption weighting and analog
    /// </summary>
    public class RadianceOfXAndYAndZAndThetaAndPhiDetector : Detector, IHistoryDetector
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;
        private double[,,,,] _tallyForOnePhoton;

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
        /// theta binning
        /// </summary>
        public DoubleRange Theta { get; set; }
        /// <summary>
        /// phi binning
        /// </summary>
        public DoubleRange Phi { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[, , , ,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary> 
        [IgnoreDataMember]
        public double[, , , ,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        public void Initialize(ITissue tissue)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count - 1, Phi.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count -1, Phi.Count - 1] : null);

            // intialize any other necessary class fields here
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, this);
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();
            _tallyForOnePhoton = _tallyForOnePhoton ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count-1, Phi.Count-1] : null);
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
            // using Acos, -1<Uz<1 goes to pi<theta<0, so first bin is most forward directed angle
            var it = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), Theta.Count - 1, Theta.Delta, Theta.Start);
            var ip = DetectorBinning.WhichBin(Math.Atan2(dp.Direction.Uy, dp.Direction.Ux), Phi.Count - 1, Phi.Delta,
                Phi.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            var regionIndex = currentRegionIndex;

            if (weight != 0.0)
            {
                Mean[ix, iy, iz, it, ip] += weight / _ops[regionIndex].Mua;
                if (TallySecondMoment)
                {
                    _tallyForOnePhoton[ix, iy, iz, it, ip] += weight / _ops[regionIndex].Mua;
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
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }            
            // second moment determined after all tallies to each detector bin for ONE photon has been complete
            if (TallySecondMoment)
            {
                for (int ix = 0; ix < X.Count - 1; ix++)
                {
                    for (int iy = 0; iy < Y.Count - 1; iy++)
                    {
                        for (int iz = 0; iz < Z.Count - 1; iz++)
                        {
                            for (int it = 0; it < Theta.Count; it++)
                            {
                                for (int ip = 0; ip < Phi.Count; ip++)
                                {
                                    SecondMoment[ix, iy, iz, it, ip] = _tallyForOnePhoton[ix, iy, iz, it, ip] * _tallyForOnePhoton[ix, iy, iz, it, ip];
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
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
                                var areaNorm = Math.Sin((it + 0.5)*Theta.Delta)*normalizationFactor;
                                Mean[ix, iy, iz, it, ip] /= areaNorm * numPhotons;
                                if (TallySecondMoment)
                                {
                                    SecondMoment[ix, iy, iz, it, ip] /= areaNorm * areaNorm * numPhotons;
                                }
                            }
                        }
                    }
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
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < Z.Count - 1; k++) {
                                    for (int l = 0; l < Theta.Count - 1; l++) {
                                        for (int m = 0; m < Phi.Count - 1; m++)
                                        {
                                            binaryWriter.Write(Mean[i, j, k, l, m]);
                                        }                                        
                                    }
                                }                               
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count - 1, Phi.Count - 1];
                        for (int i = 0; i <  X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < Z.Count - 1; k++) {
                                    for (int l = 0; l < Theta.Count - 1; l++) {
                                        for (int m = 0; m < Phi.Count - 1; m++)
                                        {
                                            Mean[i, j, k, l, m] = binaryReader.ReadDouble();
                                        }
                                        
                                    }
                                }                                
                            }
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
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < Z.Count - 1; k++) {
                                    for (int l = 0; l < Theta.Count - 1; l++) {
                                        for (int m = 0; m < Phi.Count - 1; m++)
                                        {
                                            binaryWriter.Write(SecondMoment[i, j, k, l, m]);
                                        }                                        
                                    }
                                }                                
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[X.Count - 1, Y.Count - 1, Z.Count - 1, Theta.Count - 1, Phi.Count - 1];
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < Z.Count - 1; k++) {
                                    for (int l = 0; l < Theta.Count - 1; l++) {
                                        for (int m = 0; m < Phi.Count - 1; m++)
                                        {
                                            SecondMoment[i, j, k, l, m] = binaryReader.ReadDouble();
                                        }                                       
                                    }
                                }                                
                            }
			            }
                    },
                },
            };
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