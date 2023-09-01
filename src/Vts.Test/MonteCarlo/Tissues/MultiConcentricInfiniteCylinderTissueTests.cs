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
        public void Create_instance_of_class()
        {
            _tissue = new MultiConcentricInfiniteCylinderTissue(
         new ITissueRegion[]
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 3),
                        3.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 3),
                        2.5,
                        new OpticalProperties(0.05, 0.0, 0.8, 1.4)
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
        /// Validate method GetRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            var index = _tissue.GetRegionIndex(new Position(0, 0, 7)); // outside both infinite cylinders
            Assert.AreEqual(1, index);
            index = _tissue.GetRegionIndex(new Position(0, 0, 0.1)); // inside outer cylinder outside inner
            Assert.AreEqual(3, index);
            index = _tissue.GetRegionIndex(new Position(0, 0, 1.0)); // inside inner cylinder
            Assert.AreEqual(4, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            var photon = new Photon( // on bottom of outer infinite cylinder pointed into it
                new Position(0, 0, 6.0),
                new Direction(0.0, 0, -1.0),
                1.0,
                _tissue,
                1,
                new Random());
            var index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(3, index);
            photon = new Photon( // on bottom of outer infinite cylinder pointed out of it
                new Position(0, 0, 6.0),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                3,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(1, index);
            photon = new Photon( // on bottom of inner infinite cylinder pointed into it
                new Position(0, 0, 5.5),
                new Direction(0.0, 0, -1.0),
                1.0,
                _tissue,
                3,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(4, index);
            photon = new Photon( // on bottom of inner infinite cylinder pointed out of it
                new Position(0, 0, 5.5),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                4,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(3, index);
            photon = new Photon( // on bottom of slab pointed out
                new Position(0, 0, 100.0),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                1,
                new Random());
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(2, index);
        }
        /// <summary>
        /// Test to make sure that shortest distance to layer boundary or infinite cylinders is correct
        /// </summary>
        [Test]
        public void Validate_GetDistanceToBoundary_method_returns_correct_results()
        {
            var photon = new Photon( // below outer infinite cylinder pointed into it
                new Position(0, 0, 7),
                new Direction(0.0, 0, -1.0),
                1.0,
                _tissue,
                1,
                new Random())
            {
                S = 10
            };
            var distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 1) < 1e-6);
            photon = new Photon(        // above inner infinite cylinder pointed into it
                new Position(0, 0, 0.1),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                3,
                new Random())
            {
                S = 10
            };
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 0.4) < 1e-6);
            photon = new Photon(        // inside inner infinite cylinder pointed out and down
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                3,
                new Random())
            {
                S = 10
            };
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 4.5) < 1e-6);
            photon = new Photon( // on bottom of slab pointed out
                new Position(0, 0, 95.0),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                1,
                new Random());
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 5) < 1e-6);
        }

        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon( // on top of cylinder pointed into it
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissue,
                1,
                new Random());
            var cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1,cosTheta);
        }

    }
}
