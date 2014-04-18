using NUnit.Framework;
using Vts.Factories;

namespace Vts.Test.Factories
{
    [TestFixture]
    public class SolverFactoryTests
    {
        /// <summary>
        /// Setup for the SolverFactory tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test against the SolverFactory class GetForwardSolver routine
        /// </summary>
        [Test]
        public void GetForwardSolver_ReturnsNonNull()
        {
            foreach (var fsType in EnumHelper.GetValues<ForwardSolverType>())
            {
                var fs = SolverFactory.GetForwardSolver(fsType);
                Assert.IsNotNull(fs, "The requested instance matching " + fsType + "returned null from the call to GetForwardSolver().");
            }
        }

        /// <summary>
        /// Test against the SolverFactory class GetForwardSolver routine
        /// </summary>
        [Test]
        public void GetOptimizer_ReturnsNonNull()
        {
            foreach (var oType in EnumHelper.GetValues<OptimizerType>())
            {
                var o = SolverFactory.GetOptimizer(oType);
                Assert.IsNotNull(o, "The requested instance matching " + oType + "returned null from the call to GetOptimizer().");
            }
        }

        /// <summary>
        /// Test against the SolverFactory class GetForwardSolver routine
        /// </summary>
        [Test]
        public void GetScattererType_ReturnsNonNull()
        {
            foreach (var sType in EnumHelper.GetValues<ScatteringType>())
            {
                var s = SolverFactory.GetScattererType(sType);
                Assert.IsNotNull(s, "The requested instance matching " + sType + "returned null from the call to GetScattererType().");
            }
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
