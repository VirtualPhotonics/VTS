using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for reflectance as a function 
    /// of X and Y.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    [KnownType(typeof(ROfXAndYDetector))]
    public class ROfXAndYDetector : IDetector<double[,]> 
    {
        private bool _tallySecondMoment;
        /// <summary>
        /// constructor for reflectance as a function of x and y detector input
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment information</param>
        /// <param name="name">detector name</param>
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
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user-specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// number of time detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }
        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            int ix = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);
            int iy = DetectorBinning.WhichBin(photon.DP.Position.Y, Y.Count - 1, Y.Delta, Y.Start);
            Mean[ix, iy] += photon.DP.Weight;
            if (_tallySecondMoment)
            {
                SecondMoment[ix, iy] += photon.DP.Weight * photon.DP.Weight;
            }
            TallyCount++;
        }

        /// <summary>
        /// method to normalize detector tally results
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
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
