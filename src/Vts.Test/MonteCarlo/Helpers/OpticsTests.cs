using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Helpers
{
    [TestFixture]
    public class OpticsTests
    {
        /// <summary>
        /// Validate Fresnel results.
        /// </summary>
        [Test]
        public void Validate_Fresnel_results()
        {
            // test n1 = n2
            var n1 = 1.0;  // refractive index entering ray medium
            var n2 = 1.0;  // refractive index of next medium
            var cosWithNormal = 0.0; // cosine of angle with respect to normal -> 90 degrees parallel
            var reflectionProbability = Optics.Fresnel(n1, n2, cosWithNormal, out double uzSnell);
            Assert.That(reflectionProbability < 0.000001, Is.True);
            Assert.That(uzSnell < 0.000001, Is.True);
            // test n1 != n2 and cosWithNormal < cos(90 degrees) -> parallel
            n2 = 1.4;
            reflectionProbability = Optics.Fresnel(n1, n2, cosWithNormal, out uzSnell);
            Assert.That(Math.Abs(reflectionProbability - 1.0) < 0.000001, Is.True);
            Assert.That(uzSnell < 0.000001, Is.True);
            // test n1 != n2 and cosWithNormal > cos(0 degrees) -> normal incident
            cosWithNormal = 1.0; 
            reflectionProbability = Optics.Fresnel(n1, n2, cosWithNormal, out uzSnell);
            Assert.That(Math.Abs(reflectionProbability - 0.027777) < 0.000001, Is.True);
            Assert.That(Math.Abs(uzSnell - 1.0) < 0.000001, Is.True);
            // test n1 != n2 and cos(0 degrees) < cosWithNormal < cos(90 degrees)
            cosWithNormal = Math.Cos(45.0 * Math.PI / 180); // 45 degrees
            reflectionProbability = Optics.Fresnel(n1, n2, cosWithNormal, out uzSnell);
            Assert.That(Math.Abs(reflectionProbability - 0.036578) < 0.000001, Is.True);
            Assert.That(Math.Abs(uzSnell - 0.863074) < 0.000001, Is.True);
        }
        /// <summary>
        /// Test to validate fraction of specular reflection given n1 and n2
        /// </summary>
        [Test]
        public void Validate_Specular_results()
        {
            var n1 = 1.0;
            var n2 = 1.4;
            var fractionReflected = Optics.Specular(n1, n2);
            Assert.That(Math.Abs(fractionReflected - 0.027777) < 0.000001, Is.True);
            n2 = 1.5;
            fractionReflected = Optics.Specular(n1, n2);
            Assert.That(Math.Abs(fractionReflected - 0.04) < 0.000001, Is.True);
            // test reverse
            fractionReflected = Optics.Specular(n2, n1);
            Assert.That(Math.Abs(fractionReflected - 0.04) < 0.000001, Is.True);
            n2 = 1.4;
            fractionReflected = Optics.Specular(n2, n1);
            Assert.That(Math.Abs(fractionReflected - 0.027777) < 0.000001, Is.True);

        }
    }
}

