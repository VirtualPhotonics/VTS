using System;
using System.Collections.Generic;
using BenchmarkDotNet.Running;
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
        private SimulationInput _simulationInput;
        private SimulationOutput _outputSingleCPU, _outputMultiCPU;
        private SimulationStatistics _statisticsSingleCPU, _statisticsMultiCPU;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt", // file that capture screen output of MC simulation
            "statistics.txt"
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
            _simulationInput = new SimulationInput { N = 100 };
            _simulationInput.Options.SimulationIndex = 0;  // 0 -> 1 CPUS
            _simulationInput.Options.Seed = 0;
            _simulationInput.Options.TrackStatistics = true;
            _simulationInput.DetectorInputs = new List<IDetectorInput> // choose one of each type of dimension
            {
                // double, double[], double[,], double[,,], double[,,,], double[,,,,]
                new ATotalDetectorInput() { TallySecondMoment = true },
                new ROfRhoDetectorInput() {
                    Rho = new DoubleRange(0, 10, 11), TallySecondMoment = true },
                new ROfRhoAndTimeDetectorInput() {
                    Rho = new DoubleRange(0, 10, 11),
                    Time = new DoubleRange(0,1,5) },
                new FluenceOfXAndYAndZDetectorInput() {
                    X = new DoubleRange(-10, 10, 21),
                    Y = new DoubleRange(-10, 10, 21),
                    Z = new DoubleRange(0, 10, 11)},
                new FluenceOfXAndYAndZAndTimeDetectorInput(){
                    X = new DoubleRange(-10, 10, 21),
                    Y = new DoubleRange(-10, 10, 21),
                    Z = new DoubleRange(0, 10, 11),
                    Time = new DoubleRange(0, 1, 11)},
                new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput() {
                    X = new DoubleRange(-10, 10, 21),
                    Y = new DoubleRange(-10, 10, 21),
                    Z = new DoubleRange(0, 10, 11),
                    Theta = new DoubleRange(0, Math.PI, 3),
                    Phi = new DoubleRange(-Math.PI, Math.PI, 5)},
                // Complex[], Complex[,], Complex[,,], Complex[,,,]
                new ROfFxDetectorInput() {
                    Fx = new DoubleRange(0, 1, 5)},
                new ROfFxAndTimeDetectorInput()
                {
                    Fx = new DoubleRange(0, 1, 5),
                    Time = new DoubleRange(0, 1, 11)
                },
                new FluenceOfRhoAndZAndOmegaDetectorInput()
                {
                    Rho = new DoubleRange(0, 10, 11),
                    Z = new DoubleRange(0, 10, 11),
                    Omega = new DoubleRange(0, 1, 7),
                },
                new FluenceOfXAndYAndZAndOmegaDetectorInput()
                {
                    X = new DoubleRange(-10, 10, 21),
                    Y = new DoubleRange(-10, 10, 21),
                    Z = new DoubleRange(0, 10, 11),
                    Omega = new DoubleRange(0, 1, 7),
                }
            };
            var mc = new MonteCarloSimulation(_simulationInput);
            _outputSingleCPU = mc.Run();
            // read statistics.txt from file
            _statisticsSingleCPU = mc.Statistics;

            //var testCode = new ParallelMonteCarloSimulation(2);
            //testCode.RunSingleInParallel();

            // then run same simulation with 2 CPUs
            var parallelMC = new ParallelMonteCarloSimulation(_simulationInput, 2);
            _outputMultiCPU = parallelMC.RunSingleInParallel();
            // read statistics.txt from file
            _statisticsMultiCPU = parallelMC.SummedStatistics;
        }
        // The tests are designed to a) verify the 2 CPU results and b) verify the 2 CPU
        // results are within certain limits of the single CPU results

        /// <summary>
        /// Validate method that validates that parallel processing of single value detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_single_value_double_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Atot - 0.341646) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Atot2 - 0.253490) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Atot_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot - _outputMultiCPU.Atot) < 0.1);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot2 - _outputMultiCPU.Atot2) < 0.1);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot_TallyCount - _outputMultiCPU.Atot_TallyCount) < 45000);

        }
        /// <summary>
        /// Validate method that validates that parallel processing of 1D double detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_1D_double_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_r[0] - 0.039375) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_r2[0] - 0.011950) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.R_r_TallyCount, 94);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_r[0] - _outputMultiCPU.R_r[0]) < 0.05);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_r2[0] - _outputMultiCPU.R_r2[0]) < 0.01);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_r_TallyCount - _outputMultiCPU.R_r_TallyCount) < 2);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 2D double detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_2D_double_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_rt[0, 0] - 0.157502) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.R_r_TallyCount, 94);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_rt[0, 0] - _outputMultiCPU.R_rt[0, 0]) < 0.11);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_rt_TallyCount - _outputMultiCPU.R_rt_TallyCount) < 2);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 3D double detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_3D_double_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Flu_xyz[0, 0, 0] - 0.003935) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Flu_xyz_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyz[0, 0, 0] - _outputMultiCPU.Flu_xyz[0, 0, 0]) < 0.001);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyz_TallyCount - _outputMultiCPU.Flu_xyz_TallyCount) < 45000);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 4D double detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_4D_double_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Flu_xyzt[0, 0, 0, 9] - 0.039355) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Flu_xyzt_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyzt[0, 0, 0, 9] - _outputMultiCPU.Flu_xyzt[0, 0, 0, 9]) < 0.05);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyzt_TallyCount - _outputMultiCPU.Flu_xyzt_TallyCount) < 45000);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 5D double detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_5D_double_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Rad_xyztp[0, 0, 0, 0, 0] - 0.000415) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Rad_xyztp_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Rad_xyztp[0, 0, 0, 0, 0] - _outputMultiCPU.Rad_xyztp[0, 0, 0, 0, 0]) < 0.001);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Rad_xyztp_TallyCount - _outputMultiCPU.Rad_xyztp_TallyCount) < 45000);

        }
        // Complex detectors
        /// <summary>
        /// Validate method that validates that parallel processing of 1D Complex detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_1D_Complex_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_fx[1].Real - 0.085204) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_fx[1].Imaginary + 0.059597) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.R_fx_TallyCount, 94);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_fx[1].Real - _outputMultiCPU.R_fx[1].Real) < 0.2);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_fx[1].Imaginary - _outputMultiCPU.R_fx[1].Imaginary) < 0.2);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_fx_TallyCount - _outputMultiCPU.R_fx_TallyCount) < 2);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 2D Complex detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_2D_Complex_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_fxt[1, 0].Real - 0.788927) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_fxt[1, 0].Imaginary + 0.321802) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.R_fxt_TallyCount, 94);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_fxt[1, 0].Real - _outputMultiCPU.R_fxt[1, 0].Real) < 0.2);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_fxt[1, 0].Imaginary - _outputMultiCPU.R_fxt[1, 0].Imaginary) < 1.0);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.R_fx_TallyCount - _outputMultiCPU.R_fx_TallyCount) < 2);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 3D Complex detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_3D_Complex_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Flu_rzw[0, 0, 1].Real - 0.569267) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Flu_rzw[0, 0, 1].Imaginary + 0.004910) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Flu_rzw_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_rzw[0, 0, 1].Real - _outputMultiCPU.Flu_rzw[0, 0, 1].Real) < 0.02);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_rzw[0, 0, 1].Imaginary - _outputMultiCPU.Flu_rzw[0, 0, 1].Imaginary) < 0.001);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_rzw_TallyCount - _outputMultiCPU.Flu_rzw_TallyCount) < 45000);
        }
        /// <summary>
        /// Validate method that validates that parallel processing of 4D Complex detectors
        /// return correct results
        /// </summary>
        [Test]
        public void validate_4D_Complex_detectors_are_processed_correctly()
        {
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Flu_xyzw[0, 0, 0, 1].Real + 0.001569) < 0.000001);
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Flu_xyzw[0, 0, 0, 1].Imaginary + 0.003586) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Flu_xyzw_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyzw[0, 0, 0, 1].Real - _outputMultiCPU.Flu_xyzw[0, 0, 0, 1].Real) < 0.05);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyzw[0, 0, 0, 1].Imaginary - _outputMultiCPU.Flu_xyzw[0, 0, 0, 1].Imaginary) < 0.05);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Flu_xyzw_TallyCount - _outputMultiCPU.Flu_xyzw_TallyCount) < 45000);
        }
        /// <summary>
        /// test to verify statistics are averaged correctly
        /// </summary>
        [Test]
        public void validate_statistics_are_processed_correctly()
        {
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsOutTopOfTissue, 95);
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsOutBottomOfTissue, 4);
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsAbsorbed, 0);
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsSpecularReflected, 1);
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsKilledOverMaximumPathLength, 0);
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsKilledOverMaximumCollisions, 0);
            Assert.AreEqual(_statisticsSingleCPU.NumberOfPhotonsKilledOverMaximumCollisions, 0);
            // test multi-CPU stats
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsOutTopOfTissue, 94);
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsOutBottomOfTissue, 4);
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsAbsorbed, 0);
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsSpecularReflected, 2);
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsKilledOverMaximumPathLength, 0);
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsKilledOverMaximumCollisions, 0);
            Assert.AreEqual(_statisticsMultiCPU.NumberOfPhotonsKilledOverMaximumCollisions, 0);
        }
        /// <summary>
        /// test to check that if number of CPUs specified in cpucount does not divide into N
        /// evenly, that resulting number of photons launched is correct
        /// </summary>
        [Test]
        public void check_for_N_not_divisible_by_cpucount()
        {
            var simulationInput = new SimulationInput { N = 100 };
            var parallelMC = new ParallelMonteCarloSimulation(simulationInput, 3);
            var output3CPU = parallelMC.RunSingleInParallel();
            Assert.AreEqual(output3CPU.Input.N, 99);
        }
        /// <summary>
        /// This test relies on the attribute [Benchmark] applied to 
        /// ParallelMonteCarloSimulation.RunSingleInParallel()
        /// Benchmark does not work with unit tests yet
        /// </summary>
        [Test]
        public void run_Benchmark_for_timing()
        {
        //    var parallelMonteCarloSimulation = new ParallelMonteCarloSimulation();
        //    var simulationOutput = parallelMonteCarloSimulation.RunSingleInParallel();
        //    Assert.AreEqual(96, simulationOutput.Input.N);

            var summary = BenchmarkRunner.Run<ParallelMonteCarloSimulation>();
            Console.WriteLine(summary);
        }
    }
}
