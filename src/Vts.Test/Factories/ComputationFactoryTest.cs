using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;
using System;
using Vts.Factories;

namespace Vts.Test.Factories
{
    [TestFixture]
    public class ComputationFactoryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test against the ComputationFactory class GetVectorizedIndependentVariableQueryNew routine
        /// </summary>
        [Test]
        public void GetVectorizedIndependentVariableQueryNew_can_be_called_using_string_inputs()
        {
            var test = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                "MonteCarlo",
                "RofRho",
                "R",
                "Rho",
                new double[] {1, 2, 3},
                new double[] {0.01, 1, 0.8, 1.4},
                new double[0]);
        }

        /// <summary>
        /// Test against the ComputationFactory class GetVectorizedIndependentVariableQueryNew routine
        /// </summary>
        [Test]
        public void GetVectorizedIndependentVariableQueryNew_can_be_called_using_enum_inputs()
        {
            var test = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.RofRho,
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
