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
            Assert.That(polarAzimuthalAngles, Is.InstanceOf<PolarAzimuthalAngles>());
            // fully defined
            polarAzimuthalAngles = new PolarAzimuthalAngles()
            {
                Theta = 1.0, 
                Phi = 2.0
            };
            Assert.That(polarAzimuthalAngles, Is.InstanceOf<PolarAzimuthalAngles>());
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
            Assert.That(polarAzimuthalAngles1 == polarAzimuthalAngles2, Is.True);
            var polarAzimuthalAngles3 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 3.0
            };
            Assert.That(polarAzimuthalAngles1 != polarAzimuthalAngles3, Is.True);
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
            Assert.That(polarAzimuthalAngles1.Equals(polarAzimuthalAngles2), Is.True); 
            // test when not equal
            var polarAzimuthalAngles3 = new PolarAzimuthalAngles()
            {
                Theta = 1.0,
                Phi = 3.0
            };
            Assert.That(polarAzimuthalAngles1.Equals(polarAzimuthalAngles3), Is.False);
            // test when null
            Assert.That(polarAzimuthalAngles1.Equals((PolarAzimuthalAngles) null), Is.False);

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
            Assert.That(Math.Abs(polarAzimuthalAnglesClone.Theta - 1.0) < 1e-6, Is.True);
            Assert.That(Math.Abs(polarAzimuthalAnglesClone.Phi - 2.0) < 1e-6, Is.True);
        }

        [Test]
        public void Test_get_hash_code()
        {
            var polarAzimuthalAngles = new PolarAzimuthalAngles();
            var hashCode = polarAzimuthalAngles.GetHashCode();
            Assert.That(polarAzimuthalAngles.GetHashCode(), Is.EqualTo(hashCode));
            var direction2 = new PolarAzimuthalAngles();
            Assert.That(direction2.GetHashCode(), Is.EqualTo(hashCode));
        }
    }
}

