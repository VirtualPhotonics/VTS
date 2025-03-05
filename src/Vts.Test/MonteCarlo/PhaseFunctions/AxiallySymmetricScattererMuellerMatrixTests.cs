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
            Assert.That(m.St11.Equals(st11), Is.True);
            Assert.That(m.S12.Equals(s12), Is.True);
            Assert.That(m.S22.Equals(s22), Is.True);
            Assert.That(m.S33.Equals(s33), Is.True);
            Assert.That(m.S34.Equals(s34), Is.True);
            Assert.That(m.S44.Equals(s44), Is.True);
            //Assert.That(m.Theta.Equals(theta), Is.True);
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
            Assert.That(v.S0, Is.EqualTo(0.5));
            Assert.That(v.S1, Is.EqualTo(1.0));
            Assert.That(v.S2, Is.EqualTo(1.0));
            Assert.That(v.S3, Is.EqualTo(-1.0));
        }
    }
}
