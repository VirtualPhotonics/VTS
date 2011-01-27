using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for reflectance as a function 
    /// of X and Y.
    /// </summary>
    public class ROfXAndYTally : ITerminationTally<double[,]>
    {
        private DoubleRange _x;
        private DoubleRange _y;

        public ROfXAndYTally(DoubleRange x, DoubleRange y)
        {
            _x = x;
            _y = y;
            Mean = new double[_x.Count, _y.Count];
            SecondMoment = new double[_x.Count, _y.Count];
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            int ix = DetectorBinning.WhichBin(dp.Position.X, _x.Count, _x.Delta, _x.Start);
            int iy = DetectorBinning.WhichBin(dp.Position.Y, _y.Count, _y.Delta, _y.Start);
            Mean[ix, iy] += dp.Weight;
            SecondMoment[ix, iy] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ix = 0; ix < _x.Count; ix++)
            {
                for (int iy = 0; iy < _y.Count; iy++)
                {
                    Mean[ix, iy] /= _x.Delta * _y.Delta * numPhotons;
                }
            }
        }
    }
}
