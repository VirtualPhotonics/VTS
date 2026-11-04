using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class MultiInclusionTissueInputTests
    {
        private MultiInclusionTissue _oneLayerTissueMultiInclusion,
            _twoLayerTissueMultiInclusion, _threeLayerTissueMultiInclusion;

        /// <summary>
        /// List of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOftestGeneratedFiles = ["MultiLayerTissue.txt"];

        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueMultiInclusion =
                new MultiInclusionTissue(
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
                        new DoubleRange(0.0, 10.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
            _twoLayerTissueMultiInclusion =
                new MultiInclusionTissue(
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
                        new DoubleRange(3.0, 10.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
            // define a 3-layer tissue with 2 cylinders only in one layer
            _threeLayerTissueMultiInclusion =
                new MultiInclusionTissue(
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 4.5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(3, 0, 4.5),
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
                            new DoubleRange(3.0, 6.0),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(6.0, 10.0),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(10.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]);

        }

        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            foreach (var file in _listOftestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean.
        /// Order of tissue region indices: layers, bounding region, inclusions.
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            // one layer results indices: air(0)-tissue(1)-air(2)-top cylinder(3)-bot cylinder(4)
            var index = _oneLayerTissueMultiInclusion.GetRegionIndex(new Position(0, 0, 1.5)); // 1st layer 1st cylinder
            Assert.That(index, Is.EqualTo(3));
            index = _oneLayerTissueMultiInclusion.GetRegionIndex(new Position(0, 0, 5)); // 1st layer 2nd cylinder
            Assert.That(index, Is.EqualTo(4));
            // two layer results indices: air(0)-top layer(1)-bot layer(2)-air(3)-top cylinder(4)-bot cylinder(5)
            index = _twoLayerTissueMultiInclusion.GetRegionIndex(new Position(0, 0, 1.5)); // 1st layer cylinder
            Assert.That(index, Is.EqualTo(4));
            index = _twoLayerTissueMultiInclusion.GetRegionIndex(new Position(0, 0, 5)); // 2nd layer cylinder
            Assert.That(index, Is.EqualTo(5));
            // three layer results indices: air(0)-top layer(1)-mid layer(2)-bot layer(3)-air(4)-top cylinder(5)-bot cylinder(6)
            index = _threeLayerTissueMultiInclusion.GetRegionIndex(new Position(0, 0, 4.5)); // 1st cylinder
            Assert.That(index, Is.EqualTo(5));
            index = _threeLayerTissueMultiInclusion.GetRegionIndex(new Position(3, 0, 4.5)); // 2nd cylinder
            Assert.That(index, Is.EqualTo(6));
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            // check inclusions in two layer tissue
            var photon = new Photon( // on side of top inclusion layer 1, pointing into it
                new Position(-1, 0, 1.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueMultiInclusion,
                1,
                new Random());
            var index = _twoLayerTissueMultiInclusion.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
            photon = new Photon( // on side of bottom inclusion layer 2, pointing into it
                new Position(-1, 0, 5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueMultiInclusion,
                2,
                new Random());
            index = _twoLayerTissueMultiInclusion.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(5));
            // check inclusions in three layer tissue
            photon = new Photon( // on side of first inclusion, pointing into it
                new Position(-1, 0, 4.5),
                new Direction(1.0, 0, 0),
                1.0,
                _threeLayerTissueMultiInclusion,
                2,
                new Random());
            index = _threeLayerTissueMultiInclusion.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(5));
            photon = new Photon( // on side of 2nd inclusion pointing into it
                new Position(2, 0, 4.5),
                new Direction(1.0, 0, 0),
                1.0,
                _threeLayerTissueMultiInclusion,
                2,
                new Random());
            index = _threeLayerTissueMultiInclusion.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(6));
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
            var reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1 / Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1 / Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // put photon on boundary of domain (bottom surface) to make sure base (MultiLayerTissue) call works
            currentPosition = new Position(0, 0, 10);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1 / Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1 / Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // put photon on boundary of two tissue layers with refractive index mismatch
            currentPosition = new Position(0, 0, 3);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(
                currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(1 / Math.Sqrt(2)));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1 / Math.Sqrt(2))); // reflection off layer just flips sign of Uz
            // check reflection between outer cylinder and surrounding layer: concave up reflection
            // index matched perpendicular: instance of class defines layer and outer cylinder n=1.4
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1)); // no reflection
            // index mismatched perpendicular between tissue
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0; // outer infinite cylinder has n=1.4
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1)); // reflection
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no reflection
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True);  // reflection
            // check concave down reflection
            // index matched perpendicular
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 4.0); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(-1)); // no reflection
            // index mismatched perpendicular between tissue
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0; // outer infinite cylinder has n=1.4
            currentPosition = new Position(0, 0, 4.0); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(reflectedDir.Ux, Is.EqualTo(0));
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(reflectedDir.Uz, Is.EqualTo(1)); // reflection
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 4.0); // photon top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
            Assert.That(Math.Abs(reflectedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(reflectedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(reflectedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no reflection
            // index mismatched 45 deg to tangent surface
            // set n of surrounding region to 1.0
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 4.0); // photon on top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            reflectedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(currentPosition, currentDirection);
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
            var refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
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
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
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
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
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
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // refracted
            // put photon on boundary of two tissue layers with refractive index mismatch
            currentPosition = new Position(0, 0, 10);
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            refractedDir = _twoLayerTissueMultiInclusion.GetReflectedDirection(
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
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(currentDirection.Uz)); // no refraction
            // index matched 45 deg to tangent z-plane surface
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            currentN = 1.4;
            nextN = 1.4;
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.4; // make layer n=1.4
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // no refraction
            // index mismatched perpendicular between tissue  1.4 to 1.0
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0; // make layer n=1
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed into it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom layer surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(currentDirection.Uz)); // no refraction
            // index matched perpendicular
            currentN = 1.4;
            nextN = 1.4;
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(-1)); // no refraction
            // index matched 45 deg to tangent surface
            // set n of surrounding region to 1.4
            currentN = 1.4;
            nextN = 1.4;
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.4;
            currentPosition = new Position(0, 0, 0.5); // photon top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
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
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(0, 0, -1); // pointed out of it
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(refractedDir.Ux, Is.EqualTo(0));
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(refractedDir.Uz, Is.EqualTo(-1)); // refraction but no angle change since perpendicular

            // Case 1: index mismatched 45 deg to tangent top surface n=1.4 to n=1.0 sb equal to Case 1 above
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 0.141421) < 1e-6, Is.True); // refracted
            // Case 2: index mismatched 45 deg to tangent bottom surface 1.4 to 1.0 sb equal to Case 2 above
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0; // make layer n=1.4 and cyl n=1.0
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom cyl surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 0.989949) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 0.141421) < 1e-6, Is.True); // refracted
            // Case 3: index mismatched 45 deg to top tangent surface n=1.0 to n=1.4 sb equal to Case 3 above
            currentN = 1.0;
            nextN = 1.4;
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on top infinite cylinder
            currentDirection = new Direction(0.989949, 0, 0.141421);
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cylinder surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            // not sure of following
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz - 1 / Math.Sqrt(2)) < 1e-6, Is.True);  // refraction
            // Case 4: index mismatched 45 deg to tangent z-plane surface 1.0 to 1.4 sb equal to Case 4 above
            currentN = 1.0;
            nextN = 1.4;
            _twoLayerTissueMultiInclusion.Regions[2].RegionOP.N = 1.0; // make layer n=1
            currentPosition = new Position(0, 0, 6.0); // photon on bottom infinite cylinder
            currentDirection = new Direction(0.989949, 0, -0.141421);
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to bottom cyl surface is [0,0,1]
                currentDirection, new Direction(0, 0, 1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
                currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            Assert.That(Math.Abs(refractedDir.Ux - 1 / Math.Sqrt(2)) < 1e-6, Is.True);
            Assert.That(refractedDir.Uy, Is.EqualTo(0));
            Assert.That(Math.Abs(refractedDir.Uz + 1 / Math.Sqrt(2)) < 1e-6, Is.True); // refracted

            // finally test when outside critical angle and reflects instead of refracts
            // index mismatched >45 deg going from n=1.4 to n=1.0
            currentN = 1.4;
            nextN = 1.0;
            _twoLayerTissueMultiInclusion.Regions[1].RegionOP.N = 1.0;
            currentPosition = new Position(0, 0, 0.5); // photon on bottom infinite cylinder
            currentDirection = new Direction(0.894427, 0, -0.447213); // outside critical angle
            cosTheta = Math.Abs(Direction.GetDotProduct( // normal to top cyl surface is [0,0,-1]
                currentDirection, new Direction(0, 0, -1)));
            Optics.Fresnel(currentN, nextN, cosTheta, out cosThetaSnell);
            refractedDir = _twoLayerTissueMultiInclusion.GetRefractedDirection(
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
                _twoLayerTissueMultiInclusion,
                1,
                new Random());
            var cosTheta = _twoLayerTissueMultiInclusion.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            photon = new Photon( // on top of 2nd layer pointed into it
                new Position(-2, 0, 3.0),
                new Direction(0.0, 0, 1.0),
                1,
                _twoLayerTissueMultiInclusion,
                1,
                new Random());
            cosTheta = _twoLayerTissueMultiInclusion.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            // put on side of bottom infinite cylinder pointing in
            photon.DP.Position = new Position(-1.0, 0.0, 5.0);
            photon.DP.Direction = new Direction(1.0, 0.0, 0.0);
            photon.CurrentRegionIndex = 2;
            cosTheta = _twoLayerTissueMultiInclusion.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
        }

    }
}
