using System.Collections.Generic;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    ///<summary>
    /// Defines input to the Monte Carlo Post-Processor.  This includes the input folder
    /// and database names, detector definitions, and output folder name.
    ///</summary>
    public class PostProcessorInput
    {
        /// <summary>
        /// IList of IDetectorInput
        /// </summary>
        public IList<IDetectorInput> DetectorInputs { get; set; }

        /// <summary>
        /// string input folder
        /// </summary>
        public string InputFolder { get; set; }
        /// <summary>
        /// string identifying database SimulationInput filename
        /// </summary>
        public string DatabaseSimulationInputFilename { get; set; }
        /// <summary>
        /// string identifying output folder name
        /// </summary>
        public string OutputName { get; set; }

        /// <summary>
        /// constructor for post-processor input
        /// </summary>
        /// <param name="detectorInputs">list of detector inputs</param>
        /// <param name="inputFolder">input folder name, where database file(s), etc. reside</param>
        /// <param name="databaseSimulationInputFilename">filename of simulation input file that generated database to be post-processed</param>
        /// <param name="outputName">output folder name</param>
        public PostProcessorInput(
            IList<IDetectorInput> detectorInputs,
            string inputFolder,
            string databaseSimulationInputFilename,
            string outputName)
        {
            DetectorInputs = detectorInputs;
            InputFolder = inputFolder;
            DatabaseSimulationInputFilename = databaseSimulationInputFilename;
            OutputName = outputName;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public PostProcessorInput()
            : this(
                //VirtualBoundaryType.DiffuseReflectance,
                new List<IDetectorInput>
                    {
                        new ROfRhoDetectorInput
                        {
                            Rho = new DoubleRange(0.0, 40.0, 201), // rho: nr=200 dr=0.2mm used for workshop)
                        }
                    },
                "results",
                "infile",
                "ppresults"
                ) {}

        /// <summary>
        /// Method to read this class from JSON file.
        /// </summary>
        /// <param name="filename">string file name</param>
        /// <returns>An instance of the PostProcessorInput class</returns>
        public static PostProcessorInput FromFile(string filename)
        {
            return FileIO.ReadFromJson<PostProcessorInput>(filename);
        }
        /// <summary>
        /// Method to write this class to JSON file.
        /// </summary>
        /// <param name="filename">string file name</param>
        public void ToFile(string filename)
        {
            this.WriteToJson(filename);
        }
        /// <summary>
        /// Method to read this class from file in Resources
        /// </summary>
        /// <param name="filename">filename to be read</param>
        /// <param name="project">project where file resides</param>
        /// <returns>An instance of the PostProcessorInput class</returns>
        public static PostProcessorInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromJsonInResources<PostProcessorInput>(filename, project);
        }
    }
}
