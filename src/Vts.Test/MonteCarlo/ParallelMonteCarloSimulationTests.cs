﻿using System;
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
            si.DetectorInputs = new List<IDetectorInput> // choose one of each type of dimension
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
            var mc = new MonteCarloSimulation(si);
            _outputSingleCPU = mc.Run();

            // then run same simulation with 2 CPUs
            var parallelMC = new ParallelMonteCarloSimulation(si, 2);
            _outputMultiCPU = parallelMC.RunSingleInParallel();
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
            Assert.IsTrue(Math.Abs(_outputMultiCPU.Atot2 - 0.000429) < 0.000001);
            Assert.AreEqual(_outputMultiCPU.Atot_TallyCount, 231243);

            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot - _outputMultiCPU.Atot) < 0.1);
            Assert.IsTrue(Math.Abs(_outputSingleCPU.Atot2 - _outputMultiCPU.Atot2) < 0.001);
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
            Assert.AreEqual(_outputMultiCPU.R_r_TallyCount, 94);

            Assert.IsTrue(Math.Abs(_outputMultiCPU.R_r[0] - _outputMultiCPU.R_r[0]) < 0.1);
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
    }
}
