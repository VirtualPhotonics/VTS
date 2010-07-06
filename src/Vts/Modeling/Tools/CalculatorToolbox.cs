using System;
using Meta.Numerics.Functions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Modeling
{
    /// <summary>
    /// Toolbox containing general method for the forward models. Most of these are for diffusion based 
    /// calculations.
    /// </summary>
    public class CalculatorToolbox
    {

        /// <summary>
        /// Implementation of the cubic polynomial via Scott Prahl thesis 1988, Appendix A2.9
        /// </summary>
        /// <param name="n">Refractive Index</param>
        /// <returns>Returns the boundary parameter A</returns>
        public static double GetCubicAParameter(double n)
        {
            return -0.13755 * n * n * n + 4.3390 * n * n - 4.90466 * n + 1.68960;
        }

        /// <summary>
        /// Get the radial location from the system origin to the position rho and z
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <returns>distance to (rho,z) coordinate from origin = (0,0)</returns>
        public static double GetRadius(double rho, double z)
        {
            return Math.Sqrt(rho * rho + z * z);
        }
        /// <summary>
        /// Overload of the (rho,z) dependent calculation for calculation of the radial distance from
        /// the position (x,y,z) to the origin (0,0,0)
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        /// <param name="z">z location (depth)</param>
        /// <returns>distance from coordinate (x,y,z) from the origin (0,0,0)</returns>
        public static double GetRadius(double x, double y, double z)
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Fresnel first reflection moment, via S. Prahl thesis 1988
        /// Polynomial expression, stated accurate for R1 and R2 better than 0.005.
        /// These polynomials should probably be replaced by the integral expresion! TO be DONE
        /// </summary>
        /// <param name="n">Refractive index</param>
        /// <returns>1st moment of the Fresnel reflectance</returns>
        public static double GetCubicFresnelReflectionMomentOfOrder1(double n)
        {
            return 0.7857 * n * n * n - 4.3259 * n * n + 8.26405 * n - 4.71306;
        }
        /// <summary>
        /// Fresnel second reflection moment, via S. Prahl thesis 1988.
        /// </summary>
        /// <param name="n">Refractive index</param>
        /// <returns>2nd moment of the Fresnel reflectance</returns>
        public static double GetCubicFresnelReflectionMomentOfOrder2(double n)
        {
            return -0.02043 * n * n * n - 0.38418 * n * n + 2.01132 * n - 1.62198;
        }

        /// <summary>
        /// Fresnel Reflection moment of abritrary integer order M via Gauss-Kronrod 
        /// numerical integration.
        /// </summary>
        /// <param name="M">Moment Order</param>
        /// <param name="RIamb">Ambient Refractive Index</param>
        /// <param name="RItis">Tissue Refractive Index</param>
        /// <returns>Mth Fresnel Reflection Moment</returns>
        public static double GetFresnelReflectionMomentOfOrderM(int M, double RIamb, double RItis) // need to modify for arbitrary incidence wrt RI's
        {
            var thetaCritical = RIamb / RItis;
            Meta.Numerics.Function<double, double> integrand = theta =>
            {
                return
                EvaluateFresnelReflectionCoefficientForUnpolarizedLight(theta, RItis, RIamb) *
                Math.Pow(Math.Cos(theta), M); // *Math.Sin(theta);
            };

            // equation modification according to S. Prahl thesis wrt the critical angle
            // in the integral of the Fresnel reflection coef for unpolarized light
            return
               FunctionMath.Integrate(integrand, Meta.Numerics.Interval.FromEndpoints(0, thetaCritical))
               * (M + 1) + Math.Pow(thetaCritical, M + 1);
        }

        /// <summary>
        /// Fresnel reflection coefficient for unpolarized light
        /// </summary>
        /// <param name="thetaIncident">incident angle with respect to the outward surface normal</param>
        /// <param name="riIncident">refractive index of incident (current) medium</param>
        /// <param name="riAmbient">refractive index of external ambient medium</param>
        /// <returns>Fresnel reflection coefficient</returns>
        private static double EvaluateFresnelReflectionCoefficientForUnpolarizedLight(
                double thetaIncident, double riIncident, double riAmbient)
        {
            var thetaAmbient = Math.Asin(riIncident / riAmbient * Math.Sin(thetaIncident));
            var ncosTis = riIncident * Math.Cos(thetaIncident);
            var ncosAmb = riAmbient * Math.Cos(thetaAmbient);
            return
                (((ncosAmb - ncosTis) / (ncosAmb + ncosTis)) * ((ncosAmb - ncosTis) / (ncosAmb + ncosTis)) +
                ((ncosTis - ncosAmb) / (ncosTis + ncosAmb)) * ((ncosTis - ncosAmb) / (ncosTis + ncosAmb))) / 2;
        }

        /// <summary>
        /// Allows distribution of any function exponentially in a linear direction
        /// </summary>
        /// <param name="func">function to be distributed, input is directional variable</param>
        /// <param name="muDecay">decay constant</param>
        /// <returns></returns>
        public static double EvaluateDistributedExponentialLineSourceIntegral
            (Func<double, double> func, double muDecay)
        {
            Meta.Numerics.Function<double, double> integrand = zp =>
            {
                return func(zp) *
                    Math.Exp(-muDecay * zp);
            };

            return
                FunctionMath.Integrate(
                    integrand,
                    Meta.Numerics.Interval.FromEndpoints(0.0, Double.PositiveInfinity))
                * muDecay;
        }

    }
}
