using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class LayerOpticalPropertyRegionTests
    {
        private LayerOpticalPropertyRegion _layerOpticalPropertyRegion;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            var doubleRange = new DoubleRange(0, 9, 10);
            var opticalProperties = new OpticalProperties(0.1, 1, 0.8, 1.4);
            _layerOpticalPropertyRegion = new LayerOpticalPropertyRegion(doubleRange, opticalProperties);
        }

        [Test]
        public void Test_constructor()
        {
            Assert.That(_layerOpticalPropertyRegion, Is.InstanceOf<LayerOpticalPropertyRegion>());
        }

        [Test]
        public void Test_getting_values()
        {
            var zRange = _layerOpticalPropertyRegion.ZRange;
            var regionOp = _layerOpticalPropertyRegion.RegionOP;
            Assert.That(zRange.Start, Is.EqualTo(0));
            Assert.That(zRange.Stop, Is.EqualTo(9));
            Assert.That(zRange.Count, Is.EqualTo(10));
            Assert.That(zRange.Delta, Is.EqualTo(1));
            Assert.That(regionOp.Mua, Is.EqualTo(0.1));
            Assert.That(regionOp.Musp, Is.EqualTo(1));
            Assert.That(regionOp.G, Is.EqualTo(0.8));
            Assert.That(regionOp.N, Is.EqualTo(1.4));
        }
    }
}
