using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for MultiConcentricInclusionTissue
    /// </summary>
    [TestFixture]
    public class MultiConcentricInclusionTissueTests
    {
        private MultiConcentricInclusionTissue _tissue;
        /// <summary>
        /// Validate general constructor of Tissue
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _tissue = new MultiConcentricInclusionTissue(
                new ITissueRegion[]
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 3),
                        2.95,
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
            // the following cases sets S to 0 to make sure "projected" distance is calculated
            var photon = new Photon( // below outer infinite cylinder pointed into it
                new Position(0, 0, 7),
                new Direction(0.0, 0, -1.0),
                1.0,
                _tissue,
                1,
                new Random())
            {
                S = 0
            };
            var distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 1.05) < 1e-6);
            photon = new Photon(        // above inner infinite cylinder pointed into it
                new Position(0, 0, 0.1),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                3,
                new Random())
            {
                S = 0
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
                S = 0
            };
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 4.5) < 1e-6);
            photon = new Photon( // on bottom of slab pointed out
                new Position(0, 0, 95.0),
                new Direction(0.0, 0, 1.0),
                1.0,
                _tissue,
                1,
                new Random())
            {
                S = 0
            };
            distance = _tissue.GetDistanceToBoundary(photon);
            Assert.IsTrue(Math.Abs(distance - 5) < 1e-6);
        }

        /// <summary>
        /// Validate method GetReflectedDirection return correct Direction.  Note that Photon class
        /// determines whether in critical angle and if so, whether to reflect or refract.  This unit
        /// test just tests isolated method.
        /// </summary>
        [Test]
        public void Verify_GetReflectedDirection_method_returns_correct_result()
        {
            // check reflection between tissue layer and air
            // put photon on boundary of domain (top surface) to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(0, 0, 0);
            var currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            var reflectedDir = _tissue.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.AreEqual(1 / Math.Sqrt(2), reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(1 / Math.Sqrt(2), reflectedDir.Uz); // reflection off layer just flips sign of Uz
            // put photon on boundary of domain (bottom surface) to make sure base (MultiLayerTissue) call works
            currentPosition = new Position(0, 0, 100);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            reflectedDir = _tissue.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.AreEqual(1 / Math.Sqrt(2), reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(-1 / Math.Sqrt(2), reflectedDir.Uz); // reflection off layer just flips sign of Uz

            // check reflection between outer cylinder and surrounding layer: concave up reflection
            // index matched perpendicular: instance of class defines layer and outer cylinder n=1.4
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(0, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(-1, reflectedDir.Uz); // no reflection
            // index mismatched perpendicular between tissue
            _tissue.Regions[1].RegionOP.N = 1.0; // outer infinite cylinder has n=1.4
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(0, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(1, reflectedDir.Uz); // reflection
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            _tissue.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.IsTrue(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.IsTrue(Math.Abs(reflectedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6); // no reflection
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            _tissue.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.IsTrue(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.IsTrue(Math.Abs(reflectedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6);  // reflection
            // check concave down reflection
            // index matched perpendicular
            _tissue.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.05); // photon on top outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(0, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(-1, reflectedDir.Uz); // no reflection
            // index mismatched perpendicular between tissue
            _tissue.Regions[1].RegionOP.N = 1.0; // outer infinite cylinder has n=1.4
            currentPosition = new Position(0, 0, 0.05); // photon on top outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.AreEqual(0, reflectedDir.Ux);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.AreEqual(1, reflectedDir.Uz); // reflection
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            _tissue.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.05); // photon top outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.IsTrue(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.IsTrue(Math.Abs(reflectedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6); // no reflection
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            _tissue.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.05); // photon on top outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _tissue.GetReflectedDirection(currentPosition, currentDirection);
            Assert.IsTrue(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, reflectedDir.Uy);
            Assert.IsTrue(Math.Abs(reflectedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6);  // reflection
        }

        /// <summary>
        /// Validate method GetReflectedDirection returns correct direction.
        /// </summary>
        [Test]
        public void Verify_GetRefractedDirection_method_returns_correct_result()
        {
            // check refraction between tissue layer and air
            // put photon on boundary of domain (top surface) to make sure base (MultiLayerTissue) call works
            var currentPosition = new Position(0, 0, 0);
            var currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            var currentN = 1.4;
            var nextN = 1.0;
            // cosThetaSnell obtained using cosTheta = angle with normal and Fresnel
            var cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top layer surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out var cosThetaSnell);
            var refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz + 0.141421) < 1e-6); // refracted
            // put photon on boundary of domain (bottom surface) to make sure base (MultiLayerTissue) call works
            currentPosition = new Position(0, 0, 100);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz - 0.141421) < 1e-6); // refracted

            // check refraction between outer cylinder and surrounding layer: concave up refraction
            // index matched perpendicular: instance of class defines layer and outer cylinder n=1.4
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            currentN = 1.4;
            nextN = 1.4;
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.AreEqual(0, refractedDir.Ux);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.AreEqual(currentDirection.Uz, refractedDir.Uz); // no refraction
            // index mismatched perpendicular between tissue 
            currentN = 1.4;
            nextN = 1.0;
            _tissue.Regions[1].RegionOP.N = 1.0; // make layer n=1
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.AreEqual(0, refractedDir.Ux);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.AreEqual(currentDirection.Uz, refractedDir.Uz); // no refraction
            // index matched 45 deg to tangent z-plane surface
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            currentN = 1.4;
            nextN = 1.4;
            _tissue.Regions[1].RegionOP.N = 1.4; // make layer n=1.4
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell); 
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6); // no refraction
            // index mismatched 45 deg to tangent z-plane surface
            currentN = 1.4;
            nextN = 1.0;
            _tissue.Regions[1].RegionOP.N = 1.0; // make layer n=1
            currentPosition = new Position(0, 0, 5.95); // photon on bottom outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom cyl surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz + 0.141421) < 1e-6); // refracted
            // check concave down refraction
            // index matched perpendicular
            currentN = 1.4;
            nextN = 1.4;
            _tissue.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.05); // photon on top outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.AreEqual(0, refractedDir.Ux);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.AreEqual(-1, refractedDir.Uz); // no refraction
            // index mismatched perpendicular between tissue
            currentN = 1.4;
            nextN = 1.0;
            _tissue.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.05); // photon on top outer infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.AreEqual(0, refractedDir.Ux);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.AreEqual(-1, refractedDir.Uz); // refraction but no angle change
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            currentN = 1.4;
            nextN = 1.4;
            _tissue.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.05); // photon top outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.IsTrue(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6);
            Assert.AreEqual(0, refractedDir.Uy);
            Assert.IsTrue(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6); // no refraction
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            currentN = 1.4;
            nextN = 1.0;
            _tissue.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.05); // photon on top outer infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _tissue.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            //// not sure of following
            //Assert.IsTrue(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6);
            //Assert.AreEqual(0, refractedDir.Uy);
            //Assert.IsTrue(Math.Abs(refractedDir.Uz + 0.141421) < 1e-6);  // refraction
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
