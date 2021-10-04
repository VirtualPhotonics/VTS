using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class PostProcessorInputTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "results"
        };
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "test",
            "test.txt"
        };

        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }
        /// <summary>
        /// validate deserialization of PostProcessorInput
        /// </summary>
        [Test]
        public void Validate_deserialized_PostProcessorInput_is_correct()
        {
            var i = new PostProcessorInput { OutputName = "results" };

            var iCloned = i.Clone();

            Assert.AreEqual("results",iCloned.OutputName);
        }
        /// <summary>
        ///  validate deserialization of PostProcessorInput when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_Simulation_is_correct_when_using_FileIO()
        {
            new PostProcessorInput { DatabaseSimulationInputFilename = "results" }.ToFile("test.txt");
            var iCloned = PostProcessorInput.FromFile("test.txt");

            Assert.AreEqual("results",iCloned.DatabaseSimulationInputFilename);
        }

        /// <summary>
        /// validate detector input gets serialized correctly
        /// </summary>
        [Test]
        public void Validate_detector_input_class_can_be_serialized_as_part_of_PostProcessorInput()
        {
            var detectorInput = new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };

            try
            {
                new PostProcessorInput(
                    new List<IDetectorInput>
                    {
                         detectorInput
                    },
                    "",
                    "",
                    "test"
                ).WriteToJson("test.txt");
            }
            catch (Exception)
            {
                Assert.Fail("PostProcessorInput class could not be serialized.");
            }
        }

        /// <summary>
        /// check that deserialized pMC detector input is correct
        /// </summary>
        [Test]
        public void Validate_deserialized_pmc_detector_input_class_is_correct()
        {
            var i = new pMCROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };
            var iCloned = i.Clone();

            Assert.AreEqual(10, iCloned.Rho.Start);
        }
        /// <summary>
        /// check that deserialized pMC detector input is correct when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_pmc_detector_input_class_is_correct_when_using_FileIO()
        {
            new pMCROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) }.WriteToJson("test");
            var iCloned = FileIO.ReadFromJson<pMCROfRhoDetectorInput>("test");

            Assert.AreEqual(10, iCloned.Rho.Start);
        }
    }
}
