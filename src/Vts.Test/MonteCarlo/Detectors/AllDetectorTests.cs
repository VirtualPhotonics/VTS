using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests set up all detectors using enums and test
    /// DetectorInput, CreateDetector, Detector, Binary Write and Read
    /// </summary>
    [TestFixture]
    public class AllDetectorsTests
    {
        private IEnumerable<IDetectorInput> _detectorInputs;
        private List<string> _detectorFiles;

        [OneTimeSetUp]
        public void Generate_list_of_detectors()
        {
            var inputFiles = SimulationInputProvider.GenerateAllSimulationInputs();
            // generate list of unique detectors from all sample input files
            var detectorInputGrouping = inputFiles.SelectMany(
                si => si.DetectorInputs).GroupBy(d => d.TallyType).ToList();
            _detectorInputs = detectorInputGrouping.
                Select(x => x.FirstOrDefault());
            _detectorFiles = _detectorInputs.Select(d => d.Name).ToList();
        }


        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            // make sure to delete all associated files that get generated with detector
            var currentDir = Directory.GetCurrentDirectory();
            foreach (var file in _detectorFiles)
            {
                var fileList = Directory.GetFiles(currentDir, file + "*");
                foreach (var detectorRelatedFile in fileList)
                {
                    File.Delete(detectorRelatedFile);
                }
            }
        }

        [Test]
        public void Verify_detector_classes()
        {
            // use factory to instantiate detector with CreateDetector and call Initialize
            var tissue = new MultiLayerTissue();
            var rng = new MersenneTwister(0); 
            foreach (var detectorInput in _detectorInputs)
            {
                // factory generates IDetector using CreateDetector,
                // then calls detector.Initialize method 
                var detector = DetectorFactory.GetDetector(detectorInput, tissue, rng);
                // DetectorIO methods call detector.GetBinarySerializers.WriteData and ReadData
                // check that can write detector
                DetectorIO.WriteDetectorToFile(detector, "");
                // check that can read detector 
                var readDetector = DetectorIO.ReadDetectorFromFile(
                    detector.TallyType, "");
                Assert.IsInstanceOf<IDetector>(readDetector);
            }
        }
    }
}
