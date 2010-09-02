using System;
using NUnit.Framework;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using MathNet.Numerics;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class DiffusionGreensFunctionsTest
    {
        const double thresholdValue = 1e-5;
        const double n = 1.4;
        const double g = 0.8;
        const double mua = 0.01;
        const double musp = 1;

        private static OpticalProperties ops = new OpticalProperties(mua, musp, g, n);
        private DiffusionParameters dp = DiffusionParameters.Create(ops, ForwardModel.SDA);


        // Green's Functions Tests...
        [Test]
        public void StationaryPointSourceGreensFunction_Test()
        {
            double[] rTestValues = new double[] { 0, 1, 999999999999 };
            double[] greensFunctionValues = new double[] { 1, 5, 0 }; // need to choose and verify values...

            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var relDiff = Math.Abs(
                    DiffusionGreensFunctions.StationaryPointSourceGreensFunction(dp, rTestValues[iR]) -
                    greensFunctionValues[iR]) / greensFunctionValues[iR];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalPointSourceGreensFunction_Test()
        {
            double time = 1; //nss
            double[] rTestValues = new double[] { 0, 1, 999999999999 };
            double[] greensFunctionValues = new double[] { 1, 5, 0 }; // need to choose and verify values...

            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var relDiff = Math.Abs(
                    DiffusionGreensFunctions.TemporalPointSourceGreensFunction(dp, rTestValues[iR], time) -
                    greensFunctionValues[iR]) / greensFunctionValues[iR];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with time =" + time + "with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalFrequencyPointSourceGreensFunction_Test()
        {
            double ft = 1;
            Complex k = ((mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
                           (dp.cn * dp.D)).SquareRoot();
            double[] rTestValues = new double[] { 0, 1, 999999999999 };
            double[] greensFunctionValues = new double[] { 1, 5, 0 }; // need to choose and verify values...

            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var relDiff = Complex.Abs(
                    DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunction(dp, rTestValues[iR], k) -
                    greensFunctionValues[iR]) / greensFunctionValues[iR];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiff);
            }
        }






    }
}
