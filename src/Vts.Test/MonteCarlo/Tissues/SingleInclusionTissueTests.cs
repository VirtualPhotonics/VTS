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
        private SingleInclusionTissue _tissueWithEllipsoid, _tissueWithThinCylinder;
        /// <summary>
        /// Validate general constructor of Tissue
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _tissueWithEllipsoid = new SingleInclusionTissue(new EllipsoidTissueRegion(
                new Position(0, 0, 3), 1.0, 1.0, 2.0, new OpticalProperties()),
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
            _tissueWithThinCylinder = new SingleInclusionTissue(new CylinderTissueRegion(
                    new Position(0, 0, 0), 1.0, 0.0, new OpticalProperties()),
                new ITissueRegion[]
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
                });
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            var index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 0.5)); // outside ellipsoid
            Assert.AreEqual(1, index);
            index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 2.5)); // inside ellipsoid
            Assert.AreEqual( 3,index);
            index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 1.0)); // on ellipsoid is considered in
            Assert.AreEqual(3, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for tissueWithEllipsoid return correct Boolean
        /// </summary>
        [Test]
        public void Verify_tissueWithEllipsoid_GetNeighborRegionIndex_method_correct_when_photon_on_ellipsoid()
        {
            // Ellipsoid
            var photon = new Photon( // on top of ellipsoid pointed into it
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissueWithEllipsoid,
                1,
                new Random());
            var index = _tissueWithEllipsoid.GetNeighborRegionIndex(photon);
            Assert.AreEqual(3, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for tissueWithEllipsoid return correct Boolean
        /// </summary>
        [Test]
        public void Verify_tissueWithEllipsoid_GetNeighborRegionIndex_method_correct_when_photon_bottom_slab()
        {
            // on bottom of slab pointed out
            var photon = new Photon( // have to reinitialize photon so that _onBoundary is set to false
                new Position(0, 0, 100.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissueWithEllipsoid,
                1,
                new Random());
            var index = _tissueWithEllipsoid.GetNeighborRegionIndex(photon);
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
            var currentPosition = new Position(10, 10, 0);
            var currentDirection = new Direction(1/Math.Sqrt(2), 0, -1/Math.Sqrt(2));
            var reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(1/Math.Sqrt(2), reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(1/Math.Sqrt(2), reflectedDir.Uz); // reflection off layer just flips sign of Uz
            // index matched
            currentPosition = new Position(0, 0, 2); // put photon on ellipsoid
            currentDirection = new Direction(0, 0, 1);
            reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(0, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(1, reflectedDir.Uz);
            // index mismatched
            _tissueWithEllipsoid.Regions[3].RegionOP.N = 1.5; // surrounding layer has n=1.4
            currentPosition = new Position(0, 0, 2); // put photon on top of ellipsoid
            currentDirection = new Direction(0, 0, 1); // perpendicular to tangent surface
            reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(0, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(-1, reflectedDir.Uz);
            currentDirection = new Direction(1/Math.Sqrt(2), 0, 1/Math.Sqrt(2)); // 45 deg to tangent surface
            reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.IsTrue(Math.Abs(reflectedDir.Ux - 1/Math.Sqrt(2)) < 1e-7);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.IsTrue(Math.Abs(reflectedDir.Uz + 1/Math.Sqrt(2)) < 1e-7);
        }

        /// <summary>
        /// Validate method GetReflectedDirection returns correct direction.
        /// </summary>
        [Test]
        public void Verify_GetRefractedDirection_method_returns_correct_result()
        {
            // put photon on boundary of domain to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(10, 10, 0);
            var currentDirection = new Direction(1/Math.Sqrt(2), 0, -1/Math.Sqrt(2));
            const double nCurrent = 1.4;
            var nNext = 1.4; 
            var cosThetaSnell = 1/Math.Sqrt(2);
            var refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6);
            // put photon on ellipsoid: index matched
            currentPosition = new Position(0, 0, 2); 
            currentDirection = new Direction(1/Math.Sqrt(2), 0, 1/Math.Sqrt(2));
            nNext = 1.4;
            refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6);
            // put photon on ellipsoid: index mismatched
            currentPosition = new Position(0, 0, 2);
            currentDirection = new Direction(1 / Math.Sqrt(14), 2 / Math.Sqrt(14), 3 / Math.Sqrt(14));
            nNext = 1.5;
            refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 0.104257) < 1e-6);
            Assert.IsTrue(Math.Abs(refractedDir.Uy - 0.208514) < 1e-6);
            Assert.IsTrue(Math.Abs(refractedDir.Uz - 0.972446) < 1e-6);
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
                    Position = new Position(0, 0, 2), // put photon on ellipsoid top
                    Direction = new Direction(0, 0, 1) // direction opposite surface normal
                }
            };
            var dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
            photon.DP.Position = new Position(0, 0, 4); // put photon on ellipsoid bottom
            photon.DP.Direction = new Direction(0, 0, 1); // direction in line with surface normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
            photon.DP.Position = new Position(1, 0, 3); // put photon on right
            photon.DP.Direction = new Direction(0, 0, 1); // straight down 90 degrees to normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(0, dirCosine);
            photon.DP.Position = new Position(-1, 0, 3); // put photon on left
            photon.DP.Direction = new Direction(-1, 0, 0); // in line with surface normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1,dirCosine);
            photon.DP.Position = new Position(0, 1, 3); // put photon on front
            photon.DP.Direction = new Direction(0, -1, 0); // opposite surface normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, dirCosine);
        }
    }
}
