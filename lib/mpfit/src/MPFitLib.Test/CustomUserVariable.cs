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
 *   (comparable to IDL fitting routine MPFITS
 *    see http://cow.physics.wisc.edu/~craigm/idl/idl.html)
 */

/* Test routines for MPFit library
   $Id: TestMPFit.cs,v 1.1 2010/05/04 dcuccia Exp $
*/

namespace MPFitLib.Test
{
    /// <summary>
    /// This is the a demo user-specific data structure which contains the data points
    /// and their uncertainties
    /// </summary>
    public class CustomUserVariable
    {
        public double[] X;
        public double[] Y;
        public double[] Ey;
    }
}
