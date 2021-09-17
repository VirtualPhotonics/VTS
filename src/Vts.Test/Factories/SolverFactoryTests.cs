using NUnit.Framework;
using Vts.Factories;

namespace Vts.Test.Factories
{
    [TestFixture]
    public class SolverFactoryTests
    {
        /// <summary>
        /// Test against the SolverFactory class GetForwardSolver routine
        /// </summary>
        [Test]
        public void GetForwardSolver_ReturnsNonNull()
        {
            foreach (var fsType in EnumHelper.GetValues<ForwardSolverType>())
            {
                var fs = SolverFactory.GetForwardSolver(fsType);
                Assert.IsNotNull(fs, "The requested instance matching " + fsType + " returned null from the call to GetForwardSolver().");
            }
        }

        /// <summary>
        /// Test against the SolverFactory class GetForwardSolver
        /// </summary>
        [Test]
        public void GetForwardSolver_returns_null()
        {
            var fs = SolverFactory.GetForwardSolver("NotAForwardSolver");
            Assert.IsNull(fs);
        }

        /// <summary>
        /// Test against the SolverFactory class GetOptimizer
        /// </summary>
        [Test]
        public void GetOptimizer_ReturnsNonNull()
        {
            foreach (var oType in EnumHelper.GetValues<OptimizerType>())
            {
                var o = SolverFactory.GetOptimizer(oType);
                Assert.IsNotNull(o, "The requested instance matching " + oType + " returned null from the call to GetOptimizer().");
            }
        }

        /// <summary>
        /// Test against the SolverFactory class GetOptimizer
        /// </summary>
        [Test]
        public void GetOptimizer_returns_null()
        {
            var o = SolverFactory.GetOptimizer("NotAnOptimizer");
            Assert.IsNull(o);
        }

        /// <summary>
        /// Test against the SolverFactory class GetScattererType
        /// </summary>
        [Test]
        public void GetScattererType_ReturnsNonNull()
        {
            foreach (var sType in EnumHelper.GetValues<ScatteringType>())
            {
                var s = SolverFactory.GetScattererType(sType);
                Assert.IsNotNull(s, "The requested instance matching " + sType + " returned null from the call to GetScattererType().");
            }
        }

        /// <summary>
        /// Test against the SolverFactory class GetScattererType
        /// </summary>
        [Test]
        public void GetScattererType_returns_null()
        {
            var s = SolverFactory.GetScattererType("NotAScatterer");
            Assert.IsNull(s);
        }
    }
}
