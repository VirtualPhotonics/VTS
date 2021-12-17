using System;
using Vts.Common;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// bidirectional phase function
    /// </summary>
    public class BidirectionalPhaseFunction : IPhaseFunction
    {
        private double _g;
        private Random _rng;

        /// <summary>
        /// Bidirectional phase function
        /// </summary>
        /// <param name="g">anisotropy coefficient</param>
        /// <param name="rng">random number generator</param>
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