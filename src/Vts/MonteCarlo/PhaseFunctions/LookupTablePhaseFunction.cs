using System;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class LookupTablePhaseFunction : IPhaseFunction
    {
        private Random _rng;
        private ILookupTablePhaseFunctionData _lutData;//shouldn't ILookupTablePhaseFunctionData have PDF and CDF and angles?

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
            double _phi = 0;
            double _theta = 0;
            if (_lutData.Name.Equals("PolarLookupTablePhaseFunctionData"))
            {
                var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData)_lutData;
                _phi = _rng.NextDouble() * 2 * Math.PI;//check if there is constant Pi
                double _mu = _rng.NextDouble();//random variable for picking theta

                _theta = Vts.Common.Math.Interpolation.interp1(pLookUpTablePhaseFunctionData.LutCdf, pLookUpTablePhaseFunctionData.LutAngles, _mu);
            }
            if (_lutData.Name.Equals("PolarAndAzimuthalLookupTablePhaseFunctionData"))
            {
                //TODO fill this out.
                return;
            }
            ScatterToThetaAndPhi(incomingDirectionToModify, _theta, _phi);
        }
        public void ScatterToThetaAndPhi(Direction incomingDirectionToModify, double theta, double phi)
        {
            var pLookUpTablePhaseFunctionData = (PolarLookupTablePhaseFunctionData)_lutData;

            double angle = 2 * Math.PI * _rng.NextDouble();
            double cosPsi = Math.Cos(angle);
            double sinPsi = Math.Sin(angle);

            double cosTh = Math.Cos(theta);
            double sinTh = Math.Sin(theta);
            double squz = Math.Sqrt(1 - incomingDirectionToModify.Uz * incomingDirectionToModify.Uz);
            double uxp, uyp, uzp;

            if (squz < 0.0001)
            {
                uxp = sinTh * cosPsi;
                uyp = sinTh * sinPsi;
                uzp = incomingDirectionToModify.Uz > 0 ? cosTh : -cosTh;
            }
            else
            {
                uxp = sinTh / squz * (incomingDirectionToModify.Ux * incomingDirectionToModify.Uz * cosPsi - incomingDirectionToModify.Uy * sinPsi) + incomingDirectionToModify.Ux * cosTh;
                uyp = sinTh / squz * (incomingDirectionToModify.Uy * incomingDirectionToModify.Uz * cosPsi + incomingDirectionToModify.Ux * sinPsi) + incomingDirectionToModify.Uy * cosTh;
                uzp = -sinTh * cosPsi * squz + incomingDirectionToModify.Uz * cosTh;
            }
            incomingDirectionToModify.Ux = uxp;
            incomingDirectionToModify.Uy = uyp;
            incomingDirectionToModify.Uz = uzp;
        }
    }
}