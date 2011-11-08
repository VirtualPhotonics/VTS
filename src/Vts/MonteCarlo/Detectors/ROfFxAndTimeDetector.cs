using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;
using System.Numerics;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for reflectance as a function 
    /// of Fx and Time.
    /// This implementation works for Analog, DAW and CAW.
    /// </summary>
    [KnownType(typeof(ROfFxAndTimeDetector))]
    public class ROfFxAndTimeDetector : IDetector<Complex[,]>
    {
        private bool _tallySecondMoment;
        private double[] _fxArray;

        /// <summary>
        /// Returns an instance of ROfFxDetector
        /// </summary>
        /// <param name="fx"></param>
        /// <param name="time"></param>
        /// <param name="tallySecondMoment"></param>
        /// <param name="name"></param>
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
            TallyType = TallyType.ROfRhoAndOmega;
            Name = name;
            TallyCount = 0;
        }
        /// <summary>
        /// Returns a default instance of ROfFxDetector (for serialization purposes only)
        /// </summary>
        public ROfFxAndTimeDetector()
            : this(new DoubleRange(),new DoubleRange(), true, TallyType.ROfFxAndTime.ToString())
        {
        }

        [IgnoreDataMember]
        public Complex[,] Mean { get; set; }

        [IgnoreDataMember]
        public Complex[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Fx { get; set; }

        public DoubleRange Time { get; set; }

        public void Tally(Photon photon)
        {
            Tally(photon.DP);
        }
        public void Tally(PhotonDataPoint dp)
        {
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

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true; // or, possibly test for NA or confined position, etc
            //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
        }
    }
}
