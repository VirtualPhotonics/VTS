using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo
{
    /// <summary>
    /// Unit tests for MonteCarloSimulation
    /// </summary>
    [TestFixture]
    public class MonteCarloSimulationTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt", // file that capture screen output of MC simulation
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);   
            }
        }

        /// <summary>
        /// Validate RunAll Given Two Simulations Runs Without Crashing
        /// </summary>
        [Test]
        public void validate_RunAll_given_two_simulations_runs_without_crashing()
        {
            var si1 = new SimulationInput { N = 30 };
            var si2 = new SimulationInput { N = 20 };
            var sim1 = new MonteCarloSimulation(si1);
            var sim2= new MonteCarloSimulation(si2);
            var sims = new[] {sim1, sim2};

            var outputs = MonteCarloSimulation.RunAll(sims);

            Assert.NotNull(outputs[0]);
            Assert.NotNull(outputs[1]);
            Assert.True(outputs[0].Input.N == 30);
            Assert.True(outputs[1].Input.N == 20);
        }
        /// <summary>
        /// Validate method that runs multiple CPUs runs without crashing
        /// </summary>
        [Test]
        public void validate_ExecuteLoopOverPhotonsInParallel_generates_equivalent_as_single_CPU()
        {
            // first run with single processor
            var si = new SimulationInput { N = 100 };
            si.Options.SimulationIndex = 0;  // 0 -> 1 CPUS
            si.Options.Seed = 0;
            si.DetectorInputs = new List<IDetectorInput>
            {
                new ATotalDetectorInput() { TallySecondMoment = true },
            };
            var mc =  new MonteCarloSimulation(si);
            var output = mc.Run();
            // then run same simulation with 2 CPUs
            // these will never be equal unless second sequence starts right after first!!!
            si.Options.SimulationIndex = 1;
            mc = new MonteCarloSimulation(si);
            var outputMultiCPUs = mc.Run();
            Assert.IsTrue(Math.Abs(output.Atot - outputMultiCPUs.Atot) < 0.1);
            Assert.IsTrue(Math.Abs(output.Atot2 - outputMultiCPUs.Atot2) < 0.0001);
        }
    }
}
