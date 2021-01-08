using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo
{
    /// <summary>
    /// Unit tests for ParallelMonteCarloSimulation
    /// </summary>
    [TestFixture]
    public class ParallelMonteCarloSimulationTests
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
        /// Validate method that runs multiple CPUs runs without crashing
        /// </summary>
        [Test]
        public void validate_single_in_parallel_generates_equivalent_as_single_CPU()
        {
            // first run with single processor
            var si = new SimulationInput { N = 100 };
            si.Options.SimulationIndex = 0;  // 0 -> 1 CPUS
            si.Options.Seed = 0;
            si.DetectorInputs = new List<IDetectorInput>
            {
                new ATotalDetectorInput() { TallySecondMoment = true },
                new ROfRhoDetectorInput() { Rho = new DoubleRange(0, 10, 11) }
            };
            var mc =  new MonteCarloSimulation(si);
            var output = mc.Run();
            Assert.IsTrue(Math.Abs(output.Atot - 0.431413) < 0.000001);
            Assert.IsTrue(Math.Abs(output.Atot2 - 0.000548) < 0.000001);
            Assert.AreEqual(output.Atot_TallyCount, 275320);
            Assert.IsTrue(Math.Abs(output.R_r[0]- 0.012369) < 0.000001);
            Assert.AreEqual(output.R_r_TallyCount, 95);

            // then run same simulation with 2 CPUs
            // these will never be equal unless second sequence starts right after first!!!
            si.Options.SimulationIndex = 1;
            var parallelMC = new ParallelMonteCarloSimulation(si);
            var outputMultiCPUs = parallelMC.RunSingleInParallel();
            Assert.IsTrue(Math.Abs(output.Atot - outputMultiCPUs.Atot) < 0.1);
            Assert.IsTrue(Math.Abs(output.Atot2 - outputMultiCPUs.Atot2) < 0.001);
            Assert.AreEqual(outputMultiCPUs.Atot_TallyCount, 231243);
            Assert.IsTrue(Math.Abs(output.R_r[0] - outputMultiCPUs.R_r[0]) < 0.1);
            Assert.AreEqual(outputMultiCPUs.R_r_TallyCount, 94);
        }
    }
}
