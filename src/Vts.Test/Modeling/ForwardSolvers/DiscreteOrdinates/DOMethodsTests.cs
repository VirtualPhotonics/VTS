using System;
using Meta.Numerics;
using Meta.Numerics.Matrices;
using NUnit.Framework;
using Vts.Modeling.ForwardSolvers.DiscreteOrdinates;

namespace Vts.Test.Modeling.ForwardSolvers.DiscreteOrdinates
{
    [TestFixture]
    internal class DOMethodsTests
    {
        [Test]
        public void Test_GaussLegendre_returns_correct_value()
        {
            var response = DOMethods.GaussLegendre(2);
            Assert.That(response.mu.Length > 0, Is.True);
            Assert.That(response.wt.Length > 0, Is.True);
            Assert.That(response.mu[0], Is.EqualTo(-0.577).Within(0.001));
            Assert.That(response.wt[0], Is.EqualTo(1.0).Within(0.1));
        }

        [Test]
        public void Test_GenFokkerPlanckEddington_returns_correct_value()
        {
            var response = DOMethods.GenFokkerPlanckEddington(new[] { 0.1, 0.2, 0.3, 0.4 }, new[] { 0.1, 0.2, 0.3 },
                new[] { 0.1, 0.2, 0.3 }, 2);
            Assert.That(response.Dimension, Is.EqualTo(2));
        }

        [Test]
        public void Test_LaplaceBelTrami_returns_correct_value()
        {
            var response = DOMethods.LaplaceBelTrami(new[] { 0.1, 0.2, 0.3, 0.4 }, new[] { 0.1, 0.2, 0.3, 0.4 }, 4);
            Assert.That(response.Dimension, Is.EqualTo(4));
        }

        [Ignore("Need to find the correct parameters to test")]
        [Test]
        public void Test_PWHalfSpace_returns_correct_value()
        {
            var response = DOMethods.PWHalfSpace(0.1, 1.0, new double[] { 2, 4, 6, 8, 10, 12, 14, 16 }, new double[] { 2, 4, 6, 8, 10, 12, 14, 16 },
                new SquareMatrix(8), 8);
            Assert.That(response.Length > 0, Is.True);
        }

        [Test]
        public void Test_PWHalfSpace_throws_DivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => DOMethods.PWHalfSpace(0.1, 1.0,
                new[] { 0.1, 0.2, 0.3, 0.4 }, new[] { 0.1, 0.2, 0.3, 0.4 },
                new SquareMatrix(4), 4));
        }

        [Test]
        public void Test_PWHalfSpace_throws_DimensionMismatchException()
        {
            Assert.Throws<DimensionMismatchException>(() => DOMethods.PWHalfSpace(0.1, 1.0, new[] { 0.1, 0.2, 0.3 }, new[] { 0.1, 0.2, 0.3 },
                new SquareMatrix(2), 4));
        }
    }
}
