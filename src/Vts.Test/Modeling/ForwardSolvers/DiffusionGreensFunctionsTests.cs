using System;
using MathNet.Numerics;
using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class DiffusionGreensFunctionsTests
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
                    "mm, with relative difference " + relDiffReal + " for the real compoment");
                var relDiffImag = Math.Abs(tfpsGF.Imaginary - imaginaryGreensFunctionValues[iR]) / imaginaryGreensFunctionValues[iR];
                Assert.IsTrue(relDiffImag < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiffReal + " for the imaginary compoment");
            }
        }

        [Test]
        public void StationaryPointSourceGreensFunctionZFlux_Test()
        {
            double[] greensFunctionValues = new double[] { -0.0777258, -0.00263504, -3.78754e-5 };

            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var relDiff = Math.Abs(
                    DiffusionGreensFunctions.StationaryPointSourceGreensFunctionZFlux(dp, rTestValues[iR], -dp.zp) -
                    greensFunctionValues[iR]) / greensFunctionValues[iR];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalPointSourceGreensFunctionZFlux_Test()
        {
            double[] greensFunctionValues = new double[] { -0.0280097, -0.0159038, -2.54359e-5 };

            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var relDiff = Math.Abs(
                    DiffusionGreensFunctions.TemporalPointSourceGreensFunctionZFlux(dp, rTestValues[iR], -dp.zp, time) -
                    greensFunctionValues[iR]) / greensFunctionValues[iR];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with time =" + time + "with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalFrequencyPointSourceGreensFunctionZFlux_Test()
        {
            double[] realGreensFunctionValues = new double[] { -0.0776417, -0.00257811, -2.41795e-5 };
            double[] imaginaryGreensFunctionValues = new double[] { 0.00145096, 0.000327894, 2.21896e-5 };


            for (int iR = 0; iR < rTestValues.Length; iR++)
            {
                var tfpsGF = DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunctionZFlux(dp, rTestValues[iR], -dp.zp, k);

                var relDiffReal = Math.Abs(tfpsGF.Real - realGreensFunctionValues[iR]) / realGreensFunctionValues[iR];
                Assert.IsTrue(relDiffReal < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiffReal + " for the real compoment");
                var relDiffImag = Math.Abs(tfpsGF.Imaginary - imaginaryGreensFunctionValues[iR]) / imaginaryGreensFunctionValues[iR];
                Assert.IsTrue(relDiffImag < thresholdValue, "Test failed for r =" + rTestValues[iR] +
                    "mm, with relative difference " + relDiffReal + " for the imaginary compoment");
            }
        }


    }
}
