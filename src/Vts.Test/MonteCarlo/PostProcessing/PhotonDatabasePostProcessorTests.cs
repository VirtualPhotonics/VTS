using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.PostProcessing
{
    /// <summary>
    /// PhotonTerminationDatabasePostProcessorTests tests whether the 
    /// methods within the PhotonTerminationDatabasePostProcessor class regenerates 
    /// the same results as the on the fly results.
    /// </summary>
    [TestFixture]
    public class PhotonDatabasePostProcessorTests
    {
        private static IList<IDetectorInput> _detectorInputs;
        private static ISourceInput _sourceInput;
        private static ITissueInput _tissueInput;

        [TestFixtureSetUp]
        public void setup_simulation_input_components()
        {
            _detectorInputs = new List<IDetectorInput>()
            {
                new ROfRhoAndTimeDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 10.0, 101),
                    Time = new DoubleRange(0.0, 1, 101)
                }
            };
            _sourceInput = new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1);
            _tissueInput =  new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
            );
        }
        /// <summary>
        /// validate_photon_database_postprocessor_ROfRhoAndTime reads the output data 
        /// generated on the fly by MonteCarloSimulation and using the same binning 
        /// that was used for that output, generates the output data using 
        /// the postprocessing code, and compares the two.  Thus validating that the 
        /// on the fly and postprocessing results agree.
        /// It currently tests a point source with a layered tissue geometry and 
        /// validates the R(rho,time) tally.
        /// </summary>
        [Test]
        public void validate_photon_database_postprocessor_ROfRhoAndTime_results()
        {
            // DAW postprocssing
            var DAWinput = GenerateReferenceDAWInput();
            var onTheFlyDAWOutput =  new MonteCarloSimulation(DAWinput).Run();

            var DAWdatabase = PhotonDatabase.FromFile("DiffuseReflectanceDatabase");
            var DAWpostProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.DiffuseReflectance,
                _detectorInputs,
                DAWdatabase,
                onTheFlyDAWOutput.Input);
            var postProcessedDAWOutput = DAWpostProcessor.Run();

            ValidateROfRhoAndTime(onTheFlyDAWOutput, postProcessedDAWOutput);

            // CAW postprocessing
            var CAWinput = GenerateReferenceCAWInput();
            var onTheFlyCAWOutput = new MonteCarloSimulation(CAWinput).Run();

            var CAWdatabase = PhotonDatabase.FromFile("DiffuseReflectanceDatabase");
            var CAWpostProcessor = new PhotonDatabasePostProcessor(
                VirtualBoundaryType.DiffuseReflectance,
                _detectorInputs,
                CAWdatabase,
                onTheFlyCAWOutput.Input);
            var postProcessedCAWOutput = CAWpostProcessor.Run();

            ValidateROfRhoAndTime(onTheFlyCAWOutput, postProcessedCAWOutput);
        }
        /// <summary>
        /// method to generate DAW input to the MC simulation
        /// </summary>
        /// <returns>SimulationInput</returns>
        public static SimulationInput GenerateReferenceDAWInput()
        {
            return new SimulationInput(
                100,
                "", // can't give folder name when writing to isolated storage
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { DatabaseType.DiffuseReflectance },
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    1),
                 _sourceInput,
                 _tissueInput,
                 _detectorInputs
            );
        }
        /// <summary>
        /// method to generate CAW input to the MC simulation
        /// </summary>
        /// <returns>SimulationInput</returns>
        public static SimulationInput GenerateReferenceCAWInput()
        {
            return new SimulationInput(
                100,
                "", // can't give folder name when writing to isolated storage
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { DatabaseType.DiffuseReflectance },
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    1),
                 _sourceInput,
                 _tissueInput,
                 _detectorInputs
            );
        }
        
        /// <summary>
        /// method that takes two Output classes, output1 and output2, and
        /// compares their R(rho,time) results
        /// </summary>
        /// <param name="output1"></param>
        /// <param name="output2"></param>
        private void ValidateROfRhoAndTime(SimulationOutput output1, SimulationOutput output2)
        {
            var detector = (ROfRhoAndTimeDetectorInput)_detectorInputs.
                Where(d => d.TallyType == "ROfRhoAndTime").First(); 
            // currently these are agreeing EXCEPT for last bin i=99, j=99
            var out1 = output1.R_rt[99, 99];
            var out2 = output2.R_rt[99, 99];
            Assert.Less(Math.Abs(out2 - out1)/out1, 1e-10);
            for (int i = 0; i < detector.Rho.Count - 1; i++)
            {
                for (int j = 0; j < detector.Time.Count - 1; j++)
                {
                    // round off error about 1e-18
                    if (output1.R_rt[i, j] > 0.0)
                    {
                        Assert.Less(Math.Abs(output2.R_rt[i, j] - output1.R_rt[i, j]) / output1.R_rt[i, j], 1e-10);
                    }
                }
            }
        }

        [Test]
        public void validate_database_input_with_no_detectors_specified_still_generates_database()
        {
            // make sure databases generated from previous tests are deleted
            if (FileIO.FileExists("DiffuseReflectanceDatabase.txt"))
            {
                FileIO.FileDelete("DiffuseReflectanceDatabase.txtl");
            }
            if (FileIO.FileExists("DiffuseReflectanceDatabase"))
            {
                FileIO.FileDelete("DiffuseReflectanceDatabase");
            }
            var input = new SimulationInput(
                100,
                "", // can't give folder name when writing to isolated storage
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { DatabaseType.DiffuseReflectance }, // SPECIFY DATABASE
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    1),
                 _sourceInput,
                 _tissueInput,
                 new List<IDetectorInput>(){} // specify NO DETECTORS
            );
            var output =  new MonteCarloSimulation(input).Run();
            Assert.IsTrue(FileIO.FileExists("DiffuseReflectanceDatabase"));
            Assert.IsFalse(FileIO.FileExists("DiffuseTransmittanceDatabase"));
        }
    }
}