using NUnit.Framework;
using System;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.Analyzers
{
    [TestFixture]
    internal class NumericalDerivativeExtensionsTests
    {
        [SetUp]
        public void Test_Setup()
        {
            // Make sure Delta is set to the default before each test
            NumericalDerivativeExtensions.SetDelta(0.01);
        }

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
            Assert.That(result[0], Is.EqualTo(-0.1281).Within(0.0001));
        }

        [Test]
        public void Test_NumericalDerivativeExtensions_dRdMua_change_delta()
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
            Assert.That(result[0], Is.EqualTo(-0.1281).Within(0.0001));
            
            NumericalDerivativeExtensions.SetDelta(1);

            result = func.GetDerivativeFunc(ForwardAnalysisType.dRdMua)(independentValues);
            Assert.IsNotNull(result);
            Assert.That(result[0], Is.EqualTo(-0.1337).Within(0.0001));
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
            Assert.That(result[0], Is.EqualTo(0.02113).Within(0.00001));
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
            Assert.That(result[0], Is.EqualTo(0).Within(0.1));
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
            Assert.That(result[0], Is.EqualTo(0).Within(0.1));
        }

        [Test]
        public void Test_NumericalDerivativeExtensions_throws_ArgumentOutOfRangeException()
        {
            var independentValues = new object[]
            {
                new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };

            var fs = new NurbsForwardSolver();
            Func<object[], double[]> func = forwardData => fs.ROfRho(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1]);

            Assert.Throws<ArgumentOutOfRangeException>(() => func.GetDerivativeFunc((ForwardAnalysisType)Enum.GetValues(typeof(ForwardAnalysisType)).Length + 1)(independentValues));
        }
    }
}
