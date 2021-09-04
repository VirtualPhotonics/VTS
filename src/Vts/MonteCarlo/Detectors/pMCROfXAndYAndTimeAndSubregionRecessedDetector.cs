using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for pMC estimation of reflectance as a function of X, Y, Time and Subregion
    /// recessed in air.
    /// </summary>
    public class pMCROfXAndYAndTimeAndSubregionRecessedDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of x,y,time,tissue region detector input
        /// </summary>
        public pMCROfXAndYAndTimeAndSubregionRecessedDetectorInput()
        {
            TallyType = "pMCROfXAndYAndTimeAndSubregionRecessed";
            Name = "pMCROfXAndYAndTimeAndSubregionRecessed";
            X = new DoubleRange(-10, 10, 101);
            Y = new DoubleRange(-10, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);
            ZPlane = -1.0;
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IspMCReflectanceTally = true;
        }
        /// <summary>
        /// detector x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// detector y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// z-plane above tissue in air
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// perturbed optical properties listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// numerical aperture
        /// </summary>
        public double NA { get; set; }
        /// <summary>
        /// method to create detector from input
        /// </summary>
        /// <returns>IDetector</returns>
        public IDetector CreateDetector()
        {
            return new pMCROfXAndYAndTimeAndSubregionRecessedDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                Time = this.Time,
                ZPlane = this.ZPlane,
                PerturbedOps = this.PerturbedOps,
                PerturbedRegionsIndices = this.PerturbedRegionsIndices,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }
    /// <summary>
    /// Implements IDetector.  Tally for pMC reflectance as a function  of XAndY and Time.
    /// This implementation works for DAW and CAW processing.
    /// </summary>
    public class pMCROfXAndYAndTimeAndSubregionRecessedDetector : Detector, IDetector
    {
        private IList<OpticalProperties> _referenceOps;
        private IList<OpticalProperties> _perturbedOps;
        private IList<int> _perturbedRegionsIndices;
        private ITissue _tissue;
        private Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// detector x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// detector y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        /// <summary>
        /// z-plane above tissue in air
        /// </summary>
        public double ZPlane { get; set; }
        /// <summary>
        /// Number of tissue regions for serial/deserialization
        /// </summary>
        public int NumberOfRegions { get; set; }
        /// <summary>
        /// perturbed optical properties listed in order of tissue regions
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// perturbed regions indices
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// numerical aperture
        /// </summary>
        public double NA { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// initialize the detector given the inputs and tissue definitions
        /// </summary>
        /// <param name="tissue"></param>
        /// <param name="rng"></param>
        public void Initialize(ITissue tissue, Random rng)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            NumberOfRegions = tissue.Regions.Count;
            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1,Time.Count - 1,NumberOfRegions];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1,Time.Count - 1,NumberOfRegions] : null);

            // initialize any other necessary class fields here
            _perturbedOps = PerturbedOps;
            _referenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            _perturbedRegionsIndices = PerturbedRegionsIndices;
            _tissue = tissue;
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon))
                return;

            // ray trace exit location and direction to location at ZPlane
            var positionAtZPlane = LayerTissueRegionToolbox.RayExtendToInfinitePlane(
                photon.DP.Position, photon.DP.Direction, ZPlane);

            // WhichBin to match ROfXAndYAndTimeDetector
            var ix = DetectorBinning.WhichBin(positionAtZPlane.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(positionAtZPlane.Y, Y.Count - 1, Y.Delta, Y.Start);
            //var it = DetectorBinning.WhichBin(photon.DP.TotalTime, Time.Count - 1, Time.Delta, Time.Start);
            // determine total time in each tissue region
            var pathLengthInRegion = photon.History.SubRegionInfoList.Select(p => p.PathLength).ToArray();

            if ((ix != -1) && (iy != -1))
            {
                double weightFactor = _absorbAction(
                    photon.History.SubRegionInfoList.Select(c => c.NumberOfCollisions).ToList(),
                    photon.History.SubRegionInfoList.Select(p => p.PathLength).ToList(),
                    _perturbedOps, _referenceOps, _perturbedRegionsIndices);


                for (int ir = 0; ir < NumberOfRegions; ir++)
                {
                    var timeInRegion = pathLengthInRegion[ir] / (GlobalConstants.C / _tissue.Regions[ir].RegionOP.N);
                    var it = DetectorBinning.WhichBin(timeInRegion, Time.Count - 1, Time.Delta, Time.Start);
                    if (timeInRegion > 0.0) // only tally if path length in region
                    {
                        Mean[ix, iy, it, ir] += photon.DP.Weight * weightFactor * (timeInRegion / photon.DP.TotalTime);
                        if (TallySecondMoment)
                        {
                            SecondMoment[ix, iy, it, ir] += photon.DP.Weight * weightFactor * (timeInRegion / photon.DP.TotalTime) *
                                                            photon.DP.Weight * weightFactor * (timeInRegion / photon.DP.TotalTime);
                        }
                    }
                }

                TallyCount++;
            }
        }

        /// <summary>
        /// method to normalize detector results after numPhotons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta * Time.Delta;
            for (int ix = 0; ix < X.Count - 1; ix++)
            {
                for (int iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (int it = 0; it < Time.Count - 1; it++)
                    {
                        for (int ir = 0; ir < NumberOfRegions; ir++)
                        {
                            var areaNorm = normalizationFactor;
                            Mean[ix, iy, it, ir] /= areaNorm * numPhotons;
                            if (TallySecondMoment)
                            {
                                SecondMoment[ix, iy, it, ir] /= areaNorm * areaNorm * numPhotons;
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
                                for (int k = 0; k < Time.Count - 1; k++) {
                                    for (int l = 0; l < NumberOfRegions; l++)
                                    {
                                        binaryWriter.Write(Mean[i, j, k, l]);
                                    }
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ X.Count - 1, Y.Count - 1,Time.Count - 1,NumberOfRegions];
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++)  {
                                for (int k = 0; k < Time.Count - 1; k++)  {
                                    for (int l = 0; l < NumberOfRegions; l++)
                                    {
                                        Mean[i, j, k, l] = binaryReader.ReadDouble();
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
                                for (int k = 0; k < Time.Count - 1; k++)  {
                                    for (int l = 0; l < NumberOfRegions; l++)
                                    {

                                        binaryWriter.Write(SecondMoment[i, j, k, l]);
                                    }
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ X.Count - 1, Y.Count - 1, Time.Count - 1, NumberOfRegions];
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < Time.Count - 1; k++) {
                                    for (int l = 0; l < NumberOfRegions; l++)
                                    {
                                        SecondMoment[i, j, k, l] = binaryReader.ReadDouble();
                                    }
                                }
                            }                       
			            }
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector NA
        /// pMC does not have access to PreviousDP so logic based on DP and 
        /// n1 sin(theta1) = n2 sin(theta2) 
        /// </summary>
        /// <param name="photon">photon</param>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            var detectorRegionN = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
            return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);          
        }
    }
}
