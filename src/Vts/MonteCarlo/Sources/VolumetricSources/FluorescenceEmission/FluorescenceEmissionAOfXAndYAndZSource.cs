using System;
using System.Linq;
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
        /// <param name="detectorName">Name of AOfXAndYAndZ detector binary (default=AOfXAndYAndZ)</param>
        /// <param name="initialTissueRegionIndex">Tissue region of fluorescence</param>
        public FluorescenceEmissionAOfXAndYAndZSourceInput(
            string inputFolder, string infile, string detectorName, int initialTissueRegionIndex)
        {
            SourceType = "FluorescenceEmissionAOfXAndYAndZ";
            InputFolder = inputFolder;
            Infile = infile;
            DetectorName = detectorName;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of FluorescenceEmissionAOfXAndYAndZSourceInput class
        /// </summary>
        /// <param name="inputFolder">Folder where AOfXAndYAndZ resides</param>
        /// <param name="infile">infile of simulation that generated AOfXAndYAndZ </param>
        public FluorescenceEmissionAOfXAndYAndZSourceInput(string inputFolder, string infile, int initialTissueRegionIndex)
            : this(inputFolder, infile, "AOfXAndYAndZ", initialTissueRegionIndex) { }

        /// <summary>
        /// Initializes the default constructor of FluorescenceEmissionAOfXAndYAndZSourceInput class
        /// </summary>
        public FluorescenceEmissionAOfXAndYAndZSourceInput()
            : this("", "", "AOfXandYAndZ", 0) { }

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
        /// Name of detector binary, default=AOfXAndYAndZ
        /// </summary>
        public string DetectorName { get; set; }
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
                this.DetectorName,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements FluorescenceEmissionAOfXAndYAndZSource with AOfXAndYAndZ created from prior
    /// simulation and initial tissue region index.
    /// </summary>
    public class FluorescenceEmissionAOfXAndYAndZSource : FluorescenceEmissionSourceBase
    {
        private static AOfXAndYAndZLoader _aOfXAndYAndZLoader;

        /// <summary>
        /// Returns an instance of  Fluorescence Emission AOfXAndYAndZ Source with
        /// a Lambertian angular distribution.
        /// </summary>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public FluorescenceEmissionAOfXAndYAndZSource(
            string inputFolder,
            string infile,
            string detectorName,
            int initialTissueRegionIndex)
            : base(
                inputFolder,
                infile,
                detectorName,
                initialTissueRegionIndex)
        {
            _aOfXAndYAndZLoader = new AOfXAndYAndZLoader(inputFolder, infile, detectorName, initialTissueRegionIndex);
        }

        protected override Position GetFinalPosition(ITissue tissue, int initialTissueRegionIndex, Random rng)
        {
            Position finalPosition = null;
            // determine position from CDF determined in AOfXAndYAndZLoader
            double rho = rng.NextDouble();
            for (int i = 0; i < _aOfXAndYAndZLoader.X.Count - 1; i++)
            {
                double xMidpoint = _aOfXAndYAndZLoader.X.Start + i * _aOfXAndYAndZLoader.X.Delta;
                for (int j = 0; j < _aOfXAndYAndZLoader.Y.Count - 1; j++)
                {
                    double yMidpoint = _aOfXAndYAndZLoader.X.Start + j * _aOfXAndYAndZLoader.Y.Delta;
                    for (int k = 0; k < _aOfXAndYAndZLoader.Z.Count - 1; k++)
                    {

                        double zMidpoint = _aOfXAndYAndZLoader.Z.Start + k * _aOfXAndYAndZLoader.Z.Delta;
                        if (_aOfXAndYAndZLoader.MapOfXAndYAndZ[i, j, k] == 1)
                        {
                            if (rho > _aOfXAndYAndZLoader.CDFOfXAndYAndZ[i, j, k])
                            {
                                finalPosition.X = xMidpoint;
                                finalPosition.Y = yMidpoint;
                                finalPosition.Z = zMidpoint;
                            }
                        }
                    }
                }
            }
            return finalPosition;
        }
    }

}
