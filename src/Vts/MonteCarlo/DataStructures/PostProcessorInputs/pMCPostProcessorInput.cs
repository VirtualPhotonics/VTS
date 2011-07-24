using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Factories;

namespace Vts.MonteCarlo
{
#if !SILVERLIGHT
    [Serializable]
#endif 
    
    // Detector inputs
    [KnownType(typeof(pMCROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(pMCROfRhoDetectorInput))]

    ///<summary>
    /// Defines input to the Monte Carlo post processor for pMC processing.  This includes a list
    /// of DetectorInputs, Database filename to postprocess, and the SimulationInput
    /// that generated the database.
    ///</summary>
    public class pMCPostProcessorInput
    {
        /// <summary>
        /// Default constructor loads default values for InputData
        /// </summary>
        public pMCPostProcessorInput(
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
            }

        public pMCPostProcessorInput()
            : this(
                VirtualBoundaryType.pMCDiffuseReflectance,
                new List<IDetectorInput>
                    {
                        new pMCROfRhoDetectorInput(
                            new DoubleRange(0.0, 40.0, 201),
                            // set perturbed ops to reference ops
                            new List<OpticalProperties>() { 
                                new OpticalProperties(),
                                new OpticalProperties(),
                                new OpticalProperties()
                            },
                            new List<int>() { 1 },
                            TallyType.pMCROfRho.ToString())
                    },
                false,
                "results",
                "infile",
                "pMCresults") {}

        public IList<IDetectorInput> DetectorInputs { get; set; }
        public bool TallySecondMoment { get; set; }
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
        public string DatabaseSimulationInputFilename { get; set; }
        public string InputFolder { get; set; }

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
