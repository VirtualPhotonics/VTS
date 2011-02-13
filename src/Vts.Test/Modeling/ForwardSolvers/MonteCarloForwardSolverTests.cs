using NUnit.Framework;
using System;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class MonteCarloForwardSolverTests
    {
        /// <summary>
        /// Setup for the SolverFactory tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test solver construction 
        /// </summary>
        [Test]
        public void constructor_returns_valid_solver()
        {
            var fs = new MonteCarloForwardSolver();

            Assert.IsNotNull(fs);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void simple_forward_calls_return_valid_solver()
        {
            var fs = new MonteCarloForwardSolver();
            var op = new OpticalProperties();

            var value1 = fs.RofRho(op, 10);
            Assert.IsTrue(value1>0);

            var value2 = fs.RofFx(op, 0.1);
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Tear down for the NurbsGenerator tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
        }
    }
}
