using System;
using System.IO;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Class to handle loading of a bitmap image
    /// </summary>
    public class BitmapImageLoader 
    {
        /// <summary>
        /// constructor that load excitation simulation BitmapImage
        /// </summary>
        /// <param name="inputFolder">input folder where image resides</param>
        /// <param name="imageName">name of image file</param>
        /// <param name="numberOfPixelsX">number of pixels in length (e.g. 1280)</param>
        /// <param name="numberOfPixelsY">number of pixels in width (e.g. 1024)</param>
        /// <returns>flattened image in column major order</returns>
        /// <exception cref="ArgumentException">throws ArgumentException if infile is not there</exception>
        public static double[] FlattenBitmap(string inputFolder, string imageName, int numberOfPixelsX, int numberOfPixelsY)
        {
            if (imageName == "") throw new ArgumentException("infile string is empty");
            string inputPath;
            if (inputFolder == "")
            {
                inputPath = imageName;
            }
            else
            {
                inputPath = inputFolder + @"/" + imageName;
            }

            var bitmap = new double[numberOfPixelsX, numberOfPixelsY];
            var oneDArray = new double[numberOfPixelsX * numberOfPixelsY];
            var sw = new StreamReader(@inputPath);
            // make column major flatten
            for (var i = 0; i < bitmap.Length; i += numberOfPixelsX)
            {
                for (var j = 0; j < numberOfPixelsY; j++)
                {
                    oneDArray[i+j] = sw.Read();
                }
            }

            sw.Close();
            return oneDArray;

        }

    }
}
