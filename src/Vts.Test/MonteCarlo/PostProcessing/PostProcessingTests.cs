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
    /// ValidatePostProcessing reads the output data generated on the fly by MonteCarloSimulation and
    /// using the same binning that was used for that output, generates the output data using 
    /// the postprocessing code, and compares the two.  Thus validating that the on the fly and 
    /// postprocessing results agree
    /// </summary>
    [TestFixture]
    public class PhotonTerminationDatabasePostProcessorTests
    {
        /// <summary>
        /// ValidatePhotonExitHistoryPostProcessor tests whether the methods within the
        /// PhotonTerminationDatabasePostProcessor class regenerates the same results as the on the fly results.
        /// It currently tests a point source with a layered tissue geometry.
        /// </summary>
        [Test]
        public void validate_photon_termination_database_postprocessor()
        {
            var input = GenerateReferenceInput();
            var onTheFlyOutput = GenerateReferenceOutput(input);

            var peh = PhotonTerminationDatabase.FromFile("_photonBiographies");
            var postProcessedOutput = PhotonTerminationDatabasePostProcessor.GenerateOutput(
                input.DetectorInput, peh, onTheFlyOutput);

            ValidateROfRhoAndTime(onTheFlyOutput, postProcessedOutput);
        }

        private static SimulationInput GenerateReferenceInput()
        {
            return new SimulationInput(
                100,
                "",
                new PointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)),
                new MultiLayerTissueInput(
                    new List<LayerRegion> 
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0, 2),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(10.0, double.PositiveInfinity, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete)
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
                    new DoubleRange(0.0, Math.PI / 2, 1), // angle
                    new DoubleRange(0.0, 4.0, 801), // time: nt=800 dt=0.005ns used for workshop
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-100.0, 100.0, 81), // x
                    new DoubleRange(-100.0, 100.0, 81) // y
            ));
        }

        private static Output GenerateReferenceOutput(SimulationInput input)
        {
            // turn on option to write photon exit data to database (second from last parameter "true")
            SimulationOptions options = 
                new SimulationOptions(0, RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete, false, false, true, false, 0);

            var onTheFlyOutput = new MonteCarloSimulation(input, options).Run();

            return onTheFlyOutput;
        }

        private void ValidateROfRhoAndTime(Output output1, Output output2)
        {
            for (int i = 0; i < output1.input.DetectorInput.Rho.Count; i++)
            {
                for (int j = 0; j < output1.input.DetectorInput.Time.Count; j++)
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