using System;
using Meta.Numerics.Functions;

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
        /// <param name="RIinc">Incident Refractive Index</param>
        /// <param name="RItrans">Transmitted Refractive Index</param>
        /// <returns>Mth Fresnel Reflection Moment</returns>
        public static double GetFresnelReflectionMomentOfOrderM(int M, double RIinc, double RItrans)
        {
            if (RIinc > RItrans)
            {
                return EvaluateFresnelReflectionMomentOfOrderMusingMu(M, RIinc, RItrans);
            }
            else
            {
                return EvaluateFresnelReflectionMomentOfOrderMusingTheta(M, RIinc, RItrans);
            }
        }

        private static double EvaluateFresnelReflectionMomentOfOrderMusingMu(int M, double RIinc, double RItrans)
        {
            var muCritical = Math.Sqrt(1 - RItrans * RItrans / (RIinc * RIinc));

            Meta.Numerics.Function<double, double> integrand = mu =>
            {
                return
                EvaluateFresnelReflectionCoefficientForUnpolarizedLightOfMu(mu, RIinc, RItrans) *
                Math.Pow(mu, M);
            };

            // equation modification according to S. Prahl thesis wrt the critical angle
            // in the integral of the Fresnel reflection coef for unpolarized light
            return
               FunctionMath.Integrate(integrand, Meta.Numerics.Interval.FromEndpoints(muCritical, 1))
               * (M + 1) + Math.Pow(muCritical, M + 1);
        }


        private static double EvaluateFresnelReflectionMomentOfOrderMusingTheta(int M, double RIinc, double RItrans)
        {
            var thetaCritical = Math.Asin(RItrans / RIinc);

            // equation modification according to S. Prahl thesis wrt the critical angle
            // in the integral of the Fresnel reflection coef for unpolarized light

            Meta.Numerics.Function<double, double> integrand = theta =>
            {
                return
                EvaluateFresnelReflectionCoefficientForUnpolarizedLight(theta, RIinc, RItrans) *
                Math.Pow(Math.Cos(theta), M) * Math.Sin(theta);
            };

            // equation modification according to S. Prahl thesis wrt the critical angle
            // in the integral of the Fresnel reflection coef for unpolarized light
            return
               FunctionMath.Integrate(integrand, Meta.Numerics.Interval.FromEndpoints(0, thetaCritical))
               * (M + 1) + Math.Pow(Math.Cos(thetaCritical), M + 1);
        }

        private static double EvaluateFresnelReflectionCoefficientForUnpolarizedLightOfMu(
            double mu, double RIinc, double RItrans)
        {
            var muTrans = Math.Sqrt(1 - RIinc * RIinc / (RItrans * RItrans) * (1 - mu * mu));
            var rPerp = (RIinc * mu - RItrans * muTrans) / (RIinc * mu + RItrans * muTrans);
            var rPar = (RIinc * muTrans - RItrans * mu) / (RIinc * muTrans + RItrans * mu);

            return (rPerp * rPerp + rPar * rPar) / 2;
        }
        // need to make this so it will work with arbitrary theta and arbitrary RIs
        private static double EvaluateFresnelReflectionCoefficientForUnpolarizedLight(
                double thetaIncident, double RIinc, double RItrans)
        {
            var thetaTrans = Math.Asin(RIinc / RItrans * Math.Sin(thetaIncident));
            return
                (Math.Pow(Math.Sin(thetaIncident - thetaTrans) / Math.Sin(thetaIncident + thetaTrans), 2) +
                Math.Pow(Math.Tan(thetaIncident - thetaTrans) / Math.Tan(thetaIncident + thetaTrans), 2)) / 2;
            //(((ncosAmb - ncosTis) / (ncosAmb + ncosTis)) * ((ncosAmb - ncosTis) / (ncosAmb + ncosTis)) +
            //((ncosTis - ncosAmb) / (ncosTis + ncosAmb)) * ((ncosTis - ncosAmb) / (ncosTis + ncosAmb))) / 2;
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
                    Meta.Numerics.Interval.FromEndpoints(0.0, Double.PositiveInfinity));
                //* muDecay;
        }

    }
}
