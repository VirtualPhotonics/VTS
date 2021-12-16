using System;
using System.Collections.Generic;

namespace Vts.Common.Math
{
    /// <summary>
    /// numerical methods to integrate a function
    /// </summary>
    public static class Integration
    {
        /// <summary>
        /// Integration in one dimension
        /// </summary>
        /// <param name="f">Function to integrate</param>
        /// <param name="a">Lower limit of integration</param>
        /// <param name="b">Upper limit of integration</param>
        /// <param name="epsilon">Accuracy required)</param>
        /// 
        public static double IntegrateAdaptiveSimpsonRule(Func<double, double> f, double a, double b, double epsilon)
        {
            //calculates integral of f from a to b with max error of epsilon
            return AdaptiveRecursiveSimpson(f, a, b, epsilon, IntegrateSimpsonRule(f, a, b));
        }
        /// <summary>
        /// method to integrate function using Simpson's Rule
        /// </summary>
        /// <param name="f">function to integrate</param>
        /// <param name="a">Lower limit of integration</param>
        /// <param name="b">Upper limit of integration</param>
        /// <returns></returns>
        public static double IntegrateSimpsonRule(Func<double, double> f, double a, double b)
        {
            double c = (a + b) / 2.0;
            double h3 = System.Math.Abs(b - a) / 6.0;
            double S = h3 * (f(a) + 4.0 * f(c) + f(b));
            return S;
        }
        /// <summary>
        /// adaptive recursive Simpson's rule
        /// </summary>
        /// <param name="f">function to integrate</param>
        /// <param name="a">lower bound of integration interval</param>
        /// <param name="b">upper bound of integration interval</param>
        /// <param name="epsilon">precision</param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public static double AdaptiveRecursiveSimpson(Func<double, double> f, double a, double b, double epsilon, double sum)
        {
            double c = (a + b) / 2.0;
            double left = IntegrateSimpsonRule(f, a, c);
            double right = IntegrateSimpsonRule(f, c, b);
            double R;
            if (System.Math.Abs(left + right - sum) <= 15.0 * epsilon)
                R = left + right + (left + right - sum) / 15.0;
            else
                R = AdaptiveRecursiveSimpson(f, a, c, epsilon / 2.0, left) + AdaptiveRecursiveSimpson(f, c, b, epsilon / 2.0, right);
            return R;
        }
        public static double IntegrateTrapezoidRuleForTwoLists(List<double> x, List<double> y, double a, double b)
        {
            int indexA = x.BinarySearch(a);
            if(indexA<0)
            {
                x.Insert(~indexA, a);
                x.Sort();
                indexA = x.BinarySearch(a);
                y.Insert(indexA, Vts.Common.Math.Interpolation.interp1(x, y, a));
            }
            int indexB = x.BinarySearch(b);
            if (indexB < 0)
            {
                x.Insert(~indexB, b);
                x.Sort();
                indexB = x.BinarySearch(b);
                y.Insert(indexB, Vts.Common.Math.Interpolation.interp1(x, y, b));
            }
            double sum = 0;
            if (indexA == indexB)
                return 0.0;
            for (int i = indexA; i < indexB; i++)
            {
                sum += (x[i + 1] - x[i]) * (y[i + 1] + y[i]);
            }
            return 0.5*sum;
        }
    }
}
