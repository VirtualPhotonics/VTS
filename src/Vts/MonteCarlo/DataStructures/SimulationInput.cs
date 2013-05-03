using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.Tissues;


namespace Vts.MonteCarlo
{
    ///<summary>
    /// Defines input to the Monte Carlo simulation.  This includes the output
    /// file name, number of photons to execute (N), source, tissue and detector
    /// definitions.
    ///</summary>
    #if !SILVERLIGHT
        [Serializable]
    #endif
    
    // todo: Can we do this programmatcially? DataContractResolver? Automatically via convention?
	[KnownType(typeof(CustomLineSourceInput))]
    [KnownType(typeof(DirectionalLineSourceInput))]
	[KnownType(typeof(IsotropicLineSourceInput))]
				
    [KnownType(typeof(CustomPointSourceInput))]
	[KnownType(typeof(DirectionalPointSourceInput))]
    [KnownType(typeof(IsotropicPointSourceInput))]
				
	[KnownType(typeof(LambertianSurfaceEmittingCylindricalFiberSourceInput))]
	[KnownType(typeof(CustomSurfaceEmittingSphericalSourceInput))]
	[KnownType(typeof(LambertianSurfaceEmittingSphericalSourceInput))]
	[KnownType(typeof(LambertianSurfaceEmittingTubularSourceInput))]
				
	[KnownType(typeof(CustomCircularSourceInput))]
	[KnownType(typeof(DirectionalCircularSourceInput))]
	[KnownType(typeof(CustomEllipticalSourceInput))]
	[KnownType(typeof(DirectionalEllipticalSourceInput))]
	[KnownType(typeof(CustomRectangularSourceInput))]
	[KnownType(typeof(DirectionalRectangularSourceInput))]
				
	[KnownType(typeof(CustomVolumetricEllipsoidalSourceInput))]
	[KnownType(typeof(IsotropicVolumetricEllipsoidalSourceInput))]
	[KnownType(typeof(CustomVolumetricCuboidalSourceInput))]
	[KnownType(typeof(IsotropicVolumetricCuboidalSourceInput))]
    
    // Tissue inputs
    [KnownType(typeof(MultiLayerTissueInput))]
    [KnownType(typeof(SingleEllipsoidTissueInput))]
	[KnownType(typeof(MultiEllipsoidTissueInput))]
	
    // Detector inputs
    [KnownType(typeof(RDiffuseDetectorInput))]
    [KnownType(typeof(ROfAngleDetectorInput))]
    [KnownType(typeof(ROfRhoAndAngleDetectorInput))]
    [KnownType(typeof(ROfRhoAndOmegaDetectorInput))]
    [KnownType(typeof(ROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(ROfRhoDetectorInput))]
    [KnownType(typeof(ROfXAndYDetectorInput))]
    [KnownType(typeof(ROfFxDetectorInput))]
    [KnownType(typeof(ROfFxAndTimeDetectorInput))]
    [KnownType(typeof(TDiffuseDetectorInput))]
    [KnownType(typeof(TOfAngleDetectorInput))]
    [KnownType(typeof(TOfRhoAndAngleDetectorInput))]
    [KnownType(typeof(TOfRhoDetectorInput))]
    [KnownType(typeof(RSpecularDetectorInput))]
    [KnownType(typeof(AOfRhoAndZDetectorInput))]
    [KnownType(typeof(ATotalDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZDetectorInput))]
    [KnownType(typeof(FluenceOfXAndYAndZDetectorInput))]
    [KnownType(typeof(RadianceOfRhoAndZAndAngleDetectorInput))]
    [KnownType(typeof(RadianceOfXAndYAndZAndThetaAndPhiDetectorInput))]
    [KnownType(typeof(pMCROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(pMCROfRhoDetectorInput))]
    [KnownType(typeof(pMCROfFxDetectorInput))]
    [KnownType(typeof(dMCdROfRhodMuaDetectorInput))]
    [KnownType(typeof(dMCdROfRhodMusDetectorInput))]
    [KnownType(typeof(pMCROfFxAndTimeDetectorInput))]
    [KnownType(typeof(ReflectedMTOfRhoAndSubregionHistDetectorInput))]
    [KnownType(typeof(ReflectedTimeOfRhoAndSubregionHistDetectorInput))]

    // todo: add more types?

    public class SimulationInput
    {
        // DC 3/9/2010 using public fields *specifically* for ease of use in input .xml classes
        // todo: replace DataContractSerializer loading this class with Linq to XML reading
        // of a "pure" XML input class (and make all the fields properties again). This should
        // make it much easier for users to define simulations without wading through XML namespaces, etc.
        /// <summary>
        /// string name of output file
        /// </summary>
        public string OutputName;
        /// <summary>
        /// number of photons launched from source
        /// </summary>
        public long N;
        /// <summary>
        /// SimulationOptions specify, for example, absorption weighting type
        /// </summary>
        public SimulationOptions Options;
        /// <summary>
        /// source input (ISourceInput)
        /// </summary>
        public ISourceInput SourceInput;
        /// <summary>
        /// tissue input (ITissueInput)
        /// </summary>
        public ITissueInput TissueInput;
        /// <summary>
        /// detector input (IList of IDetectorInput)
        /// </summary>
        public IList<IDetectorInput> DetectorInputs;

        /// <summary>
        /// Monte Carlo simulation input data
        /// </summary>
        /// <param name="numberOfPhotons">long number indicating number of photons launched from source</param>
        /// <param name="outputName">string indicating output name</param>
        /// <param name="simulationOptions">options to execute simulation</param>
        /// <param name="sourceInput">ISourceInput specifying source of light</param>
        /// <param name="tissueInput">ITissueInput specifying tissue definition</param>
        /// <param name="detectorInputs">IDetectorInput specifying which detectors to tally</param>
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
            // check if detectorInputs list is null and if so make empty
            if (DetectorInputs == null)
            {
                DetectorInputs = new List<IDetectorInput>() {};
            }
        }
        /// <summary>
        /// SimulationInput default constructor
        /// </summary>
        public SimulationInput()
            : this(
                100,
                "results",
                new SimulationOptions(
                    -1, // get random seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new HenyeyGreensteinPhaseFunctionInput(),
                    new List<DatabaseType>() { },
                    true, // compute Second Moment
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(),

                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        new HenyeyGreensteinPhaseFunctionInput()),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        new HenyeyGreensteinPhaseFunctionInput()),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        new HenyeyGreensteinPhaseFunctionInput())
                    }),

                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                }
                ) { }
        /// <summary>
        /// Method to read SimulationInput from file
        /// </summary>
        /// <param name="filename">string filename of file to be read</param>
        /// <returns>SimulationInput</returns>
        public static SimulationInput FromFile(string filename)
        {
            return FileIO.ReadFromXML<SimulationInput>(filename);
        }
        /// <summary>
        /// Method to write SimulationInput to file
        /// </summary>
        /// <param name="filename">string filename to write to</param>
        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }
        /// <summary>
        /// Method to read SimulationInput xml from file in resources
        /// </summary>
        /// <param name="filename">string filename</param>
        /// <param name="project">string project name</param>
        /// <returns>SimulationInput</returns>
        public static SimulationInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromXMLInResources<SimulationInput>(filename, project);
        }
    }
}
