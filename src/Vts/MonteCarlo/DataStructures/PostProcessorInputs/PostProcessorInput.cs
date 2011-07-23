using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
#if !SILVERLIGHT
    [Serializable]
#endif     
    // Detector inputs
    [KnownType(typeof(AOfRhoAndZDetectorInput))]
    [KnownType(typeof(ATotalDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZDetectorInput))]
    [KnownType(typeof(RDiffuseDetectorInput))]
    [KnownType(typeof(ROfAngleDetectorInput))]
    [KnownType(typeof(ROfRhoAndAngleDetectorInput))]
    [KnownType(typeof(ROfRhoAndOmegaDetectorInput))]
    [KnownType(typeof(ROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(ROfRhoDetectorInput))]
    [KnownType(typeof(ROfXAndYDetectorInput))]
    [KnownType(typeof(TDiffuseDetectorInput))]
    [KnownType(typeof(TOfAngleDetectorInput))]
    [KnownType(typeof(TOfRhoAndAngleDetectorInput))]
    [KnownType(typeof(TOfRhoDetectorInput))]

    ///<summary>
    /// Defines input to the Monte Carlo post processor.  This includes a list
    /// of DetectorInputs, Database filename to postprocess, and the SimulationInput
    /// that generated the datbase.
    ///</summary>
    public class PostProcessorInput
    {
        public string DatabaseSimulationInputFilename;
        public IList<string> DatabaseFilenames;
        public IList<DatabaseType> DatabaseTypes;
        public IList<IDetectorInput> DetectorInputs;
        public string OutputName;

        /// <summary>
        /// Default constructor loads default values for InputData
        /// </summary>
        public PostProcessorInput(
            IList<IDetectorInput> detectorInputs,
            string inputFolder,
            IList<string> databaseFilenames,
            IList<DatabaseType> databaseTypes,
            string databaseSimulationInputFilename,
            string outputName)
        {
            DetectorInputs = detectorInputs;
            DatabaseFilenames = databaseFilenames;
            DatabaseTypes = databaseTypes;
            DatabaseSimulationInputFilename = databaseSimulationInputFilename;
            outputName = OutputName;
        }

        public PostProcessorInput()
            : this(
                new List<IDetectorInput>
                    {
                        new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                    },
                "results",
                new List<string> 
                    {
                        "_photonExitDataPoints"
                    },
                new List<DatabaseType>
                {
                    DatabaseType.PhotonExitDataPoints
                },
                "infile",
                "Output"
                ) {}

        public static PostProcessorInput FromFile(string filename)
        {
            return FileIO.ReadFromXML<PostProcessorInput>(filename);
        }

        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }

        public static PostProcessorInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromXMLInResources<PostProcessorInput>(filename, project);
        }
    }
}
