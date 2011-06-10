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
    // todo: Can we do this programmatcially? DataContractResolver? Automatically via convention?
    
    // Source inputs
    [KnownType(typeof(CustomPointSourceInput))]

    // Tissue inputs
    [KnownType(typeof(MultiLayerTissueInput))]
    
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

    // todo: add more types?
    ///<summary>
    /// Defines input to the Monte Carlo simulation.  This includes the output
    /// file name, number of photons to execute (N), source, tissue and detector
    /// definitions.
    ///</summary>
    public class SimulationInput
    {
        // DC 3/9/2010 using public fields *specifically* for ease of use in input .xml classes
        // todo: replace DataContractSerializer loading this class with Linq to XML reading
        // of a "pure" XML input class (and make all the fields properties again). This should
        // make it much easier for users to define simulations without wading through XML namespaces, etc.
        public string OutputName;
        public long N;
        public SimulationOptions Options;
        public ISourceInput SourceInput;
        public ITissueInput TissueInput;
        public IList<IDetectorInput> DetectorInputs;

        /// <summary>
        /// Default constructor loads default values for InputData
        /// </summary>
        public SimulationInput(
            long numberOfPhotons, 
            string outputName,
            SimulationOptions simulationOptions,
            ISourceInput sourceInput, 
            ITissueInput tissueInput,  
            IList<IDetectorInput> detectorInputs)
        {
            N = numberOfPhotons;
            OutputName = outputName;
            Options = simulationOptions;
            SourceInput = sourceInput;
            TissueInput = tissueInput;
            DetectorInputs = detectorInputs;
        }

        public SimulationInput()
            : this(
                1000000,
                "results",
                new SimulationOptions(
                    SimulationOptions.GetRandomSeed(), 
                    RandomNumberGeneratorType.MersenneTwister, 
                    AbsorptionWeightingType.Discrete, 
                    PhaseFunctionType.HenyeyGreenstein,
                    null, // databases written
                    true, // compute Second Moment
                    0),
                new CustomPointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    0),
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0))
                    }),
                new List<IDetectorInput>
                    {
                        new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                    }
                )
        {
            
        }

        public static SimulationInput FromFile(string filename)
        {
            return FileIO.ReadFromXML<SimulationInput>(filename);
        }

        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }

        public static SimulationInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromXMLInResources<SimulationInput>(filename, project);
        }
    }
}
