using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.PostProcessing;
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
        public void validate_photon_database_postprocessor()
        {
            var input = GenerateReferenceInput();
            var onTheFlyOutput =  new MonteCarloSimulation(input).Run();

            var database = PhotonDatabase.FromFile("DiffuseReflectanceDatabase");
            var postProcessedOutput = PhotonDatabasePostProcessor.GenerateOutput(
                VirtualBoundaryType.DiffuseReflectance,
                _detectorInputs,
                false, // tally second moment
                database,
                onTheFlyOutput.Input);

            ValidateROfRhoAndTime(onTheFlyOutput, postProcessedOutput);
        }
        /// <summary>
        /// method to generate input to the MC simulation
        /// </summary>
        /// <returns>SimulationInput</returns>
        public static SimulationInput GenerateReferenceInput()
        {
            _detectorInputs = new List<IDetectorInput>()
            {
                new ROfRhoAndTimeDetectorInput(
                    new DoubleRange(0.0, 10.0, 101),
                    new DoubleRange(0.0, 1, 101))
            };
            return new SimulationInput(
                100,
                "", // can't give folder name when writing to isolated storage
                new SimulationOptions(
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    //new List<DatabaseType>() { DatabaseType.PhotonExitDataPoints },
                    true, // compute Second Moment
                    false, // track statistics
                    1),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1),                 
                new MultiLayerTissueInput(
                    new List<ITissueRegion> 
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IVirtualBoundaryInput> 
                {
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.DiffuseReflectance,
                        _detectorInputs,
                        true,
                        VirtualBoundaryType.DiffuseReflectance.ToString()
                    ),
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.DiffuseTransmittance,
                        null,
                        false,
                        VirtualBoundaryType.DiffuseTransmittance.ToString()
                    )
                }
            );
        }
        
        /// <summary>
        /// method that takes two Output classes, output1 and output2, and
        /// compares their R(rho,time) results
        /// </summary>
        /// <param name="output1"></param>
        /// <param name="output2"></param>
        private void ValidateROfRhoAndTime(Output output1, Output output2)
        {
            var detector = (ROfRhoAndTimeDetectorInput)_detectorInputs.
                Where(d => d.TallyType == TallyType.ROfRhoAndTime).First(); 
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
    }
}