using System.Linq;
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
        /// constructor that load excitation simulation AOfXAndYAndZ
        /// </summary>
        public AOfXAndYAndZLoader(string inputFolder, string infile, string detectorName,
            int fluorescentTissueRegionIndex)
        {
            InitializeFluorescentRegionArrays(inputFolder, infile, detectorName, fluorescentTissueRegionIndex);
        }

        private void InitializeFluorescentRegionArrays(string inputFolder, string infile, string detectorName,
            int fluorescentTissueRegionIndex)
        {
            var aOfXAndYAndZDetector = (AOfXAndYAndZDetector)DetectorIO.ReadDetectorFromFile(
                "AOfXAndYAndZ",
                inputFolder + "/" + detectorName);
            // use DoubleRange X,Y,Z to match detector dimensions
            X = ((AOfXAndYAndZDetector)aOfXAndYAndZDetector).X;
            Y = ((AOfXAndYAndZDetector)aOfXAndYAndZDetector).Y;
            Z = ((AOfXAndYAndZDetector)aOfXAndYAndZDetector).Z;
            AOfXAndYAndZ = ((AOfXAndYAndZDetector)aOfXAndYAndZDetector).Mean;

            MapOfXAndYAndZ = new int[X.Count - 1, Y.Count - 1, Z.Count - 1];
            PDFOfXAndYAndZ = new double[X.Count - 1, Y.Count - 1, Z.Count - 1];

            var exciteInfile = SimulationInput.FromFile(inputFolder + "/" + infile);
            var fluorescentTissueRegion = exciteInfile.TissueInput.Regions[fluorescentTissueRegionIndex];

            // the following algorithm assumes that if the midpoint of the voxel is inside the 
            // fluorescent tissue region, then it is part of emission
            TotalProb = 0.0;
            for (int i = 0; i < X.Count - 1; i++)
            {
                double xMidpoint = X.Start + i * X.Delta;
                for (int j = 0; j < Y.Count - 1; j++)
                {
                    var yMidpoint = Y.Start + j * Y.Delta;
                    for (int k = 0; k < Z.Count - 1; k++)
                    {
                        var zMidpoint = Z.Start + k * Z.Delta;
                        bool inFluorescentTissue = fluorescentTissueRegion.ContainsPosition(
                            new Position(xMidpoint, yMidpoint, zMidpoint));
                        // default values of numeric array elements are set to 0 so no else needed
                        if (inFluorescentTissue)
                        {
                            MapOfXAndYAndZ[i, j, k] = 1;
                            PDFOfXAndYAndZ[i, j, k] = AOfXAndYAndZ[i, j, k];
                            TotalProb += AOfXAndYAndZ[i, j, k];
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
                            CDFOfXAndYAndZ[i, j, k] += PDFOfXAndYAndZ[i, j, k];
                        }                    
                    }
                }
            }
        }
    }
}
