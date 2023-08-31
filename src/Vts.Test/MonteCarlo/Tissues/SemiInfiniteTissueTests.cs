using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// NOTE! this class is not finished so the unit tests are not as they should be
    /// </summary>
    [TestFixture]
    public class SemiInfiniteTissueTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOftestGeneratedFiles = new()
        {
            "SemiInfiniteTissue.txt"
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
        /// verify SemiInfiniteTissueInput deserializes correctly
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var i = new SemiInfiniteTissueInput(
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4))
            );

            var iCloned = i.Clone();

            Assert.IsTrue(Math.Abs(iCloned.Regions[0].RegionOP.Mus - i.Regions[0].RegionOP.Mus) < 1e-6);
        }
        /// <summary>
        /// verify SemiInfiniteTissueInput deserializes correctly when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_class_is_correct_when_using_FileIO()
        {
            var i = new SemiInfiniteTissueInput(
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4))
            );
            i.WriteToJson("SemiInfiniteTissue.txt");
            var iCloned = FileIO.ReadFromJson<SemiInfiniteTissueInput>("SemiInfiniteTissue.txt");

            Assert.IsTrue(Math.Abs(iCloned.Regions[0].RegionOP.Mus - i.Regions[0].RegionOP.Mus) < 1e-6);
        }
        /// <summary>
        /// CreateTissue is not written yet so this tests for exception
        /// </summary>
        [Test]
        public void Verify_CreateTissue_throws_exception()
        {
            var i = new SemiInfiniteTissueInput(
                new LayerTissueRegion(
                    new DoubleRange(0.0, 100.0),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4))
            );

            Assert.Throws<NotImplementedException>(() => 
                i.CreateTissue(
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    0.0));
        }
    }
}
