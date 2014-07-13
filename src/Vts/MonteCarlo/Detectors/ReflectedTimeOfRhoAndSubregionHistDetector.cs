using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for ReflectedTime as a function of Rho and Z.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class ReflectedTimeOfRhoAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for ReflectedTime as a function of rho and tissue subregion detector input
        /// </summary>
        public ReflectedTimeOfRhoAndSubregionHistDetectorInput()
        {
            TallyType = "ReflectedTimeOfRhoAndSubregionHist";
            Name = "ReflectedTimeOfRhoAndSubregionHist";
            Rho = new DoubleRange(0.0, 10, 101);
            Time = new DoubleRange(0.0, 1.0, 101);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = true;
            TallyDetails.IsNotImplementedForDAW = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
        ///// <summary>
        ///// subregion index binning, needed by DetectorIO
        ///// </summary>
        //public int NumSubregions { get; set; }

        public IDetector CreateDetector()
        {
            return new ReflectedTimeOfRhoAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                //NumSubregions = this.NumSubregions,
                Time = this.Time,
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of Rho and tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ReflectedTimeOfRhoAndSubregionHistDetector : Detector, IDetector
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
        public DoubleRange Time { get; set; }
        /// <summary>
        /// fractional time bins binning
        /// </summary>
        public DoubleRange FractionalTimeBins { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[,,] Mean { get; set; }

        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[,,] SecondMoment { get; set; }

        /// <summary>
        /// fraction of Time spent in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] FractionalTime { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Zs detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public int NumSubregions { get; set; }

        public void Initialize(ITissue tissue)
        {
            _tissue = tissue;

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[Rho.Count - 1, NumSubregions, Time.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1, NumSubregions, Time.Count - 1] : null);
            FractionalTime = FractionalTime ?? new double[Rho.Count - 1, NumSubregions];
        }


        /// <summary>
        /// method to tally reflected photon by determining cumulative MT in each tissue subregion and binning in MT
        /// </summary>
        /// <param name="photon">Photon (includes HistoryData)</param>
        public void Tally(Photon photon)
        {
            // calculate the radial and time bin of reflected photon
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            for (int i = 0; i < NumSubregions; i++)
            {
                var timeInSubRegion = DetectorBinning.GetTimeDelay(photon.History.SubRegionInfoList[i].PathLength,
                                                                   _tissue.Regions[i].RegionOP.N);
                // make sure floating point round in Photon's update to S and subsequently to PathLength in SRIL doesn't get tallied
                if (timeInSubRegion > 1e-14)
                {
                    var it = DetectorBinning.WhichBin(timeInSubRegion, Time.Count - 1, Time.Delta, Time.Start);
                    // tally Continuous Absorption Weighting (CAW) 
                    var tally = Math.Exp(-_tissue.Regions[i].RegionOP.Mua * photon.History.SubRegionInfoList[i].PathLength);
                    Mean[ir, i, it] += tally;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ir, i, it] += tally * tally;
                    }
                }
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize tally
        /// </summary>
        /// <param name="numPhotons">number of photons launched from source</param>
        public void Normalize(long numPhotons)
        {
            var totalTimeForThisSubregion = new double[Rho.Count - 1, NumSubregions];
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var totalTimeOverAllSubregions = 0.0;
                for (int isr = 0; isr < NumSubregions; isr++)
                {
                    totalTimeForThisSubregion[ir, isr] =
                        Enumerable.Range(0, Mean.GetLength(2)).Sum(i => Mean[ir, isr, i]);
                    for (int it = 0; it < Time.Count - 1; it++)
                    {
                        totalTimeOverAllSubregions += Mean[ir, isr, it];
                        // normalize by area of surface area ring and N
                        var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                        Mean[ir, isr, it] /= areaNorm * numPhotons;
                        if (TallySecondMoment)
                        {
                            SecondMoment[ir, isr, it] /= areaNorm * areaNorm * numPhotons;
                        }
                    }
                }
                for (int isr = 0; isr < NumSubregions; isr++)
                {
                    FractionalTime[ir, isr] = totalTimeForThisSubregion[ir, isr] / totalTimeOverAllSubregions;
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
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < NumSubregions; j++) {
                                for (int k = 0; k < Time.Count - 1; k++) {                                   
                                    binaryWriter.Write(Mean[i, j, k]);
                                }                              
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ Rho.Count - 1, NumSubregions, Time.Count - 1];
                        for (int i = 0; i <  Rho.Count - 1; i++) {
                            for (int j = 0; j < NumSubregions; j++) {
                                for (int k = 0; k < Time.Count - 1; k++)
                                {
                                     Mean[i, j, k] = binaryReader.ReadDouble(); 
                                }                           
                            }
                        }
                    }
                },
                new BinaryArraySerializer {
                    DataArray = FractionalTime,
                    Name = "FractionalTime",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int k = 0; k < NumSubregions; k++) {
                                    binaryWriter.Write(FractionalTime[i, k]);
                            } 
                        }
                    },
                    ReadData = binaryReader => {
                        FractionalTime = FractionalTime ?? new double[ Rho.Count - 1, NumSubregions];
                        for (int i = 0; i <  Rho.Count - 1; i++) {
                            for (int k = 0; k < NumSubregions; k++) {
                                    FractionalTime[i, k] = binaryReader.ReadDouble(); 
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
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < NumSubregions; j++) {
                                for (int k = 0; k < Time.Count - 1; k++)
                                {
                                   binaryWriter.Write(SecondMoment[i, j, k]); 
                                }
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ Rho.Count - 1, NumSubregions, Time.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < NumSubregions; j++) {
                                for (int k = 0; k < Time.Count - 1; k++)
                                {
                                    SecondMoment[i, j, k] = binaryReader.ReadDouble();
                                }
                            }                       
			            }
                    },
                },
            };
        }
        /// <summary>
        /// Method to determine if photon is within detector
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <returns>method always returns true</returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }

    }
}
