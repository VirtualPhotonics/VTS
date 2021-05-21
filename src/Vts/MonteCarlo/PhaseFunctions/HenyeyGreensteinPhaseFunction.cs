using System;
using Vts.Common;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class HenyeyGreensteinPhaseFunction : IPhaseFunction
    {
        private double _g;
        private Random _rng;

        public HenyeyGreensteinPhaseFunction(double g, Random rng)
        {
            _g = g;
            _rng = rng;
        }

        /// <summary>
        /// Method to scatter photon based on the Henyey Greenstein phase-function
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        /// <param name="currentRegionIndex">The tissue index of the current tissue region</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify)
        {
            // readability eased with local copies of following
            double ux = incomingDirectionToModify.Ux;
            double uy = incomingDirectionToModify.Uy;
            double uz = incomingDirectionToModify.Uz;

            //double g = _tissue.Regions[_tissueRegionIndex].RegionOP.G;
            double cost, sint;    /* cosine and sine of theta */
            double cosp, sinp;    /* cosine and sine of phi */
            double psi;

            if (_g == 0.0)
                cost = 2 * _rng.NextDouble() - 1;
            else
            {
                double temp = (1 - _g * _g) / (1 - _g + 2 * _g * _rng.NextDouble());
                cost = (1 + _g * _g - temp * temp) / (2 * _g);
                if (cost < -1) cost = -1;
                else if (cost > 1) cost = 1;
            }
            sint = Math.Sqrt(1.0 - cost * cost);

            psi = 2.0 * Math.PI * _rng.NextDouble();
            cosp = Math.Cos(psi);
            sinp = Math.Sin(psi);

            if (Math.Abs(incomingDirectionToModify.Uz) > (1 - 1e-10))
            {   /* normal incident. */
                incomingDirectionToModify.Ux = sint * cosp;
                incomingDirectionToModify.Uy = sint * sinp;
                incomingDirectionToModify.Uz = cost * incomingDirectionToModify.Uz / Math.Abs(incomingDirectionToModify.Uz);
            }
            else
            {
                double temp = Math.Sqrt(1.0 - uz * uz);
                incomingDirectionToModify.Ux = sint * (ux * uz * cosp - uy * sinp) / temp + ux * cost;
                incomingDirectionToModify.Uy = sint * (uy * uz * cosp + ux * sinp) / temp + uy * cost;
                incomingDirectionToModify.Uz = -sint * cosp * temp + uz * cost;
            }
        }
    }
}