using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    internal class NurbsValuesTests
    {
        [Test]
        public void NurbsValues_constructor_test()
        {
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };

            var nurbsValues = new NurbsValues(NurbsValuesDimensions.time, knots, 0.9, 2);
            Assert.That(nurbsValues.ValuesDimensions, Is.EqualTo(NurbsValuesDimensions.time));
            Assert.That(nurbsValues.KnotVector[5], Is.EqualTo(knots[5]));
            Assert.That(nurbsValues.MaxValue, Is.EqualTo(0.9));
            Assert.That(nurbsValues.Degree, Is.EqualTo(2));
        }

        [Test]
        public void NurbsValues_constructor_overload_test()
        {
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };

            var nurbsValues = new NurbsValues(knots, 2, 0.9, controlPoints);
            Assert.That(nurbsValues.KnotVector[5], Is.EqualTo(knots[5]));
            Assert.That(nurbsValues.MaxValue, Is.EqualTo(0.9));
            Assert.That(nurbsValues.Degree, Is.EqualTo(2));
            Assert.That(nurbsValues.ControlPoints[3], Is.EqualTo(controlPoints[3]));
        }
    }
}
