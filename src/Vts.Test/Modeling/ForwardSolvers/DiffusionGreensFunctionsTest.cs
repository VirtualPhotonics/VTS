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
        private static DiffusionParameters dp = DiffusionParameters.Create(ops, ForwardModel.SDA);

        private double[] rTestValues = new double[] { 1, 3, 10 };
        private const double time = 0.05; //ns
        private const double ft = 0.5; //GHz
        private Complex k = ((mua * dp.cn + Complex.ImaginaryOne * ft * 2 * Math.PI) /
                            (dp.cn * dp.D)).SquareRoot();

        // Green's Functions Tests...
        [Test]
        public void StationaryPointSourceGreensFunction_Test()
        {
            double[] greensFunctionValues = new double[] { 0.202598, 0.0476782, 0.00422923 };

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
            double[] greensFunctionValues = new double[] { 0.605791, 0.343966, 0.000550124 };

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
            double[] realGreensFunctionValues = new double[] { 0.195264, 0.0411688, 0.00145147 };
            double[] imaginaryGreensFunctionValues = new double[] { -0.0212481, -0.0138797, -0.00274176 };


            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var tfpsGF = DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunction(dp, rTestValues[iR], k);

                var relDiffReal = Math.Abs(tfpsGF.Real - realGreensFunctionValues[iR]) / realGreensFunctionValues[iR];
                Assert.IsTrue(relDiffReal < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiffReal + " for the real compoment" + ". For tfpsGF.Real = " + tfpsGF.Real +
                    " with magnitude " + tfpsGF.Magnitude);
                var relDiffImag = Math.Abs(tfpsGF.Imaginary - imaginaryGreensFunctionValues[iR]) / imaginaryGreensFunctionValues[iR];
                Assert.IsTrue(relDiffImag < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiffReal + " for the imaginary compoment");
            }
        }
    }
}
