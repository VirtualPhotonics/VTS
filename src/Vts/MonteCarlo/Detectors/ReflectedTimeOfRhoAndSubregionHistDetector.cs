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
    /// Implements IDetector&lt;double[,,]&gt;.  Tally for reflected Time(rho,subregion,time).
    /// Currently, the tally only works for Continuous Absorption Weighting (CAW)
    /// </summary>
    [KnownType(typeof(ReflectedTimeOfRhoAndSubregionHistDetector))]
    public class ReflectedTimeOfRhoAndSubregionHistDetector : IDetector<double[,,]> 
    {
        private bool _tallySecondMoment;
        private ITissue _tissue;

        /// <summary>
        /// constructor for reflected time as a function of rho and tissue subregion detector 
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="time">time binning</param>
        /// <param name="tissue">ITissue used to determine subregion binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment data for error results</param>
        /// <param name="name">detector name</param>
        public ReflectedTimeOfRhoAndSubregionHistDetector(
            DoubleRange rho, 
            DoubleRange time,
            ITissue tissue, 
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            _tissue = tissue;
            SubregionIndices = new IntRange(0, _tissue.Regions.Count - 1, _tissue.Regions.Count); // needed for DetectorIO
            Time = time;
            Mean = new double[Rho.Count - 1, SubregionIndices.Count, Time.Count - 1];
            FractionalTime = new double[Rho.Count -1, SubregionIndices.Count];
            _tallySecondMoment = tallySecondMoment;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, SubregionIndices.Count, Time.Count - 1];
            }
            TallyType = TallyType.ReflectedTimeOfRhoAndSubregionHist; 
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ReflectTimeOfRhoAndSubRegionDetector (for serialization purposes only)
        /// </summary>
        public ReflectedTimeOfRhoAndSubregionHistDetector()
            : this(
            new DoubleRange(0.0, 10.0, 101), 
            new DoubleRange(0.0, 1.0, 101),
            new MultiLayerTissue(), 
            true, // tally SecondMoment
            TallyType.ReflectedTimeOfRhoAndSubregionHist.ToString())
        {
        }
        /// <summary>
        /// mean of tally
        /// </summary>
        [IgnoreDataMember]
        public double[,,] Mean { get; set; }
        /// <summary>
        /// 2nd moment of tally
        /// </summary>
        [IgnoreDataMember]
        public double[,,] SecondMoment { get; set; }
        /// <summary>
        /// fraction of time spent in each subregion
        /// </summary>
        [IgnoreDataMember]
        public double[,] FractionalTime { get; set; }

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
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }

        /// <summary>
        /// method to tally reflected photon by determining cumulative MT in each tissue subregion and binning in MT
        /// </summary>
        /// <param name="photon">Photon (includes HistoryData)</param>
        public void Tally(Photon photon)
        {
            // calculate the radial and time bin of reflected photon
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            
            for (int i = 0; i < SubregionIndices.Count; i++)
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
                    if (_tallySecondMoment)
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
            var totalTimeForThisSubregion = new double[Rho.Count - 1,SubregionIndices.Count];
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var totalTimeOverAllSubregions = 0.0;
                for (int isr = 0; isr < SubregionIndices.Count - 1; isr++)
                {
                    totalTimeForThisSubregion[ir, isr] =
                        Enumerable.Range(0, Mean.GetLength(2)).Sum(i => Mean[ir, isr, i]);
                    for (int it = 0; it < Time.Count - 1; it++)
                    {
                        totalTimeOverAllSubregions += Mean[ir, isr, it];
                        // normalize by area of surface area ring and N
                        var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                        Mean[ir, isr, it] /= areaNorm *numPhotons;
                        if (_tallySecondMoment)
                        {
                            SecondMoment[ir, isr, it] /= areaNorm * areaNorm * numPhotons;
                        }
                    }
                }
                for (int isr = 0; isr < SubregionIndices.Count - 1; isr++)
                {
                    FractionalTime[ir, isr] =  totalTimeForThisSubregion[ir, isr] / totalTimeOverAllSubregions;
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