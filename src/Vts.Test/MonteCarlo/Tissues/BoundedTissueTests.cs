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
        private BoundedTissue _oneLayerTissueBoundedByVoxel, _twoLayerTissueBoundedByVoxel;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue cylinder
        /// </summary>

        /// <summary>
        /// Test default constructor
        /// </summary>
        [Test]
        public void Validate_default_constructor()
        {
            var boundedTissue = new BoundedTissue();
            Assert.IsInstanceOf<BoundedTissue>(boundedTissue);
        }

        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueBoundedByVoxel = new BoundedTissue(
                new CaplessVoxelTissueRegion(
                    new DoubleRange(-1, 1, 2), // x range
                    new DoubleRange(-1, 1, 2), // y range
                    new DoubleRange(0, 10, 2),  // z range
                    new OpticalProperties(0.01, 1.0, 0.8, 1.0)),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 10.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
            _twoLayerTissueBoundedByVoxel = new BoundedTissue(
                new VoxelTissueRegion(
                    new DoubleRange(-1, 1, 2), // x range
                    new DoubleRange(-1, 1, 2), // y range
                    new DoubleRange(1, 10, 2),  // z range
                    new OpticalProperties(0.01, 1.0, 0.8, 1.0)),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 2.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(2.0, 10.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            var index = _oneLayerTissueBoundedByVoxel.GetRegionIndex(new Position(0, 0, 0.5)); // inside voxel
            Assert.AreEqual(1, index);
            index = _oneLayerTissueBoundedByVoxel.GetRegionIndex(new Position(2.5, 0, 2.5)); // inside bounding voxel
            Assert.AreEqual(3, index);
            index = _oneLayerTissueBoundedByVoxel.GetRegionIndex(new Position(0.0, 0.0, 0.0)); // on top sb inside voxel
            Assert.AreEqual(1, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for oneLayerTissueBoundedByVoxel return correct Boolean
        /// </summary>
        [Test]
        public void Verify_oneLayerTissueBoundedByVoxel_GetNeighborRegionIndex_method_correct_when_photon_on_ellipsoid()
        {
            var photon = new Photon( // on side of voxel pointed into it
                new Position(-1.0, 0, 1.0),
                new Direction(1.0, 0, 0.0),
                1,
                _oneLayerTissueBoundedByVoxel,
                1,
                new Random());
            var index = _oneLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.AreEqual(3, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for oneLayerTissueBoundedByVoxel return correct Boolean
        /// </summary>
        [Test]
        public void Verify_oneLayerTissueBoundedByVoxel_GetNeighborRegionIndex_method_correct_when_photon_bottom_slab()
        {
            // on bottom of slab pointed out
            var photon = new Photon( // have to reinitialize photon so that _onBoundary is set to false
                new Position(0, 0, 10.0),
                new Direction(0.0, 0, 1.0),
                1,
                _oneLayerTissueBoundedByVoxel,
                1,
                new Random());
            var index = _oneLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.AreEqual(2, index);
        }


        /// <summary>
        /// Validate method GetReflectedDirection return correct Direction.  Note that Photon class
        /// determines whether in critical angle and if so, whether to reflect or refract.  This unit
        /// test just tests isolated method.
        /// </summary>
        [Test]
        public void Verify_GetReflectedDirection_method_returns_correct_result()
        {
            // put photon on boundary of domain to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(0, 0, 0);
            var currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            var reflectedDir = _oneLayerTissueBoundedByVoxel.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.AreEqual(1 / Math.Sqrt(2), reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(1 / Math.Sqrt(2), reflectedDir.Uz); // reflection off layer just flips sign of Uz
            // index mismatched between tissue and voxel
            currentPosition = new Position(-1, 0, 2); // put photon on bounding voxel
            currentDirection = new Direction(1, 0, 0);
            reflectedDir = _oneLayerTissueBoundedByVoxel.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(-1, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(0, reflectedDir.Uz);
            // index matched
            _oneLayerTissueBoundedByVoxel.Regions[3].RegionOP.N = 1.4; // surrounding layer has n=1.4
            currentPosition = new Position(1, 0, 2); // put photon on bounding voxel
            currentDirection = new Direction(-1, 0, 0); // perpendicular to tangent surface
            reflectedDir = _oneLayerTissueBoundedByVoxel.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(-1, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(0, reflectedDir.Uz);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2)); // 45 deg to tangent surface
            reflectedDir = _oneLayerTissueBoundedByVoxel.GetReflectedDirection(currentPosition, currentDirection);
            Assert.IsTrue(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.IsTrue(Math.Abs(reflectedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6);
            // set n of surrounding region back
            _oneLayerTissueBoundedByVoxel.Regions[3].RegionOP.N = 1.0; 
        }
        /// <summary>
        /// Validate method GetReflectedDirection returns correct direction.
        /// </summary>
        [Test]
        public void Verify_GetRefractedDirection_method_returns_correct_result()
        {
            // put photon on boundary of domain to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(0, 0, 10);
            var currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            var nCurrent = 1.4;
            var nNext = 1.4;
            var cosThetaSnell = 1 / Math.Sqrt(2);
            var refractedDir = _oneLayerTissueBoundedByVoxel.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6);
            // put photon on voxel: index matched
            currentPosition = new Position(1, 0, 1);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            nNext = 1.4;
            refractedDir = _oneLayerTissueBoundedByVoxel.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6);
            // put photon on voxel: index mismatched
            currentPosition = new Position(-1, 0, 1);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            nCurrent = 1.0;
            refractedDir = _oneLayerTissueBoundedByVoxel.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 0.965519) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz - 0.260331) < 1e-6);
            Assert.IsTrue(Math.Sqrt(refractedDir.Ux * refractedDir.Ux +
                                    refractedDir.Uy * refractedDir.Uy +
                                    refractedDir.Uz * refractedDir.Uz) - 1 < 1e-6);
        }
        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct value.   Note that this
        /// gets called by Photon method CrossRegionOrReflect.  All return values
        /// from GetAngleRelativeToBoundaryNormal are positive to be used successfully by Photon.
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon
            {
                DP =
                {
                    Position = new Position(0, 0, 0), // put photon on voxel top
                    Direction = new Direction(0, 0, 1) // direction opposite surface normal
                }
            };
            var dirCosine = _oneLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
            photon.DP.Position = new Position(0, 0, 10); // put photon on voxel bottom
            photon.DP.Direction = new Direction(0, 0, 1); // direction in line with surface normal
            dirCosine = _oneLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
            photon.DP.Position = new Position(1, 0, 3); // put photon on right
            photon.DP.Direction = new Direction(0, 0, 1); // straight down 90 degrees to normal
            dirCosine = _oneLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(0, dirCosine);
            photon.DP.Position = new Position(-1, 0, 3); // put photon on left
            photon.DP.Direction = new Direction(-1, 0, 0); // in line with surface normal
            dirCosine = _oneLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
            photon.DP.Position = new Position(0, 1, 3); // put photon on front
            photon.DP.Direction = new Direction(0, -1, 0); // opposite surface normal
            dirCosine = _oneLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
        }


    }
}
