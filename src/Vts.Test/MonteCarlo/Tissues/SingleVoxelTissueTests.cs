using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class SingleVoxelTissueTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "SingleVoxelTissue.txt"
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
        /// verify SingleVoxelTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new SingleVoxelTissueInput(
                new VoxelTissueRegion(
                    new DoubleRange(-1, 1), 
                    new DoubleRange(-1, 1),
                    new DoubleRange(1, 2),
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
        /// verify SingleVoxelTissueInput deserializes correctly when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new SingleVoxelTissueInput(
                new VoxelTissueRegion(
                    new DoubleRange(-1, 1),
                    new DoubleRange(-1, 1),
                    new DoubleRange(1, 2),
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
            i.WriteToJson("SingleVoxelTissue.txt");
            var iCloned = FileIO.ReadFromJson<SingleVoxelTissueInput>("SingleVoxelTissue.txt");

            Assert.AreEqual(iCloned.Regions[1].RegionOP.Mua, i.Regions[1].RegionOP.Mua);
        }
    }
}
