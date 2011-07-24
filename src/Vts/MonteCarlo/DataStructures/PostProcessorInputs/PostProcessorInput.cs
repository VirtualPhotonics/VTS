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

    ///<summary>
    /// Defines input to the Monte Carlo post processor.  This includes a list
    /// of DetectorInputs, Database filename to postprocess, and the SimulationInput
    /// that generated the datbase.
    ///</summary>
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
