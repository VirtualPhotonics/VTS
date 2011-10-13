using System.Linq;
using NUnit.Framework;
using System;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class pMCForwardSolverTests
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
        [Ignore("this test fails because pMC database is not loaded, will be fixed once lazy-loading working")]
        public void constructor_returns_valid_solver()
        {
            var fs = new pMCForwardSolver();
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        [Ignore("this test fails because pMC database is not loaded, will be fixed once lazy-loading working")]
        public void simple_forward_calls_return_nonzero_result()
        {
            var fs = new pMCForwardSolver();
            var op = new OpticalProperties();

            var value1 = fs.RofRho(op, 10);
            Assert.IsTrue(value1 > 0);

            var value2 = fs.RofFx(op, 0.1);
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        [Ignore("this test fails because pMC database is not loaded, will be fixed once lazy-loading working")]
        public void enumerable_forward_calls_return_nonzero_result()
        {
            var fs = new pMCForwardSolver();
            var ops = new OpticalProperties().AsEnumerable();
            var rhos = 10D.AsEnumerable();
            var fxs = 0.1.AsEnumerable();

            var value1 = fs.RofRho(ops, rhos).First();
            Assert.IsTrue(value1 > 0);

            var value2 = fs.RofFx(ops, fxs).First();
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        [Ignore("this test fails because pMC database is not loaded, will be fixed once lazy-loading working")]
        public void array_forward_calls_return_nonzero_result()
        {
            var fs = new pMCForwardSolver();
            var ops = new[] { new OpticalProperties() };
            var rhos = new[] { 10.0 };
            var fxs = new[] { 0.1 };

            var value1 = fs.RofRho(ops, rhos);
            Assert.IsTrue(value1[0] > 0);

            var value2 = fs.RofFx(ops, fxs);
            Assert.IsTrue(value2[0] > 0);
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
