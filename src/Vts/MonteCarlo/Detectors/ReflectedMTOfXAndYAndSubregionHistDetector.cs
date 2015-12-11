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
    /// Tally for ReflectedMT as a function of X and Y.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class ReflectedMTOfXAndYAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for ReflectedMT as a function of x and y and tissue subregion detector input
        /// </summary>
        public ReflectedMTOfXAndYAndSubregionHistDetectorInput()
        {
            TallyType = "ReflectedMTOfXAndYAndSubregionHist";
            Name = "ReflectedMTOfXAndYAndSubregionHist";
            X = new DoubleRange(-10.0, 10.0, 101);
            Y = new DoubleRange(-10.0, 10.0, 101);
            MTBins = new DoubleRange(0.0, 500.0, 51);

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true;
        }

        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        ///// <summary>
        ///// subregion index binning, needed by DetectorIO
        ///// </summary>
        //public int NumSubregions { get; set; }
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
            return new ReflectedMTOfXAndYAndSubregionHistDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallySecondMoment = this.TallySecondMoment,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                X = this.X,
                Y = this.Y,
                //NumSubregions = this.NumSubregions,
                MTBins = this.MTBins,
                FractionalMTBins = this.FractionalMTBins
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of X and Y and tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ReflectedMTOfXAndYAndSubregionHistDetector : Detector, IDetector
    {
        private ITissue _tissue;
        private IList<OpticalProperties> _ops;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
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
        public double[, ,] Mean { get; set; }

        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[, ,] SecondMoment { get; set; }

        /// <summary>
        /// fraction of MT spent in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[, , , ,] FractionalMT { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        /// <summary>
        /// number of Zs detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// Z binning
        /// </summary>
        public int NumSubregions { get; set; }

        public void Initialize(ITissue tissue, Random rng)
        {
            // intialize any necessary class fields here
            _tissue = tissue;
            _ops = _tissue.Regions.Select(r => r.RegionOP).ToArray();

            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;
            NumSubregions = _tissue.Regions.Count;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1] : null);
            // Fractional MT has FractionalMTBins.Count numnber of bins PLUS 2, one for =1, an d one for =0
            FractionalMT = FractionalMT ?? new double[X.Count - 1, Y.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1];
        }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            // calculate the radial bin to attribute the deposition
            var ix = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);
            var iy = DetectorBinning.WhichBin(photon.DP.Position.Y, Y.Count - 1, Y.Delta, Y.Start);

            var subregionMT = new double[NumSubregions];
            bool talliedMT = false;

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
                    // get angle between current and next
                    double cosineBetweenTrajectories = Direction.GetDotProduct(currentDP.Direction, nextDP.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    subregionMT[csr] += momentumTransfer;
                    talliedMT = true;
                }
                previousDP = currentDP;
                currentDP = nextDP;
            }
            // tally total MT
            double totalMT = subregionMT.Sum();
            if (totalMT > 0.0)  // only tally if momentum transfer accumulated
            {
                var imt = DetectorBinning.WhichBin(totalMT, MTBins.Count - 1, MTBins.Delta, MTBins.Start);
                Mean[ix, iy, imt] += photon.DP.Weight;
                if (TallySecondMoment)
                {
                    SecondMoment[ix, iy, imt] += photon.DP.Weight * photon.DP.Weight;
                }

                if (talliedMT) TallyCount++;

                // tally fractional MT in each subregion
                int ifrac;
                for (int isr = 0; isr < NumSubregions; isr++)
                {
                    // add 1 to ifrac to offset bin 0 added for =0 only tallies
                    ifrac = DetectorBinning.WhichBin(subregionMT[isr] / totalMT,
                        FractionalMTBins.Count - 1, FractionalMTBins.Delta, FractionalMTBins.Start) + 1;
                    // put identically 0 fractional MT into separate bin at index 0
                    if (subregionMT[isr] / totalMT == 0.0)
                    {
                        ifrac = 0;
                    }
                    // put identically 1 fractional MT into separate bin at index Count+1 -1
                    if (subregionMT[isr] / totalMT == 1.0)
                    {
                        ifrac = FractionalMTBins.Count;
                    }
                    FractionalMT[ix, iy, imt, isr, ifrac] += photon.DP.Weight;
                }
            }
        }

        /// <summary>
        /// method to normalize detector results after all photons launched
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            var areaNorm = X.Delta * Y.Delta;
            for (int ix = 0; ix < X.Count - 1; ix++)
            {
                for (int iy = 0; iy < Y.Count - 1; iy++)
                {
                    for (int imt = 0; imt < MTBins.Count - 1; imt++)
                    {
                        // normalize by area dx * dy and N
                        Mean[ix, iy, imt] /= areaNorm * numPhotons;
                        if (TallySecondMoment)
                        {
                            SecondMoment[ix, iy, imt] /= areaNorm * areaNorm * numPhotons;
                        }
                        for (int isr = 0; isr < NumSubregions; isr++)
                        {
                            for (int ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                            {
                                FractionalMT[ix, iy, imt, isr, ifrac] /= areaNorm * numPhotons;
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
                                for (int k = 0; k < MTBins.Count - 1; k++)
                                {                                
                                    binaryWriter.Write(Mean[i, j, k]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ X.Count - 1,  Y.Count - 1, MTBins.Count - 1];
                        for (int i = 0; i <  X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; i++) {
                                for (int k = 0; k < MTBins.Count - 1; k++)
                                {
                                    Mean[i, j, k] = binaryReader.ReadDouble();
                                }
                            }
                        }
                    }
                },
                new BinaryArraySerializer {
                    DataArray = FractionalMT,
                    Name = "FractionalMT",
                    FileTag = "_FractionalMT",
                    WriteData = binaryWriter => {
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < MTBins.Count - 1; k++) {
                                    for (int l = 0; l < NumSubregions; l++) {
                                        for (int m = 0; m < FractionalMTBins.Count + 1; m++)
                                        {
                                            binaryWriter.Write(FractionalMT[i, j, k, l, m]);
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        FractionalMT = FractionalMT ?? new double[ X.Count - 1, Y.Count, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1];
                        for (int i = 0; i <  X.Count - 1; i++) {    
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < MTBins.Count - 1; k++) {
                                    for (int l = 0; l < NumSubregions; l++) {
                                        for (int m = 0; m < FractionalMTBins.Count + 1; m++)
                                        {
                                            FractionalMT[i, j, k, l, m] = binaryReader.ReadDouble();
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
                            for (int j = 0; j < X.Count - 1; j++) {
                                for (int k = 0; k < MTBins.Count - 1; k++)
                                {
                                    binaryWriter.Write(SecondMoment[i, j, k]);
                                }
                            }
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ X.Count - 1, Y.Count, MTBins.Count - 1];
                        for (int i = 0; i < X.Count - 1; i++) {
                            for (int j = 0; j < Y.Count - 1; j++) {
                                for (int k = 0; k < MTBins.Count - 1; k++)
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
