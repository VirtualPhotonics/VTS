using System;
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
        public void VerifyROfRhoSDAMeasuredNoNoiseSDAModel()
        {
            var independentValues = new double[] { 10, 11, 12, 13, 14, 15 }; // rho [mm] 
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess =     new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);
            var lowerBounds = new double[] { 0, 0, 0, 0 };
            var upperBounds = new double[] {double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity};

            var simulatedMeasured = ComputationFactory.ComputeReflectance(
                ForwardSolverType.DistributedPointSourceSDA,
                SolutionDomainType.ROfRho,
                ForwardAnalysisType.R,
                new object[] {new[] {actualProperties}, independentValues});

            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.SolveInverse(
                ForwardSolverType.DistributedPointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                simulatedMeasured,
                standardDeviation,
                InverseFitType.MuaMusp,
                new object[] { new[]{ initialGuess}, independentValues },
                lowerBounds,
                upperBounds);

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.That(Math.Abs(convergedMua - 0.01), Is.LessThan(1e-6));
            Assert.That(Math.Abs(convergedMusp - 1.0), Is.LessThan(1e-6));
        }

        /// <summary>
        /// Tests R(rho) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the SDA as the model predictor
        /// </summary>
        [Test]
        public void VerifyROfRhoMonteCarloMeasuredNoNoiseSDAModel()
        {
            var independentValues = new double[] { 10, 11, 12, 13, 14, 15 }; // rho [mm] 
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess = new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);
            var lowerBounds = new double[] { 0, 0, 0, 0 };
            var upperBounds = new double[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };

            var simulatedMeasured = ComputationFactory.ComputeReflectance(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.ROfRho,
                ForwardAnalysisType.R,
                new object[] { new[] { actualProperties }, independentValues });

            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.SolveInverse(
                ForwardSolverType.DistributedPointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                simulatedMeasured,
                standardDeviation,
                InverseFitType.MuaMusp,
                new object[] { new[]{ initialGuess}, independentValues },
                lowerBounds,
                upperBounds);

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.That(Math.Abs(convergedMua - 0.01), Is.LessThan(0.002));
            Assert.That(Math.Abs(convergedMusp - 1.0), Is.LessThan(0.11));
        }

        /// <summary>
        /// Tests R(rho) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the Monte Carlo as the model predictor. 
        /// </summary>
        [Test]
        public void VerifyROfRhoMonteCarloMeasuredNoNoiseMonteCarloModel()
        {
            var independentValues = new double[] { 1, 2, 3, 4, 5, 6 }; // rho [mm]
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess = new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);

            var simulatedMeasured = ComputationFactory.ComputeReflectance(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.ROfRho,
                ForwardAnalysisType.R,
                new object[] { new[] { actualProperties }, independentValues });
            
            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.SolveInverse(
                ForwardSolverType.MonteCarlo,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                simulatedMeasured,
                standardDeviation,
                InverseFitType.MuaMusp,
                new object[] { new[]{ initialGuess}, independentValues });

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.That(Math.Abs(convergedMua - 0.01), Is.LessThan(1e-6));
            Assert.That(Math.Abs(convergedMusp - 1.0), Is.LessThan(1e-6));
        }

        /// <summary>  
        /// Tests R(rho,ft) inverse solution using Monte Carlo simulated measured data (0% noise)
        /// using the Monte Carlo as the model predictor. Test model with two independent variables.
        /// </summary>
        [Test]
        public void VerifyROfRhoAndFtMonteCarloMeasuredNoNoiseMonteCarloModel()
        {
            var actualProperties = new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4);
            var initialGuess = new OpticalProperties(mua: 0.02, musp: 1.2, g: 0.8, n: 1.4);
            var rhos = new double[]{1,2,3,4};
            var fts = new double[] {0.1, 0.2};

        var simulatedMeasured = ComputationFactory.ComputeReflectance(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.ROfRhoAndFt,
                ForwardAnalysisType.R,
                new object[] { new[] { actualProperties }, rhos , fts });
            
            var standardDeviation = simulatedMeasured;

            double[] fit = ComputationFactory.SolveInverse(
                ForwardSolverType.MonteCarlo,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRhoAndFt,
                simulatedMeasured,
                standardDeviation,
                InverseFitType.MuaMusp,
                new object[] { new[]{ initialGuess}, rhos, fts });

            var convergedMua = fit[0];
            var convergedMusp = fit[1];

            Assert.That(Math.Abs(convergedMua - 0.01), Is.LessThan(1e-6));
            Assert.That(Math.Abs(convergedMusp - 1.0), Is.LessThan(1e-6));
        }

    } 
}
