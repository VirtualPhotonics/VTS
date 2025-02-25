using NUnit.Framework;
using System;
using Vts.Common;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Extensions
{
    [TestFixture]
    internal class TypeExtensionsTests
    {
        [Test]
        public void Test_implements_type_returns_true()
        {
            ILayerOpticalPropertyRegion layerOpticalPropertyRegion = new LayerOpticalPropertyRegion(new DoubleRange(0, 9, 10), new OpticalProperties());
            Assert.IsInstanceOf<ILayerOpticalPropertyRegion>(layerOpticalPropertyRegion);
            var type = layerOpticalPropertyRegion.GetType();
            var instance = type.Implements<ILayerOpticalPropertyRegion>(layerOpticalPropertyRegion);
            Assert.That(instance, Is.True);
        }

        [Test]
        public void Test_implements_type_returns_false()
        {
            var type = typeof(ILayerOpticalPropertyRegion);
            IForwardSolver forwardSolver = new PointSourceSDAForwardSolver();
            Assert.IsInstanceOf<IForwardSolver>(forwardSolver);
            var instance = type.Implements<IForwardSolver>(forwardSolver);
            Assert.That(instance, Is.False);
        }
        [Test]
        public void Test_implements_type_throws_exception()
        {
            var forwardSolver = new PointSourceSDAForwardSolver();
            var type = forwardSolver.GetType();
            Assert.Throws<ArgumentException>(() => type.Implements<ForwardSolverBase>(forwardSolver));
        }
    }
}
