using System;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Provides common optics methods (e.g. Fresnel, Specular)
    /// </summary>
    public class Optics
    {
        private const double COS90D = 1.0E-6;
        private const double COSZERO = (1.0 - 1e-12);
        /// <summary>
        /// Fresnel calculates the reflection coefficient when light moves from one medium to
        /// another
        /// </summary>
        /// <param name="n1">refractive index of current medium</param>
        /// <param name="n2">refractive index of next medium</param>
        /// <param name="ci">cosine of the angle with interface normal</param>
        /// <param name="uz_snell">cosine of the transmitted light with interface normal</param>
        /// <returns>reflection probability</returns>
        public static double Fresnel(double n1, double n2, double ci, out double uz_snell)
        {
            double ct, si, st;  /* cos(thi), cos(tht), sin(thi), sin(tht) */
            double cd, cs, sd, ss; /* cos(thi+tht), cos(thi-tht), etc */

            if (n1 == n2)
            {
                uz_snell = ci;
                return 0.0;
            }
            else if (ci > COSZERO)
            {     /* normal incidence */
                uz_snell = ci;
                return (n2 - n1) * (n2 - n1) / ((n2 + n1) * (n2 + n1));
            }
            else if (ci < COS90D)
            {
                uz_snell = 0.0;
                return 1.0;
            }
            else
            {
                si = Math.Sqrt(1 - ci * ci);
                st = n1 / n2 * si;
                ct = Math.Sqrt(1 - st * st);
                uz_snell = ct;

                /* Use trig identities */
                sd = si * ct - ci * st;
                ss = si * ct + ci * st;
                cd = ci * ct + si * st;
                cs = ci * ct - si * st;
                /* printf("cd= %12.4e    ss = %12.4e\n"); */
                return 0.5 * (sd * sd / (ss * ss) + sd * sd * cs * cs / (cd * cd * ss * ss));
            }
        }
        /// <summary>
        /// Specular determines the fraction of the light reflected for normally incident light
        /// </summary>
        /// <param name="n1">refractive index of the current medium</param>
        /// <param name="n2">refractive index of the next medium</param>
        /// <returns>double</returns>
        public static double Specular(double n1, double n2)
        {
            return (n1 - n2) * (n1 - n2) / ((n1 + n2) * (n1 + n2));
        }
    }
}
