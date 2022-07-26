using System;
using Meta.Numerics.Functions;

namespace Vts.Modeling
{
    /// <summary>
    /// Transforms linearly discrete data via a middle Riemann sum. This method is not mathematically sound, 
    /// but is a low order approximation.
    /// </summary>
    public class LinearDiscreteHankelTransform 
    {
        /// <summary>
        /// method to get rho values
        /// </summary>
        /// <param name="mua">absorption coefficient</param>
        /// <param name="musp">reduced scattering coefficient</param>
        /// <param name="drho">delta rho</param>
        /// <returns>array of rho values</returns>
        public static double[] GetRho(double mua, double musp, out double drho)
        {
            double  rMaxFactor = 25;
            double inverseDrFactor = 10;
            drho = 1/(musp + mua)/inverseDrFactor;
            int Nrho = (int)Math.Floor(rMaxFactor * inverseDrFactor);
            double[] rhoValues = new double[Nrho];
            for (int i = 0; i < Nrho; i++)
            {   
                rhoValues[i] = i * drho;
            }
            return rhoValues;
        }

        /// <summary>
        /// Calculate the Hankel Transform using a discrete Riemann middle sum
        /// </summary>
        /// <param name="rho">vector of discrete rho values</param>
        /// <param name="ROfRho">vector of discrete R(rho) values</param>
        /// <param name="drho">delta rho</param>
        /// <param name="fx">the spatial frequency at which to evaluate</param>
        /// <returns>Hankel Transform result</returns>
        public static double GetHankelTransform(double[] rho, double[] ROfRho, double drho, double fx)
        {
            if (rho.Length != ROfRho.Length)
            {
                throw new Meta.Numerics.DimensionMismatchException();
            }
            double sum = 0.0;
            for (int i = 0; i < rho.Length; i++)
            {
                sum += rho[i] * AdvancedMath.BesselJ(0, 2 * Math.PI * fx * rho[i]) *
                        ROfRho[i] * drho;
            }
            return 2 * Math.PI * sum;
        }
    }
}