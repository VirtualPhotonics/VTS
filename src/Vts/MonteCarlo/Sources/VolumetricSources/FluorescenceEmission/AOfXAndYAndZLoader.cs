using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// class to handle loading of absorbed energy results from prior simulation
    /// </summary>
    public class AOfXAndYAndZLoader
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
        /// z bins 
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// AOfXAndYAndZ of *entire* tissue
        /// </summary>
        public double[,,] AOfXAndYAndZ { get; set; }
        /// <summary>
        /// Map of fluorescent tissue region with 1's identifying region within AOfXAndYAndZ for debugging purposes
        /// </summary>
        public int[,,] MapOfXAndYAndZ { get; set; }
        /// <summary>
        /// PDF of fluorescent tissue region subset of AOfXAndYAndZ 
        /// </summary>
        public double[,,] PDFOfXAndYAndZ { get; set; }
        /// <summary>
        /// sum of ProbOfXAndYAndZ
        /// </summary>
        public double TotalProb { get; set; }
        /// <summary>
        /// CDF of fluorescent tissue region first index=row dominant index of MapOfXAndYAndZ
        /// </summary>
        public double[,,] CDFOfXAndYAndZ { get; set; }
        /// <summary>
        /// tissue region of fluorescence
        /// </summary>
        public ITissueRegion FluorescentTissueRegion;
        /// <summary>
        /// dictionary that maps key=count to triple of indices to go through AOfXAndYAndZ fluorescent region in order
        /// </summary>
        public Dictionary<int, List<int>> FluorescentRegionIndicesInOrder;

        /// <summary>
        /// constructor that load excitation simulation AOfXAndYAndZ
        /// </summary>
        public AOfXAndYAndZLoader(string inputFolder, string infile, int fluorescentTissueRegionIndex)
        {
            if (infile != "")
            {
                var inputPath = "";
                if (inputFolder == "")
                {
                    inputPath = infile;
                }
                else
                {
                    inputPath = inputFolder + @"/" + infile;
                }
                var aOfXAndYAndZDetector = (AOfXAndYAndZDetector) DetectorIO.ReadDetectorFromFile(
                    "AOfXAndYAndZ", inputFolder);
                // use DoubleRange X,Y,Z to match detector dimensions
                X = ((AOfXAndYAndZDetector) aOfXAndYAndZDetector).X;
                Y = ((AOfXAndYAndZDetector) aOfXAndYAndZDetector).Y;
                Z = ((AOfXAndYAndZDetector) aOfXAndYAndZDetector).Z;
                AOfXAndYAndZ = ((AOfXAndYAndZDetector) aOfXAndYAndZDetector).Mean;

                var exciteInfile = SimulationInput.FromFile(inputPath);
                FluorescentTissueRegion = exciteInfile.TissueInput.Regions[fluorescentTissueRegionIndex];

                // separate setup of arrays so can unit test method
                InitializeFluorescentRegionArrays();
                SetupRegionIndices();
            }
            else
            {
                throw new ArgumentException("infile string is empty");
            }
        }

        public void InitializeFluorescentRegionArrays()
        {     
            MapOfXAndYAndZ = new int[X.Count - 1, Y.Count - 1, Z.Count - 1];
            PDFOfXAndYAndZ = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            CDFOfXAndYAndZ = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];

            // the following algorithm assumes that if the midpoint of the voxel is inside the 
            // fluorescent tissue region, then it is part of emission
            TotalProb = 0.0;
            for (int i = 0; i < X.Count - 1; i++)
            {
                double xMidpoint = X.Start + i * X.Delta + X.Delta / 2;
                for (int j = 0; j < Y.Count - 1; j++)
                {
                    var yMidpoint = Y.Start + j * Y.Delta + Y.Delta / 2;
                    for (int k = 0; k < Z.Count - 1; k++)
                    {
                        var zMidpoint = Z.Start + k * Z.Delta + Z.Delta / 2;
                        bool inFluorescentTissue = FluorescentTissueRegion.ContainsPosition(
                            new Position(xMidpoint, yMidpoint, zMidpoint));
                        // default values of numeric array elements are set to 0 so no else needed
                        if (inFluorescentTissue)
                        {
                            MapOfXAndYAndZ[i, j, k] = 1;
                            PDFOfXAndYAndZ[i, j, k] = AOfXAndYAndZ[i, j, k];
                            TotalProb += AOfXAndYAndZ[i, j, k];
                            CDFOfXAndYAndZ[i, j, k] += TotalProb;
                        }
                    }
                }
            }
            // create pdf and cdf
            for (int i = 0; i < X.Count - 1; i++)
            {
                for (int j = 0; j < Y.Count - 1; j++)
                {
                    for (int k = 0; k < Z.Count - 1; k++)
                    {
                        if (MapOfXAndYAndZ[i, j, k] == 1)
                        {
                            PDFOfXAndYAndZ[i, j, k] /= TotalProb;
                            CDFOfXAndYAndZ[i, j, k] /= TotalProb;
                        }                    
                    }
                }
            }
        }

        // set up order of fluor region indices using row major
        private void SetupRegionIndices()
        {
            FluorescentRegionIndicesInOrder = new Dictionary<int, List<int>>();
            int count = 0;
            for (int i = 0; i < X.Count - 1; i++)
            {
                for (int j = 0; j < Y.Count - 1; j++)
                {
                    for (int k = 0; k < Z.Count - 1; k++)
                    {
                        if (MapOfXAndYAndZ[i, j, k] == 1)
                        { 
                            FluorescentRegionIndicesInOrder.Add(count, new List<int>() { i, j, k });
                            count = count + 1;
                        }
                    }
                }
            }
        }
    }
}
