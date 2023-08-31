using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for LayerTissueRegion
    /// </summary>
    [TestFixture]
    public class LayerTissueRegionTests
    {
        private LayerTissueRegion _layerTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _layerTissueRegion = new LayerTissueRegion(
                new DoubleRange(0, 10), new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_layer_properties()
        {
            Assert.AreEqual(0.0, _layerTissueRegion.ZRange.Start);
            Assert.AreEqual(10.0, _layerTissueRegion.ZRange.Stop);
            Assert.AreEqual(5.0, _layerTissueRegion.Center.Z);
        }
        /// <summary>
        /// Validate method OnBoundary return correct Boolean.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns false if *exactly* on boundary
            var result = _layerTissueRegion.OnBoundary(new Position(0, 0, 1.0));
            Assert.IsFalse(result);
            result = _layerTissueRegion.OnBoundary(new Position(0, 0, 10.0));
            Assert.IsTrue(result);
            result = _layerTissueRegion.OnBoundary(new Position(0, 0, 2.0));
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _layerTissueRegion.ContainsPosition(new Position(0, 0, 3.0)); // inside
            Assert.IsTrue(result);
            result = _layerTissueRegion.ContainsPosition(new Position(0, 0, 10.0)); // on boundary
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Validate method RayIntersectBoundary return correct result
        /// Note: LayerTissueRegion.RayIntersectBoundary does NOT use photon.S in the calculation
        /// so all photons (unless horizontal) intersect the top or bottom of the layer and the
        /// returned distance is not infinity (unless horizontal)
        /// </summary>
        [Test]
        public void Verify_RayIntersectBoundary_method_returns_correct_result()
        {
            var photon = new Photon
            {
                DP =
                {
                    Position = new Position(0, 0, 2),
                    Direction = new Direction(0, 0, 1)
                }
            };
            //photon.S = 100.0; // definitely intersect 
            double distanceToBoundary;
            var result = _layerTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(8.0, distanceToBoundary);
        }
    }
}
