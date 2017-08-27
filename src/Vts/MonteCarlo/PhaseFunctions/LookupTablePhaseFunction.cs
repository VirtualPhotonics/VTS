using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// A class that describes a tabulated phase function for a POLAR angle and uniform azimuthal scattering angle.
    /// </summary>
    /// <param name="_lutData">Stores the polar angles and phase function values evaluated at those polar angles.</param>
    /// <param name="_rng">Random number generator.</param>
    /// CKH 8/25/17 should this be named PolarPhaseFunction?  And why does it inherit PolarAndAzi...?
    public class LookupTablePhaseFunction : PolarAndAzimuthalPhaseFunction, IPhaseFunction
    {
        private Random _rng;
        //private ILookupTablePhaseFunctionData _lutData; 

        /// <summary>
        /// Constructor that intializes private member variables.
        /// </summary>
        public LookupTablePhaseFunction(ILookupTablePhaseFunctionData lutData, Random rng)
        {
            _rng = rng;
            //_lutData = lutData;
            LutData = lutData;
        }

        public ILookupTablePhaseFunctionData LutData; // CKH add 8/24/17
        /// <summary>
        /// Method to scatter based on a discretized lookup table
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify)
        {
            double phi = 0;
            double theta = 0;
            var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData) LutData; // _lutData; 
            phi = _rng.NextDouble() * 2 * Math.PI;
            double mu = _rng.NextDouble();//random variable for picking theta
            theta = Vts.Common.Math.Interpolation.interp1(pLookUpTablePhaseFunctionData.LutCdf, pLookUpTablePhaseFunctionData.LutAngles, mu);
            Scatter(incomingDirectionToModify, theta, phi);
        }
    }
}