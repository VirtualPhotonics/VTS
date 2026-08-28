using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class MultiInfiniteCylinderTissueInputTests
    {
        private MultiInclusionTissue _oneLayerTissueMultiInfiniteCylinder,
            _twoLayerTissueMultiInfiniteCylinder;

        /// <summary>
        /// List of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOftestGeneratedFiles = ["MultiLayerTissue.txt"];

        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueMultiInfiniteCylinder =
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
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
            _twoLayerTissueMultiInfiniteCylinder =
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
                        new DoubleRange(3.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
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
        /// Test default constructor
        /// </summary>
        [Test]
        public void Validate_default_constructor()
        {
            var i = new MultiInfiniteCylinderTissueInput();
            var infiniteCylinders = i.InfiniteCylinderRegions;
            var layers = i.LayerRegions;
            Assert.That(infiniteCylinders[0].Center.X, Is.EqualTo(0.0));
            Assert.That(infiniteCylinders[0].Center.Y, Is.EqualTo(0.0));
            Assert.That(infiniteCylinders[0].Center.Z, Is.EqualTo(1.0)); 
            Assert.That(infiniteCylinders[1].Center.X, Is.EqualTo(0.0));
            Assert.That(infiniteCylinders[1].Center.Y, Is.EqualTo(0.0));
            Assert.That(infiniteCylinders[1].Center.Z, Is.EqualTo(5.0));
            Assert.That(layers[1].Center.Z, Is.EqualTo(25.0));
        }

        /// <summary>
        /// verify MultiInfiniteCylinderTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new MultiInfiniteCylinderTissueInput([
                    new InfiniteCylinderTissueRegion(new Position(0, 0, 1), 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new InfiniteCylinderTissueRegion(new Position(0, 0, 2), 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),

                ], [
                    new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]
            );

            var iCloned = i.Clone();

            Assert.That(i.InfiniteCylinderRegions[1].RegionOP.Mus, Is.EqualTo(iCloned.InfiniteCylinderRegions[1].RegionOP.Mus));
            Assert.That(i.Regions[1].RegionOP.Mus, Is.EqualTo(iCloned.Regions[1].RegionOP.Mus));
        }

        /// <summary>
        /// Verify MultiInfiniteCylinderTissueInput deserializes when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new MultiInfiniteCylinderTissueInput([
                    new InfiniteCylinderTissueRegion(new Position(0, 0, 1), 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new InfiniteCylinderTissueRegion(new Position(0, 1, 0),0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                ], [
                    new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]
            );
            i.WriteToJson("MultiLayerTissue.txt");
            var iCloned = FileIO.ReadFromJson<MultiInfiniteCylinderTissueInput>("MultiLayerTissue.txt");

            Assert.That(i.InfiniteCylinderRegions[1].RegionOP.Mus, Is.EqualTo(iCloned.InfiniteCylinderRegions[1].RegionOP.Mus));
            Assert.That(i.Regions[1].RegionOP.Mus, Is.EqualTo(iCloned.Regions[1].RegionOP.Mus));
        }

        /// <summary>
        /// Verify CreateTissue generates ITissue
        /// </summary>
        [Test]
        public void Verify_CreateTissue_creates_class()
        {
            var i = new MultiInfiniteCylinderTissueInput([
                new InfiniteCylinderTissueRegion(new Position(0, 0, 1), 0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                new InfiniteCylinderTissueRegion(new Position(0, 0, 2), 0.25,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4))

            ], [
                new LayerTissueRegion(
                    new DoubleRange(double.NegativeInfinity, 0.0),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                new LayerTissueRegion(
                    new DoubleRange(0.0, 100.0),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                new LayerTissueRegion(
                    new DoubleRange(100.0, double.PositiveInfinity),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
            ]);
            Assert.That(i.CreateTissue(
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    0.0), Is.InstanceOf<ITissue>());
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean.
        /// Order of tissue region indices: layers, bounding region, inclusions.
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            // one layer results indices: air(0)-tissue(1)-air(2)-top cylinder(3)-bot cylinder(4)
            var index = _oneLayerTissueMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 1.5)); // 1st layer 1st cylinder
            Assert.That(index, Is.EqualTo(3));
            index = _oneLayerTissueMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 5)); // 1st layer 2nd cylinder
            Assert.That(index, Is.EqualTo(4));
            // two layer results indices: air(0)-top layer(1)-bot layer(2)-air(3)-top cylinder(4)-bot cylinder(5)
            index = _twoLayerTissueMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 1.5)); // 1st layer cylinder
            Assert.That(index, Is.EqualTo(4));
            index = _twoLayerTissueMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 5)); // 2nd layer cylinder
            Assert.That(index, Is.EqualTo(5));
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
                _twoLayerTissueMultiInfiniteCylinder,
                1,
                new Random());
            var index = _twoLayerTissueMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
            photon = new Photon( // on side of bottom inclusion layer 2, pointing into it
                new Position(-1, 0, 5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueMultiInfiniteCylinder,
                2,
                new Random());
            index = _twoLayerTissueMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(5));
        }


    }
}
