/* 
 * MINPACK-1 Least Squares Fitting Library
 *
 * Original public domain version by B. Garbow, K. Hillstrom, J. More'
 *   (Argonne National Laboratory, MINPACK project, March 1980)
 * See the file DISCLAIMER for copyright information.
 * 
 * Tranlation to C Language by S. Moshier (moshier.net)
 * Translation to C# Language by D. Cuccia (http://davidcuccia.wordpress.com)
 * 
 * Enhancements and packaging by C. Markwardt
 *   (comparable to IDL fitting routine MPFIT
 *    see http://cow.physics.wisc.edu/~craigm/idl/idl.html)
 */

/* Main MPFit library routines (double precision) 
   $Id: MPFit.cs,v 1.1 2010/05/04 dcuccia Exp $
 */

namespace MPFitLib
{
    /* Definition of results structure, for when fit completes */
    public class mp_result
    {
        public double bestnorm;     /* Final chi^2 */
        public double orignorm;     /* Starting value of chi^2 */
        public int niter;           /* Number of iterations */
        public int nfev;            /* Number of function evaluations */
        public int status;          /* Fitting status code */

        public int npar;            /* Total number of parameters */
        public int nfree;           /* Number of free parameters */
        public int npegged;         /* Number of pegged parameters */
        public int nfunc;           /* Number of residuals (= num. of data points) */

        public double[] resid;       /* Final residuals
			          nfunc-vector, or 0 if not desired */
        public double[] xerror;      /* Final parameter uncertainties (1-sigma)
			          npar-vector, or 0 if not desired */
        public double[] covar;       /* Final parameter covariance matrix
			          npar x npar array, or 0 if not desired */
        public string version;    /* MPFIT version string */

        public mp_result(int numParameters)
        {
            xerror = new double[numParameters];
        }
    }
}
