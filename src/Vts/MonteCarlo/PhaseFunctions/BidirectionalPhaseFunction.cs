using System;
using Vts.Common;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class BidirectionalPhaseFunction : IPhaseFunction
    {
        private double _g;
        private Random _rng;

        public BidirectionalPhaseFunction(double g, Random rng)
        {
            _g = g;
            _rng = rng;
        }

        /// <summary>
        /// Method to scatter bidirectionally
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify)
        {
            if (_rng.NextDouble() < ((1 + _g) / 2.0))
                incomingDirectionToModify.Uz *= 1.0;
            else
                incomingDirectionToModify.Uz *= -1.0;
        }
    }
}