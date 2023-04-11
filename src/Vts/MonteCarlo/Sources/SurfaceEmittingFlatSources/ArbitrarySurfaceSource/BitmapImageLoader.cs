using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Class to handle loading of a bitmap image
    /// </summary>
    public class BitmapImageLoader
    {
        /// <summary>
        /// x bins 
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y bins centers
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// BitmapImage of source
        /// </summary>
        public double[,] BitmapImage { get; set; }

        /// <summary>
        /// dictionary that maps key=count to triple of indices to go through BitmapImage fluorescent region in order
        /// </summary>
        public Dictionary<int, List<int>> RegionIndicesInOrder { get; set; }

        /// <summary>
        /// constructor that load excitation simulation BitmapImage
        /// </summary>
        /// <param name="inputFolder">input folder where excitation results reside</param>
        /// <param name="infile">simulation infile of excitation simulation</param>
        /// <param name="numberOfPixelsX">number of pixels in length (e.g. 1280)</param>
        /// <param name="numberOfPixelsY">number of pixels in width (e.g. 1024)</param>
        /// <exception cref="ArgumentException">throws ArgumentException if infile is not there</exception>
        public static double[] LinearizeBitmap(string inputFolder, string infile, int numberOfPixelsX, int numberOfPixelsY)
        {
            if (infile != "")
            {
                string inputPath;
                if (inputFolder == "")
                {
                    inputPath = infile;
                }
                else
                {
                    inputPath = inputFolder + @"/" + infile;
                }

                var bitmap = new double[numberOfPixelsX, numberOfPixelsY];
                var oneDArray = new double[numberOfPixelsX * numberOfPixelsY];
                var sw = new StreamReader(@inputPath);
                for (var i = 0; i < bitmap.Length; i += numberOfPixelsY)
                {
                    for (var j = 0; j < numberOfPixelsY; j++)
                    {
                        oneDArray[i+j] = sw.Read();
                    }
                }

                sw.Close();
                return oneDArray;
            }
            else
            {
                throw new ArgumentException("infile string is empty");
            }
        }


        //// set up order of image indices using row major
        //private void SetupRegionIndices()
        //{
        //    ImageIndicesInOrder = new Dictionary<int, List<int>>();
        //    var count = 0;
        //    for (var i = 0; i < X.Count - 3; i++)
        //    {
        //        for (var j = 0; j < Y.Count - 3; j++)
        //        {
        //            if (MapOfXAndYAndZ[i, j] > 0.0) continue;
        //            ImageIndicesInOrder.Add(count, new List<int> { i, j, k });
        //            count += 1;
                    
        //        }
        //    }
        //}
    }
}
