using NUnit.Framework;
using System;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    [TestFixture]
    public class MieScattererMuellerMatrixTests
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            double[] theta = new[] { 0.0, Math.PI };
            double[] st11 = new[] { 0.0, 1.0 };
            double[] s12 = new[] { 0.0, 1.0 };
            double[] s22 = new[] { 0.0, 1.0 };
            double[] s33 = new[] { 0.0, 1.0 };
            double[] s34 = new[] { 0.0, 1.0 };
            AxiallySymmetricScattererMuellerMatrix m = new AxiallySymmetricScattererMuellerMatrix(theta, st11, s12, s22, s33, s34, s44);
            Assert.IsTrue(m.St11.Equals(st11));
            Assert.IsTrue(m.S12.Equals(s12));
            Assert.IsTrue(m.S22.Equals(s22));
            Assert.IsTrue(m.S33.Equals(s33));
            Assert.IsTrue(m.S34.Equals(s34));
            Assert.IsTrue(m.Theta.Equals(theta));
        }
        /// <summary>
        /// Tests to validate if integral of PDF is CDF.
        /// </summary>
        [Test]
        public void validate_MultiplyByVector()
        {
            double[] theta = new[] { 0.0, Math.PI };
            double[] st11 = new[] { 0.0, 0.5 };
            double[] s12 = new[] { 0.0, 1.0 };
            double[] s22 = new[] { 0.0, 0.5 };
            double[] s33 = new[] { 0.0, 1.0 };
            double[] s34 = new[] { 0.0, 1.0 };
            AxiallySymmetricScattererMuellerMatrix m = new AxiallySymmetricScattererMuellerMatrix(theta, st11, s12, s22, s33, s34, s44);
            StokesVector v = new StokesVector(1, 0.5, 0.7, 0.6);//linearly polarized stokes vector.
            m.MultiplyByVector(v, Math.PI);
            Assert.AreEqual(v.S0, 1.0);
            Assert.AreEqual(v.S1, 1.25);
            Assert.AreEqual(v.S2, 1.3);
            Assert.AreEqual(v.S3, -0.1);
        }
    }
}
