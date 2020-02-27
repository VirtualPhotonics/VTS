using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for BoundedTissue
    /// </summary>
    [TestFixture]
    public class BoundedTissueTests
    {
        private BoundedTissue _oneLayerTissue, _twoLayerTissue;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue cylinder
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _oneLayerTissue = new BoundedTissue(new CylinderTissueRegion(
                new Position(0, 0, 50), 1.0, 100.0, new OpticalProperties(),
                "HenyeyGreensteinKey4"),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                });
         
            _twoLayerTissue = new BoundedTissue(new CylinderTissueRegion(
                    new Position(0, 0, 50), 1.0, 100.0, new OpticalProperties(),
                    "HenyeyGreensteinKey5"),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 1.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(1.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                });
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetRegionIndex_method_returns_correct_result()
        {
            int index = _oneLayerTissue.GetRegionIndex(new Position(10, 0, 0)); // outside cylinder
            Assert.AreEqual(index, 3);
            index = _oneLayerTissue.GetRegionIndex(new Position(0, 0, 2.5)); // inside cylinder
            Assert.AreEqual(index, 1);
            index = _oneLayerTissue.GetRegionIndex(new Position(0, 0, 0)); // on cylinder is considered in
            Assert.AreEqual(index, 1);
            // two layer results
            index = _twoLayerTissue.GetRegionIndex(new Position(10, 0, 0)); // outside cylinder
            Assert.AreEqual(index, 4);
            index = _twoLayerTissue.GetRegionIndex(new Position(0, 0, 2.5)); // inside cylinder
            Assert.AreEqual(index, 2);
            index = _twoLayerTissue.GetRegionIndex(new Position(0, 0, 0)); // on cylinder is considered in
            Assert.AreEqual(index, 1);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            Photon photon = new Photon( // on side of cylinder pointed into it
                new Position(-1, 0, 1),
                new Direction(1.0, 0, 0),
                1.0,
                _oneLayerTissue,
                3,
                new Random());
            var index = _oneLayerTissue.GetNeighborRegionIndex(photon); 
            Assert.AreEqual(index, 1);
            photon = new Photon( // on side of cylinder pointed out of it
                new Position(-1, 0, 1),
                new Direction(-1.0, 0, 0),
                1.0,
                _oneLayerTissue,
                1,
                new Random());
            index = _oneLayerTissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 3);
            // two layer results
            photon = new Photon( // on side of cylinder pointed into LAYER 1
                new Position(-1, 0, 0.5),  
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissue,
                4,
                new Random());
            index = _twoLayerTissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 1);
            photon = new Photon( // on side of cylinder in LAYER 1 pointed out of it
                new Position(-1, 0, 0.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissue,
                1,
                new Random());
            index = _twoLayerTissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 4);
            photon = new Photon( // on side of cylinder pointed into LAYER 2
                new Position(-1, 0, 1.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissue,
                4,
                new Random());
            index = _twoLayerTissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 2);
            photon = new Photon( // on side of cylinder in LAYER 2 pointed out of it
                new Position(-1, 0, 1.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissue,
                1,
                new Random());
            index = _twoLayerTissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 4);
        }

        ///// <summary>
        ///// Validate method GetAngleRelativeToBoundaryNormal return correct boolean
        ///// </summary>
        //[Test]
        //public void verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        //{
        //    Photon photon = new Photon( // on top of ellipsoid pointed into it
        //        new Position(0, 0, 1.0),
        //        new Direction(0.0, 0, 1.0),
        //        _tissue,
        //        1,
        //        new Random());
        //    double cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(photon);
        //    Assert.AreEqual(cosTheta, 1);
        //}

    }
}
