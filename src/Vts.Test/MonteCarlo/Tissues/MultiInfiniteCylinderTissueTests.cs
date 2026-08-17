using NUnit.Framework;
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
        /// <summary>
        /// List of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOftestGeneratedFiles = ["MultiLayerTissue.txt"];

        /// <summary>
        /// Clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
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
    }
}
