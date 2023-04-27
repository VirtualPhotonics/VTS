using NUnit.Framework;
using System;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.Analyzers
{
    [TestFixture]
    internal class NumericalDerivativeExtensionsTests
    {
        [Test]
        public void Test_NumericalDerivativeExtensions_dRdMua()
        {
            var independentValues = new object[]
            {
                new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };

            var fs = new NurbsForwardSolver();
            Func<object[], double[]> func = forwardData => fs.ROfRho(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1]);

            var result = func.GetDerivativeFunc(ForwardAnalysisType.dRdMua)(independentValues);
            Assert.IsNotNull(result);
            Assert.AreEqual(-0.1281, result[0], 0.0001);
        }

        [Test]
        public void Test_NumericalDerivativeExtensions_dRdMusp()
        {
            var independentValues = new object[]
            {
                new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };

            var fs = new NurbsForwardSolver();
            Func<object[], double[]> func = forwardData => fs.ROfRho(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1]);

            var result = func.GetDerivativeFunc(ForwardAnalysisType.dRdMusp)(independentValues);
            Assert.IsNotNull(result);
            Assert.AreEqual(0.02113, result[0], 0.00001);
        }

        [Test]
        public void Test_NumericalDerivativeExtensions_dRdG()
        {
            var independentValues = new object[]
            {
                new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };

            var fs = new NurbsForwardSolver();
            Func<object[], double[]> func = forwardData => fs.ROfRho(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1]);

            var result = func.GetDerivativeFunc(ForwardAnalysisType.dRdG)(independentValues);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result[0], 0.1);
        }

        [Test]
        public void Test_NumericalDerivativeExtensions_dRdN()
        {
            var independentValues = new object[]
            {
                new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };

            var fs = new NurbsForwardSolver();
            Func<object[], double[]> func = forwardData => fs.ROfRho(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1]);

            var result = func.GetDerivativeFunc(ForwardAnalysisType.dRdN)(independentValues);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result[0], 0.1);
        }
    }
}
