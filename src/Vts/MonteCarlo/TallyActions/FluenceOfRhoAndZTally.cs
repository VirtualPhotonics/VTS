
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITally<double[,]>.  Tally for Fluence(rho,z) reflectance.
    /// </summary>
    public class FluenceOfRhoAndZTally : ITally<double[,]>
    {
        private DoubleRange _rho;
        private DoubleRange _z;

        public FluenceOfRhoAndZTally(DoubleRange rho, DoubleRange z)
        {
            _rho = rho;
            _z = z;
            Mean = new double[_rho.Count, _z.Count];
            SecondMoment = new double[_rho.Count, _z.Count];
        }

        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), _rho.Count, _rho.Delta, _rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, _z.Count, _z.Delta, _z.Start);
            Mean[ir, iz] += dp.Weight;
            SecondMoment[ir, iz] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ir = 0; ir < _rho.Count; ir++)
            {
                for (int iz = 0; iz < _z.Count; iz++)
                {
                    Mean[ir, iz] /= numPhotons;
                }
            }
        }
        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

    }
}