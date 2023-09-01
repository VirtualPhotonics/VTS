using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class MultiEllipsoidTissueInputTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOftestGeneratedFiles = new()
        {
            "MultiLayerTissue.txt"
        };

        /// <summary>
        /// clear all generated folders and files
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
        /// test default constructor
        /// </summary>
        [Test]
        public void Validate_default_constructor()
        {
            var i = new MultiEllipsoidTissueInput();
            var ellipsoids = i.EllipsoidRegions;
            var layers = i.LayerRegions;
            Assert.AreEqual(10.0, ellipsoids[0].Center.X);
            Assert.AreEqual(0.0, ellipsoids[0].Center.Y);
            Assert.AreEqual(10.0, ellipsoids[0].Center.Z); 
            Assert.AreEqual(0.0, ellipsoids[1].Center.X);
            Assert.AreEqual(0.0, ellipsoids[1].Center.Y);
            Assert.AreEqual(40.0, ellipsoids[1].Center.Z);
            Assert.AreEqual(25.0, layers[1].Center.Z);
        }
        /// <summary>
        /// verify MultiEllipsoidTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new MultiEllipsoidTissueInput(new ITissueRegion[]
                    {
                        new EllipsoidTissueRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new EllipsoidTissueRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                    }, new ITissueRegion[]
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

            Assert.AreEqual(iCloned.EllipsoidRegions[1].RegionOP.Mus, i.EllipsoidRegions[1].RegionOP.Mus);
            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }
        /// <summary>
        /// verify MultiEllipsoidTissueInput deserializes when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new MultiEllipsoidTissueInput(new ITissueRegion[] 
                    {
                        new EllipsoidTissueRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                        new EllipsoidTissueRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))

                    }, new ITissueRegion[]
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
            i.WriteToJson("MultiLayerTissue.txt");
            var iCloned = FileIO.ReadFromJson<MultiEllipsoidTissueInput>("MultiLayerTissue.txt");

            Assert.AreEqual(iCloned.EllipsoidRegions[1].RegionOP.Mus, i.EllipsoidRegions[1].RegionOP.Mus);
            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mus, i.Regions[1].RegionOP.Mus);
        }
        /// <summary>
        /// CreateTissue not implemented yet
        /// </summary>
        [Test]
        public void Verify_CreateTissue_throws_exception()
        {
            var i = new MultiEllipsoidTissueInput(new ITissueRegion[]
            {
                new EllipsoidTissueRegion(new Position(0, 0, 1), 0.5, 0.5, 0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                new EllipsoidTissueRegion(new Position(0, 1, 0), 0.25, 0.25, 0.25,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4))

            }, new ITissueRegion[]
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
            });
            Assert.Throws<NotImplementedException>(() =>
                i.CreateTissue(
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    0.0));
        }
    }
}
