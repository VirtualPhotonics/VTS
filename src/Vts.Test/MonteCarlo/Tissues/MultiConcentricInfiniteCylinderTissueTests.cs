using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.IO;
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
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "MultiConcentricInfiniteCylinderTissue.txt"
        };

        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// test default constructor
        /// </summary>
        [Test]
        public void Validate_default_constructor()
        {
            var i = new MultiConcentricInfiniteCylinderTissueInput();
            var cylinder1 = i.Regions[^2];
            var cylinder2 = i.Regions[^1];
            var tissueLayer = i.Regions[1];
            Assert.AreEqual(0.0, cylinder1.Center.X);
            Assert.AreEqual(0.0, cylinder1.Center.Y);
            Assert.AreEqual(1.0, cylinder1.Center.Z);
            Assert.AreEqual(0.0, cylinder2.Center.X);
            Assert.AreEqual(0.0, cylinder2.Center.Y);
            Assert.AreEqual(1.0, cylinder2.Center.Z);
            Assert.AreEqual(50.0, tissueLayer.Center.Z);
        }
        /// <summary>
        /// verify MultiConcentricInfiniteCylinderTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new MultiConcentricInfiniteCylinderTissueInput(
                new ITissueRegion[]
                {
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.75,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                },
                new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );

            var iCloned = i.Clone();

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mua, i.Regions[1].RegionOP.Mua);
        }
        /// <summary>
        /// verify MultiConcentricInfiniteCylinderTissueInput deserializes correctly when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new MultiConcentricInfiniteCylinderTissueInput(
                new ITissueRegion[]
                {
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.75,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                },
                new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );
            i.WriteToJson("MultiConcentricInfiniteCylinderTissue.txt");
            var iCloned = FileIO.ReadFromJson<MultiConcentricInfiniteCylinderTissueInput>("MultiConcentricInfiniteCylinderTissue.txt");

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mua, i.Regions[1].RegionOP.Mua);
        }
        /// <summary>
        /// verify CreateTissue generates ITissue
        /// </summary>
        [Test]
        public void Validate_CreateTissue_creates_class()
        {
            var i = new MultiConcentricInfiniteCylinderTissueInput(
                new ITissueRegion[]
                {
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.75,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                },
                new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                );

            Assert.IsInstanceOf<ITissue>(i.CreateTissue(
                AbsorptionWeightingType.Continuous,
                PhaseFunctionType.HenyeyGreenstein,
                0.0));
        }
    }
}

