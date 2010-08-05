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
        //private double[,] _rOfXAndY;
        //private double[,] _rOfXAndYSecondMoment;

        public ROfXAndYTally(DoubleRange x, DoubleRange y)
        {
            _x = x;
            _y = y;
            Mean = new double[2 * _x.Count, 2 * _y.Count];
            SecondMoment = new double[2 * _x.Count, 2 * _y.Count];
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            int ix = DetectorBinning.WhichBin(dp.Position.X, 2 * _x.Count, _x.Delta, -(_x.Count * _x.Delta));
            int iy = DetectorBinning.WhichBin(dp.Position.Y, 2 * _y.Count, _y.Delta, -(_y.Count * _y.Delta));
            Mean[ix, iy] += dp.Weight;
            SecondMoment[ix, iy] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ix = 0; ix < 2 * _x.Count; ix++)
            {
                for (int iy = 0; iy < 2 * _y.Count; iy++)
                {
                    Mean[ix, iy] /= _x.Delta * _y.Delta * numPhotons;
                }
            }
        }

        //public double[,] Mean 
        //{
        //    get { return _rOfXAndY; }
        //}
        //public double[,] SecondMoment
        //{
        //    get { return _rOfXAndYSecondMoment; }
        //}
    }
}
