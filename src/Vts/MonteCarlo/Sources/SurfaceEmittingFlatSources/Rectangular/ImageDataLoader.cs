using System;
using System.IO;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Class to handle loading of a bitmap image
    /// </summary>
    public class ImageDataLoader 
    {
        /// <summary>
        /// constructor that load excitation simulation BitmapImage
        /// </summary>
        /// <param name="inputFolder">input folder where image resides</param>
        /// <param name="cSvFileName">name of image file</param>
        /// <param name="numberOfPixelsX">number of pixels in length (e.g. 1280)</param>
        /// <param name="numberOfPixelsY">number of pixels in width (e.g. 1024)</param>
        /// <returns>flattened image in column major order</returns>
        /// <exception cref="ArgumentException">throws ArgumentException if infile is not there</exception>
        public static double[] ReadAndFlattenCsv(string inputFolder, string cSvFileName, int numberOfPixelsX, int numberOfPixelsY)
        {
            if (cSvFileName == "") throw new ArgumentException("infile string is empty");
            string inputPath;
            if (inputFolder == "")
            {
                inputPath = cSvFileName;
            }
            else
            {
                inputPath = inputFolder + @"/" + cSvFileName;
            }

            var oneDArray = new double[numberOfPixelsX * numberOfPixelsY];

            // read in CSV file into 1D column major array
            using var reader = new StreamReader(inputPath);
            var row = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split(',');
                for (var i = 0; i < numberOfPixelsX; i++)
                {
                    oneDArray[row + numberOfPixelsY * i] = Convert.ToDouble(values[i]);
                }
                row += 1;
            }

            return oneDArray;

        }

    }
}
