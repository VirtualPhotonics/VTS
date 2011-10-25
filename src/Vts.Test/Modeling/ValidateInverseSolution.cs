using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Factories;

namespace Vts.Test.Modeling
{
    [TestFixture]
    public class InverseSolutionTests
    {
        /// <summary>
        /// Tests R(rho) inverse solution using SDA simulated measured data (0% noise)
        /// using the SDA as the model predictor
        /// </summary>
        [Test]
        public void VerifyRofRhoSDAMeasuredNoNoiseSDAModel()
        {
            var independentValues = new double[] { 10, 11, 12, 13, 14, 15 }; // rho [mm]
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess =     new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);

            var simulatedMeasured = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                ForwardSolverType.DistributedPointSourceSDA,
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                actualProperties).ToArray();

            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                ForwardSolverType.DistributedPointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.RofRho,
                IndependentVariableAxis.Rho,
                independentValues,
                simulatedMeasured,
                standardDeviation,
                initialGuess,
                InverseFitType.MuaMusp);

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.Less(Math.Abs(convergedMua - 0.01), 1e-6);
            Assert.Less(Math.Abs(convergedMusp - 1.0), 1e-6);
        }

        /// <summary>
        /// Tests R(rho) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the SDA as the model predictor
        /// </summary>
        [Test]
        public void VerifyRofRhoMonteCarloMeasuredNoNoiseSDAModel()
        {
            var independentValues = new double[] { 10, 11, 12, 13, 14, 15 }; // rho [mm]
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess = new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);

            var simulatedMeasured = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                actualProperties).ToArray();

            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                ForwardSolverType.DistributedPointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.RofRho,
                IndependentVariableAxis.Rho,
                independentValues,
                simulatedMeasured,
                standardDeviation,
                initialGuess,
                InverseFitType.MuaMusp);

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.Less(Math.Abs(convergedMua - 0.01), 0.002);
            Assert.Less(Math.Abs(convergedMusp - 1.0), 0.11);
        }

        /// <summary>
        /// Tests R(rho) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the Monte Carlo as the model predictor
        /// </summary>
        [Test]
        public void VerifyRofRhoMonteCarloMeasuredNoNoiseMonteCarloModel()
        {
            var independentValues = new double[] { 1, 2, 3, 4, 5, 6 }; // rho [mm]
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess = new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);

            var simulatedMeasured = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                actualProperties).ToArray();
            
            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                ForwardSolverType.MonteCarlo,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.RofRho,
                IndependentVariableAxis.Rho,
                independentValues,
                simulatedMeasured,
                standardDeviation,
                initialGuess,
                InverseFitType.MuaMusp);

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.Less(Math.Abs(convergedMua - 0.01), 1e-6);
            Assert.Less(Math.Abs(convergedMusp - 1.0), 1e-6);
        }
    }
}
