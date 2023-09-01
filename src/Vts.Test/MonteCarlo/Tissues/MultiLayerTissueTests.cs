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
    public class MultiLayerTissueTests
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
        /// verify MultiLayerTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var tissueInput = new MultiLayerTissueInput(new ITissueRegion[]
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

            var iCloned = tissueInput.Clone();

            Assert.AreEqual(tissueInput.Regions[1].RegionOP.Mus, iCloned.Regions[1].RegionOP.Mus);
        }
        /// <summary>
        /// verify MultiLayerTissueInput deserializes correctly when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var tissueInput = new MultiLayerTissueInput(new ITissueRegion[]
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
            tissueInput.WriteToJson("MultiLayerTissue.txt");
            var iCloned = FileIO.ReadFromJson<MultiLayerTissueInput>("MultiLayerTissue.txt");

            Assert.AreEqual(tissueInput.Regions[1].RegionOP.Mus, iCloned.Regions[1].RegionOP.Mus);
        }
        /// <summary>
        /// verify exception thrown when GetNeighborIndex is called and the photon
        /// Direction Uz=0 (parallel with any layer)
        /// </summary>
        [Test]
        public void Verify_exception_when_calling_GetNeighborIndex_with_Uz_equal_0()
        {
            var tissue = new MultiLayerTissue();
            var photon = new Photon(
                new Position(0, 0, 0),
                new Direction(0, 0, 0), // specify Uz=0.0
                1,
                tissue,
                1,
                null);
            Assert.Throws<ArgumentException>(() => tissue.GetNeighborRegionIndex(photon));
        }
    }
}
