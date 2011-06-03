using System;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Provides statistical error estimation given 2nd moment calculation
    /// </summary>
    public class ErrorCalculation
    {
        /// <summary>
        /// StandardDeviation calculates the square root of the variance
        /// </summary>
        /// <param name="numberOfPhotons">number of photons launched</param>
        /// <param name="mean">average value of first moment</param>
        /// <param name="secondMoment">average value of second moment</param>
        /// <returns>standard deviation</returns>
        public static double StandardDeviation(long numberOfPhotons, 
            double mean, double secondMoment)
        {
            var variance = (secondMoment - mean * mean) / numberOfPhotons;
            return Math.Sqrt(variance);            
        }
        
    }
}
