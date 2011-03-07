using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[,]>.  Tally for reflectance as a function 
    /// of X and Y.
    /// This implementation works for Analog, DAW and CAW processing.
    /// </summary>
    public class ROfXAndYTally : ITerminationTally<double[,]>
    {
        private DoubleRange _x;
        private DoubleRange _y;

        public ROfXAndYTally(DoubleRange x, DoubleRange y)
        {
            _x = x;
            _y = y;
            Mean = new double[_x.Count - 1, _y.Count - 1];
            SecondMoment = new double[_x.Count - 1, _y.Count - 1];
            TallyType = TallyType.ROfXAndY;
        }

        public double[,] Mean { get; set; }
        public double[,] SecondMoment { get; set; }
        public TallyType TallyType { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            int ix = DetectorBinning.WhichBin(dp.Position.X, _x.Count - 1, _x.Delta, _x.Start);
            int iy = DetectorBinning.WhichBin(dp.Position.Y, _y.Count - 1, _y.Delta, _y.Start);
            Mean[ix, iy] += dp.Weight;
            SecondMoment[ix, iy] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = _x.Delta * _y.Delta * numPhotons;
            for (int ix = 0; ix < _x.Count - 1; ix++)
            {
                for (int iy = 0; iy < _y.Count - 1; iy++)
                {
                    Mean[ix, iy] /= normalizationFactor;
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }
    }
}
