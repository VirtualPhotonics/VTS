using System;
using System.Runtime.Serialization;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for ReflectedMT as a function of Rho and Z.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class ReflectedMTOfRhoAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for ReflectedMT as a function of rho and tissue subregion detector input
        /// </summary>
        public ReflectedMTOfRhoAndSubregionHistDetectorInput()
        {
            TallyType = "ReflectedMTOfRhoAndSubregionHist";
            Name = "ReflectedMTOfRhoAndSubregionHist";
            Rho = new DoubleRange(0.0, 10, 101);
            MTBins = new DoubleRange(0.0, 500.0, 51);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }

        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// detector numerical aperture
        /// </summary>
        public double NA { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new ReflectedMTOfRhoAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                //NumSubregions = this.NumSubregions,
                MTBins = this.MTBins,
                FractionalMTBins = this.FractionalMTBins,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of Rho and tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ReflectedMTOfRhoAndSubregionHistDetector : Detector, IDetector
    {
        private ITissue _tissue;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// detector numerical aperture
        /// </summary>
        public double NA { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember] 
        public double[,] Mean { get; set; }

        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        /// <summary>
        /// fraction of MT spent in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] FractionalMT { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Zs detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public int NumSubregions { get; set; }

        /// <summary>
        /// Method to initialize detector
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="rng">random number generator</param>
        public void Initialize(ITissue tissue, Random rng)
        {
            // initialize any necessary class fields here
            _tissue = tissue;

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[Rho.Count - 1, MTBins.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1, MTBins.Count - 1] : null);
            // Fractional MT has FractionalMTBins.Count numnber of bins PLUS 2, one for =1, an d one for =0
            FractionalMT = FractionalMT ?? new double[Rho.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1];
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon)) return;

            // calculate the radial bin to attribute the deposition
            var irho = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var subregionMt = new double[NumSubregions];
            var talliedMt = false;

            // go through photon history and calculate momentum transfer
            // assumes that no MT tallied at pseudo-collisions (reflections and refractions)
            // this algorithm needs to look ahead to angle of next DP, but needs info from previous to determine whether real or pseudo-collision
            var previousDp = photon.History.HistoryData.First();
            var currentDp = photon.History.HistoryData.Skip(1).Take(1).First();
            foreach (var nextDp in photon.History.HistoryData.Skip(2))
            {
                if (previousDp.Weight != currentDp.Weight) // only for true collision points
                {
                    var csr = _tissue.GetRegionIndex(currentDp.Position); // get current region index
                    // get angle between current and next
                    var cosineBetweenTrajectories = Direction.GetDotProduct(currentDp.Direction, nextDp.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    subregionMt[csr] += momentumTransfer;
                    talliedMt = true;
                }
                previousDp = currentDp;
                currentDp = nextDp;
            }
            // tally total MT
            var totalMt = subregionMt.Sum();
            if (totalMt <= 0.0) return; // only tally if momentum transfer accumulated
            var imt = DetectorBinning.WhichBin(totalMt, MTBins.Count - 1, MTBins.Delta, MTBins.Start);
            Mean[irho, imt] += photon.DP.Weight;
            if (TallySecondMoment)
            {
                SecondMoment[irho, imt] += photon.DP.Weight * photon.DP.Weight;                    
            }

            if (talliedMt) TallyCount++;

            // tally fractional MT in each sub-region
            int ifrac;
            for (var isr = 0; isr < NumSubregions; isr++)
            {
                // add 1 to ifrac to offset bin 0 added for =0 only tallies
                ifrac = DetectorBinning.WhichBin(subregionMt[isr]/totalMt,
                    FractionalMTBins.Count - 1, FractionalMTBins.Delta, FractionalMTBins.Start) + 1;
                // put identically 0 fractional MT into separate bin at index 0
                if (subregionMt[isr]/totalMt == 0.0)
                {
                    ifrac = 0;
                }
                // put identically 1 fractional MT into separate bin at index Count+1 -1
                if (subregionMt[isr]/totalMt == 1.0)
                {
                    ifrac = FractionalMTBins.Count;
                }
                FractionalMT[irho, imt, isr, ifrac] += photon.DP.Weight;
            }
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (var imt = 0; imt < MTBins.Count - 1; imt++)
                {
                    // normalize by area of surface area ring and N
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, imt] /= areaNorm * numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ir, imt] /= areaNorm * areaNorm * numPhotons;
                    }
                    for (var isr = 0; isr < NumSubregions; isr++)
                    {
                        for (var ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                        {
                            FractionalMT[ir, imt, isr, ifrac] /= areaNorm*numPhotons;
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
            return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < MTBins.Count - 1; j++)
                            {                                
                                binaryWriter.Write(Mean[i, j]);
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ Rho.Count - 1, MTBins.Count - 1];
                        for (var i = 0; i <  Rho.Count - 1; i++) {
                            for (var j = 0; j < MTBins.Count - 1; j++)
                            {
                               Mean[i, j] = binaryReader.ReadDouble(); 
                            }
                        }
                    }
                },
                new BinaryArraySerializer {
                    DataArray = FractionalMT,
                    Name = "FractionalMT",
                    FileTag = "_FractionalMT",
                    WriteData = binaryWriter => {
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < MTBins.Count - 1; j++) {
                                for (var k = 0; k < NumSubregions; k++) {
                                    for (var l = 0; l < FractionalMTBins.Count + 1; l++)
                                    {
                                        binaryWriter.Write(FractionalMT[i, j, k, l]);
                                    } 
                                }                                                             
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        FractionalMT = FractionalMT ?? new double[ Rho.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1];
                        for (var i = 0; i <  Rho.Count - 1; i++) {
                            for (var j = 0; j < MTBins.Count - 1; j++) {
                                for (var k = 0; k < NumSubregions; k++) {
                                    for (var l = 0; l < FractionalMTBins.Count + 1; l++)
                                    {
                                        FractionalMT[i, j, k, l] = binaryReader.ReadDouble(); 
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
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < MTBins.Count - 1; j++)
                            {
                                binaryWriter.Write(SecondMoment[i, j]);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ Rho.Count - 1, MTBins.Count - 1];
                        for (var i = 0; i < Rho.Count - 1; i++) {
                            for (var j = 0; j < MTBins.Count - 1; j++)
                            {
                                SecondMoment[i, j] = binaryReader.ReadDouble();
                            }                       
			            }
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector NA
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Boolean indicating whether photon is within detector</returns>
        public bool IsWithinDetectorAperture(Photon photon)
        {
            if (photon.CurrentRegionIndex == FinalTissueRegionIndex)
            {
                var detectorRegionN = _tissue.Regions[photon.CurrentRegionIndex].RegionOP.N;
                return photon.DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
            else // determine n of prior tissue region
            {
                var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                return photon.History.PreviousDP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            }
        }

    }
}
