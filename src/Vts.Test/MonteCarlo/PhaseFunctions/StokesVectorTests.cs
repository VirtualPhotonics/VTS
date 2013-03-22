﻿using NUnit.Framework;
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
            StokesVector v = new StokesVector(1, 2, 3, 4);
            Assert.AreEqual(v.S0, 1);
            Assert.AreEqual(v.S1, 2);
            Assert.AreEqual(v.S2, 3);
            Assert.AreEqual(v.S3, 4);
        }

        public void validate_default_constructor()
        {
            StokesVector v = new StokesVector();
            Assert.AreEqual(v.S0, 1);
            Assert.AreEqual(v.S1, 0);
            Assert.AreEqual(v.S2, 0);
            Assert.AreEqual(v.S3, 0);
        }
        public void validate_rotate()
        {
            StokesVector v = new StokesVector(1, 0, 1, 0);
    }
}