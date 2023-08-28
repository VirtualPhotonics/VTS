using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources.SourceProfiles
{

    /// <summary>
    /// Defines Image Source Profile
    /// </summary>
    public class ImageSourceProfile : ISourceProfile
    {
        /// <summary>
        /// Returns Image profile type
        /// </summary>
        //[IgnoreDataMember]
        public SourceProfileType SourceProfileType => SourceProfileType.Image;

        /// <summary>
        /// 1D array containing 2D image of values
        /// </summary>
        public double[] Image { get; set; }

        /// <summary>
        /// x-axis pixel edges
        /// </summary>
        public DoubleRange X { get; set; }

        /// <summary>
        /// y-axis pixel edges
        /// </summary>
        public DoubleRange Y { get; set; }

        /// <summary>
        /// Probability density function created from image
        /// </summary>
        private double[] _probabilityDensityFunction;

        /// <summary>
        /// Cumulative density function
        /// </summary>
        private double[] _cumulativeDensityFunction;
       
        /// <summary>
        /// Initializes a new instance of the ImageSourceProfile class
        /// </summary>
        /// <param name="image">1D array containing 2D image of values</param>
        /// <param name="numberOfPixelsWidthX">number of pixels along x-axis</param>
        /// <param name="numberOfPixelsHeightY">number of pixels along y-axis</param>
        /// <param name="pixelSizeX">pixel size along x-axis</param>
        /// <param name="pixelSizeY">pixel size along y-axis</param>
        /// <param name="imageCenter">Position of image center (note z value not used)</param>
        public ImageSourceProfile(double[] image, int numberOfPixelsWidthX, int numberOfPixelsHeightY, 
            double pixelSizeX, double pixelSizeY, Position imageCenter)
        {
            Image = image;
            _probabilityDensityFunction = Image.Select(s => s / Image.Sum()).ToArray();
            var xStart = imageCenter.X - numberOfPixelsWidthX * pixelSizeX / 2;
            var xEnd = imageCenter.X + numberOfPixelsWidthX * pixelSizeX / 2;
            X = new DoubleRange(xStart, xEnd, numberOfPixelsWidthX + 1);
            var yStart = imageCenter.Y - numberOfPixelsHeightY * pixelSizeY / 2;
            var yEnd = imageCenter.Y + numberOfPixelsHeightY * pixelSizeY / 2;
            Y = new DoubleRange(yStart, yEnd, numberOfPixelsHeightY + 1);
            InitializeSamplingArray();
        }

        /// <summary>
        /// Initializes the default constructor of ImageSourceProfile class (intensity = 1.0)
        /// </summary>
        public ImageSourceProfile()
            : this(new double[] { 255 }, 1, 1, 0.1, 0.1, 
                new Position(0,0, 0))
        {
        }

        private void InitializeSamplingArray()
        {
            _probabilityDensityFunction = new double[Image.Length];
            _cumulativeDensityFunction = new double[Image.Length];
            var totalProb = Image.Sum();
            var runningCumulativeSum = 0.0;
            for (var i = 0; i < Image.Length - 1; i++)
            {
                _probabilityDensityFunction[i] = Image[i] / totalProb;
                runningCumulativeSum += _probabilityDensityFunction[i];
                _cumulativeDensityFunction[i] = runningCumulativeSum;
            }
            _cumulativeDensityFunction[Image.Length - 1] = 1.0; // set last point
        }
        /// <summary>
        /// Generates binary pixel map of image
        /// </summary>
        /// <returns>array of integers indicating if pixel value> 1e-10 or not</returns>
        /// <remarks>Currently, only binary maps are implemented. Non-zero (1e-10D) values will be 1, 0 otherwise</remarks>
        public int[] GetBinaryPixelMap()
        {
            var binaryPixelMap = new int[Image.Length];
            for (var i = 0; i < Image.Length; i++)
            {
                var pixelValue = Image[i];
                if (Math.Abs(pixelValue) > 1e-10D)
                {
                    binaryPixelMap[i] = 1;
                }
            }
            return binaryPixelMap; 
        }
        /// <summary>
        /// Method to obtain where to launch photon based on PDF and CDF created from Image
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>Position of starting photon</returns>
        public Position GetPositionInARectangleBasedOnImageIntensity(Random rng)
        { 
            // use pixel intensity percentage of total to determine how many photons to launch from pixel
            var rho = rng.NextDouble();
            // determine position from CumulativeDensityFunction
            for (var i = 0; i < Image.Length; i++)
            {
                if (rho >= _cumulativeDensityFunction[i]) continue;
                // determine index into X and Y based on flattened CDF, (i,j) starts upper left corner
                // and is column major
                var iX = (int)Math.Floor((double)i / (Y.Count - 1));
                int iY;
                if (iX != 0)  // handle possible divide by 0 by mod function
                    iY = i % ((Y.Count - 1) * iX);
                else
                    iY = i;
                var xMidpoint = X.Start + iX * X.Delta + X.Delta / 2;
                var yMidpoint = Y.Start + iY * Y.Delta + Y.Delta / 2;
                return new Position(xMidpoint, yMidpoint, 0.0);
            }
            return null;
        }

    }
}
