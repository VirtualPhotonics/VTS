﻿using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class SingleCylinderTissueTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "SingleCylinderTissue.txt"
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
            var i = new SingleCylinderTissueInput();
            var cylinder = i.CylinderRegion;
            var layers = i.LayerRegions;
            Assert.That(cylinder.Center.X, Is.EqualTo(0.0));
            Assert.That(cylinder.Center.Y, Is.EqualTo(0.0));
            Assert.That(cylinder.Center.Z, Is.EqualTo(3.0));
            Assert.That(layers[1].Center.Z, Is.EqualTo(50.0));
        }
        /// <summary>
        /// verify SingleCylinderTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new SingleCylinderTissueInput(
                new CylinderTissueRegion(
                    new Position(0, 0, 1), 
                    0.5, 
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

            Assert.That(i.Regions[1].RegionOP.Mua, Is.EqualTo(iCloned.Regions[1].RegionOP.Mua));
        }
        /// <summary>
        /// verify SingleCylinderTissueInput deserializes correctly when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new SingleCylinderTissueInput(
                new CylinderTissueRegion(
                    new Position(0, 0, 1), 
                    0.5, 
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
            i.WriteToJson("SingleCylinderTissue.txt");
            var iCloned = FileIO.ReadFromJson<SingleCylinderTissueInput>("SingleCylinderTissue.txt");

            Assert.That(i.Regions[1].RegionOP.Mua, Is.EqualTo(iCloned.Regions[1].RegionOP.Mua));
        }
        /// <summary>
        /// verify CreateTissue generates ITissue
        /// </summary>
        [Test]
        public void Validate_CreateTissue_creates_class()
        {
            var i = new SingleCylinderTissueInput(
                new CylinderTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
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

            Assert.That(i.CreateTissue(
                AbsorptionWeightingType.Continuous,
                PhaseFunctionType.HenyeyGreenstein,
                0.0), Is.InstanceOf<ITissue>());
        }
    }
}
