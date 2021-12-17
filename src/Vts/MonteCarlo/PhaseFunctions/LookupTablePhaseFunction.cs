using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// A class that describes a tabulated phase function for a POLAR angle and uniform azimuthal scattering angle.
    /// In ScatterToNewDirection the polar angle theta is determined from lutCdf and the azimuthal angle phi is determine
    /// from Unif[0,2pi].  With theta, phi determined, the Scatter function in PolarAndAzimuthalPhaseFunction is then 
    /// called to determine updated direction cosines of photon.

    public class LookupTablePhaseFunction : PolarAndAzimuthalPhaseFunction, IPhaseFunction
    {
        private readonly Random _rng;
        private readonly ILookupTablePhaseFunctionData _lutData;

        /// <summary>
        /// Constructor that initializes private member variables.
        /// </summary>
        /// <param name="lutData">Stores the polar angles and phase function values evaluated at those polar angles.</param>
        /// <param name="rng">Random number generator.</param>
        public LookupTablePhaseFunction(ILookupTablePhaseFunctionData lutData, Random rng)
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
            var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData) _lutData; 
            phi = _rng.NextDouble() * 2 * Math.PI;
            double mu = _rng.NextDouble();//random variable for picking theta
            theta = Vts.Common.Math.Interpolation.interp1(pLookUpTablePhaseFunctionData.LutCdf, pLookUpTablePhaseFunctionData.LutAngles, mu);
            Scatter(incomingDirectionToModify, theta, phi);
        }
    }
}