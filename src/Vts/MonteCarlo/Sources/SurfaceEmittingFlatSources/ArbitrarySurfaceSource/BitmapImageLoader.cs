using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.IO;

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
        /// <exception cref="ArgumentException">throws ArgumentException if infile is not there</exception>
        public BitmapImageLoader(string inputFolder, string infile, int numberOfPixelsX, int numberOfPixelsY,
            double pixelLengthX, double pixelWidthY)
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

                double ReadMap(BinaryReader b) => b.ReadDouble();
                var temp = FileIO.ReadFromBinaryCustom<double>(inputPath, ReadMap);
                //Stream bitmapStream = null;
                //var ofd = new OpenFileDialog();
                //if (ofd.ShowDialog() == DialogResult.OK)
                //{
                //    try
                //    {
                //        bitmapStream = ofd.OpenFile();
                //        using (bitmapStream)
                //        {
                //            BitmapImage = new Bitmap(bitmapStream);
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        throw new ArgumentException($"Could not open image file '{typeof()}");
                //    }
                //}

            }
            else
            {
                throw new ArgumentException("infile string is empty");
            }
        }
        /// <summary>
        /// method to initialize the source matrix
        /// </summary>
        public void InitializeFluorescentRegionArrays()
        {
            // the following algorithm assumes that if the midpoint of the voxel is inside the 
            // fluorescent tissue region, then it is part of emission
            TotalProb = 0.0;
            for (var i = 0; i < X.Count - 1; i++)
            {
                var xMidpoint = X.Start + i * X.Delta + X.Delta / 2;
                for (var j = 0; j < Y.Count - 1; j++)
                {
                    var yMidpoint = Y.Start + j * Y.Delta + Y.Delta / 2;
                    TotalProb += BitmapImage[i, j];
                    
                }
            }
            // create pdf and cdf
            for (var i = 0; i < X.Count - 1; i++)
            {
                for (var j = 0; j < Y.Count - 1; j++)
                {
                        if (MapOfXAndYAndZ[i, j] != 1) continue;
                        PDFOfXAndYAndZ[i, j] /= TotalProb;
                        CDFOfXAndYAndZ[i, j] /= TotalProb;
                }
            }
        }

        // set up order of fluorescent region indices using row major
        private void SetupRegionIndices()
        {
            FluorescentRegionIndicesInOrder = new Dictionary<int, List<int>>();
            var count = 0;
            for (var i = 0; i < X.Count - 3; i++)
            {
                for (var j = 0; j < Y.Count - 3; j++)
                {
                    for (var k = 0; k < Z.Count - 2; k++)
                    {
                        if (MapOfXAndYAndZ[i, j, k] != 1) continue;
                        FluorescentRegionIndicesInOrder.Add(count, new List<int> { i, j, k });
                        count += 1;
                    }
                }
            }
            // output number of voxels in fluorescent region so that can normalize results
            Console.WriteLine("number of fluorescent voxels = " + count.ToString(""));
            Console.WriteLine("if using Uniform SamplingMethod, multiply results by this factor");
        }
    }
}
