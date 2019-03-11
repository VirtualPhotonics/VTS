using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for SingleInclusionTissue
    /// </summary>
    [TestFixture]
    public class SingleInclusionTissueTests
    {
        private SingleInclusionTissue _tissue;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _tissue = new SingleInclusionTissue(new EllipsoidTissueRegion(
                new Position(0, 0, 3), 1.0, 1.0, 2.0, new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetRegionIndex_method_returns_correct_result()
        {
            int index = _tissue.GetRegionIndex(new Position(0, 0, 0.5)); // outside ellipsoid
            Assert.AreEqual(index, 1);
            index = _tissue.GetRegionIndex(new Position(0, 0, 2.5)); // inside ellipsoid
            Assert.AreEqual(index, 3);
            index = _tissue.GetRegionIndex(new Position(0, 0, 1.0)); // on ellipsoid is considered in
            Assert.AreEqual(index, 3);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            Photon photon = new Photon( // on top of ellipsoid pointed into it
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                _tissue,
                1,
                new Random());
            var index = _tissue.GetNeighborRegionIndex(photon); 
            Assert.AreEqual(index, 3);
            photon.DP.Position = new Position(0, 0, 100.0); // at bottom of slab pointed out
            index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 2);
        }

        /// <summary>
        /// Validate method GetReflectedDirection return correct Direction.  Note that Photon class
        /// determines whether in critical angle and if so, whether to reflect or refract.  This unit
        /// test just tests isolated method.
        /// </summary>
        [Test]
        public void verify_GetReflectedDirection_method_returns_correct_result()
        {
            // index matched
            var currentPosition = new Position(0, 0, 2); // put photon on ellipsoid
            var currentDirection = new Direction(0, 0, 1);
            Direction reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(reflectedDir, new Direction(0, 0, 1));
            // index mismatched
            _tissue.Regions[3].RegionOP.N = 1.5; // surrounding layer has n=1.4
            currentPosition = new Position(0, 0, 2); // put photon on ellipsoid
            currentDirection = new Direction(0, 0, 1);
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(reflectedDir, new Direction(0, 0, -1));
        }
        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct value.   Note that this
        /// gets called by Photon method CrossRegionOrReflect.  All return values
        /// from GetAngleRelativeToBoundaryNormal are positive to be used successfully by Photon.
        /// </summary>
        [Test]
        public void verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon();
            photon.DP.Position = new Position(0, 0, 2); // put photon on ellipsoid top
            photon.DP.Direction = new Direction(0, 0, 1); // direction opposite surface normal
            var dirCosine = _tissue.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(dirCosine, 1);
            photon.DP.Position = new Position(0, 0, 4); // put photon on ellipsoid bottom
            photon.DP.Direction = new Direction(0, 0, 1);
            dirCosine = _tissue.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(dirCosine, 1);
        }

    }
}
