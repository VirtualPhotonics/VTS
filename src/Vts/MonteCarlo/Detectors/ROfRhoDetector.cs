using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetectory&lt;double[]&gt;.  Tally for reflectance as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(ROfRhoDetector))]
    public class ROfRhoDetector : IDetector<double[]> 
    {
        private bool _tallySecondMoment;
        //private double[,] _weightGrid;
        //private DoubleRange _xRange;
        //private DoubleRange _zRange;
        /// <summary>
        /// constructor for reflectance as a function of rho detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public ROfRhoDetector(DoubleRange rho, bool tallySecondMoment, String name)
        {
            Rho = rho;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[Rho.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1];
            }
            TallyType = TallyType.ROfRho;
            Name = name;
            TallyCount = 0;
            //_xRange = new DoubleRange(-0.4975, 0.4975, 200); // 5um pixel
            //_zRange = new DoubleRange(0.0, 0.995, 200);
            //_weightGrid = new double[_xRange.Count - 1, _zRange.Count - 1];
        }

        /// <summary>
        ///  Returns a default instance of ROfRhoDetector (for serialization purposes only)
        /// </summary>
        public ROfRhoDetector()
            : this(new DoubleRange(), true, TallyType.ROfRho.ToString())
        {
        }
        /// <summary>
        /// detector mean
        /// </summary>
        [IgnoreDataMember]
        public double[] Mean { get; set; }
        /// <summary>
        /// detector second moment
        /// </summary>
        [IgnoreDataMember]
        public double[] SecondMoment { get; set; }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user-specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(photon.DP.Position.X, photon.DP.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);

            Mean[ir] += photon.DP.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ir] += photon.DP.Weight * photon.DP.Weight;
            }
            TallyCount++;
            //// START of code that determines the forward P(V&D)=P(V)*P(D|V) using the first detector bin
            //// todo: move code out to appropriate place, comment out for now
            //// P(V) is photon weight at voxel, P(D|V) is weight change from voxel to out detector
            //// so P(V)*P(D|V) is exiting photon weight
            //// add final weight to all voxels, photon *visited* (could not have collided there)
            //if (ir==0)
            //{
            //    var prevCol = photon.History.HistoryData.First();
            //    var previx = DetectorBinning.WhichBin(prevCol.Position.X, _xRange.Count - 1, _xRange.Delta, _xRange.Start);
            //    var previz = DetectorBinning.WhichBin(prevCol.Position.Z, _zRange.Count - 1, _zRange.Delta, _zRange.Start);
            //    _weightGrid[previx, previz] += photon.DP.Weight;
            //    int ix, iz;
            //    foreach (var col in photon.History.HistoryData.Skip(1))
            //    {
            //        ix = DetectorBinning.WhichBin(col.Position.X, _xRange.Count - 1, _xRange.Delta, _xRange.Start);
            //        iz = DetectorBinning.WhichBin(col.Position.Z, _zRange.Count - 1, _zRange.Delta, _zRange.Start);
            //        // don't recollect if in same voxel
            //        // take care of case when only traversing changing dx's
            //        if ((ix != previx) && (iz == previz))
            //        {
            //            if (ix > previx)
            //            {
            //                for (int i = previx + 1; i <= ix; i++)
            //                {
            //                    _weightGrid[i, iz] += photon.DP.Weight;
            //                }
            //            }
            //            else
            //            {
            //                for (int i = previx - 1; i >= ix; i--)
            //                {
            //                    _weightGrid[i, iz] += photon.DP.Weight;
            //                }
            //            }

            //        }
            //            // take care of case when only traversing changing dz's
            //        else if ((ix == previx) && (iz != previz))
            //        {
            //            if (iz > previz)
            //            {
            //                for (int j = previz + 1; j <= iz; j++)
            //                {
            //                    _weightGrid[ix, j] += photon.DP.Weight;
            //                }
            //            }
            //            else
            //            {
            //                for (int j = previz - 1; j >= iz; j--)
            //                {
            //                    _weightGrid[ix, j] += photon.DP.Weight;
            //                }
            //            }
            //        }
            //            // if both changing need to determine intersection points
            //        else if ((ix != previx) && (iz != previz))
            //        {
            //            // algorithm found on playtechs.blogspot.com/2007/03/raytracing-on-grid.html
            //            int n = 1;
            //            double dtDx = 1.0/Math.Abs(col.Position.X - prevCol.Position.X);
            //            double dtDz = 1.0/Math.Abs(col.Position.Z - prevCol.Position.Z);
            //            int xInc, zInc;
            //            int newix = previx;
            //            int newiz = previz;
            //            double tNextHorizontal, tNextVertical;
            //            if (ix > previx)
            //            {
            //                xInc = 1;
            //                n += ix - previx;
            //                tNextHorizontal = (ix - previx)*dtDx;
            //            }
            //            else
            //            {
            //                xInc = -1;
            //                n += previx - ix;
            //                tNextHorizontal = (ix - previx)*dtDx;
            //            }
            //            if (iz > previz)
            //            {
            //                zInc = 1;
            //                n += iz - previz;
            //                tNextVertical = (iz - previz)*dtDz;
            //            }
            //            else
            //            {
            //                zInc = -1;
            //                n += previz - iz;
            //                tNextVertical = (iz - previz)*dtDz;
            //            }
            //            for (int i = n; i > 1; i--) // i>1 so to not do first index like above
            //            {
            //                if ((tNextVertical != 0.0) || (tNextHorizontal != 0.0))
            //                {
            //                    if (tNextVertical < tNextHorizontal)
            //                    {
            //                        newiz += zInc;
            //                        tNextVertical += dtDz;
            //                    }
            //                    else
            //                    {
            //                        newix += xInc;
            //                        tNextHorizontal += dtDx;
            //                    }
            //                    if (((newix >= 0) && (newix < _xRange.Count - 1)) &&
            //                        ((newiz >= 0) && (newiz < _zRange.Count - 1)))
            //                    {
            //                        _weightGrid[newix, newiz] += photon.DP.Weight;
            //                    }
            //                }
            //            }
            //        }
            //        previx = ix;
            //        previz = iz;
            //    }
            //}
            // END of P(V&D) code addition
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            // normalization accounts for Rho.Start != 0
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                Mean[ir] /= areaNorm * numPhotons;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir] /= areaNorm * areaNorm * numPhotons;
                }
            }
            /// normalization for P(V&D) code addition
            //for (int iz = 0; iz < _zRange.Count - 1; iz++)
            //{
            //    for (int ix = 0; ix < _xRange.Count - 1; ix++)
            //    {
            //        _weightGrid[ix, iz] /= numPhotons;
            //        Console.Write(" {0:E} ", _weightGrid[ix, iz]);
            //    }
            //    Console.WriteLine();
            //}
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
