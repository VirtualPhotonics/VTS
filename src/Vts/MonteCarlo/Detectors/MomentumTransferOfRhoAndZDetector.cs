using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for MomentumTransfer(rho,z).
    /// </summary>
    [KnownType(typeof(MomentumTransferOfRhoAndZDetector))]
    public class MomentumTransferOfRhoAndZDetector : IDetector<double[,]> 
    {
        private bool _tallySecondMoment;

        /// <summary>
        /// constructor for momentum transfer as a function of rho and z detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment data for error results</param>
        /// <param name="name">detector name</param>
        public MomentumTransferOfRhoAndZDetector(
            DoubleRange rho, 
            DoubleRange z, 
            bool tallySecondMoment,
            String name)
        {
            Rho = rho;
            Z = z;
            Mean = new double[Rho.Count - 1, Z.Count - 1];
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            }
            TallyType = TallyType.MomentumTransferOfRhoAndZ;
            _tallySecondMoment = tallySecondMoment;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of MomentumTransferOfRhoAndZDetector (for serialization purposes only)
        /// </summary>
        public MomentumTransferOfRhoAndZDetector()
            : this(
            new DoubleRange(), 
            new DoubleRange(), 
            true, // tally SecondMoment
            TallyType.MomentumTransferOfRhoAndZ.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

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
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }

        public void Tally(Photon photon)
        {
            // todo: is this logically consistent at any place that could call Tally(photon)?
            Tally(photon.History.PreviousDP, photon.History.CurrentDP);
        }

        // collision tally
        public void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp)
        {
            //// comment out until we determine a way to accummulate momentum transfer
            //// calculate momentum transfer
            //double cosineBetweenTrajectories = Direction.GetDotProduct(previousDP.Direction, dp.Direction);

            //var momentumTransfer = 1 - cosineBetweenTrajectories;

            //// calculate the radial and time bins to attribute the deposition
            //var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            //var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            //Mean[ir, iz] += momentumTransfer;
            //if (_tallySecondMoment)
            //{
            //    SecondMoment[ir, iz] += momentumTransfer * momentumTransfer;
            //}
            //TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    // need to check that this normalization makes sense for momentum transfer
                    Mean[ir, iz] /=  areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
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