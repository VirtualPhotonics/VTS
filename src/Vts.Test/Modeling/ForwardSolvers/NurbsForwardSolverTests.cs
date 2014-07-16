using System.Linq;
using NUnit.Framework;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    /// <summary>
    /// Class used to test NurbsForwardSolver methods
    /// </summary>
    [TestFixture]
    public class NurbsForwardSolverTests
    {
        private NurbsForwardSolver nurbsForwardSolver = null;

        /// <summary>
        /// Setup for the NurbsForwardSolver tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            nurbsForwardSolver = new NurbsForwardSolver();
        }
        
        /// <summary>
        /// Test against NurbsForwardSolver.
        /// </summary>
        [Test]
        public void ROfRhoAndT_TimeValueSmallerThenMinimalTimeOfFlight_ReturnsZero()
        {
            INurbs fakeNurbsGenerator = new StubNurbsGenerator();
            nurbsForwardSolver = new NurbsForwardSolver(fakeNurbsGenerator);
            OpticalProperties op = new OpticalProperties(0.0, 1.0, 0.8, 1.4);
            Assert.AreEqual(0.0, nurbsForwardSolver.ROfRhoAndTime(op, 10.0,0.01),
                  "The returned value should be 0.0");
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void simple_forward_calls_return_nonzero_result()
        {
            var fs = new NurbsForwardSolver();
            var op = new OpticalProperties();

            var value1 = fs.ROfRho(op, 10);
            Assert.IsTrue(value1 > 0);

            var value2 = fs.ROfFx(op, 0.1);
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void enumerable_forward_calls_return_nonzero_result()
        {
            var fs = new NurbsForwardSolver();
            var ops = new OpticalProperties().AsEnumerable();
            var rhos = 10D.AsEnumerable();
            var fxs = 0.1.AsEnumerable();

            var value1 = fs.ROfRho(ops, rhos).First();
            Assert.IsTrue(value1 > 0);

            var value2 = fs.ROfFx(ops, fxs).First();
            Assert.IsTrue(value2 > 0);
        }

        /// <summary>
        /// Test simple solver calls 
        /// </summary>
        [Test]
        public void array_forward_calls_return_nonzero_result()
        {
            var fs = new NurbsForwardSolver();
            var ops = new[] { new OpticalProperties() };
            var rhos = new[] { 10.0 };
            var fxs = new[] { 0.1 };

            var value1 = fs.ROfRho(ops, rhos);
            Assert.IsTrue(value1[0] > 0);

            var value2 = fs.ROfFx(ops, fxs);
            Assert.IsTrue(value2[0] > 0);
        }
        
        /// <summary>
        /// Tear Down for the NurbsForwardSolver tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            nurbsForwardSolver = null;
        }

    }
}
