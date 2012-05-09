using System;
using Vts.Common;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class LookupTablePhaseFunction : IPhaseFunction
    {
        private Random _rng;
        private ILookupTablePhaseFunctionData _lutData;

        public LookupTablePhaseFunction(Random rng, ILookupTablePhaseFunctionData lutData)
        {
            _rng = rng;
            _lutData = lutData;
        }

        /// <summary>
        /// Method to scatter based on a discretized lookup table
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify)
        {
            // do stuff with _rng and _lutData
        }
    }
}