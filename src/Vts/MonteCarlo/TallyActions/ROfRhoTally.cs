using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for reflectance as a function 
    /// of Rho.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfRhoTally : ITerminationTally<double[]>
    {
        private DoubleRange _rho;

        public ROfRhoTally(DoubleRange rho)
        {
            _rho = rho;
            Mean = new double[_rho.Count - 1];
            SecondMoment = new double[_rho.Count - 1];
        }

        public double[] Mean { get; set; }
        public double[] SecondMoment { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count - 1, _rho.Delta, _rho.Start);

            Mean[ir] += dp.Weight;
            SecondMoment[ir] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count - 1; ir++)
            {
                Mean[ir] /= 2.0 * Math.PI * (ir + 0.5) * _rho.Delta * _rho.Delta * numPhotons;
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

    }
}
