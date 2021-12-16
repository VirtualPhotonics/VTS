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
    public class SimulationInputTests
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
        /// validate deserialization of SimulationInput
        /// </summary>
        [Test]
        public void Validate_deserialized_SimulationInput_is_correct()
        {
            var i = new SimulationInput { N = 10 };

            var iCloned = i.Clone();

            Assert.AreEqual(10, iCloned.N);
        }
        /// <summary>
        ///  validate deserialization of SimulationInput when using FileIO
        /// </summary>
        [Test]
        public void Validate_deserialized_SimulationInput_is_correct_when_using_FileIO()
        {
            new SimulationInput { N = 10 }.ToFile("test.txt");
            var iCloned = SimulationInput.FromFile("test.txt");

            Assert.AreEqual(10, iCloned.N);
        }
        /// <summary>
        /// check that null detector input in SimulationInput gets converted
        /// to empty list
        /// </summary>
        [Test]
        public void Validate_null_detector_input_gets_converted_to_empty_list_correctly()
        {
            var si = new SimulationInput(
                100,
                "results",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiLayerTissueInput(),
                null
                );
            Assert.IsTrue(si.DetectorInputs.Count == 0);
        }
        /// <summary>
        /// validate detector input gets serialized correctly
        /// </summary>
        [Test]
        public void Validate_detector_input_class_can_be_serialized_as_part_of_SimulationInput()
        {
            var detectorInput = new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };

            try
            {
                new SimulationInput(
                    10,
                    "",
                    new SimulationOptions(),
                    new DirectionalPointSourceInput(),
                    new MultiLayerTissueInput(),
                    new List<IDetectorInput>
                    {
                         detectorInput
                    }
                    ).WriteToJson("test.txt");
            }
            catch (Exception)
            {
                Assert.Fail("SimulationInput class could not be serialized.");
            }
        }
        /// <summary>
        /// check that deserialized detector class is correct
        /// </summary>
        [Test]
        public void Validate_deserialized_detector_input_class_is_correct()
        {
            var i = new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };
            var iCloned = i.Clone();

            Assert.AreEqual(10, iCloned.Rho.Start);
        }
        /// <summary>
        /// check that deserialized detector input is correct
        /// </summary>
        [Test]
        public void Validate_deserialized_detector_input_class_is_correct_when_using_FileIO()
        {
            new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) }.WriteToJson("test");
            var iCloned = FileIO.ReadFromJson<ROfRhoDetectorInput>("test");

            Assert.AreEqual(10, iCloned.Rho.Start);
        }

    }
}
