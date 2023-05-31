using System;
using NUnit.Framework;
using Convert = Vts.Common.Math.Convert;

namespace Vts.Test.Common.Math
{
    [TestFixture]
    public class ConvertTests
    {
        [Test]
        public void Validate_ToPhase_returns_correct_values()
        {
            var real = new double[4] { 2.03e-2, 2.02e-2, 2.01e-2, 2.00e-2 };
            var imaginary = new double[4] { -4.68e-4, -9.26e-4, -1.37e-3, -1.79e-3 };
            var phase = Convert.ToPhase(real, imaginary);
            // compare results with results calculated elsewhere (using perl script pham)
            Assert.IsTrue(System.Math.Abs(phase[0] - 1.3207) < 0.001);
            Assert.IsTrue(System.Math.Abs(phase[1] - 2.6247) < 0.001);
            Assert.IsTrue(System.Math.Abs(phase[2] - 3.8992) < 0.001);
            Assert.IsTrue(System.Math.Abs(phase[3] - 5.1143) < 0.001);
        }
        [Test]
        public void Validate_ToAmplitude_returns_correct_values()
        {
            var real = new double[4] { 2.03e-2, 2.02e-2, 2.01e-2, 2.00e-2 };
            var imaginary = new double[4] { -4.68e-4, -9.26e-4, -1.37e-3, -1.79e-3 };
            var amp = Convert.ToAmplitude(real, imaginary);
            // compare results with results calculated elsewhere (using perl script pham)
            Assert.IsTrue(System.Math.Abs(amp[0] - 0.0203) < 0.0001);
            Assert.IsTrue(System.Math.Abs(amp[1] - 0.0202) < 0.0001);
            Assert.IsTrue(System.Math.Abs(amp[2] - 0.0201) < 0.0001);
            Assert.IsTrue(System.Math.Abs(amp[3] - 0.0201) < 0.0001);
        }

        [Test]
        public void Test_ToPhase_throws_argument_exception()
        {
            var real = new double[] { 1, 2, 3 };
            var imaginary = new double[] { 1, 2, 3, 4 };
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    Convert.ToPhase(real, imaginary);
                }
                catch (Exception e)
                {
                    Assert.AreEqual("Error in ToPhase: real and imaginary arrays are not the same size!", e.Message);
                    throw;
                }
            });
        }

        [Test]
        public void Test_ToAmplitude_throws_argument_exception()
        {
            var real = new double[] { 1, 2, 3 };
            var imaginary = new double[] { 1, 2, 3, 4 };
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    Convert.ToAmplitude(real, imaginary);
                }
                catch (Exception e)
                {
                    Assert.AreEqual("Error in ToAmplitude: real and imaginary arrays are not the same size!", e.Message);
                    throw;
                }
            });
        }
    }
}
