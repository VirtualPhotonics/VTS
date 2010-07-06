using System;
using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    /// <summary>
    /// Class used to test NurbsForwardSolver methods
    /// </summary>
    [TestFixture]
    public class NurbsForwardSolverTest
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
        public void RofRhoAndT_TimeValueSmallerThenMinimalTimeOfFlight_ReturnsZero()
        {
            INurbs fakeNurbsGenerator = new StubNurbsGenerator();
            nurbsForwardSolver = new NurbsForwardSolver(fakeNurbsGenerator);
            OpticalProperties op = new OpticalProperties(0.0, 1.0, 0.8, 1.4);
            Assert.AreEqual(0.0, nurbsForwardSolver.RofRhoAndT(op, 10.0,0.01),
                  "The returned value should be 0.0");
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
