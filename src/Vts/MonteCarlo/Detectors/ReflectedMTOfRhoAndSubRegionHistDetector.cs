using System;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for reflected MomentumTransfer(rho,momentumtransfer).
    /// Also tallies FractionalMT(rho,momentumtransfer,subregion), the average fractional MT each photon tallied
    /// in each subregiion.
    /// </summary>
    [KnownType(typeof(ReflectedMTOfRhoAndSubregionHistDetector))]
    public class ReflectedMTOfRhoAndSubregionHistDetector : IDetector<double[,]> 
    {
        private bool _tallySecondMoment;
        private ITissue _tissue;
        private DoubleRange _fractionalMTBins;

        /// <summary>
        /// constructor for momentum transfer as a function of rho and tissue subregion with histogram for MT detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="momentumTransferBins">bins for Momentum Transfer</param>
        /// <param name="tissue">ITissue used to determine subregion binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment data for error results</param>
        /// <param name="name">detector name</param>
        public ReflectedMTOfRhoAndSubregionHistDetector(
            DoubleRange rho, 
            DoubleRange momentumTransferBins,
            DoubleRange fractionalMTBins,
            ITissue tissue, 
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            _tissue = tissue;
            SubregionIndices = new IntRange(0, _tissue.Regions.Count - 1, _tissue.Regions.Count); // needed for DetectorIO
            MTBins = momentumTransferBins;
            Mean = new double[Rho.Count - 1, MTBins.Count - 1];
            _fractionalMTBins = fractionalMTBins;
            FractionalMT = new double[Rho.Count - 1,MTBins.Count - 1,SubregionIndices.Count, _fractionalMTBins.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, MTBins.Count - 1];
            }
            TallyType = TallyType.ReflectedMTOfRhoAndSubregionHist; 
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ReflectMTOfRhoAndSubRegionHistDetector (for serialization purposes only)
        /// </summary>
        public ReflectedMTOfRhoAndSubregionHistDetector()
            : this(
            new DoubleRange(0.0, 10.0, 101), 
            new DoubleRange(),
            new DoubleRange(0.0, 1.0, 11), 
            new MultiLayerTissue(), 
            true, // tally SecondMoment
            TallyType.ReflectedMTOfRhoAndSubregionHist.ToString())
        {
        }
        /// <summary>
        /// mean of tally
        /// </summary>
        [IgnoreDataMember]
        public double[,] Mean { get; set; }
        /// <summary>
        /// 2nd moment of tally
        /// </summary>
        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        /// <summary>
        /// fraction of MT spent in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,,,] FractionalMT { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// name of detector, default uses TallyType 
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times this detector gets tallied 
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// subregion index binning, needed by DetectorIO
        /// </summary>
        public IntRange SubregionIndices { get; set; } 
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }

        /// <summary>
        /// method to tally reflected photon by determining cumulative MT in each tissue subregion and binning in MT
        /// </summary>
        /// <param name="photon">Photon (includes HistoryData)</param>
        public void Tally(Photon photon)
        {
            // calculate the radial bin to attribute the deposition
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y),
                                              Rho.Count - 1, Rho.Delta, Rho.Start);

            var subregionMT = new double[SubregionIndices.Count];
            bool talliedMT = false;

            // go through photon history and calculate momentum transfer
            // assumes that no MT tallied at pseudo-collisions (reflections and refractions)
            // this algorithm needs to look ahead to angle of next DP, but needs info from previous to determine whether real or pseudo-collision
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            PhotonDataPoint currentDP = photon.History.HistoryData.Skip(1).Take(1).First();
            foreach (PhotonDataPoint nextDP in photon.History.HistoryData.Skip(2))
            {
                if (previousDP.Weight != currentDP.Weight) // only for true collision points
                {
                    var isr = _tissue.GetRegionIndex(currentDP.Position); // get current region index
                    // get angle between current and next
                    double cosineBetweenTrajectories = Direction.GetDotProduct(currentDP.Direction, nextDP.Direction);
                    var momentumTransfer = 1 - cosineBetweenTrajectories;
                    subregionMT[isr] += momentumTransfer;
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
                Mean[ir, imt] += photon.DP.Weight;
                SecondMoment[ir, imt] += photon.DP.Weight * photon.DP.Weight;

                if (talliedMT) TallyCount++;

                // tally fractional MT in each subregion
                for (int isr = 0; isr < SubregionIndices.Count; isr++)
                {
                    var ifrac = DetectorBinning.WhichBin(subregionMT[isr] / totalMT,
                                                         _fractionalMTBins.Count - 1, _fractionalMTBins.Delta, _fractionalMTBins.Start);
                    FractionalMT[ir, imt, isr, ifrac] += photon.DP.Weight;         
                }
            }
        }
        
        /// <summary>
        /// method to normalize tally
        /// </summary>
        /// <param name="numPhotons">number of photons launched from source</param>
        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int imt = 0; imt < MTBins.Count - 1; imt++)
                {
                    // normalize by area of surface area ring and N
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, imt] /= areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, imt] /= areaNorm * areaNorm * numPhotons;
                    }
                    for (int isr = 0; isr < SubregionIndices.Count; isr++)
                    {
                        for (int ifrac = 0; ifrac < _fractionalMTBins.Count - 1; ifrac++)
                        {
                            FractionalMT[ir, imt, isr, ifrac] /= areaNorm*numPhotons;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// method to determine if detector contains point, set to always be true for now (not used).
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}