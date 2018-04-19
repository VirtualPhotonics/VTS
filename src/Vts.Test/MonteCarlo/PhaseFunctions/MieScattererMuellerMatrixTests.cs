using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
            List <double> theta = new List<double>();
            theta.Add(0.0);
            theta.Add(Math.PI);
            double[] st11 = new[] { 0.0, 1.0 };
            double[] s12 = new[] { 0.0, 1.0 };
            double[] s22 = new[] { 0.0, 1.0 };
            double[] s33 = new[] { 0.0, 1.0 };
            double[] s34 = new[] { 0.0, 1.0 };
            MieScattererMuellerMatrix m = new MieScattererMuellerMatrix(theta, st11, s12, s22, s33, s34);
            Assert.IsTrue(m.St11.Equals(st11));
            Assert.IsTrue(m.S12.Equals(s12));
            Assert.IsTrue(m.S22.Equals(s22));
            Assert.IsTrue(m.S33.Equals(s33));
            Assert.IsTrue(m.S34.Equals(s34));
            Assert.IsTrue(m.Theta.Equals(theta));
        }
        /// <summary>
        /// Tests to validate if multiplying by Vector returns correct value.
        /// </summary>
        [Test]
        public void validate_MultiplyByVector()
        {
            List <double> theta = new List <double>();
            theta.Add(0.0);
            theta.Add(Math.PI);
            double[] st11 = new[] { 0.0, 0.5 };
            double[] s12 = new[] { 0.0, 1.0 };
            double[] s22 = new[] { 0.0, 0.5 };
            double[] s33 = new[] { 0.0, 1.0 };
            double[] s34 = new[] { 0.0, 1.0 };
            MieScattererMuellerMatrix m = new MieScattererMuellerMatrix(theta, st11, s12, s22, s33, s34);
            StokesVector v = new StokesVector(1, 0.5, 0.7, 0.6);//linearly polarized stokes vector.
            m.MultiplyByVector(v, Math.PI);
            Assert.LessOrEqual(Math.Abs(v.S0 - 1.0), 1.0 * 0.0001);
            Assert.LessOrEqual(Math.Abs(v.S1 - 1.25), 1.25 * 0.0001);
            Assert.LessOrEqual(Math.Abs(v.S2 - 1.3), 1.3 * 0.0001);
            Assert.LessOrEqual(Math.Abs(v.S3 + 0.1), 0.1 * 0.0001);
        }
        
    }
}
