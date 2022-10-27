using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Tissues;

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
        public ITissueRegion FluorescentTissueRegion { get; set; }
        /// <summary>
        /// potential bounding of FluorescentTissueRegion
        /// </summary>
        public ITissueRegion BoundedTissueRegion;
        /// <summary>
        /// dictionary that maps key=count to triple of indices to go through AOfXAndYAndZ fluorescent region in order
        /// </summary>
        public Dictionary<int, List<int>> FluorescentRegionIndicesInOrder { get; set; }

        /// <summary>
        /// constructor that load excitation simulation AOfXAndYAndZ
        /// </summary>
        /// <param name="inputFolder">input folder where excitation results reside</param>
        /// <param name="infile">simulation infile of excitation simulation</param>
        /// <param name="fluorescentTissueRegionIndex">integer index of tissue region where fluorescence is</param>
        /// <exception cref="ArgumentException">throws ArgumentException if infile is not there</exception>
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
                X = aOfXAndYAndZDetector.X;
                Y = aOfXAndYAndZDetector.Y;
                Z = aOfXAndYAndZDetector.Z;
                AOfXAndYAndZ = aOfXAndYAndZDetector.Mean;

                var exciteInfile = SimulationInput.FromFile(inputPath);

                FluorescentTissueRegion = exciteInfile.TissueInput.Regions[fluorescentTissueRegionIndex];
                BoundedTissueRegion = null;
                // check if tissue bounded CH: can this be made more generic?
                if (exciteInfile.TissueInput.TissueType == "BoundingCylinder")
                {
                    var cylinderIndex = exciteInfile.TissueInput.Regions.Length - 1;
                    CaplessCylinderTissueRegion boundingCylinder = 
                        (CaplessCylinderTissueRegion)exciteInfile.TissueInput.Regions[cylinderIndex];
                    BoundedTissueRegion = new CaplessCylinderTissueRegion(
                        boundingCylinder.Center,
                        boundingCylinder.Radius,
                        boundingCylinder.Height,
                        boundingCylinder.RegionOP);
                }

                // separate setup of arrays so can unit test method
                InitializeFluorescentRegionArrays();
                SetupRegionIndices();
            }
            else
            {
                throw new ArgumentException("infile string is empty");
            }
        }
        /// <summary>
        /// method to initialize the fluorescent region arrays that are used to sample fluorescent source
        /// </summary>
        public void InitializeFluorescentRegionArrays()
        {     
            MapOfXAndYAndZ = new int[X.Count - 1, Y.Count - 1, Z.Count - 1];
            PDFOfXAndYAndZ = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];
            CDFOfXAndYAndZ = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];

            // the following algorithm assumes that if the midpoint of the voxel is inside the 
            // fluorescent tissue region, then it is part of emission
            TotalProb = 0.0;
            for (var i = 0; i < X.Count - 1; i++)
            {
                double xMidpoint = X.Start + i * X.Delta + X.Delta / 2;
                for (var j = 0; j < Y.Count - 1; j++)
                {
                    var yMidpoint = Y.Start + j * Y.Delta + Y.Delta / 2;
                    for (var k = 0; k < Z.Count - 1; k++)
                    {
                        var zMidpoint = Z.Start + k * Z.Delta + Z.Delta / 2;
                        // first check if in tissue region
                        var inFluorescentTissue = FluorescentTissueRegion.ContainsPosition(
                            new Position(xMidpoint, yMidpoint, zMidpoint));
                        // next check if not in bounding region if exists
                        if (BoundedTissueRegion != null)
                        {
                            inFluorescentTissue = BoundedTissueRegion.ContainsPosition(
                                new Position(xMidpoint, yMidpoint, zMidpoint));
                        }
                        // default values of numeric array elements are set to 0 so no else needed
                        if (!inFluorescentTissue) continue;
                        MapOfXAndYAndZ[i, j, k] = 1;
                        PDFOfXAndYAndZ[i, j, k] = AOfXAndYAndZ[i, j, k];
                        TotalProb += AOfXAndYAndZ[i, j, k];
                        CDFOfXAndYAndZ[i, j, k] += TotalProb;
                    }
                }
            }
            // create pdf and cdf
            for (var i = 0; i < X.Count - 1; i++)
            {
                for (var j = 0; j < Y.Count - 1; j++)
                {
                    for (var k = 0; k < Z.Count - 1; k++)
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

        // set up order of fluorescent region indices using row major
        private void SetupRegionIndices()
        {
            FluorescentRegionIndicesInOrder = new Dictionary<int, List<int>>();
            var count = 0;
            for (var i = 0; i < X.Count - 1; i++)
            {
                for (var j = 0; j < Y.Count - 1; j++)
                {
                    for (var k = 0; k < Z.Count - 1; k++)
                    {
                        if (MapOfXAndYAndZ[i, j, k] != 1) continue;
                        FluorescentRegionIndicesInOrder.Add(count, new List<int>() { i, j, k });
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
