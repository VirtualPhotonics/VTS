using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class SingleInfiniteCylinderTissueTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "SingleInfiniteCylinderTissue.txt"
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
            var i = new SingleInfiniteCylinderTissueInput();
            var cylinder = i.InfiniteCylinderRegion;
            var layers = i.LayerRegions;
            Assert.AreEqual(0.0, cylinder.Center.X);
            Assert.AreEqual(0.0, cylinder.Center.Y);
            Assert.AreEqual(1.0, cylinder.Center.Z);
            Assert.AreEqual(50.0, layers[1].Center.Z);
        }

        /// <summary>
        /// verify SingleInfiniteCylinderTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new SingleInfiniteCylinderTissueInput(
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1), 
                    0.5, 
                new OpticalProperties(0.05, 1.0, 0.8, 1.4)), 
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
        /// verify SingleInfiniteCylinderTissueInput deserializes correctly when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new SingleInfiniteCylinderTissueInput(
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1), 
                    0.5, 
                new OpticalProperties(0.05, 1.0, 0.8, 1.4)), 
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
            i.WriteToJson("SingleInfiniteCylinderTissue.txt");
            var iCloned = FileIO.ReadFromJson<SingleInfiniteCylinderTissueInput>("SingleInfiniteCylinderTissue.txt");

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mua, i.Regions[1].RegionOP.Mua);
        }

        /// <summary>
        /// verify CreateTissue generates ITissue
        /// </summary>
        [Test]
        public void Validate_CreateTissue_creates_class()
        {
            var i = new SingleInfiniteCylinderTissueInput(
                new InfiniteCylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
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
