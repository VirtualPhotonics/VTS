using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for MultiConcentricInfiniteCylinderTissue
    /// </summary>
    [TestFixture]
    public class MultiConcentricInfiniteCylinderTissueTests
    {
        private MultiConcentricInfiniteCylinderTissue _tissue;
        /// <summary>
        /// Validate general constructor of Tissue
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _tissue = new MultiConcentricInfiniteCylinderTissue(
         new ITissueRegion[]
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        1.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.75,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    )
                },
                new ITissueRegion []
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                }
            );
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetRegionIndex_method_returns_correct_result()
        {
            int index = _tissue.GetRegionIndex(new Position(0, 0, 5)); // outside both infinite cylinders
            Assert.AreEqual(index, 1);
            index = _tissue.GetRegionIndex(new Position(0, 0, 1.8)); // inside outer cylinder outside inner
            Assert.AreEqual(index, 3);
            index = _tissue.GetRegionIndex(new Position(0, 0, 1.0)); // inside inner cylinder
            Assert.AreEqual(index, 4);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            Photon photon = new Photon( // on bottom of outer infinite cylinder pointed into it
                new Position(0, 0, 2.0),
                new Direction(0.0, 0, -1.0),
                _tissue,
                1,
                new Random());
            var index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 3);
            photon = new Photon( // on bottom of outer infinite cylinder pointed out of it
                new Position(0, 0, 2.0),
                new Direction(0.0, 0, 1.0),
                _tissue,
                3,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 1);
            photon = new Photon( // on bottom of inner infinite cylinder pointed into it
                new Position(0, 0, 1.75),
                new Direction(0.0, 0, -1.0),
                _tissue,
                3,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 4);
            photon = new Photon( // on bottom of inner infinite cylinder pointed out of it
                new Position(0, 0, 1.75),
                new Direction(0.0, 0, 1.0),
                _tissue,
                4,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 3);
            photon = new Photon( // on bottom of slab pointed out
                new Position(0, 0, 100.0),
                new Direction(0.0, 0, 1.0),
                _tissue,
                1,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 2);
        }
        /// <summary>
        /// Test to make sure that shortest distance to layer boundary or infinite cylinders is correct
        /// </summary>
        [Test]
        public void validate_GetDistanceToBoundary_method_returns_correct_results()
        {
            Photon photon = new Photon( // below outer infinite cylinder pointed into it
                new Position(0, 0, 3),
                new Direction(0.0, 0, -1.0),
                _tissue,
                1,
                new Random());
            photon.S = 10;
            var distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 1) < 1e-6);
            photon = new Photon(        // above inner infinite cylinder pointed into it
                new Position(0, 0, 0.1),
                new Direction(0.0, 0, 1.0),
                _tissue,
                3,
                new Random());
            photon.S = 10;
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 0.15) < 1e-6);
            photon = new Photon(        // inside inner infinite cylinder pointed out and down
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                _tissue,
                3,
                new Random());
            photon.S = 10;
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 0.75) < 1e-6);
            photon = new Photon( // on bottom of slab pointed out
                new Position(0, 0, 95.0),
                new Direction(0.0, 0, 1.0),
                _tissue,
                1,
                new Random());
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 5) < 1e-6);
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
