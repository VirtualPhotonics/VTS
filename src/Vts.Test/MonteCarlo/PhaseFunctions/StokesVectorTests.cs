using NUnit.Framework;
using System;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class StokesvectorTests
    {
        /// <summary>
        /// Tests whether ScatterToNewDirection returns correct value
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            StokesVector v = new StokesVector(1, 0, 1, 0);
            Assert.That(v.S0, Is.EqualTo(1));
            Assert.That(v.S1, Is.EqualTo(0));
            Assert.That(v.S2, Is.EqualTo(1));
            Assert.That(v.S3, Is.EqualTo(0));
        }

        public void validate_default_constructor()
        {
            StokesVector v = new StokesVector();
            Assert.That(v.S0, Is.EqualTo(1));
            Assert.That(v.S1, Is.EqualTo(0));
            Assert.That(v.S2, Is.EqualTo(0));
            Assert.That(v.S3, Is.EqualTo(0));
        }
        public void validate_rotate()
        {
            StokesVector v = new StokesVector(1, 0, 1, 0);
            MuellerMatrix m = new MuellerMatrix();
            v.Rotate(Math.PI / 4, Math.PI / 6, m);
            Assert.That(v.S0, Is.EqualTo(1));
            Assert.That(v.S1, Is.EqualTo(-1));
            Assert.That(v.S2, Is.EqualTo(0));
            Assert.That(v.S3, Is.EqualTo(0));

        }
    }
}