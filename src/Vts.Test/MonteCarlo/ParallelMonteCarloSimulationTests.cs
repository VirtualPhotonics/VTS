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
        private SimulationOutput _outputSingleCPU;
        private SimulationOutput _outputMultiCPU;
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
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);   
            }
        }
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

            // first run with single processor
            var si = new SimulationInput { N = 100 };
            si.Options.SimulationIndex = 0;  // 0 -> 1 CPUS
            si.Options.Seed = 0;
            si.DetectorInputs = new List<IDetectorInput>
            {
                new ATotalDetectorInput() { TallySecondMoment = true },
                new ROfRhoDetectorInput() { Rho = new DoubleRange(0, 10, 11), TallySecondMoment = true },
                new ROfRhoAndTimeDetectorInput() { Rho = new DoubleRange(0, 10, 11),
                                            Time = new DoubleRange(0,1,5) }
            };
            var mc = new MonteCarloSimulation(si);
            _outputSingleCPU = mc.Run();

            // then run same simulation with 2 CPUs
            var parallelMC = new ParallelMonteCarloSimulation(si, 2);
            _outputMultiCPU = parallelMC.RunSingleInParallel();
        }
        /// <summary>
        /// Validate method that validates that parallel processing of single value detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_single_value_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot - 0.431413) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot2 - 0.000548) < 0.000001);
            Assert.AreEqual(_outputSingleCPU.Atot_TallyCount, 275320);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot - _outputMultiCPU.Atot) < 0.1);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot2 - _outputMultiCPU.Atot2) < 0.001);
            Assert.AreEqual(_outputMultiCPU.Atot_TallyCount, 231243);;
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 1D detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_1D_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_r[0] - 0.012369) < 0.000001);
            Assert.AreEqual(_outputSingleCPU.R_r_TallyCount, 95);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_r[0] - _outputMultiCPU.R_r[0]) < 0.1);
            Assert.AreEqual(_outputMultiCPU.R_r_TallyCount, 94);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 2D detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_2D_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_rt[0, 0] - 0.049477) < 0.000001);
            Assert.AreEqual(_outputSingleCPU.R_rt_TallyCount, 95);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_rt[0, 0] - _outputMultiCPU.R_rt[0, 0]) < 0.11);
            Assert.AreEqual(_outputMultiCPU.R_rt_TallyCount, 94);
        }
    }
}
