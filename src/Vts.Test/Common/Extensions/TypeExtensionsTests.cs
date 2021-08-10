using System;
using NLog;
using NUnit.Framework;
using Vts.Common;
using Vts.Extensions;
using Vts.Factories;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Common.Extensions
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
            Assert.IsTrue(instance);
        }

        [Test]
        public void Test_implements_type_returns_false()
        {
            var type = typeof(ILayerOpticalPropertyRegion);
            IForwardSolver forwardSolver = new PointSourceSDAForwardSolver();
            Assert.IsInstanceOf<IForwardSolver>(forwardSolver);
            var instance = type.Implements<IForwardSolver>(forwardSolver);
            Assert.IsFalse(instance);
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
