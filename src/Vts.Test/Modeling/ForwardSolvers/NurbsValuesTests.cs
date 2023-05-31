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
            Assert.AreEqual(NurbsValuesDimensions.time, nurbsValues.ValuesDimensions);
            Assert.AreEqual(knots[5], nurbsValues.KnotVector[5]);
            Assert.AreEqual(0.9, nurbsValues.MaxValue);
            Assert.AreEqual(2, nurbsValues.Degree);
        }

        [Test]
        public void NurbsValues_constructor_overload_test()
        {
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };

            var nurbsValues = new NurbsValues(knots, 2, 0.9, controlPoints);
            Assert.AreEqual(knots[5], nurbsValues.KnotVector[5]);
            Assert.AreEqual(0.9, nurbsValues.MaxValue);
            Assert.AreEqual(2, nurbsValues.Degree);
            Assert.AreEqual(controlPoints[3], nurbsValues.ControlPoints[3]);
        }
    }
}
