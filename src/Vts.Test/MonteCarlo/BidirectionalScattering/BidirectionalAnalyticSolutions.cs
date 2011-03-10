using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Vts.Test.MonteCarlo.BidirectionalScattering
{
    /// <summary>
    /// Class of analytic solutions that involve 1D bidirectional scattering
    /// </summary>
    public class BidirectionalAnalyticSolutions
    {
        /// <summary>
        /// Analytic solution for homogeneous slab of length T.  This assumes
        /// Q0, the source term, is 1.
        /// </summary>
        /// <param name="slabThickness">thickness of slab</param>
        /// <param name="ops">optical properties of slab</param>
        /// <param name="direction">direction of radiance (uz=1 downward, uz=-1 upward)</param>
        /// <param name="position">position of interest in slab</param>
        /// <returns></returns>
        public static double GetBidirectionalRadianceInSlab(double slabThickness, 
            OpticalProperties ops, int direction, double position)
        {
            // Pf = prob. of forward scattering, Pb = prob. of backward scattering
            // Pf+Pb = 1, Pf-Pb=g  => Pf=(1+g)/2
            double Pf = (1 + ops.G) / 2;
            double a = (ops.Mua + ops.Mus) - ops.Mus * Pf;
            double b = ops.Mus * (1 - Pf);
            double delta = Math.Sqrt(a * a - b * b);
            double denom = (delta + a) + (delta - a) * Math.Exp(-2 * delta * slabThickness);
            if (direction == 1) // downward moving radiance
            {
                // answer=d1*exp(delta*x)+d2*exp(-delta*x)
                // below is the more numerically stable equivalent
                var d1 = (delta - a) * Math.Exp(-2 * delta * slabThickness) / denom;
                var d2 = (delta + a) / denom;
                return ((delta - a) * Math.Exp(-2 * delta * slabThickness + delta * position) +
                        (delta + a) * Math.Exp(-delta * position)) / denom;

            }
            else // upward moving radiance
            {
                // answer=d1*exp(delta*x)+d2*exp(-delta*x)
                // below is the more nuerically stable equivalent
                var d1 = -b * Math.Exp(-2 * delta * slabThickness) / denom;
                var d2 = b / denom;
                return (-b * Math.Exp(-2 * delta * slabThickness + delta * position) +
                         b * Math.Exp(-delta * position)) / denom;
            }
        }
        public static double GetBidirectionalRadianceIntegratedOverInterval(double slabThickness,
            OpticalProperties ops, int direction, double position1, double position2)
        {
            double Pf = (1 + ops.G) / 2;
            double a = (ops.Mua + ops.Mus) * ops.Mus * Pf;
            double b = ops.Mus * (1 - Pf);
            double delta = Math.Sqrt(a * a - b * b);
            double denom = (delta + a) + (delta - a) * Math.Exp(-2 * delta * slabThickness);
            if (direction == 1) // downward moving radiance
            {  
                //d1=-b*Q0*exp(-2*delta*T)/denom;
                //d2=b*Q0/denom;
                // answer=(1/delta)*(d1*exp(delta*x)-d2*exp(-delta*x)); 
                var d1 = (1.0 / delta) * ((delta - a) * Math.Exp(-2 * delta * slabThickness + delta * position2) -
                         (delta + a) * Math.Exp(-delta * position2)) / denom;
                var d2 = (1.0 / delta) * ((delta - a) * Math.Exp(-2 * delta * slabThickness + delta * position1) -
                                         (delta + a) * Math.Exp(-delta * position1)) / denom;
                return (d1 - d2);
            }
            else
            { // upward moving radiance
                //d1=Q0*(delta-a)*exp(-2*delta*T)/denom;
                //d2=Q0*(delta+a)/denom;
                // answer=(1/delta)*(d1*exp(delta*x)-d2*exp(-delta*x));  
                var d1 = (1.0 / delta) * (-b * Math.Exp(-2 * delta * slabThickness + delta * position2) -
                                           b * Math.Exp(-delta * position2)) / denom;
                var d2 = (1.0 / delta) * (-b * Math.Exp(-2 * delta * slabThickness + delta * position1) -
                                           b * Math.Exp(-delta * position1)) / denom;
                return (d1 - d2);
            }
        }
    }
}
