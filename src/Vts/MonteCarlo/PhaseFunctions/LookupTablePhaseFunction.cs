using System;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class LookupTablePhaseFunction : PolarAndAzimuthalRejectionSampledLookUpTablePhaseFunction
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
            double phi = 0;
            double theta = 0;
            var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData)_lutData;
            phi = _rng.NextDouble() * 2 * Math.PI;
            double mu = _rng.NextDouble();//random variable for picking theta
            theta = Vts.Common.Math.Interpolation.interp1(pLookUpTablePhaseFunctionData.LutCdf, pLookUpTablePhaseFunctionData.LutAngles, mu);
            Scatter(incomingDirectionToModify, theta, phi);
        }
    }
}