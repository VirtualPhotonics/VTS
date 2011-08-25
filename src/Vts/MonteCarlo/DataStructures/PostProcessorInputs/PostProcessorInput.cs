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
    
    ///<summary>
    /// Defines input to the Monte Carlo post processor.  This includes a list
    /// of DetectorInputs, Database filename to postprocess, and the SimulationInput
    /// that generated the database.
    ///</summary>
    
    [KnownType(typeof(AOfRhoAndZDetectorInput))]
    [KnownType(typeof(ATotalDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZDetectorInput))]
    [KnownType(typeof(pMCROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(pMCROfRhoDetectorInput))]
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

    public class PostProcessorInput
    {
        public IList<IDetectorInput> DetectorInputs;
        public bool TallySecondMoment;
        public string InputFolder;
        public VirtualBoundaryType VirtualBoundaryType;
        public string DatabaseSimulationInputFilename;
        public string OutputName;

        /// <summary>
        /// Default constructor loads default values for InputData
        /// </summary>
        public PostProcessorInput(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            string inputFolder,
            string databaseSimulationInputFilename,
            string outputName)
        {
            DetectorInputs = detectorInputs;
            TallySecondMoment = tallySecondMoment;
            InputFolder = inputFolder;
            VirtualBoundaryType = virtualBoundaryType;
            DatabaseSimulationInputFilename = databaseSimulationInputFilename;
            OutputName = outputName;
        }

        public PostProcessorInput()
            : this(
                VirtualBoundaryType.DiffuseReflectance,
                new List<IDetectorInput>
                    {
                        new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                    },
                false, // tally second moment
                "results",
                "infile",
                "ppresults"
                ) {}

        /// <summary>
        /// Method to read this class from xml file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static PostProcessorInput FromFile(string filename)
        {
            return FileIO.ReadFromXML<PostProcessorInput>(filename);
        }
        /// <summary>
        /// Method to write this class to xml file.
        /// </summary>
        /// <param name="filename"></param>
        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }
        /// <summary>
        /// Method to read this class from file in Resources
        /// </summary>
        /// <param name="filename">filename to be read</param>
        /// <param name="project">project where file resides</param>
        /// <returns></returns>
        public static PostProcessorInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromXMLInResources<PostProcessorInput>(filename, project);
        }
    }
}
