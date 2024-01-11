using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Tally for Transmitted dynamic MT as a function of Rho using blood volume fraction in each tissue region.
    /// This detector also tallies the total and dynamic MT as a function of Z.   If a random number is less
    /// than blood volume fraction for the tissue region in which the collision occurred, then hit blood and considered
    /// "dynamic" event.  Otherwise, it is a "static" event.
    /// This works for Analog and DAW processing.
    /// </summary>
    public class TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput : DetectorInput, IDetectorInput
    {
        /// <summary>
        /// constructor for TransmittedMT as a function of rho and tissue subregion detector input
        /// </summary>
        public TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput()
        {
            TallyType = "TransmittedDynamicMTOfRhoAndSubregionHist";
            Name = "TransmittedDynamicMTOfRhoAndSubregionHist";
            Rho = new DoubleRange(0.0, 10, 101);
            Z = new DoubleRange(0.0, 10, 101);
            MTBins = new DoubleRange(0.0, 500.0, 51);
            NA = double.PositiveInfinity; // set default NA completely open regardless of detector region refractive index
            FinalTissueRegionIndex = 2; // assume detector is in air below tissue

            // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsTransmittanceTally = true;
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
        /// <summary>
        /// Detector region index
        /// </summary>
        public int FinalTissueRegionIndex { get; set; }
        /// <summary>
        /// numerical aperture
        /// </summary>
        public double NA { get; set; }
        /// <summary>
        /// number of dynamic and static collisions in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] SubregionCollisions { get; set; }

        /// <summary>
        /// Method to create detector from detector input
        /// </summary>
        /// <returns>created IDetector</returns>
        public IDetector CreateDetector()
        {
            return new TransmittedDynamicMTOfRhoAndSubregionHistDetector
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
                FractionalMTBins = this.FractionalMTBins,
                NA = this.NA,
                FinalTissueRegionIndex = this.FinalTissueRegionIndex
            };
        }
    }

    /// <summary>
    /// Implements IDetector.  Tally for momentum transfer as a function  of Rho and tissue subregion
    /// using blood volume fraction in each tissue subregion.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TransmittedDynamicMTOfRhoAndSubregionHistDetector : Detector, IDetector
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
        /// <summary>
        /// number of dynamic and static collisions in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] SubregionCollisions { get; set; }

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

            SubregionCollisions = new double[NumSubregions, 2]; // 2nd index: 0=static, 1=dynamic  

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

            // calculate the radial bin to attribute the deposition
            var irho = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var tissueMt = new double[2]; // 2 is for [static, dynamic] tally separation
            bool talliedMt = false;
            double totalMt = 0;
            var totalMtOfZForOnePhoton = new double[Rho.Count - 1, Z.Count - 1];
            var dynamicMtOfZForOnePhoton = new double[Rho.Count - 1, Z.Count - 1];

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
                    double cosineBetweenTrajectories = Direction.GetDotProduct(currentDp.Direction, nextDp.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    totalMt += momentumTransfer;
                    TotalMTOfZ[irho, iz] += photon.DP.Weight * momentumTransfer;
                    totalMtOfZForOnePhoton[irho, iz] += photon.DP.Weight * momentumTransfer;
                    if (_rng.NextDouble() < _bloodVolumeFraction[csr]) // hit blood 
                    {
                        tissueMt[1] += momentumTransfer;
                        DynamicMTOfZ[irho, iz] += photon.DP.Weight * momentumTransfer;
                        dynamicMtOfZForOnePhoton[irho, iz] += photon.DP.Weight * momentumTransfer;
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
            var imt = DetectorBinning.WhichBin(totalMt, MTBins.Count - 1, MTBins.Delta, MTBins.Start);
            Mean[irho, imt] += photon.DP.Weight;
            if (TallySecondMoment)
            {
                SecondMoment[irho, imt] += photon.DP.Weight * photon.DP.Weight; 
                for (var i = 0; i < Rho.Count - 1; i++)
                {
                    for (var j = 0; j < Z.Count - 1; j++)
                    {
                        TotalMTOfZSecondMoment[i, j] += totalMtOfZForOnePhoton[i, j] * totalMtOfZForOnePhoton[i, j];
                        DynamicMTOfZSecondMoment[i, j] += dynamicMtOfZForOnePhoton[i, j] * dynamicMtOfZForOnePhoton[i, j];
                    }
                }                   
            }

            if (talliedMt) TallyCount++;

            // tally DYNAMIC fractional MT in each subregion
            int ifrac;
            for (var isr = 0; isr < NumSubregions; isr++)
            {
                // add 1 to ifrac to offset bin 0 added for =0 only tallies
                ifrac = DetectorBinning.WhichBin(tissueMt[1] / totalMt,
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
            for (var ir = 0; ir < Rho.Count - 1; ir++)
            {
                // normalize by area of surface area ring and N
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                for (var imt = 0; imt < MTBins.Count - 1; imt++)
                {
                    Mean[ir, imt] /= areaNorm * numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ir, imt] /= areaNorm * areaNorm * numPhotons;
                    }
                    for (var ifrac = 0; ifrac < FractionalMTBins.Count + 1; ifrac++)
                    {
                        FractionalMT[ir, imt, ifrac] /= areaNorm * numPhotons;
                    } 
                }
                for (var iz = 0; iz < Z.Count - 1; iz++)
                {
                    TotalMTOfZ[ir, iz] /= areaNorm * numPhotons;
                    DynamicMTOfZ[ir, iz] /= areaNorm * numPhotons;
                    if (!TallySecondMoment) continue;
                    TotalMTOfZSecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
                    DynamicMTOfZSecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
                }
            }
        }

        /// <summary>
        /// this is to allow saving of large arrays separately as a binary file
        /// </summary>
        /// <returns>BinaryArraySerializer[]</returns>
        public BinaryArraySerializer[] GetBinarySerializers()
        {
            Mean ??= new double[Rho.Count - 1, Z.Count - 1];
            FractionalMT ??= new double[Rho.Count - 1, MTBins.Count - 1, FractionalMTBins.Count + 1];
            TotalMTOfZ ??= new double[Rho.Count - 1, Z.Count - 1];
            DynamicMTOfZ ??= new double[Rho.Count - 1, Z.Count - 1];
            SubregionCollisions ??= new double[NumSubregions, 2];
            if (TallySecondMoment)
            {
                SecondMoment ??= new double[Rho.Count - 1, Z.Count - 1];
                TotalMTOfZSecondMoment ??= new double[Rho.Count - 1, Z.Count - 1];
                DynamicMTOfZSecondMoment ??= new double[Rho.Count - 1, Z.Count - 1];
            }
            var allSerializers = new List<BinaryArraySerializer>
            {
                BinaryArraySerializerFactory.GetSerializer(
                    Mean, "Mean", ""),
                BinaryArraySerializerFactory.GetSerializer(
                    FractionalMT, "FractionalMT", "_FractionalMT"),
                BinaryArraySerializerFactory.GetSerializer(
                    TotalMTOfZ, "TotalMTOfZ","_TotalMTOfZ"),
                BinaryArraySerializerFactory.GetSerializer(
                    DynamicMTOfZ, "DynamicMTOfZ","_DynamicMTOfZ"),
                BinaryArraySerializerFactory.GetSerializer(
                    SubregionCollisions,"SubregionCollisions","_SubregionCollisions"),
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    SecondMoment, "SecondMoment", "_2") : null,
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    TotalMTOfZSecondMoment, "TotalMTOfZSecondMoment", "_TotalMTOfZ_2") : null,
                TallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                    DynamicMTOfZSecondMoment, "DynamicMTOfZSecondMoment", "_DynamicMTOfZ_2") : null
            };
            return allSerializers.Where(s => s is not null).ToArray();
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
                return photon.DP.IsWithinNA(NA, Direction.AlongPositiveZAxis, detectorRegionN);
            }
            else // determine n of prior tissue region
            {
                var detectorRegionN = _tissue.Regions[FinalTissueRegionIndex].RegionOP.N;
                return photon.History.PreviousDP.IsWithinNA(NA, Direction.AlongPositiveZAxis, detectorRegionN);
            }
        }

    }
}
