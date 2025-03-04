using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Vts.Common.Math;

namespace Vts.Test.Common.Math
{
    [TestFixture]
    internal class IntegrationTests
    {
        [Test]
        public void Test_IntegrateAdaptiveSimpsonRule_returns_double()
        {
            Func<double, double> f = value => value * 2;
            var result = Integration.IntegrateAdaptiveSimpsonRule(f, 2, 4, 2);
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void Test_IntegrateSimpsonRule_returns_double()
        {
            Func<double, double> f = value => value * 2;
            var result = Integration.IntegrateSimpsonRule(f, 2, 4);
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void Test_AdaptiveRecursiveSimpson_returns_double()
        {
            Func<double, double> f = value => value * 2;
            var result = Integration.AdaptiveRecursiveSimpson(f, 2, 4, 10, 2);
            Assert.That(result, Is.EqualTo(12.66).Within(0.01));
        }

        [Test]
        public void Test_AdaptiveRecursiveSimpson_using_recursion_returns_double()
        {
            Func<double, double> f = value => value * 2;
            var result = Integration.AdaptiveRecursiveSimpson(f, 2, 4, 0, 2);
            Assert.That(result, Is.EqualTo(12).Within(0.01));
        }
    }
}
