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
            Assert.AreEqual(0, zRange.Start);
            Assert.AreEqual(9, zRange.Stop);
            Assert.AreEqual(10, zRange.Count);
            Assert.AreEqual(1, zRange.Delta);
            Assert.AreEqual(0.1, regionOp.Mua);
            Assert.AreEqual(1, regionOp.Musp);
            Assert.AreEqual(0.8, regionOp.G);
            Assert.AreEqual(1.4, regionOp.N);
        }
    }
}
