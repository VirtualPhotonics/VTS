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
    /// Tally for Reflected dynamic MT as a function of Rho using blood volume fraction in each tissue region.
    /// This detector also tallies the total and dynamic MT as a function of Z.  If a random number is less
    /// than blood volume fraction for the tissue region in which the collision occurred, then hit blood and considered
    /// "dynamic" event.  Otherwise, it is a "static" event.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for ReflectedMT as a function of rho and blood volume frac. in tissue subregion detector input
        /// </summary>
        public ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput()
        {
            TallyType = "ReflectedDynamicMTOfRhoAndSubregionHist";
            Name = "ReflectedDynamicMTOfRhoAndSubregionHist";
            Rho = new DoubleRange(0.0, 10, 101);
            Z = new DoubleRange(0.0, 10, 101);
            MTBins = new DoubleRange(0.0, 500.0, 51);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = true;
        }

        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// subregion blood volume fraction
        /// </summary>
        public IList<double> BloodVolumeFraction { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }

        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }
        
        public IDetector CreateDetector()
        {
            return new ReflectedDynamicMTOfRhoAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Rho = this.Rho,
                Z = this.Z,
                MTBins = this.MTBins,
                BloodVolumeFraction = this.BloodVolumeFraction,
                FractionalMTBins = this.FractionalMTBins
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of Rho and tissue subregion
    /// using blood volume fraction in each tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ReflectedDynamicMTOfRhoAndSubregionHistDetector : Detector, IDetector
    {
        private ITissue _tissue;
        private IList<double> _bloodVolumeFraction;
        private Random _rng;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
        /// <summary>
        /// subregion blood volume fraction
        /// </summary>
        public IList<double> BloodVolumeFraction { get; set; }
        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }

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
        /// total MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[,] TotalMTOfZ { get; set; }

        /// <summary>
        /// total MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[,] TotalMTOfZSecondMoment { get; set; }

        /// <summary>
        /// dynamic MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[,] DynamicMTOfZ { get; set; }

        /// <summary>
        /// dynamic MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public double[,] DynamicMTOfZSecondMoment { get; set; }

        /// <summary>
        /// fraction of DYNAMIC MT spent in tissue
        /// </summary>
        [IgnoreDataMember]
        public double[,,] FractionalMT { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Zs detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public int NumSubregions { get; set; }
        //public double[,] SubregionCollisions { get; set; } //debug
        //public int[] TotalCollisions { get; set; } //debug

        public void Initialize(ITissue tissue, Random rng)
        {
            // intialize any necessary class fields here
            _tissue = tissue;
            _rng = rng;

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[Rho.Count - 1, MTBins.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[Rho.Count - 1, MTBins.Count - 1] : null);

            TotalMTOfZ = TotalMTOfZ ?? new double[Rho.Count - 1, Z.Count - 1];
            DynamicMTOfZ = DynamicMTOfZ ?? new double[Rho.Count - 1, Z.Count - 1];
            TotalMTOfZSecondMoment = TotalMTOfZSecondMoment ?? new double[Rho.Count - 1, Z.Count - 1];
            DynamicMTOfZSecondMoment = DynamicMTOfZSecondMoment ?? new double[Rho.Count - 1, Z.Count - 1];
         
            // Fractional MT has FractionalMTBins.Count numnber of bins PLUS 2, one for =1, an d one for =0
            FractionalMT = FractionalMT ?? new double[Rho.Count - 1, MTBins.Count - 1, FractionalMTBins.Count + 1];

            // intialize any other necessary class fields here
            _bloodVolumeFraction = BloodVolumeFraction;

            //SubregionCollisions = new double[NumSubregions, 2]; //debug
            //TotalCollisions = new int[NumSubregions]; // debug
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // calculate the radial bin to attribute the deposition
            var irho = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var tissueMT = new double[2]; // 2 is for [static, dynamic] tally separation
            bool talliedMT = false;
            double totalMT = 0;
            var totalMTOfZForOnePhoton = new double[Rho.Count - 1, Z.Count - 1];
            var dynamicMTOfZForOnePhoton = new double[Rho.Count - 1, Z.Count - 1];

            // go through photon history and claculate momentum transfer
            // assumes that no MT tallied at pseudo-collisions (reflections and refractions)
            // this algorithm needs to look ahead to angle of next DP, but needs info from previous to determine whether real or pseudo-collision
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            PhotonDataPoint currentDP = photon.History.HistoryData.Skip(1).Take(1).First();
            foreach (PhotonDataPoint nextDP in photon.History.HistoryData.Skip(2))
            {
                if (previousDP.Weight != currentDP.Weight) // only for true collision points
                {
                    var csr = _tissue.GetRegionIndex(currentDP.Position); // get current region index
                    // get z bin of current position
                    var iz = DetectorBinning.WhichBin(currentDP.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
                    // get angle between current and next
                    double cosineBetweenTrajectories = Direction.GetDotProduct(currentDP.Direction, nextDP.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    totalMT += momentumTransfer;
                    TotalMTOfZ[irho, iz] += photon.DP.Weight * momentumTransfer;
                    totalMTOfZForOnePhoton[irho, iz] += photon.DP.Weight*momentumTransfer;
                    //TotalCollisions[csr] += 1; //debug
                    if (_rng.NextDouble() < _bloodVolumeFraction[csr]) // hit blood 
                    {
                        tissueMT[1] += momentumTransfer;
                        DynamicMTOfZ[irho, iz] += photon.DP.Weight * momentumTransfer;
                        dynamicMTOfZForOnePhoton[irho, iz] += photon.DP.Weight*momentumTransfer;
                        //    SubregionCollisions[csr, 1] += 1; //debug
                    }
                    else // index 0 captures static events
                    {
                        tissueMT[0] += momentumTransfer;
                        //SubregionCollisions[csr, 0] += 1; //debug
                    }
                    talliedMT = true;
                }
                previousDP = currentDP;
                currentDP = nextDP;
            }
            if (totalMT > 0.0)  // only tally if momentum transfer accumulated
            {
                var imt = DetectorBinning.WhichBin(totalMT, MTBins.Count - 1, MTBins.Delta, MTBins.Start);
                Mean[irho, imt] += photon.DP.Weight;
                if (TallySecondMoment)
                {
                    SecondMoment[irho, imt] += photon.DP.Weight * photon.DP.Weight;
                    for (int i = 0; i < Rho.Count - 1; i++)
                    {
                        for (int j = 0; j < Z.Count - 1; j++)
                        {
                            TotalMTOfZSecondMoment[i, j] += totalMTOfZForOnePhoton[i, j] * totalMTOfZForOnePhoton[i, j];
                            DynamicMTOfZSecondMoment[i, j] += dynamicMTOfZForOnePhoton[i, j] * dynamicMTOfZForOnePhoton[i, j];
                        }
                    }
                }

                if (talliedMT) TallyCount++;

                // tally DYNAMIC fractional MT in entire tissue

                // add 1 to ifrac to offset bin 0 added for =0 only tallies
                int ifrac = DetectorBinning.WhichBin(tissueMT[1] / totalMT,
                    FractionalMTBins.Count - 1, FractionalMTBins.Delta, FractionalMTBins.Start) + 1;
                // put identically 0 fractional MT into separate bin at index 0
                if (tissueMT[1] / totalMT == 0.0)
                {
                    ifrac = 0;
                }
                // put identically 1 fractional MT into separate bin at index Count+1 -1
                if (tissueMT[1] / totalMT == 1.0)
                {
                    ifrac = FractionalMTBins.Count;
                }
                FractionalMT[irho, imt, ifrac] += photon.DP.Weight;
            }
       
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                // normalize by area of surface area ring and N
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                for (int imt = 0; imt < MTBins.Count - 1; imt++)
                {
                    Mean[ir, imt] /= areaNorm * numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ir, imt] /= areaNorm * areaNorm * numPhotons;
                    }
                    for (int ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                    {
                        FractionalMT[ir, imt, ifrac] /= areaNorm * numPhotons;
                    } 
                }
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    TotalMTOfZ[ir, iz] /= areaNorm * numPhotons;
                    DynamicMTOfZ[ir, iz] /= areaNorm * numPhotons;
                    if (TallySecondMoment)
                    {
                        TotalMTOfZSecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons; 
                        DynamicMTOfZSecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;              
                    }
                }
            }
            //for (int i = 1; i < NumSubregions-1; i++) //debug
            //{ //debug
            //    SubregionCollisions[i, 0] /= TotalCollisions[i]; //debug
            //    SubregionCollisions[i, 1] /= TotalCollisions[i]; //debug
            //} //debug
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
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {                                
                                binaryWriter.Write(Mean[i, j]);
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ Rho.Count - 1, MTBins.Count - 1];
                        for (int i = 0; i <  Rho.Count - 1; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
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
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < Rho.Count - 1; i++)
                        {
                            for (int k = 0; k < MTBins.Count - 1; k++)
                            {
                                for (int m = 0; m < FractionalMTBins.Count + 1; m++)
                                {
                                    binaryWriter.Write(FractionalMT[i, k, m]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        FractionalMT = FractionalMT ?? new double[ Rho.Count - 1, MTBins.Count - 1, FractionalMTBins.Count + 1];
                        for (int i = 0; i < Rho.Count - 1; i++)
                        {
                            for (int k = 0; k < MTBins.Count - 1; k++)
                            {
                                for (int m = 0; m < FractionalMTBins.Count + 1; m++)
                                {
                                    FractionalMT[i, k, m] = binaryReader.ReadDouble();
                                }
                            }
                        }
                    }
                },
                new BinaryArraySerializer
                {
                    DataArray = TotalMTOfZ,
                    Name = "TotalMTOfZ",
                    FileTag = "_TotalMTOfZ",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < Rho.Count - 1; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                binaryWriter.Write(TotalMTOfZ[i, l]);
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        TotalMTOfZ = TotalMTOfZ ??
                                       new double[Rho.Count - 1, Z.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                    TotalMTOfZ[i, l] = binaryReader.ReadDouble();
                            }
                        }
                    }
                },
                new BinaryArraySerializer
                {
                    DataArray = DynamicMTOfZ,
                    Name = "DynamicMTOfZ",
                    FileTag = "_DynamicMTOfZ",
                    WriteData = binaryWriter =>
                    {
                        for (int i = 0; i < Rho.Count - 1; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                binaryWriter.Write(DynamicMTOfZ[i, l]);
                            }
                        }
                    },
                    ReadData = binaryReader =>
                    {
                        DynamicMTOfZ = DynamicMTOfZ ??
                                       new double[Rho.Count - 1, Z.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++)
                        {
                            for (int l = 0; l < Z.Count - 1; l++)
                            {
                                DynamicMTOfZ[i, l] = binaryReader.ReadDouble();
                            }
                        }
                    }
                },
                // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null :  
                new BinaryArraySerializer {
                    DataArray = TotalMTOfZSecondMoment,
                    Name = "TotalMTOfZSecondMoment",
                    FileTag = "_TotalMTOfZ_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                binaryWriter.Write(TotalMTOfZSecondMoment[i, j]);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
                        TotalMTOfZSecondMoment = new double[ Rho.Count - 1, Z.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                TotalMTOfZSecondMoment[i, j] = binaryReader.ReadDouble();
                            }                       
                        }
                    },
                },
                new BinaryArraySerializer {
                    DataArray = DynamicMTOfZSecondMoment,
                    Name = "DynamicMTOfZSecondMoment",
                    FileTag = "_DynamicMTOfZ_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                binaryWriter.Write(DynamicMTOfZSecondMoment[i, j]);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
                        DynamicMTOfZSecondMoment = new double[ Rho.Count - 1, Z.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < Z.Count - 1; j++)
                            {
                                DynamicMTOfZSecondMoment[i, j] = binaryReader.ReadDouble();
                            }                       
			            }
                    },
                },
                new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {
                                binaryWriter.Write(SecondMoment[i, j]);
                            }                            
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ Rho.Count - 1, MTBins.Count - 1];
                        for (int i = 0; i < Rho.Count - 1; i++) {
                            for (int j = 0; j < MTBins.Count - 1; j++)
                            {
                                SecondMoment[i, j] = binaryReader.ReadDouble();
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
