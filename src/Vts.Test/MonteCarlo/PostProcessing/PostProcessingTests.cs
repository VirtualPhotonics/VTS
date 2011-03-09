using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
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
    public class PhotonTerminationDatabasePostProcessorTests
    {
        /// <summary>
        /// validate_photon_termination_database_postprocessor reads the output data 
        /// generated on the fly by MonteCarloSimulation and using the same binning 
        /// that was used for that output, generates the output data using 
        /// the postprocessing code, and compares the two.  Thus validating that the 
        /// on the fly and postprocessing results agree.
        /// It currently tests a point source with a layered tissue geometry and 
        /// validates the R(rho,time) tally.
        /// </summary>
        [Test]
        public void validate_photon_termination_database_postprocessor()
        {
            var input = GenerateReferenceInput();
            var onTheFlyOutput = GenerateReferenceOutput(input);

            var database = PhotonTerminationDatabase.FromFile("postprocessing_photonBiographies");
            var postProcessedOutput = PhotonTerminationDatabasePostProcessor.GenerateOutput(
                input.DetectorInput, database, onTheFlyOutput.Input);

            ValidateROfRhoAndTime(onTheFlyOutput, postProcessedOutput);
        }
        /// <summary>
        /// method to generate input to the MC simulation
        /// </summary>
        /// <returns>SimulationInput</returns>
        private static SimulationInput GenerateReferenceInput()
        {
            return new SimulationInput(
                100,
                "postprocessing",
                new SimulationOptions(
                    0, 
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete, 
                    true, 
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
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
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
                    new DoubleRange(0.0, Math.PI / 2, 2), // angle
                    new DoubleRange(0.0, 4.0, 801), // time: nt=800 dt=0.005ns used for workshop
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-100.0, 100.0, 81), // x
                    new DoubleRange(-100.0, 100.0, 81) // y
            ));
        }
        /// <summary>
        /// method to execute the MC and return the output from the tallies
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Output</returns>
        private static Output GenerateReferenceOutput(SimulationInput input)
        {
            return new MonteCarloSimulation(input).Run();
        }
        /// <summary>
        /// method that takes two Output classes, output1 and output2, and
        /// compares their R(rho,time) results
        /// </summary>
        /// <param name="output1"></param>
        /// <param name="output2"></param>
        private void ValidateROfRhoAndTime(Output output1, Output output2)
        {
            for (int i = 0; i < output1.Input.DetectorInput.Rho.Count - 1; i++)
            {
                for (int j = 0; j < output1.Input.DetectorInput.Time.Count - 1; j++)
                {
                    // round off error about 1e-18
                    if (output1.R_rt[i, j] > 0.0)
                    {
                        Assert.Less(Math.Abs(output2.R_rt[i, j] - output1.R_rt[i, j]) / output1.R_rt[i, j], 1e-10);
                    }
                }
            }
        }
    }
}