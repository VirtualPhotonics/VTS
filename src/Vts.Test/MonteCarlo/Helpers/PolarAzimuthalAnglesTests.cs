using System;
using NUnit.Framework;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Helpers
{
    [TestFixture]
    public class PolarAzimuthalAnglesTests
    {
        /// <summary>
        /// Validate default and fully-defined constructor
        /// </summary>
        [Test]
        public void Validate_constructor_results()
        {
            // default constructor
            var polarAzimuthalAngles = new PolarAzimuthalAngles();
            Assert.IsInstanceOf<PolarAzimuthalAngles>(polarAzimuthalAngles);
            // fully defined
            polarAzimuthalAngles = new PolarAzimuthalAngles()
            {
                Theta = 1.0, 
                Phi = 2.0
            };
            Assert.IsInstanceOf<PolarAzimuthalAngles>(polarAzimuthalAngles);
        }
        /// <summary>
        /// Validate equals and not equals operator method
        /// </summary>
        [Test]
        public void Validate_equals_and_not_equals_operator_results()
        {
            var polarAzimuthalAngles1 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 2.0
            };
            var polarAzimuthalAngles2 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 2.0
            };
            Assert.IsTrue(polarAzimuthalAngles1 == polarAzimuthalAngles2);
            var polarAzimuthalAngles3 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 3.0
            };
            Assert.IsTrue(polarAzimuthalAngles1 != polarAzimuthalAngles3);
        }
        /// <summary>
        /// Validate Equals method
        /// </summary>
        [Test]
        public void Validate_Equals_results()
        {
            // test when equal
            var polarAzimuthalAngles1 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 2.0
            };
            var polarAzimuthalAngles2 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 2.0
            };
            Assert.IsTrue(polarAzimuthalAngles1.Equals(polarAzimuthalAngles2)); 
            // test when not equal
            var polarAzimuthalAngles3 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 3.0
            };
            Assert.IsFalse(polarAzimuthalAngles1.Equals(polarAzimuthalAngles3));
            // test when null
            Assert.IsFalse(polarAzimuthalAngles1.Equals((PolarAzimuthalAngles) null));

        }
        /// <summary>
        /// Validate Clone method
        /// </summary>
        [Test]
        public void Validate_Clone_results()
        {
            var polarAzimuthalAngles = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 2.0
            };
            var polarAzimuthalAnglesClone = polarAzimuthalAngles.Clone();
            Assert.IsTrue(Math.Abs(polarAzimuthalAnglesClone.Theta - 1.0) < 1e-6);
            Assert.IsTrue(Math.Abs(polarAzimuthalAnglesClone.Phi - 2.0) < 1e-6);
        }

    }
}

