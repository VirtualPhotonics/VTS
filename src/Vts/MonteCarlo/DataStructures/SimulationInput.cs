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

    [KnownType(typeof(CustomPointSourceInput))]
    [KnownType(typeof(MultiLayerTissueInput))]
    [KnownType(typeof(DetectorInput))]
    
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
        public string OutputFileName;
        public long N;
        public SimulationOptions Options;
        public ISourceInput SourceInput;
        public ITissueInput TissueInput;
        public IDetectorInput DetectorInput;

        /// <summary>
        /// Default constructor loads default values for InputData
        /// </summary>
        public SimulationInput(
            long numberOfPhotons, 
            string outputFilename,
            SimulationOptions simulationOptions,
            ISourceInput sourceInput, 
            ITissueInput tissueInput,  
            IDetectorInput detectorInput)
        {
            N = numberOfPhotons;
            OutputFileName = outputFilename;
            Options = simulationOptions;
            SourceInput = sourceInput;
            TissueInput = tissueInput;
            DetectorInput = detectorInput;
        }
        public SimulationInput()
            : this(
                //(long)1e5, 
                //"Output", 
                //new PointSourceInput(),
                //new MultiLayerTissueInput(), 
                //new DetectorInput()) { }
                1000000,  // FIX 1e6 takes about 70 minutes my laptop
                "Output",
                new SimulationOptions(
                    SimulationOptions.GetRandomSeed(), 
                    RandomNumberGeneratorType.MersenneTwister, 
                    AbsorptionWeightingType.Discrete, 
                    false, 
                    0),
                new CustomPointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)),
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
                new DetectorInput(
                    new List<TallyType>()
                    {
                        TallyType.RDiffuse,
                        TallyType.ROfAngle,
                        TallyType.ROfRho,
                        TallyType.ROfRhoAndAngle,
                        TallyType.ROfRhoAndTime,
                        TallyType.ROfXAndY,
                        TallyType.ROfRhoAndOmega,
                        TallyType.TDiffuse,
                        TallyType.TOfAngle,
                        TallyType.TOfRho,
                        TallyType.TOfRhoAndAngle,
                    },
                    new DoubleRange(0.0, 40.0, 201), // rho: nr=200 dr=0.2mm used for workshop
                    new DoubleRange(0.0, 10.0, 11),  // z
                    new DoubleRange(0.0, Math.PI / 2, 1), // angle
                    new DoubleRange(0.0, 4.0, 801), // time: nt=800 dt=0.005ns used for workshop
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-100.0, 100.0, 81), // x
                    new DoubleRange(-100.0, 100.0, 81) // y
                )) {}

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
