using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Vts.MonteCarlo.PhaseFunctions
{
    [TestFixture]
    public class AxiallySymmetricScattererMuellerMatrixTests
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            List <double> theta = new List<double>();
            theta.Add(0.0);
            theta.Add(Math.PI);
            double [] st11 = new[] { 0.0, 1.0 };
            double [] s12 = new[] { 0.0, 1.0 };
            double [] s22 = new[] { 0.0, 1.0 };
            double [] s33 = new[] { 0.0, 1.0 };
            double [] s34 = new[] { 0.0, 1.0 };
            double [] s44 = new[] { 0.0, 1.0 };
            AxiallySymmetricScattererMuellerMatrix m = new AxiallySymmetricScattererMuellerMatrix(theta, st11, s12, s22, s33, s34, s44);
            Assert.IsTrue(m.St11.Equals(st11));
            Assert.IsTrue(m.S12.Equals(s12));
            Assert.IsTrue(m.S22.Equals(s22));
            Assert.IsTrue(m.S33.Equals(s33));
            Assert.IsTrue(m.S34.Equals(s34));
            Assert.IsTrue(m.S44.Equals(s44));
            //Assert.IsTrue(m.Theta.Equals(theta));
        }
        /// <summary>
        /// Test to see if vector multiplication is implemented correctly.
        /// </summary>
        [Test]
        public void validate_MultiplyByVector()
        {
            List<double> theta = new List<double>();
            theta.Add(0.0);
            theta.Add(Math.PI);
            double[] st11 = new[] { 0.0, 0.5 };
            double[] s12 = new[] { 0.0, 1.0 };
            double[] s22 = new[] { 0.0, 0.5 };
            double[] s33 = new[] { 0.0, 1.0 };
            double[] s34 = new[] { 0.0, 1.0 };
            double[] s44 = new[] { 0.0, 1.0 };
            AxiallySymmetricScattererMuellerMatrix m = new AxiallySymmetricScattererMuellerMatrix(theta, st11, s12, s22, s33, s34, s44);
            StokesVector v = new StokesVector(1, 0, 1, 0);//linearly polarized stokes vector.
            m.MultiplyByVector(v, Math.PI);
            Assert.AreEqual(v.S0, 0.5);
            Assert.AreEqual(v.S1, 1.0);
            Assert.AreEqual(v.S2, 1.0);
            Assert.AreEqual(v.S3, -1.0);
        }
    }
}
