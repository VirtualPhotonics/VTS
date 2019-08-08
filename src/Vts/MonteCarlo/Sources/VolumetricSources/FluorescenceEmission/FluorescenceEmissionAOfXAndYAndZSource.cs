using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for FluorescenceEmissionAOfXAndYAndZSource 
    /// implementation.  This source reads the Cartesian coordinate absorbed energy results of a
    /// prior simulation and uses it to generate an emission source.
    /// </summary>
    public class FluorescenceEmissionAOfXAndYAndZSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of FluorescenceEmissionAOfXAndYAndZSourceInput class
        /// </summary>
        /// <param name="inputFolder">Folder where AOfXAndYAndZ resides</param>
        /// <param name="infile">Infile for simulation that generated AOfXAndYAndZ</param>
        /// <param name="initialTissueRegionIndex">Tissue region of fluorescence</param>
        public FluorescenceEmissionAOfXAndYAndZSourceInput(
            string inputFolder, string infile, int initialTissueRegionIndex)
        {
            SourceType = "FluorescenceEmissionAOfXAndYAndZ";
            InputFolder = inputFolder;
            Infile = infile;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of FluorescenceEmissionAOfXAndYAndZSourceInput class
        /// </summary>
        public FluorescenceEmissionAOfXAndYAndZSourceInput()
            : this("", "",  0) { }

        /// <summary>
        ///  fluorescence emission source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// Input folder where AE(x,y,z) resides
        /// </summary>
        public string InputFolder { get; set; }
        /// <summary>
        /// Infile that generated AOfXAndYAndZ
        /// </summary>
        public string Infile { get; set; }
        /// <summary>
        /// Initial tissue region index = tissue region index of fluorescence
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new FluorescenceEmissionAOfXAndYAndZSource(
                this.InputFolder,
                this.Infile,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements FluorescenceEmissionAOfXAndYAndZSource with AOfXAndYAndZ created from prior
    /// simulation and initial tissue region index.
    /// </summary>
    public class FluorescenceEmissionAOfXAndYAndZSource : FluorescenceEmissionSourceBase
    {
        /// <summary>
        /// class that holds all Source arrays for proper initiation
        /// </summary>
        public AOfXAndYAndZLoader Loader { get; set; }
 
        /// <summary>
        /// Returns an instance of  Fluorescence Emission AOfXAndYAndZ Source with
        /// a Lambertian angular distribution.
        /// </summary>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public FluorescenceEmissionAOfXAndYAndZSource(
            string inputFolder,
            string infile,
            int initialTissueRegionIndex)
            : base(
                inputFolder,
                infile,
                initialTissueRegionIndex)
        {
            Loader = new AOfXAndYAndZLoader(inputFolder, infile, initialTissueRegionIndex);
        }

        protected override Position GetFinalPosition(Random rng)
        {
            Position finalPosition = null;
            // determine position from CDF determined in AOfXAndYAndZLoader
            // due to ordering of indices CDF will be increasing with each increment
            double rho = rng.NextDouble();
            for (int i = 0; i < Loader.X.Count - 1; i++)
            {
                for (int j = 0; j < Loader.Y.Count - 1; j++)
                {
                    for (int k = 0; k < Loader.Z.Count - 1; k++)
                    {
                        if (Loader.MapOfXAndYAndZ[i, j, k] == 1)
                        {
                            if (rho < Loader.CDFOfXAndYAndZ[i, j, k]) 
                            {
                                double xMidpoint = Loader.X.Start + i * Loader.X.Delta + Loader.X.Delta / 2;
                                double yMidpoint = Loader.Y.Start + j * Loader.Y.Delta + Loader.Y.Delta / 2;
                                double zMidpoint = Loader.Z.Start + k * Loader.Z.Delta + Loader.Z.Delta / 2;
                                return new Position(xMidpoint, yMidpoint, zMidpoint);
                            }
                        }
                    }
                }
            }
            return finalPosition;
        }
    }

}
