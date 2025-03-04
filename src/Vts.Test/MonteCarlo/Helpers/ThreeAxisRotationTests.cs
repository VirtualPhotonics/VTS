using System;
using NUnit.Framework;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Helpers
{
    [TestFixture]
    public class ThreeAxisRotationTests
    {
        /// <summary>
        /// Validate default and fully-defined constructor
        /// </summary>
        [Test]
        public void Validate_constructor_results()
        {
            // default constructor
            var threeAxisRotation = new ThreeAxisRotation();
            Assert.That(threeAxisRotation, Is.InstanceOf<ThreeAxisRotation>());
            // fully defined
            threeAxisRotation = new ThreeAxisRotation()
            {
                XRotation = 1.0,
                YRotation = 2.0,
                ZRotation = 3.0
            };
            Assert.That(threeAxisRotation, Is.InstanceOf<ThreeAxisRotation>());
        }
        /// <summary>
        /// Validate Clone method
        /// </summary>
        [Test]
        public void Validate_Clone_results()
        {
            var threeAxisRotation = new ThreeAxisRotation()
            {
                XRotation = 1.0,
                YRotation = 2.0,
                ZRotation = 3.0
            };
            var threeAxisRotationClone = threeAxisRotation.Clone();
            Assert.That(Math.Abs(threeAxisRotationClone.XRotation - 1.0) < 1e-6, Is.True);
            Assert.That(Math.Abs(threeAxisRotationClone.YRotation - 2.0) < 1e-6, Is.True);
            Assert.That(Math.Abs(threeAxisRotationClone.ZRotation - 3.0) < 1e-6, Is.True);
        }

    }
}

