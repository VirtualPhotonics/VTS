using System;
using System.Linq;
using Vts.MonteCarlo;

namespace Vts.Modeling.ForwardSolvers
{
    public class TwoLayerSDAForwardSolver : ForwardSolverBase
    {
        public override double ROfRho(ITissueRegion[] regions, double rho)
        {
            double result;
            var 
            result = 0.118*phi(rho, 0) + 0.306*D1*dphi(rho, 0);
            return result;
        }
    }
}
