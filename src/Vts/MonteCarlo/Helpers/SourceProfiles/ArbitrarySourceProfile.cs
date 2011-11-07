using System;
using System.Linq;
using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
using System.Collections.Generic;

namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    /// <summary>
    /// Defines Arbitrary Source Profile
    /// </summary>
    public class ArbitrarySourceProfile : ISourceProfile
    {
        private int _numNonZeroPixels;
        private int[] _nonZeroPixelMap;

        /// <summary>
        /// Initializes a new instance of the ArbitrarySourceProfile class
        /// </summary>
        /// <param name="image">1D array containing 2D image of values</param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <remarks>Currently, only binary maps are implemented. Non-zero (1e-10D) values will be 1, 0 otherwise</remarks>
        public ArbitrarySourceProfile(double[] image, int pixelWidth, int pixelHeight)
        {
            Image = image;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            //IsBinary = true; // not implemented for arbitrary structure // only binary currently implemented
            //if (IsBinary)
            //{
                var binaryPixelMap = new List<int>(image.Length);
                for (int i = 0; i < image.Length; i++)
                {
                    var pixelValue = image[i];
                    if (Math.Abs(pixelValue) > 1e-10D)
                    {
                        binaryPixelMap.Add(i);
                    }
                }
                _nonZeroPixelMap = binaryPixelMap.ToArray();
                _numNonZeroPixels = _nonZeroPixelMap.Length;
            //}
        }

        /// <summary>
        /// Initializes the default constructor of ArbitrarySourceProfile class (intensity = 1.0)
        /// </summary>
        public ArbitrarySourceProfile()
            : this(new double[] { 255 }, 1, 1)
        {
        }

        /// <summary>
        /// Returns Arbitrary profile type
        /// </summary>
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Arbitrary; } }

        public double[] Image { get; set; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public bool IsBinary { get; set; }
    }
}
