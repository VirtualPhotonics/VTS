using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,,]&gt;.  Tally for reflected MomentumTransfer(rho,subregion,momentumtransfer).
    /// </summary>
    [KnownType(typeof(ReflectedMTOfRhoAndSubRegionHistDetector))]
    public class ReflectedMTOfRhoAndSubRegionHistDetector : IHistoryDetector<double[,,]> 
    {
        private bool _tallySecondMoment;
        private ITissue _tissue;

        /// <summary>
        /// constructor for momentum transfer as a function of rho and tissue subregion with histogram for MT detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="tissue">ITissue used to determine subregion binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment data for error results</param>
        /// <param name="name">detector name</param>
        public ReflectedMTOfRhoAndSubRegionHistDetector(
            DoubleRange rho, 
            DoubleRange momentumTransferBins,
            ITissue tissue, 
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            SubRegions = new DoubleRange(0, tissue.Regions.Count - 1);
            MTBins = momentumTransferBins;
            _tissue = tissue;
            Mean = new double[Rho.Count - 1, SubRegions.Count - 1, MTBins.Count - 1];
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, SubRegions.Count - 1, MTBins.Count - 1];
            }
            TallyType = TallyType.ReflectedMTOfRhoAndSubRegionHist;
            _tallySecondMoment = tallySecondMoment;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ReflectMTOfRhoAndSubRegionHistDetector (for serialization purposes only)
        /// </summary>
        public ReflectedMTOfRhoAndSubRegionHistDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(),
            new MultiLayerTissue(), 
            true, // tally SecondMoment
            TallyType.ReflectedMTOfRhoAndSubRegionHist.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,,] SecondMoment { get; set; }

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
        /// subregion binning
        /// </summary>
        public DoubleRange SubRegions { get; set; }
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }

        /// <summary>
        /// method to tally given two consecutive photon data points
        /// </summary>
        /// <param name="previousDP">previous data point</param>
        /// <param name="dp">current data point</param>
        /// <param name="currentRegionIndex">index of region photon current is in</param>
        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            //// calculate momentum transfer
            double cosineBetweenTrajectories = Direction.GetDotProduct(previousDP.Direction, dp.Direction);

            var momentumTransfer = 1 - cosineBetweenTrajectories;

            //// calculate the radial and time bins to attribute the deposition
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var isr = currentRegionIndex;
            var imt = DetectorBinning.WhichBin(momentumTransfer, MTBins.Count - 1, MTBins.Delta, MTBins.Start);

            Mean[ir, isr, imt] += momentumTransfer;
            if (_tallySecondMoment)
            {
                SecondMoment[ir, isr, imt] += momentumTransfer * momentumTransfer;
            }
            TallyCount++;
        }
        
        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int isr = 0; isr < SubRegions.Count - 1; isr++)
                {
                    for (int imt = 0; imt < MTBins.Count - 1; imt++)
                    {
                        // only normalize by area of surface area ring and N
                        var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                        Mean[ir, isr, imt] /= areaNorm*numPhotons;
                        if (_tallySecondMoment)
                        {
                            SecondMoment[ir, isr, imt] /= areaNorm * areaNorm * numPhotons;
                        }
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}