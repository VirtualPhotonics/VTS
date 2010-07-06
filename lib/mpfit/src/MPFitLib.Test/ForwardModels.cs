/* 
 * MINPACK-1 Least Squares Fitting Library
 *
 * Original public domain version by B. Garbow, K. Hillstrom, J. More'
 *   (Argonne National Laboratory, MINPACK project, March 1980)
 * 
 * Translation to C Language by S. Moshier (moshier.net)
 * Translation to C# Language by D. Cuccia (http://davidcuccia.wordpress.com)
 * 
 * Enhancements and packaging by C. Markwardt
 *   (comparable to IDL fitting routine MPFIT
 *    see http://cow.physics.wisc.edu/~craigm/idl/idl.html)
 */

/* Test routines for MPFit library
   $Id: TestMPFit.cs,v 1.1 2010/05/04 dcuccia Exp $
*/

using System;
using System.Collections.Generic;

namespace MPFitLib.Test
{
    public static class ForwardModels
    {
        /* 
         * linear fit function
         *
         * m - number of data points
         * n - number of parameters (2)
         * p - array of fit parameters 
         * dy - array of residuals to be returned
         * CustomUserVariable - private data (struct vars_struct *)
         *
         * RETURNS: error code (0 = success)
         */
        public static int LinFunc(double[] p, double[] dy, IList<double>[] dvec, object vars)
        {
            int i;
            double[] x, y, ey;
            double f;

            CustomUserVariable v = (CustomUserVariable)vars;

            x = v.X;
            y = v.Y;
            ey = v.Ey;

            for (i = 0; i < dy.Length; i++)
            {
                f = p[0] - p[1] * x[i];     /* Linear fit function */
                dy[i] = (y[i] - f) / ey[i];
            }

            return 0;
        }

        /* 
        * quadratic fit function
        *
        * m - number of data points
        * n - number of parameters (2)
        * p - array of fit parameters 
        * dy - array of residuals to be returned
        * CustomUserVariable - private data (struct vars_struct *)
        *
        * RETURNS: error code (0 = success)
        */
        public static int QuadFunc(double[] p, double[] dy, IList<double>[] dvec, object vars)
        {
            int i;
            double[] x, y, ey;

            CustomUserVariable v = (CustomUserVariable)vars;
            x = v.X;
            y = v.Y;
            ey = v.Ey;

            /* Console.Write ("QuadFunc %f %f %f\n", p[0], p[1], p[2]); */

            for (i = 0; i < dy.Length; i++)
            {
                dy[i] = (y[i] - p[0] - p[1] * x[i] - p[2] * x[i] * x[i]) / ey[i];
            }

            return 0;
        }


        /* 
         * gaussian fit function
         *
         * m - number of data points
         * n - number of parameters (4)
         * p - array of fit parameters 
         * dy - array of residuals to be returned
         * CustomUserVariable - private data (struct vars_struct *)
         *
         * RETURNS: error code (0 = success)
         */
        public static int GaussFunc(double[] p, double[] dy, IList<double>[] dvec, object vars)
        {
            int i;
            CustomUserVariable v = (CustomUserVariable)vars;
            double[] x, y, ey;
            double xc, sig2;

            x = v.X;
            y = v.Y;
            ey = v.Ey;

            sig2 = p[3] * p[3];

            for (i = 0; i < dy.Length; i++)
            {
                xc = x[i] - p[2];
                dy[i] = (y[i] - p[1] * Math.Exp(-0.5 * xc * xc / sig2) - p[0]) / ey[i];
            }

            return 0;
        }
    }
}
