using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// A Look-up Table Phase Function that uses the rejection sampling method to sample the polar and azimuthal angles.
    /// </summary>
    public class RejectionSampledLookupTablePhaseFunction : PolarAndAzimuthalPhaseFunction
        //comment: I'm not sure how to scatter without the stokes vector information when rejection sampling.  IPhaseFunction requires a
        //ScatterToNewDirection() method without the input of a stokes vector.  I know this class should probably implement the IPhaseFunction
        //interface, but I'm not sure how to go about doing that.
    {
        private Random _rng;
        private ILookupTablePhaseFunctionData _lutData;
        private MuellerMatrix _m;

        /// <summary>
        /// Constructor that initializes private member variables.
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <param name="lutData">Lookup table data</param>
        /// <param name="m">Mueller Matrix</param>
        public RejectionSampledLookupTablePhaseFunction(Random rng, ILookupTablePhaseFunctionData lutData, MuellerMatrix m)
        {
            _rng = rng;
            _lutData = lutData;
            _m = m;
        }

        /// <summary>
        /// Method to scatter based on a discretized lookup table.  Rotates the Stokes vector, s, appropriately as well.
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        /// <param name="s">Stokes Vector</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify, StokesVector s)
        {
            double phi = 0;
            double theta = 0;
            int theta_index;
            double rand1, rand2, rand3, temp = 0; 
            //var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData)_lutData;
            
            for (int i = 0; i < 5000; i++) {
        		rand1 = _rng.NextDouble();
        		rand2 = _rng.NextDouble();
        		rand3 = _rng.NextDouble();
		        phi = rand1*2*Math.PI;
		        theta = rand2*Math.PI;
		        theta_index = (int)Math.Ceiling(theta*180.0/Math.PI*4.0);
        		//Normalization = 1.3*pmax;
                temp = (_m.St11[theta_index] * Math.Sin(theta) * s.S0 + _m.S12[theta_index] * Math.Sin(theta) * (s.S1 * Math.Cos(2 * phi) + s.S2 * Math.Sin(2 * phi)));
                //Normalization;
		        if (rand3 > temp)
		            phi = -1.0;
		        if (phi > -1.0) 
                    break;
	        }
            s.Rotate(theta, phi, _m);
            Scatter(incomingDirectionToModify, theta, phi);
        }
    }
}