using System;
using System.Linq;
using Vts.MonteCarlo;

namespace Vts.Modeling.ForwardSolvers
{
    public class MultiLayerSDAForwardSolver : ForwardSolverBase
    {
        public override double ROfRho(ITissueRegion[] regions, double rho)
        {
            return regions.Select(region => region.RegionOP.Musp - region.RegionOP.Mua).Average() * Math.Exp(-rho * rho);
        }
    }
}
