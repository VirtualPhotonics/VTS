using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Factories;

namespace Vts.Test.Modeling
{
    [TestFixture]
    public class InverseSolutionTests
    {
        IEnumerable<double> independentValues;
        IEnumerable<double> standardDeviation;
        double[] constantValues;
        OpticalProperties actualProperties = new OpticalProperties();
        OpticalProperties initialGuess = new OpticalProperties();
        OpticalProperties convergedValue = new OpticalProperties();
        IEnumerable<double> simulatedMeasured;

        /// <summary>
        /// Tests R(rho) inverse solution using SDA simulated measured data (0% noise)
        /// using the SDA as the model predictor
        /// </summary>
        [Test]
        public void VerifyRofRhoSDAMeasuredNoNoiseSDAModel()
        {
            independentValues = new double[] { 10, 11, 12, 13, 14, 15 }; // rho [mm]
            standardDeviation = new List<double>();
            actualProperties.Mua = 0.01;
            actualProperties.Musp = 1.0;
            actualProperties.G = 0.8;
            actualProperties.N = 1.4;
            initialGuess.Mua = 0.02;
            initialGuess.Musp = 1.2;
            initialGuess.G = 0.8;
            initialGuess.N = 1.4;
            constantValues = new double[1] { 0 };
            simulatedMeasured = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(ForwardSolverType.DistributedPointSourceSDA),
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                actualProperties,
                constantValues);
            standardDeviation = simulatedMeasured;
            double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                SolverFactory.GetForwardSolver(ForwardSolverType.DistributedPointSourceSDA),
                SolverFactory.GetOptimizer(OptimizerType.MPFitLevenbergMarquardt),
                SolutionDomainType.RofRho,
                IndependentVariableAxis.Rho,
                independentValues,
                simulatedMeasured,
                standardDeviation,
                initialGuess,
                InverseFitType.MuaMusp,
                constantValues);
            convergedValue.Mua = fit[0];
            convergedValue.Musp = fit[1];
            Assert.Less(Math.Abs(convergedValue.Mua - 0.01), 1e-6);
            Assert.Less(Math.Abs(convergedValue.Musp - 1.0), 1e-6);
        }

        /// <summary>
        /// Tests R(rho) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the SDA as the model predictor
        /// </summary>
        [Test]
        public void VerifyRofRhoMonteCarloMeasuredNoNoiseSDAModel()
        {
            independentValues = new double[6] { 10, 11, 12, 13, 14, 15 }; // rho [mm]
            standardDeviation = new List<double>();
            actualProperties.Mua = 0.01;
            actualProperties.Musp = 1.0;
            actualProperties.G = 0.8;
            actualProperties.N = 1.4;
            initialGuess.Mua = 0.02;
            initialGuess.Musp = 1.2;
            initialGuess.G = 0.8;
            initialGuess.N = 1.4;
            constantValues = new double[1] { 0 };
            simulatedMeasured = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(ForwardSolverType.MonteCarlo),
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                actualProperties,
                constantValues);
            standardDeviation = simulatedMeasured;
            double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                SolverFactory.GetForwardSolver(ForwardSolverType.DistributedPointSourceSDA),
                SolverFactory.GetOptimizer(OptimizerType.MPFitLevenbergMarquardt),
                SolutionDomainType.RofRho,
                IndependentVariableAxis.Rho,
                independentValues,
                simulatedMeasured,
                standardDeviation,
                initialGuess,
                InverseFitType.MuaMusp,
                constantValues);
            convergedValue.Mua = fit[0];
            convergedValue.Musp = fit[1];
            Assert.Less(Math.Abs(convergedValue.Mua - 0.01), 0.002);
            Assert.Less(Math.Abs(convergedValue.Musp - 1.0), 0.11);
        }

        /// <summary>
        /// Tests R(rho) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the Monte Carlo as the model predictor
        /// </summary>
        [Test]
        public void VerifyRofRhoMonteCarloMeasuredNoNoiseMonteCarloModel()
        {
            independentValues = new double[6] { 1, 2, 3, 4, 5, 6 }; // rho [mm]
            standardDeviation = new List<double>();
            actualProperties.Mua = 0.01;
            actualProperties.Musp = 1.0;
            actualProperties.G = 0.8;
            actualProperties.N = 1.4;
            initialGuess.Mua = 0.02;
            initialGuess.Musp = 1.2;
            initialGuess.G = 0.8;
            initialGuess.N = 1.4;
            constantValues = new double[1] { 0 };
            simulatedMeasured = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(ForwardSolverType.MonteCarlo),
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                actualProperties,
                constantValues);
            standardDeviation = simulatedMeasured;
            double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                SolverFactory.GetForwardSolver(ForwardSolverType.MonteCarlo),
                SolverFactory.GetOptimizer(OptimizerType.MPFitLevenbergMarquardt),
                SolutionDomainType.RofRho,
                IndependentVariableAxis.Rho,
                independentValues,
                simulatedMeasured,
                standardDeviation,
                initialGuess,
                InverseFitType.MuaMusp,
                constantValues);
            convergedValue.Mua = fit[0];
            convergedValue.Musp = fit[1];
            Assert.Less(Math.Abs(convergedValue.Mua - 0.01), 1e-6);
            Assert.Less(Math.Abs(convergedValue.Musp - 1.0), 1e-6);
        }
    }
}
