using System;
using System.Linq;
using MathNet.Numerics;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;Complex[,]&gt;.  Tally for reflectance as a function 
    /// of Fx and Time.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(ROfFxAndTimeDetector))]
    public class ROfFxAndTimeDetector : IDetector<Complex[,]>
    {
        private bool _tallySecondMoment;
        private double[] _fxArray;

        /// <summary>
        /// Returns an instance of ROfFxAndTimeDetector
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="time">time binning</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment info for error results</param>
        /// <param name="name">detector name</param>
        public ROfFxAndTimeDetector(DoubleRange fx, DoubleRange time, bool tallySecondMoment, String name)
        {
            Fx = fx;
            _fxArray = fx.AsEnumerable().ToArray();
            _tallySecondMoment = tallySecondMoment;
            Mean = new Complex[Fx.Count, time.Count - 1];
            SecondMoment = null;
            if (_tallySecondMoment)
            {
                SecondMoment = new Complex[Fx.Count, time.Count - 1];
            }
            TallyType = TallyType.ROfFxAndTime;
            Name = name;
            TallyCount = 0;
        }
        /// <summary>
        /// Returns a default instance of ROfFxAndTimeDetector (for serialization purposes only)
        /// </summary>
        public ROfFxAndTimeDetector()
            : this(new DoubleRange(),new DoubleRange(), true, TallyType.ROfFxAndTime.ToString())
        {
        }

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
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }

        /// <summary>
        /// Name of detector
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// number of times detector gets tallied to
        /// </summary>
        public long TallyCount { get; set; }

        /// <summary>
        /// fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }

        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }

        /// <summary>
        /// method to tally to detector
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        public void Tally(Photon photon)
        {
            var dp = photon.DP;

            var it = DetectorBinning.WhichBin(dp.TotalTime, Time.Count - 1, Time.Delta, Time.Start);
            // var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var x = dp.Position.X;
            for (int ifx = 0; ifx < _fxArray.Length; ++ifx)
            {
                double freq = _fxArray[ifx];

                var sinNegativeTwoPiFX = Math.Sin(-2 * Math.PI * freq * x);
                var cosNegativeTwoPiFX = Math.Cos(-2 * Math.PI * freq * x);

                /* convert to Hz-sec from MHz-ns 1e-6*1e9=1e-3 */
                // convert to Hz-sec from GHz-ns 1e-9*1e9=1
                var deltaWeight = dp.Weight * cosNegativeTwoPiFX + Complex.ImaginaryOne * sinNegativeTwoPiFX;
                Mean[ifx, it] += deltaWeight;
                if (_tallySecondMoment)
                {
                    var deltaWeight2 =
                        dp.Weight * dp.Weight * cosNegativeTwoPiFX * cosNegativeTwoPiFX +
                        Complex.ImaginaryOne * dp.Weight * dp.Weight * sinNegativeTwoPiFX * sinNegativeTwoPiFX;
                    // second moment of complex tally is square of real and imag separately
                    SecondMoment[ifx, it] += deltaWeight2;
                }
            }
            TallyCount++;
        }

        /// <summary>
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">Number of photons launched</param>
        public void Normalize(long numPhotons)
        {
            for (int ifx = 0; ifx < Fx.Count; ifx++)
            {
                for (int it = 0; it < Time.Count - 1; it++)
                {
                    Mean[ifx,it] /= numPhotons * Time.Delta;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ifx, it] /= numPhotons * Time.Delta;
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
