using NUnit.Framework;
using Vts.Factories;

namespace Vts.Test.Factories
{
    [TestFixture]
    public class ComputationFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeReflectance routine
        /// </summary>
        [Test]
        public void GetVectorizedIndependentVariableQueryNew_can_be_called_using_string_inputs()
        {
            var test = ComputationFactory.ComputeReflectance(
                "MonteCarlo",
                "ROfRho",
                "R",
                "Rho",
                new double[] {1, 2, 3},
                new double[] {0.01, 1, 0.8, 1.4},
                new double[0]);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeReflectance routine
        /// </summary>
        [Test]
        public void GetVectorizedIndependentVariableQueryNew_can_be_called_using_enum_inputs()
        {
            var test = ComputationFactory.ComputeReflectance(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.ROfRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                new double[] { 1, 2, 3 },
                new OpticalProperties(0.01, 1, 0.8, 1.4), // todo: incongruent with string version
                new double[0]);
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
