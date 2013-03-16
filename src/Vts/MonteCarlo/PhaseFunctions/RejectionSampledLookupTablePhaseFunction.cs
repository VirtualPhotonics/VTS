using System;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class RejectionSampledLookupTablePhaseFunction : PolarAndAzimuthalRejectionSampledLookUpTablePhaseFunction
    {
        private Random _rng;
        private ILookupTablePhaseFunctionData _lutData;//shouldn't ILookupTablePhaseFunctionData have PDF and CDF and angles?
        private MuellerMatrix _m;
        //private double[] _st11, _s12;

        public RejectionSampledLookupTablePhaseFunction(Random rng, ILookupTablePhaseFunctionData lutData, MuellerMatrix m)
        {
            _rng = rng;
            _lutData = lutData;
            _m = m;
        }

        /// <summary>
        /// Method to scatter based on a discretized lookup table
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify, StokesVector s)
        {
            double phi = 0;
            double theta = 0;
            int theta_index;
            double rand1, rand2, rand3, temp = 0; 
            var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData)_lutData;
            
            for (int i = 0; i < 5000; i++) {
        		rand1 = _rng.NextDouble();
        		rand2 = _rng.NextDouble();
        		rand3 = _rng.NextDouble();
		        phi = rand1*2*Math.PI;
		        theta = rand2*Math.PI;
		        theta_index = (int)Math.Ceiling(theta*180.0/Math.PI*4.0);
        		//Normalization = 1.3*pmax;
                temp = (_m.St11[theta_index] * Math.Sin(theta) * s.S0 + _m.S12[theta_index] * Math.Sin(theta) * (s.S1 * Math.Cos(2 * phi) + s.S2 * Math.Sin(2 * phi)));
                    ///Normalization;
		        if (rand3 > temp)
		            phi = -1.0;
		        if (phi > -1.0) 
                    break;
	        }
            s.rotate(theta, phi, _m);
            Scatter(incomingDirectionToModify, theta, phi);
        }
    }
}