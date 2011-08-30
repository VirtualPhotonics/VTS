using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;


namespace Vts.MonteCarlo
{
#if !SILVERLIGHT
    [Serializable]
#endif
    ///<summary>
    /// Defines input to the Monte Carlo simulation.  This includes the output
    /// file name, number of photons to execute (N), source, tissue and detector
    /// definitions.
    ///</summary>
    
    // todo: Can we do this programmatcially? DataContractResolver? Automatically via convention?
    
    [KnownType(typeof(DirectionalPointSourceInput))]
    [KnownType(typeof(IsotropicPointSourceInput))]
    [KnownType(typeof(CustomPointSourceInput))]
    [KnownType(typeof(DirectionalLineSourceInput))]

    // Tissue inputs
    [KnownType(typeof(MultiLayerTissueInput))]
    [KnownType(typeof(SingleEllipsoidTissueInput))]
    
    // Detector inputs
    [KnownType(typeof(SurfaceVirtualBoundaryInput))]
    [KnownType(typeof(GenericVolumeVirtualBoundaryInput))]
    [KnownType(typeof(pMCSurfaceVirtualBoundaryInput))]

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
    [KnownType(typeof(RSpecularDetectorInput))]
    [KnownType(typeof(AOfRhoAndZDetectorInput))]
    [KnownType(typeof(ATotalDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZDetectorInput))]
    [KnownType(typeof(RadianceOfRhoAndZAndAngleDetectorInput))]
    [KnownType(typeof(pMCROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(pMCROfRhoDetectorInput))]

    // todo: add more types?

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
        //public IList<IDetectorInput> DetectorInputs;
        public IList<IVirtualBoundaryInput> VirtualBoundaryInputs;

        /// <summary>
        /// Default constructor loads default values for InputData
        /// </summary>
        public SimulationInput(
            long numberOfPhotons, 
            string outputName,
            SimulationOptions simulationOptions,
            ISourceInput sourceInput,
            ITissueInput tissueInput,  
            //IList<IDetectorInput> detectorInputs)
            IList<IVirtualBoundaryInput> virtualBoundaryInputs)
        {
            N = numberOfPhotons;
            OutputName = outputName;
            Options = simulationOptions;
            SourceInput = sourceInput;
            TissueInput = tissueInput;
            //DetectorInputs = detectorInputs;
            VirtualBoundaryInputs = virtualBoundaryInputs;
        }

        public SimulationInput()
            : this(
                100,
                "results",
                new SimulationOptions(
                    SimulationOptions.GetRandomSeed(),
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    true, // compute Second Moment
                    false, // track statistics
                    0),
                new DirectionalPointSourceInput(),

                new MultiLayerTissueInput(
                    new List<ITissueRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }),

                new List<IVirtualBoundaryInput>
                    {
                        new SurfaceVirtualBoundaryInput(
                            VirtualBoundaryType.DiffuseReflectance,
                            new List<IDetectorInput>
                            {
                                new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                            },
                            false,
                            VirtualBoundaryType.DiffuseReflectance.ToString()),
                        new SurfaceVirtualBoundaryInput(
                            VirtualBoundaryType.DiffuseTransmittance,
                            new List<IDetectorInput>
                            {
                            },
                            false,
                            VirtualBoundaryType.DiffuseTransmittance.ToString())
                    }
                ) { }

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
