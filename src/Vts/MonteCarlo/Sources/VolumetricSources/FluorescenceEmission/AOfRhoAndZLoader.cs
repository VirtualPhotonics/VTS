using NLog;
using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// class to handle loading of absorbed energy results from prior simulation.
    /// Because our code tallies photon contributions beyond the final grid bin, into
    /// the final grid bin, the final grid bins are excluded from the fluorescent source.
    /// </summary>
    public class AOfRhoAndZLoader
    {
        /// <summary>
        /// rho bins 
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z bins 
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// AOfRhoAndZ of *entire* tissue
        /// </summary>
        public double[,] AOfRhoAndZ { get; set; }
        /// <summary>
        /// Map of fluorescent tissue region with 1's identifying region within AOfRhoAndZ for debugging purposes
        /// </summary>
        public int[,] MapOfRhoAndZ { get; set; }
        /// <summary>
        /// PDF of fluorescent tissue region subset of AOfRhoAndZ 
        /// </summary>
        public double[,] PDFOfRhoAndZ { get; set; }
        /// <summary>
        /// sum of ProbOfRhoAndZ
        /// </summary>
        public double TotalProb { get; set; }
        /// <summary>
        /// CDF of fluorescent tissue region first index=row dominant index of MapOfRhoAndZ
        /// </summary>
        public double[,] CDFOfRhoAndZ { get; set; }
        /// <summary>
        /// Total Absorbed energy determined from AOfRhoAndZ minus final bins
        /// </summary>
        public double TotalAbsorbedEnergy { get; set; }
        /// <summary>
        /// tissue region of fluorescence
        /// </summary>
        public ITissueRegion FluorescentTissueRegion;
        /// <summary>
        /// potential bounding of FluorescentTissueRegion
        /// </summary>
        public ITissueRegion BoundedTissueRegion;
        /// <summary>
        /// dictionary that maps key=count to triple of indices to go through AOfRhoAndZ fluorescent region in order
        /// </summary>
        public Dictionary<int, List<int>> FluorescentRegionIndicesInOrder;

        /// <summary>
        /// constructor that load excitation simulation AOfRhoAndZ
        /// </summary>
        /// <param name="inputFolder">input folder where excitation results reside</param>
        /// <param name="infile">simulation infile of excitation simulation</param>
        /// <param name="fluorescentTissueRegionIndex">integer index of tissue region where fluorescence is</param>
        /// <exception cref="ArgumentException">throws ArgumentException if infile is not there</exception>
        public AOfRhoAndZLoader(string inputFolder, string infile, int fluorescentTissueRegionIndex)
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
                var aOfRhoAndZDetector = (AOfRhoAndZDetector) DetectorIO.ReadDetectorFromFile(
                    "AOfRhoAndZ", inputFolder);
                // use DoubleRange Rho,Z to match detector dimensions
                Rho = aOfRhoAndZDetector.Rho;
                Z = aOfRhoAndZDetector.Z;
                AOfRhoAndZ = aOfRhoAndZDetector.Mean;

                var exciteInfile = SimulationInput.FromFile(inputPath);
                FluorescentTissueRegion = exciteInfile.TissueInput.Regions[fluorescentTissueRegionIndex];
                BoundedTissueRegion = null;
                // check if tissue bounded CH: can this be made more generic?
                if (exciteInfile.TissueInput.TissueType == "BoundingCylinder")
                {
                    var cylinderIndex = exciteInfile.TissueInput.Regions.Length - 1;
                    var boundingCylinder =
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
        /// method to initialize fluorescent region arrays that are used to sample the fluorescent source
        /// </summary>
        public void InitializeFluorescentRegionArrays()
        {                
            // Omit final bins from fluorescent source so Count-2.
            // First, check if results have dimension of at least 2
            if (Rho.Count < 2 || Z.Count < 2)
            {
                throw new ArgumentException("Absorbed Energy grid needs at least 2 elements in each dimension");
            }
            MapOfRhoAndZ = new int[Rho.Count - 2, Z.Count - 2];
            PDFOfRhoAndZ = new double[Rho.Count - 2, Z.Count - 2];
            CDFOfRhoAndZ = new double[Rho.Count - 2, Z.Count - 2];

            // the following algorithm assumes that if the midpoint of the voxel is inside the 
            // fluorescent tissue region, then it is part of emission
            TotalProb = 0.0;
            TotalAbsorbedEnergy = 0.0;
            for (var i = 0; i < Rho.Count - 2; i++)
            {
                // following code assumes tissue region is cylindrical in nature
                // therefore, if (rho,z) in region, (x=rho,0,z) is in region
                // algorithm adopted from Jacques' mcfluor.c
                var r1 = i * Rho.Delta;
                var r2 = (i + 1) * Rho.Delta;
                var xMidpoint = (2.0 / 3.0) * (r2 * r2 + r2 * r1 + r1 * r1) / (r1 + r2);
                var yMidpoint = 0.0;
                for (var k = 0; k < Z.Count - 2; k++)
                {
                    var zMidpoint = Z.Start + k * Z.Delta + Z.Delta / 2;
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
                    MapOfRhoAndZ[i, k] = 1;
                    PDFOfRhoAndZ[i, k] = AOfRhoAndZ[i, k];
                    TotalProb += AOfRhoAndZ[i, k];
                    CDFOfRhoAndZ[i, k] += TotalProb;
                    // determine total absorbed energy minus final bins
                    var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
                    var rhoZNorm = (Rho.Start + (i + 0.5) * Rho.Delta) * normalizationFactor;
                    TotalAbsorbedEnergy += AOfRhoAndZ[i, k] * rhoZNorm;

                }
            }
            // create pdf and cdf
            for (var i = 0; i < Rho.Count - 2; i++)
            {
                for (var k = 0; k < Z.Count - 2; k++)
                {
                    if (MapOfRhoAndZ[i, k] != 1) continue;
                    PDFOfRhoAndZ[i, k] /= TotalProb;
                    CDFOfRhoAndZ[i, k] /= TotalProb;
                }               
            }
        }

        // set up order of fluorescent region indices using row major
        private void SetupRegionIndices()
        {
            FluorescentRegionIndicesInOrder = new Dictionary<int, List<int>>();
            var count = 0;
            for (var i = 0; i < Rho.Count - 2; i++)
            {
                for (var k = 0; k < Z.Count - 2; k++)
                {
                    if (MapOfRhoAndZ[i, k] != 1) continue;
                    FluorescentRegionIndicesInOrder.Add(count, new List<int> { i, k });
                    count += 1;
                }
            }
            // output number of voxels in fluorescent region so that can normalize results
            Console.WriteLine("number of fluorescent voxels = " + count.ToString(""));
            Console.WriteLine("if using Uniform SamplingMethod, multiply results by this factor");
        }
    }
}
