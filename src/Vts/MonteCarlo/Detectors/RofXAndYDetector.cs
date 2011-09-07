using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements ISurfaceDetector&lt;double[,]&gt;.  Tally for reflectance as a function 
    /// of X and Y.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(ROfXAndYDetector))]
    public class ROfXAndYDetector : IDetector<double[,]> //ISurfaceDetector<double[,]>
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// Returns an instance of ROfXAndYDetector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ROfXAndYDetector(DoubleRange x, DoubleRange y, bool tallySecondMoment, String name)
        {
            X = x;
            Y = y;
            _tallySecondMoment = tallySecondMoment;
            Mean = new double[X.Count - 1, Y.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new double[X.Count - 1, Y.Count - 1];
            }
            TallyType = TallyType.ROfXAndY;
            Name = name;
            TallyCount = 0;
        }

        /// <summary>
        /// Returns a default instance of ROfXAndYDetector (for serialization purposes only)
        /// </summary>
        public ROfXAndYDetector()
            : this(new DoubleRange(), new DoubleRange(), true, TallyType.ROfXAndY.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange X { get; set; }

        public DoubleRange Y { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        public void Tally(PhotonDataPoint dp)
        {
            int ix = DetectorBinning.WhichBin(dp.Position.X, X.Count - 1, X.Delta, X.Start);
            int iy = DetectorBinning.WhichBin(dp.Position.Y, Y.Count - 1, Y.Delta, Y.Start);
            Mean[ix, iy] += dp.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ix, iy] += dp.Weight * dp.Weight;
            }
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = X.Delta * Y.Delta;
            for (int ix = 0; ix < X.Count - 1; ix++)
            {
                for (int iy = 0; iy < Y.Count - 1; iy++)
                {
                    Mean[ix, iy] /= normalizationFactor * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ix, iy] /= normalizationFactor * normalizationFactor * numPhotons;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
