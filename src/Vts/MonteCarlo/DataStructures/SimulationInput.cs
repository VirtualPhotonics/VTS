using System.Collections.Generic;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    ///<summary>
    /// Defines input to the Monte Carlo simulation.  This includes the output
    /// file name, number of photons to execute (N), source, tissue and detector
    /// definitions.
    ///</summary>

    public class SimulationInput
    {
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
                    new List<DatabaseType>() {},
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(),

                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey1"),
                            new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            "HenyeyGreensteinKey2"),
                            new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey3")
                    }),

                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 40.0, 201)
                    }, // rho: nr=200 dr=0.2mm used for workshop)
                }
                )
            {
                TissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
                TissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
                TissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            }
    
        /// <summary>
        /// Method to read SimulationInput from JSON file
        /// </summary>
        /// <param name="filename">string filename of file to be read</param>
        /// <returns>SimulationInput</returns>
        public static SimulationInput FromFile(string filename)
        {
            return FileIO.ReadFromJson<SimulationInput>(filename);
        }

        public MonteCarloSimulation CreateSimulation()
        {
            return new MonteCarloSimulation(this);
        }

        /// <summary>
        /// Method to write SimulationInput to file
        /// </summary>
        /// <param name="filename">string filename to write to</param>
        public void ToFile(string filename)
        {
            FileIO.WriteToJson(this, filename);
        }
        /// <summary>
        /// Method to read SimulationInput json from file in resources
        /// </summary>
        /// <param name="filename">string filename</param>
        /// <param name="project">string project name</param>
        /// <returns>SimulationInput</returns>
        public static SimulationInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromJsonInResources<SimulationInput>(filename, project);
        }
    }
}
