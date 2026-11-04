using NUnit.Framework;
using System;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for BoundedMultiInclusionTissue 
    /// </summary>
    [TestFixture]
    public class BoundedMultiInclusionTissueTests
    {
        private BoundedMultiInclusionTissue _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder, 
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue voxel
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder = 
                new BoundedMultiInclusionTissue(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-2, 2, 2), // x range
                        new DoubleRange(-2, 2, 2), // y range
                        new DoubleRange(0, 10.0, 2),  // z range spans tissue
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)), 
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1.5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                            ],
                    [
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder = 
                new BoundedMultiInclusionTissue(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-2, 2, 2), // x range
                        new DoubleRange(-2, 2, 2), // y range
                        new DoubleRange(0, 100.0, 2),  // z range spans tissue
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1.5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                    ],
                [
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 3.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(3.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean.
        /// Order of tissue region indices: layers, bounding region, inclusions.
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            // one layer results indices: air(0)-tissue(1)-air(2)-top cylinder(3)-bot cylinder(4)-voxel(5)
            // 1st layer 1st cylinder
            var index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 1.5)); 
            Assert.That(index, Is.EqualTo(3));
            // 1st layer 2nd cylinder
            index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 5)); 
            Assert.That(index, Is.EqualTo(4));
            // on voxel considered in
            index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 0)); 
            Assert.That(index, Is.EqualTo(1));
            // two layer results indices: air(0)-top layer(1)-bot layer(2)-air(3)-top cylinder(4)-bot cylinder(5)-voxel(6)
            // 1st layer cylinder
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 1.5)); 
            Assert.That(index, Is.EqualTo(4));
            // 2nd layer cylinder
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 5)); 
            Assert.That(index, Is.EqualTo(5));
            // outside voxel
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(10, 0, 0)); 
            Assert.That(index, Is.EqualTo(6));
            // inside voxel top layer 1st cylinder
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 2.5)); 
            Assert.That(index, Is.EqualTo(4));
            // on voxel is considered in
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 0)); 
            Assert.That(index, Is.EqualTo(1));
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            // check one layer results
            var photon = new Photon( // on side of voxel pointed into it
                new Position(-2, 0, 1),
                new Direction(1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                5,
                new Random());
            var index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon); 
            Assert.That(index, Is.EqualTo(1));
            photon = new Photon( // on side of voxel pointed out of it
                new Position(-2, 0, 1),
                new Direction(-1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(5));
            // check two layer results
            photon = new Photon( // on side of voxel pointed into LAYER 1
                new Position(2, 0, 0.5),  
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                6,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(1));
            photon = new Photon( // on side of voxel in LAYER 1 pointed out of it
                new Position(2, 0, 0.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(6));
            photon = new Photon( // on side of voxel pointed into LAYER 2
                new Position(-2, 0, 3.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                6,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(2));
            photon = new Photon( // on side of voxel in LAYER 2 pointed out of it
                new Position(-2, 0, 3.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(6));
            // check inclusions in two layer tissue
            photon = new Photon( // on side of top inclusion layer 1, pointing into it
                new Position(-1, 0, 1.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
            photon = new Photon( // on side of bottom inclusion layer 2, pointing into it
                new Position(-1, 0, 5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(5));
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
            var reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1 / Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1 / Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // put photon on boundary of domain (bottom surface) to make sure base (MultiLayerTissue) call works
            currentPosition = new Position(0, 0, 100);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1 / Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1 / Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // put photon on boundary of two tissue layers with refractive index mismatch
            currentPosition = new Position(0, 0, 3);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1 / Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1 / Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // check reflection between outer cylinder and surrounding layer: concave up reflection
            // index matched perpendicular: instance of class defines layer and outer cylinder n=1.4
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1)); // no reflection
            // index mismatched perpendicular between tissue
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0; // outer infinite cylinder has n=1.4
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1)); // reflection
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no reflection
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True);  // reflection
            // check concave down reflection
            // index matched perpendicular
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 4.0); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1)); // no reflection
            // index mismatched perpendicular between tissue
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0; // outer infinite cylinder has n=1.4
            currentPosition = new Position(0, 0, 4.0); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1)); // reflection
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 4.0); // photon top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no reflection
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 4.0); // photon on top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True);  // reflection
        }

        /// <summary>
        /// Validate method GetRefractedDirection returns correct direction.  These tests first test
        /// the surrounding boundary layer n=1.4 mismatch with air n=1 and determines refracted directions
        /// Case 1: exiting top at 45 deg angle
        /// Case 2: exiting bottom at 45 deg angle
        /// Case 3: entering top at exiting angle of Case 1 (opposite direction)
        /// Case 4: entering bottom at exiting angle of Case 2 (opposite direction)
        /// These cases validation values are used when testing refraction out of/into cylinder when
        /// tangent is z=constant plane
        /// </summary>
        [Test]
        public void Verify_GetRefractedDirection_method_returns_correct_result()
        {
            // check refraction between tissue layer and air
            // put photon on boundary of domain (top layer surface) to make sure base (MultiLayerTissue)
            // call works

            // Case 1: index mismatched 45 deg to top z-plane surface 1.4 to 1.0 
            var currentPosition = new Position(0, 0, 0);
            var currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            var currentN = 1.4;
            var nextN = 1.0; // 1.4 to 1.0
            var cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top layer surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out var cosThetaSnell);
            var refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 0.141421) < 1e-6, Is.True); // refracted
            // put photon on boundary of domain (bottom surface) to make sure base call works
            // Case 2: index mismatched 45 deg to tangent z-plane surface still 1.4 to 1.0
            currentPosition = new Position(0, 0, 100);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 0.141421) < 1e-6, Is.True); // refracted
            // now test opposite direction from n=1.0 to n=1.4
            // Case 3: index mismatched 45 deg into top layer surface 
            currentPosition = new Position(0, 0, 0);
            currentDirection = new Direction(0.989949, 0, 0.141421);
            currentN = 1.0;
            nextN = 1.4; // n=1.0 to n=1.4
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top layer surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True); // refracted
            // Case 4: index mismatched 45 deg into bottom layer surface 1.0 to 1.4
            currentPosition = new Position(0, 0, 100);
            currentDirection = new Direction(0.989949, 0, -0.141421);
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // refracted
            // put photon on boundary of two tissue layers with refractive index mismatch
            currentPosition = new Position(0, 0, 3.0);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // refracted

            // check cases where NO refraction should occur between cylinder and surrounding layer
            // index matched perpendicular: instance of class defines layer and cylinder n=1.4
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            currentN = 1.4;
            nextN = 1.4;
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(currentDirection.Uz)); // no refraction
            // index matched 45 deg to tangent z-plane surface
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            currentN = 1.4;
            nextN = 1.4;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.4; // make layer n=1.4
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no refraction
            // index mismatched perpendicular between tissue  1.4 to 1.0
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0; // make layer n=1
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(currentDirection.Uz)); // no refraction
            // index matched perpendicular
            currentN = 1.4;
            nextN = 1.4;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(-1)); // no refraction
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            currentN = 1.4;
            nextN = 1.4;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.5); // photon top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no refraction

            // check cases where WITH refraction should occur between cylinder and surrounding layer
            // results should be the same as results above for refraction through plane since tangent
            // to cylinder at topmost or bottommost position is z=constant plane
            // index mismatched perpendicular between tissue n=1.4 to n=1.0
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(-1)); // refraction but no angle change since perpendicular

            // Case 1: index mismatched 45 deg to tangent top surface n=1.4 to n=1.0 sb equal to Case 1 above
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 0.141421) < 1e-6, Is.True); // refracted
            // Case 2: index mismatched 45 deg to tangent bottom surface 1.4 to 1.0 sb equal to Case 2 above
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0; // make layer n=1.4 and cyl n=1.0
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom cyl surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 0.141421) < 1e-6, Is.True); // refracted
            // Case 3: index mismatched 45 deg to top tangent surface n=1.0 to n=1.4 sb equal to Case 3 above
            currentN = 1.0;
            nextN = 1.4;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(0.989949, 0, 0.141421);
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            // not sure of following
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True);  // refraction
            // Case 4: index mismatched 45 deg to tangent z-plane surface 1.0 to 1.4 sb equal to Case 4 above
            currentN = 1.0;
            nextN = 1.4;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[2].RegionOP.N = 1.0; // make layer n=1
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0.989949, 0, -0.141421);
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom cyl surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // refracted

            // finally test when outside critical angle and reflects instead of refracts
            // index mismatched >45 deg going from n=1.4 to n=1.0
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on bottom infinite cylinder
            currentDirection = new Direction(0.894427, 0, -0.447213); // outside critical angle
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cyl surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.894427) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 0.447213) < 1e-6, Is.True); // refracted
        }



        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct Boolean.
        /// Boundaries are considered to be top and bottom of tissue and bounding.
        /// Note: Math.Abs taken in method to ensure that the angle is always positive,
        /// so Assert check is always positive.
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon( // on top of tissue pointed into it
                new Position(0, 0, 0.0),
                new Direction(0.0, 0, 1.0),
                1,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                0,
                new Random());
            var cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            photon = new Photon( // on top of 2nd layer pointed into it, inside voxel
                new Position(-1, 0, 3.0),
                new Direction(0.0, 0, 1.0),
                1,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            // put on side of bottom infinite cylinder pointing in
            photon.DP.Position = new Position(-1.0, 0.0, 5.0);
            photon.DP.Direction = new Direction(1.0, 0.0, 0.0);
            photon.CurrentRegionIndex = 2;
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            photon = new Photon( // on top of tissue pointed into it
                new Position(0, 0, 0.0),
                new Direction(0.0, 0, 1.0),
                1,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            photon = new Photon( // on bounding voxel pointed into it from top layer
                new Position(-2.0, 0, 1.0), // add a bit so not right on boundary
                new Direction(-1.0, 0, 0.0),
                1,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            // put on side of bottom infinite cylinder pointing in
            photon.DP.Position = new Position(-1.0, 0.0, 5.0);
            photon.DP.Direction = new Direction(1.0, 0.0, 0.0);
            photon.CurrentRegionIndex = 2;
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
        }


    }
}
