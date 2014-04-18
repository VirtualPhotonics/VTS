using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common.Math;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers.DiscreteOrdinates;

namespace Vts.Modeling.ForwardSolvers
{
    public class DiscreteOrdinatesForwardSolver : ForwardSolverBase
    {
        public override IEnumerable<double> ROfTheta(IEnumerable<OpticalProperties> ops, IEnumerable<double> thetas)
        {
            // this method solves for R(μ) in an index-matched 1-dimensional half-space, where mu is cos(theta) 
            // just a place-holder until we 1) find the right place for lower-dimensional solvers and 
            // 2) get this set up for the full "1+2" SFD or "3+2" real domain solutions

            var muInterp = thetas.Select(theta => Math.Cos(theta));
            foreach (var op in ops)
            {
                // for now, base N on # of input mus, assuming it's even (temp)
                int n = 32;

                double g = op.G;

                var f = new[] { g, g * g, g * g * g, g * g * g * g };

                var coeffs = DOMethods.GaussLegendre(n);

                var el = DOMethods.GenFokkerPlanckEddington(f, coeffs.mu, coeffs.wt, n);

                var r = DOMethods.PWHalfSpace(op.Mua, op.Mus, coeffs.mu, coeffs.wt, el, n);

                var rInterp = Interpolation.interp1(coeffs.mu, r, muInterp);

                foreach (var ri in rInterp)
                {
                    yield return ri;
                }
            }
        }

        public override double ROfTheta(OpticalProperties op, double theta)
        {
            return ROfTheta(op.AsEnumerable(), theta.AsEnumerable()).FirstOrDefault();
        }
    }
}
