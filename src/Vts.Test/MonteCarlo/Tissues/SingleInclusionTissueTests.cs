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
        private SingleInclusionTissue _tissueWithEllipsoid, _tissueWithInfiniteCylinder;
        /// <summary>
        /// Validate general constructor of Tissue
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _tissueWithEllipsoid = new SingleInclusionTissue(new EllipsoidTissueRegion(
                new Position(0, 0, 3), 1.0, 1.0, 2.0, 
                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
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
            // set up similar to Vts issue #202
            _tissueWithInfiniteCylinder = new SingleInclusionTissue(new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1), 0.75, 
                    new OpticalProperties(0.1, 1.0, 0.8, 1.4)),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    // put air around cylinder
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 2.0),
                        new OpticalProperties(1e-10, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(2.0, double.PositiveInfinity),
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
            Assert.That(index, Is.EqualTo(1));
            index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 2.5)); // inside ellipsoid
            Assert.That(index, Is.EqualTo( 3));
            index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 1.0)); // on ellipsoid is considered in
            Assert.That(index, Is.EqualTo(3));
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
            Assert.That(index, Is.EqualTo(3));
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
            Assert.That(index, Is.EqualTo(2));
        }

        /// <summary>
        /// Validate method GetReflectedDirection return correct Direction.  Note that Photon class
        /// determines whether in critical angle and if so, whether to reflect or refract.  This unit
        /// test just tests isolated method.
        /// </summary>
        [Test]
        public void Verify_tissueWithEllipsoid_GetReflectedDirection_method_returns_correct_result()
        {
            // put photon on boundary of domain to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(10, 10, 0);
            var currentDirection = new Direction(1/Math.Sqrt(2), 0, -1/Math.Sqrt(2));
            var reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1/Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1/Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // index matched
            currentPosition = new Position(0, 0, 2); // put photon on ellipsoid
            currentDirection = new Direction(0, 0, 1);
            reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1));
            // index mismatched
            _tissueWithEllipsoid.Regions[3].RegionOP.N = 1.5; // surrounding layer has n=1.4
            currentPosition = new Position(0, 0, 2); // put photon on top of ellipsoid
            currentDirection = new Direction(0, 0, 1); // perpendicular to tangent surface
            reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1));
            currentDirection = new Direction(1/Math.Sqrt(2), 0, 1/Math.Sqrt(2)); // 45 deg to tangent surface
            reflectedDir = _tissueWithEllipsoid.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1/Math.Sqrt(2)) < 1e-7, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz + 1/Math.Sqrt(2)) < 1e-7, Is.True);
        }

        /// <summary>
        /// Validate method GetRefractedDirection returns correct direction.
        /// </summary>
        [Test]
        public void Verify_tissueWithEllipsoid_GetRefractedDirection_method_returns_correct_result()
        {
            // put photon on boundary of domain to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(10, 10, 0);
            var currentDirection = new Direction(1/Math.Sqrt(2), 0, -1/Math.Sqrt(2));
            var nCurrent = 1.4;
            var nNext = 1.4; 
            var cosThetaSnell = 1/Math.Sqrt(2);
            var refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            // put photon on ellipsoid: index matched
            currentPosition = new Position(0, 0, 2); 
            currentDirection = new Direction(1/Math.Sqrt(2), 0, 1/Math.Sqrt(2));
            nNext = 1.4;
            refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            // put photon on ellipsoid: index mismatched at 45 deg angle
            currentPosition = new Position(0, 0, 2);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0.0, 1 / Math.Sqrt(2));
            nCurrent = 1.0;
            nNext = 1.5;
            refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            // validation numbers determined using snell's law and direction cosines
            // refracted direction is 28.1255 deg
            Assert.That(Math.Abs(refractedDir.Ux - 0.471404) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 0.881917) < 1e-6, Is.True);
            Assert.That(Math.Sqrt(refractedDir.Ux * refractedDir.Ux +
                                  refractedDir.Uy * refractedDir.Uy +
                                  refractedDir.Uz * refractedDir.Uz) - 1, Is.LessThan(1e-6));
            // put photon on ellipsoid: index mismatched to verify Uy!=0, based on prior run
            currentPosition = new Position(0, 0, 2);
            // actual direction cosines below ( 0.267261, 0.534522, 0.801783)
            currentDirection = new Direction(1 / Math.Sqrt(14), 2 / Math.Sqrt(14), 3 / Math.Sqrt(14));
            nCurrent = 1.4;
            nNext = 1.5;
            refractedDir = _tissueWithEllipsoid.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.249443) < 1e-6, Is.True);
            Assert.That(Math.Abs(refractedDir.Uy - 0.498887) < 1e-6, Is.True);
            Assert.That(Math.Abs(refractedDir.Uz - 0.829993) < 1e-6, Is.True);
            Assert.That(Math.Sqrt(refractedDir.Ux * refractedDir.Ux +
                                    refractedDir.Uy * refractedDir.Uy +
                                    refractedDir.Uz * refractedDir.Uz) - 1, Is.LessThan(1e-6));
        }

        /// <summary>
        /// Validate method GetRefractedDirection returns correct direction.
        /// Trials that test cylinder in air similar to Vts issue #202
        /// </summary>
        [Test]
        public void Verify_tissueWithCylinder_GetRefractedDirection_method_returns_correct_result()
        {
            // change n of cylinder to n=1+epsilon and check little change to refracted direction
            const double nCurrent = 1.0;
            var nNext = 1.00000001;
            var cosThetaSnell = 1 / Math.Sqrt(2); // this value is not used but needs to be passed in
            var currentPosition = new Position(-0.033001, -0.021780, 0.032651);
            var currentDirection = new Direction(0, 0, 1);
            var refractedDir = _tissueWithInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.0) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 1.0) < 1e-6, Is.True);
            // now change n of cylinder to be n=1.2 and check large change to refracted direction
            // nCurrent = 1.0;
            nNext = 1.2;
            refractedDir = _tissueWithInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.0) > 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0)); // no change to Uy in this system
            Assert.That(Math.Abs(refractedDir.Uz - 1.0) > 1e-6, Is.True);
            // put source below cylinder
            // change n of cylinder to n=1+epsilon and check little change to refracted direction
            nNext = 1.00000001;
            currentPosition = new Position(-0.033001, -0.021780, 2 - 0.032651);
            currentDirection = new Direction(0, 0, -1);
            refractedDir = _tissueWithInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.0) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1.0) < 1e-6, Is.True);
            nNext = 1.2;
            refractedDir = _tissueWithInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.0) > 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0)); // no change to Uy in this system
            Assert.That(Math.Abs(refractedDir.Uz - 1.0) > 1e-6, Is.True);
            // put source to side of cylinder
            // change n of cylinder to n=1+epsilon and check little change to refracted direction
            nNext = 1.00000001;
            currentPosition = new Position(-0.033001, -0.021780, 0.032651);
            currentDirection = new Direction(-1, 0, 0);
            refractedDir = _tissueWithInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux + 1.0) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 0.0) < 1e-6, Is.True);
            nNext = 1.2;
            refractedDir = _tissueWithInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux + 1.0) > 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0)); // no change to Uy in this system
            Assert.That(Math.Abs(refractedDir.Uz + 0.0) > 1e-6, Is.True);
        }

        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct value.   Note that this
        /// gets called by Photon method CrossRegionOrReflect.  All return values
        /// from GetAngleRelativeToBoundaryNormal are positive to be used successfully by Photon.
        /// </summary>
        [Test]
        public void Verify_tissueWithEllipsoid_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
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
            Assert.That(dirCosine, Is.EqualTo(1));
            photon.DP.Position = new Position(0, 0, 4); // put photon on ellipsoid bottom
            photon.DP.Direction = new Direction(0, 0, 1); // direction in line with surface normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(dirCosine, Is.EqualTo(1));
            photon.DP.Position = new Position(1, 0, 3); // put photon on right
            photon.DP.Direction = new Direction(0, 0, 1); // straight down 90 degrees to normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(dirCosine, Is.EqualTo(0));
            photon.DP.Position = new Position(-1, 0, 3); // put photon on left
            photon.DP.Direction = new Direction(-1, 0, 0); // in line with surface normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(dirCosine, Is.EqualTo(1));
            photon.DP.Position = new Position(0, 1, 3); // put photon on front
            photon.DP.Direction = new Direction(0, -1, 0); // opposite surface normal
            dirCosine = _tissueWithEllipsoid.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(dirCosine, Is.EqualTo(1));
        }
    }
}
