using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// The <see cref="Helpers"/> namespace contains the Monte Carlo helper classes
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary> 
    /// Methods that aid in determining which detector bin to tally.
    /// </summary>
    public class DetectorBinning
    {
        /// <summary>
        /// WhichBin determines which uniform bin "value" is in.
        /// If data is beyond last bin, the last bin is returned.
        /// If the data is smaller than first bin, the first bin is returned.
        /// This assumes bins are contiguous.
        /// </summary>
        /// <param name="value">value to be binned</param>
        /// <param name="binSize">bin size</param>
        /// <param name="numberOfBins">size of array</param>
        /// <param name="binStart">starting location of binning</param>
        /// <returns>integer index of bin value is in</returns>
        public static int WhichBin(double value, int numberOfBins, double binSize, double binStart)
        {
            var bin = (int)Math.Floor((value - binStart) / binSize);
            if (bin > numberOfBins - 1)
                return numberOfBins - 1;
            return bin < 0 ? 0 : bin;
        }

        /// <summary>
        /// WhichBin determines which bin "value" is in given a list of bin centers and bin sizes.
        /// If value not in any bin, -1 returned.
        /// This allows for non-contiguous bins and nonuniformly spaced bins.
        /// </summary>
        /// <param name="value">value to be binned</param>
        /// <param name="binSize">bin size</param>
        /// <param name="binCenters">list of bin centers</param>
        /// <returns>integer index of bin value is in</returns>
        public static int WhichBin(double value, double binSize, double[] binCenters)
        {
            for (var i = 0; i < binCenters.Count(); i++)
			{
                if (value >= binCenters[i] - binSize / 2 && value < binCenters[i] + binSize / 2)
                    return i;
            }
            return -1; // for now
        }
        /// <summary>
        /// WhichBin determines which bin "value" is in given a list of bin stops.
        /// If value not in any bin, -1 returned.
        /// This allows for non-uniformly spaced bins within assumed contiguous span starting from 0.
        /// </summary>
        /// <param name="value">value to be binned</param>
        /// <param name="binStops">list of bin stops</param>
        /// <returns>integer index of bin value is in</returns>
        public static int WhichBin(double value, double[] binStops)
        {
            if (value >= 0.0 && value < binStops[0])
                return 0;
            for (var i = 1; i < binStops.Length; i++)
            {
                if (value >= binStops[i-1] && value < binStops[i])  return i;
            }
            return -1; // for now
        }

        /// <summary>
        /// WhichBin determines which uniform bin "value" is in.
        /// If data is beyond last bin, it is not binned and -1 is returned
        /// If the data is smaller than first bin, it is not binned and -1 is returned
        /// This assumes bins are contiguous.
        /// </summary>
        /// <param name="value">value to be binned</param>
        /// <param name="binSize">bin size</param>
        /// <param name="numberOfBins">size of array</param>
        /// <param name="binStart">starting location of binning</param>
        /// <returns>integer index of bin value is in</returns>
        public static int WhichBinExclusive(double value, int numberOfBins, double binSize, double binStart)
        {
            var bin = (int)Math.Floor((value - binStart) / binSize);
            if (bin > numberOfBins - 1) return -1;
            if (bin < 0) return -1;

            return bin;
        }

        /// <summary>
        /// Method to determine time delay of photon given its pathlength and refractive index
        /// of medium where pathlength is determined
        /// </summary>
        /// <param name="pathlength">distance photon has traveled</param>
        /// <param name="n">refractive index of medium where pathlength is determined</param>
        /// <returns>time delay according to path length and tissue n</returns>
        public static double GetTimeDelay(double pathlength, double n)
        {
            return pathlength / (GlobalConstants.C / n);
        }
        /// <summary>
        /// Method to determine rho given x and y coordinates
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>sqrt(x*x+y*y)</returns>
        public static double GetRho(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
}
