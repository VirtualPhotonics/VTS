using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using System.IO;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for Reflected dynamic MT as a function of Fx using blood volume fraction in each tissue region.
    /// This detector also tallies the total and dynamic MT as a function of Z.  If a random number is less
    /// than blood volume fraction for the tissue region in which the collision occurred, then hit blood and considered
    /// "dynamic" event.  Otherwise, it is a "static" event.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class ReflectedDynamicMTOfFxAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for ReflectedMT as a function of Fx and blood volume frac. in tissue subregion detector input
        /// </summary>
        public ReflectedDynamicMTOfFxAndSubregionHistDetectorInput()
        {
            TallyType = "ReflectedDynamicMTOfFxAndSubregionHist";
            Name = "ReflectedDynamicMTOfFxAndSubregionHist";
            Fx = new DoubleRange(0.0, 0.5, 51);
            Z = new DoubleRange(0.0, 10, 101);
            MTBins = new DoubleRange(0.0, 500.0, 51);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 0; // assume detector is in air

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
            TallyDetails.IsCylindricalTally = false;
        }

        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// sub-region blood volume fraction
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
            return new ReflectedDynamicMTOfFxAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                Fx = this.Fx,
                Z = this.Z,
                MTBins = this.MTBins,
                BloodVolumeFraction = this.BloodVolumeFraction,
                FractionalMTBins = this.FractionalMTBins,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of Fx and tissue subregion
    /// using blood volume fraction in each tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ReflectedDynamicMTOfFxAndSubregionHistDetector : Detector, IDetector
    {
        private ITissue _tissue;
        private IList<double> _bloodVolumeFraction;
        private Random _rng;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// Fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
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
        public Complex[,] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }
        /// <summary>
        /// total MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] TotalMTOfZ { get; set; }
        /// <summary>
        /// total MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] TotalMTOfZSecondMoment { get; set; }
        /// <summary>
        /// dynamic MT as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] DynamicMTOfZ { get; set; }
        /// <summary>
        /// dynamic MT Second Moment as a function of Z multiplied by final photon weight
        /// </summary>
        [IgnoreDataMember]
        public Complex[,] DynamicMTOfZSecondMoment { get; set; }
        /// <summary>
        /// fraction of DYNAMIC MT spent in tissue
        /// </summary>
        [IgnoreDataMember]
        public Complex[,,] FractionalMT { get; set; }
        /// <summary>
        /// number of dynamic and static collisions in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] SubregionCollisions { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of tallies to detector
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// number of tissue subregions
        /// </summary>
        public int NumSubregions { get; set; } 
        /// <summary>
        /// number of total collisions
        /// </summary>
        public int[] TotalCollisions { get; set; } // debug

        /// <summary>
        /// Method to initialize detector
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="rng">random number generator</param>
        public void Initialize(ITissue tissue, Random rng)
        {
            // initialize any necessary class fields here
            _tissue = tissue;
            _rng = rng;

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new Complex[Fx.Count, MTBins.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new Complex[Fx.Count, MTBins.Count - 1] : null);

            TotalMTOfZ = TotalMTOfZ ?? new Complex[Fx.Count, Z.Count - 1];
            DynamicMTOfZ = DynamicMTOfZ ?? new Complex[Fx.Count, Z.Count - 1];
            TotalMTOfZSecondMoment = TotalMTOfZSecondMoment ?? new Complex[Fx.Count, Z.Count - 1];
            DynamicMTOfZSecondMoment = DynamicMTOfZSecondMoment ?? new Complex[Fx.Count, Z.Count - 1];
         
            // Fractional MT has FractionalMTBins.Count numnber of bins PLUS 2, one for =1, an d one for =0
            FractionalMT = FractionalMT ?? new Complex[Fx.Count, MTBins.Count - 1, FractionalMTBins.Count + 1];

            SubregionCollisions = new double[NumSubregions, 2]; // 2nd index: 0=static, 1=dynamic
            TotalCollisions = new int[NumSubregions]; //debug

            // initialize any other necessary class fields here
            _bloodVolumeFraction = BloodVolumeFraction;

        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            if (!IsWithinDetectorAperture(photon)) return;

            var tissueMt = new double[2]; // 2 is for [static, dynamic] tally separation
            var talliedMt = false;
            double totalMt = 0;
            var totalMtOfZForOnePhoton = new Complex[Fx.Count, Z.Count - 1];
            var dynamicMtOfZForOnePhoton = new Complex[Fx.Count, Z.Count - 1];
            var fxArray = Fx.ToArray();
            var x = photon.DP.Position.X; // use final exiting x position
            var sinNegativeTwoPiFx = fxArray.Select(fx => Math.Sin(-2 * Math.PI * fx * x)).ToArray();
            var cosNegativeTwoPiFx = fxArray.Select(fx => Math.Cos(-2 * Math.PI * fx * x)).ToArray();

            // go through photon history and claculate momentum transfer
            // assumes that no MT tallied at pseudo-collisions (reflections and refractions)
            // this algorithm needs to look ahead to angle of next DP, but needs info from previous to determine whether real or pseudo-collision
            var previousDp = photon.History.HistoryData.First();
            var currentDp = photon.History.HistoryData.Skip(1).Take(1).First();
            foreach (var nextDp in photon.History.HistoryData.Skip(2))
            {
                if (previousDp.Weight != currentDp.Weight) // only for true collision points
                {
                    var csr = _tissue.GetRegionIndex(currentDp.Position); // get current region index
                    // get z bin of current position
                    var iz = DetectorBinning.WhichBin(currentDp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);
                    // get angle between current and next
                    var cosineBetweenTrajectories = Direction.GetDotProduct(currentDp.Direction, nextDp.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    totalMt += momentumTransfer;
                    for (var ifx = 0; ifx < Fx.Count; ifx++)
                    {
                        var deltaWeight = photon.DP.Weight * cosNegativeTwoPiFx[ifx] +
                                      Complex.ImaginaryOne * sinNegativeTwoPiFx[ifx];
                        TotalMTOfZ[ifx, iz] += deltaWeight * momentumTransfer;
                        totalMtOfZForOnePhoton[ifx, iz] += deltaWeight * momentumTransfer;
                    }
                    //TotalCollisions[csr] += 1; //debug
                    if (_rng.NextDouble() < _bloodVolumeFraction[csr]) // hit blood 
                    {
                        tissueMt[1] += momentumTransfer;
                        for (var ifx = 0; ifx < Fx.Count; ifx++)
                        {
                            var deltaWeight = photon.DP.Weight * cosNegativeTwoPiFx[ifx] +
                                          Complex.ImaginaryOne * sinNegativeTwoPiFx[ifx];
                            DynamicMTOfZ[ifx, iz] += deltaWeight * momentumTransfer;
                            dynamicMtOfZForOnePhoton[ifx, iz] += deltaWeight * momentumTransfer;
                        }

                        SubregionCollisions[csr, 1] += 1; // add to dynamic collision count
                    }
                    else // index 0 captures static events
                    {
                        tissueMt[0] += momentumTransfer;
                        SubregionCollisions[csr, 0] += 1; // add to static collision count
                    }
                    talliedMt = true;
                }
                previousDp = currentDp;
                currentDp = nextDp;
            }

            if (totalMt <= 0.0) return; // only tally if momentum transfer accumulated
            {
                var imt = DetectorBinning.WhichBin(totalMt, MTBins.Count - 1, MTBins.Delta, MTBins.Start);
                for (var ifx = 0; ifx < Fx.Count; ifx++)
                {
                    var deltaWeight = photon.DP.Weight * cosNegativeTwoPiFx[ifx] +
                                      Complex.ImaginaryOne * sinNegativeTwoPiFx[ifx];
                    Mean[ifx, imt] += deltaWeight;
                    if (TallySecondMoment)
                    {
                        // 2nd moment is E[xx*]=E[xReal^2]+E[xImag^2] and with cos^2+sin^2=1 => weight^2
                        SecondMoment[ifx, imt] += photon.DP.Weight * photon.DP.Weight;
                        for (var i = 0; i < Fx.Count - 1; i++)
                        {
                            for (var j = 0; j < Z.Count - 1; j++)
                            {
                                TotalMTOfZSecondMoment[i, j] +=
                                    totalMtOfZForOnePhoton[i, j] * totalMtOfZForOnePhoton[i, j];
                                DynamicMTOfZSecondMoment[i, j] +=
                                    dynamicMtOfZForOnePhoton[i, j] * dynamicMtOfZForOnePhoton[i, j];
                            }
                        }
                    }

                    if (talliedMt) TallyCount++;

                    // tally DYNAMIC fractional MT in entire tissue

                    // add 1 to ifrac to offset bin 0 added for =0 only tallies
                    var ifrac = DetectorBinning.WhichBin(tissueMt[1] / totalMt,
                        FractionalMTBins.Count - 1, FractionalMTBins.Delta, FractionalMTBins.Start) + 1;
                    // put identically 0 fractional MT into separate bin at index 0
                    if (tissueMt[1] / totalMt == 0.0)
                    {
                        ifrac = 0;
                    }

                    // put identically 1 fractional MT into separate bin at index Count+1 -1
                    if (tissueMt[1] / totalMt == 1.0)
                    {
                        ifrac = FractionalMTBins.Count;
                    }

                    FractionalMT[ifx, imt, ifrac] += deltaWeight;
                }
            }

        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            for (var ifx = 0; ifx < Fx.Count; ifx++)
            {
                for (var imt = 0; imt < MTBins.Count - 1; imt++)
                {
                    Mean[ifx, imt] /= numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ifx, imt] /= numPhotons;
                    }
                    for (var ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                    {
                        FractionalMT[ifx, imt, ifrac] /= numPhotons;
                    } 
                }
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    TotalMTOfZ[ifx, iz] /= numPhotons;
                    DynamicMTOfZ[ifx, iz] /= numPhotons;
                    if (TallySecondMoment)
                    {
                        TotalMTOfZSecondMoment[ifx, iz] /= numPhotons; 
                        DynamicMTOfZSecondMoment[ifx, iz] /= numPhotons;              
                    }
                }
            } 
        }

        private void MeanWriter(BinaryWriter binaryWriter)
        {
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < MTBins.Count - 1; j++)
                {
                    binaryWriter.Write(Mean[i, j].Real);
                    binaryWriter.Write(Mean[i, j].Imaginary);
                }
            }
        }

        private void MeanReader(BinaryReader binaryReader)
        {
            Mean ??= new Complex[Fx.Count, MTBins.Count - 1];
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < MTBins.Count - 1; j++)
                {
                    var real = binaryReader.ReadDouble();
                    var imag = binaryReader.ReadDouble();
                    Mean[i, j] = new Complex(real, imag);
                }
            }
        }

        private void FractionalMtWriter(BinaryWriter binaryWriter)
        {
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var k = 0; k < MTBins.Count - 1; k++)
                {
                    for (var m = 0; m < FractionalMTBins.Count + 1; m++)
                    {
                        binaryWriter.Write(FractionalMT[i, k, m].Real);
                        binaryWriter.Write(FractionalMT[i, k, m].Imaginary);
                    }
                }
            }
        }

        private void FractionalMtReader(BinaryReader binaryReader)
        {
            FractionalMT ??= new Complex[Fx.Count, MTBins.Count - 1, FractionalMTBins.Count + 1];
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var k = 0; k < MTBins.Count - 1; k++)
                {
                    for (var m = 0; m < FractionalMTBins.Count + 1; m++)
                    {
                        var real = binaryReader.ReadDouble();
                        var imag = binaryReader.ReadDouble();
                        FractionalMT[i, k, m] = new Complex(real, imag);
                    }
                }
            }
        }

        private void TotalMtOfZWriter(BinaryWriter binaryWriter)
        {
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var l = 0; l < Z.Count - 1; l++)
                {
                    binaryWriter.Write(TotalMTOfZ[i, l].Real);
                    binaryWriter.Write(TotalMTOfZ[i, l].Imaginary);
                }
            }
        }

        private void TotalMtOfZReader(BinaryReader binaryReader)
        {
            TotalMTOfZ ??= new Complex[Fx.Count, Z.Count - 1];
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var l = 0; l < Z.Count - 1; l++)
                {
                    var real = binaryReader.ReadDouble();
                    var imag = binaryReader.ReadDouble();
                    TotalMTOfZ[i, l] = new Complex(real, imag);
                }
            }
        }

        private void DynamicMtOfZWriter(BinaryWriter binaryWriter)
        {
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var l = 0; l < Z.Count - 1; l++)
                {
                    binaryWriter.Write(DynamicMTOfZ[i, l].Real);
                    binaryWriter.Write(DynamicMTOfZ[i, l].Imaginary);
                }
            }
        }

        private void DynamicMtOfZReader(BinaryReader binaryReader)
        {
            DynamicMTOfZ ??= new Complex[Fx.Count, Z.Count - 1];
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var l = 0; l < Z.Count - 1; l++)
                {
                    var real = binaryReader.ReadDouble();
                    var imag = binaryReader.ReadDouble();
                    DynamicMTOfZ[i, l] = new Complex(real, imag);
                }
            }
        }

        private void SubRegionCollisionsWriter(BinaryWriter binaryWriter)
        {
            for (var i = 0; i < NumSubregions; i++)
            {
                for (var l = 0; l < 2; l++)
                {
                    binaryWriter.Write(SubregionCollisions[i, l]);
                }
            }
        }

        private void SubRegionCollisionsReader(BinaryReader binaryReader)
        {
            SubregionCollisions ??= new double[NumSubregions, 2];
            for (var i = 0; i < NumSubregions; i++)
            {
                for (var l = 0; l < 2; l++)
                {
                    SubregionCollisions[i, l] = binaryReader.ReadDouble();
                }
            }
        }

        private void TotalMtOfZSecondMomentWriter(BinaryWriter binaryWriter)
        {
            if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    binaryWriter.Write(TotalMTOfZSecondMoment[i, j].Real);
                    binaryWriter.Write(TotalMTOfZSecondMoment[i, j].Imaginary);
                }
            }
        }

        private void TotalMtOfZSecondMomentReader(BinaryReader binaryReader)
        {
            if (!TallySecondMoment || TotalMTOfZSecondMoment == null) return;
            TotalMTOfZSecondMoment = new Complex[Fx.Count, Z.Count - 1];
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    var real = binaryReader.ReadDouble();
                    var imag = binaryReader.ReadDouble();
                    TotalMTOfZSecondMoment[i, j] = new Complex(real, imag);
                }
            }
        }

        private void DynamicMtOfZSecondMomentWriter(BinaryWriter binaryWriter)
        {
            if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    binaryWriter.Write(DynamicMTOfZSecondMoment[i, j].Real);
                    binaryWriter.Write(DynamicMTOfZSecondMoment[i, j].Imaginary);
                }
            }
        }

        private void DynamicMtOfZSecondMomentReader(BinaryReader binaryReader)
        {
            if (!TallySecondMoment || DynamicMTOfZSecondMoment == null) return;
            DynamicMTOfZSecondMoment = new Complex[Fx.Count, Z.Count - 1];
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < Z.Count - 1; j++)
                {
                    var real = binaryReader.ReadDouble();
                    var imag = binaryReader.ReadDouble();
                    DynamicMTOfZSecondMoment[i, j] = new Complex(real, imag);
                }
            }
        }

        private void SecondMomentWriter(BinaryWriter binaryWriter)
        {
            if (!TallySecondMoment || SecondMoment == null) return;
            for (var i = 0; i < Fx.Count; i++)
            {
                for (var j = 0; j < MTBins.Count - 1; j++)
                {
                    binaryWriter.Write(SecondMoment[i, j].Real);
                    binaryWriter.Write(SecondMoment[i, j].Imaginary);
                }
            }
        }

        private void SecondMomentReader(BinaryReader binaryReader)
        {
            if (!TallySecondMoment || SecondMoment == null) return;
            SecondMoment = new Complex[Fx.Count, MTBins.Count - 1];
            for (var i = 0; i < Fx.Count - 1; i++)
            {
                for (var j = 0; j < MTBins.Count - 1; j++)
                {
                    var real = binaryReader.ReadDouble();
                    var imag = binaryReader.ReadDouble();
                    SecondMoment[i, j] = new Complex(real, imag);
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
                    WriteData = MeanWriter,
                    ReadData = MeanReader
                },
                new BinaryArraySerializer {
                    DataArray = FractionalMT,
                    Name = "FractionalMT",
                    FileTag = "_FractionalMT",
                    WriteData = FractionalMtWriter,
                    ReadData = FractionalMtReader
                },
                new BinaryArraySerializer
                {
                    DataArray = TotalMTOfZ,
                    Name = "TotalMTOfZ",
                    FileTag = "_TotalMTOfZ",
                    WriteData = TotalMtOfZWriter,
                    ReadData = TotalMtOfZReader
                },
                new BinaryArraySerializer
                {
                    DataArray = DynamicMTOfZ,
                    Name = "DynamicMTOfZ",
                    FileTag = "_DynamicMTOfZ",
                    WriteData = DynamicMtOfZWriter,
                    ReadData = DynamicMtOfZReader
                },
                new BinaryArraySerializer
                {
                    DataArray = SubregionCollisions,
                    Name = "SubregionCollisions",
                    FileTag = "_SubregionCollisions",
                    WriteData = SubRegionCollisionsWriter,
                    ReadData = SubRegionCollisionsReader
                },
                // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null : new BinaryArraySerializer 
                {
                    DataArray = TotalMTOfZSecondMoment,
                    Name = "TotalMTOfZSecondMoment",
                    FileTag = "_TotalMTOfZ_2",
                    WriteData = TotalMtOfZSecondMomentWriter,
                    ReadData = TotalMtOfZSecondMomentReader
                },
                new BinaryArraySerializer {
                    DataArray = DynamicMTOfZSecondMoment,
                    Name = "DynamicMTOfZSecondMoment",
                    FileTag = "_DynamicMTOfZ_2",
                    WriteData = DynamicMtOfZSecondMomentWriter,
                    ReadData = DynamicMtOfZSecondMomentReader,
                },
                new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = SecondMomentWriter,
                    ReadData = SecondMomentReader,
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
