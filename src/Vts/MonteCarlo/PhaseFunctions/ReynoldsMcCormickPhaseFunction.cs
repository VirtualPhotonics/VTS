using System;
using Vts.Common;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class ReynoldsMcCormickPhaseFunction : IPhaseFunction
    {
        private double _g;
        private double _alpha;
        private Random _rng;

        public ReynoldsMcCormickPhaseFunction(double g, double alpha, Random rng)
        {
            _g = g;
            _alpha = alpha;
            _rng = rng;
        }

        /// <summary>
        /// Method to scatter photon based on the Henyey Greenstein phase-function
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        public void ScatterToNewDirection(Direction incomingDirectionToModify)
        {
            // readability eased with local copies of following
            double ux = incomingDirectionToModify.Ux;
            double uy = incomingDirectionToModify.Uy;
            double uz = incomingDirectionToModify.Uz;

            double cost, sint;    /* cosine and sine of theta */
            double cosp, sinp;    /* cosine and sine of phi */
            double psi;

            double Kpart1 = _alpha * _g * Math.Pow(1 - _g * _g, 2 * _alpha);
            double Kpart2 = 1 / (Math.Pow(1 + _g, 2 * _alpha) - Math.Pow(1 - _g, 2 * _alpha));
            double K = 2 * Kpart1 * Kpart2; // 2 factor to get (1/4pi) in front of entire equation 
            double dum = Math.Pow(1 + _g, -2 * _alpha);
            cost = -1 / (2 * _g) * (
                Math.Pow(_rng.NextDouble() * 2 * _g * _alpha / K + dum, -1 / _alpha) - 1 - _g * _g);

            //if (cost < -1) cost = -1;
            //else if (cost > 1) cost = 1;
            
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