using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.Test.MonteCarlo.IO;
using Vts.IO;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DetectorBinarySerializationTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// TallySecondMoment is set to true for all detectors
        /// </summary>
        private readonly List<string> _listOfTestDetectors = new()
        {
            // 0D detectors
            "testrdiffuse",
            "testrdiffuse_2",
            "testtdiffuse",
            "testtdiffuse_2",
            "testatotal",
            "testatotal_2",
            // 1D detectors
            "testrofangle",
            "testrofangle_2",
            "testrofrho",
            "testrofrho_2",
            "testtofangle",
            "testtofangle_2",
            "testtofrho",
            "testtofrho_2",
            "testpmcrofrho",
            "testpmcrofrho_2",
            // 2D detectors
            "testdmcdrofrhodmua",
            "testdmcdrorrhodmua_2",
            "testdmcdrofrhodmus",
            "testdmcdrofrhodmus_2",
            "testfluenceoffxandz",
            "testfluenceoffxandz_2",
            "testfluenceofrhoandz",
            "testfluenceofrhoandz_2",
            "testrofrhoandangle",
            "testrofrhoandangle_2",
            "testrofrhoandtime",
            "testrofrhoandtime_2",
            "testrofrhoandomega",
            "testrofrhoandomega_2",
            "testrofxandy_2",
            "testtofrhoandangle",
            "testrofrhoandangle_2",
            "testaofrhoandz",
            "testaofrhoandz_2",
            "testreflectedmtofrhoandsubregionhist",
            "testreflectedmtofrhoandsubregionhist_2",
            "testreflectedmtofrhoandsubregionhist_FractionalMT", // additional output for this detector
            "testpmcrofrhoandtime",
            "testpmcrofrhoandtime_2",
            // 3D detectors
            "testfluenceofrhoandzandtime",
            "testfluenceofrhoandzandtime_2",
            "testfluenceofrhoandzandomega",
            "testfluenceofrhoandzandomega_2",
            "testfluenceofxandyandz",
            "testfluenceofxandyandz_2",
            // 4D detectors
            "testfluenceofxandyandzandtime",
            "testfluenceofxandyandzandtime_2",
        };

        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeSetUp]
        public void Clear_previously_generated_files()
        {
            // delete any previously generated files
            foreach (var testDetector in _listOfTestDetectors)
            {
                if (File.Exists(testDetector))
                {
                    File.Delete(testDetector);
                }

                if (File.Exists(testDetector + ".txt"))
                {
                    File.Delete(testDetector + ".txt");
                }
            }
        }
        //FolderCleanup.DeleteDirectoryContaining(currentPath, "two_layer_ROfRho_mua1");
        //FolderCleanup.DeleteDirectoryContaining(currentPath, "one_layer_ROfRho_FluenceOfRhoAndZ_seed");


        /// <summary>
        /// clear all newly generated files
        /// </summary>
        [OneTimeTearDown]
        public void Clear_newly_generated_files()
        {
            // delete any newly generated files
            foreach (var testDetector in _listOfTestDetectors)
            {
                if (File.Exists(testDetector))
                {
                    File.Delete(testDetector);
                }

                if (File.Exists(testDetector + ".txt"))
                {
                    File.Delete(testDetector + ".txt");
                }
            }
        }

        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 0D detectors.
        /// </summary>

        [Test]
        public void Validate_ATotalDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            //const string detectorName = "testatotal";
            //var detector = new ATotalDetector
            //{
            //    TallySecondMoment = true, // tally SecondMoment
            //    Name = detectorName,
            //    Mean = new double(),
            //    SecondMoment = new double()
            //};

            //DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            //Assert.AreEqual(Array.Empty<BinaryArraySerializer>(), detector.Mean);
        }

        [Test]
        public void Validate_RDiffuseDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrdiffuse";
            IDetectorInput detectorInput = new RDiffuseDetectorInput { TallySecondMoment = true, Name = detectorName };
            var detector = (RDiffuseDetector)detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (RDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, detector.Name);
            Assert.AreEqual(100, detector.Mean);
            Assert.AreEqual(50, detector.SecondMoment); // 0D detectors 2nd moment written to .txt file
        }

        [Test]
        public void Validate_TDiffuseDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testtdiffuse";
            IDetectorInput detectorInput = new TDiffuseDetectorInput { TallySecondMoment = false, Name = detectorName };
            var detector = (TDiffuseDetector)detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, detector.Name);
            Assert.AreEqual(100, detector.Mean);
            Assert.AreEqual(50, detector.SecondMoment); // 0D detectors 2nd moment written to .txt file

        }


        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 1D detector.
        /// </summary>
        [Test]
        public void Validate_dMCdROfRhodMuaDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testdmcdrofrhodmua";
            var detector = new dMCdROfRhodMuaDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_dMCdROfRhodMusDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testdmcdrofrhodmus";
            var detector = new dMCdROfRhodMuaDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }
        [Test]
        public void Validate_pMCROFxDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testpmcroffx";
            var detector = new pMCROfFxDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                Name = detectorName,
                Mean = new[]
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                SecondMoment = new[]
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne },
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.SecondMoment[0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.SecondMoment[1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.SecondMoment[2]);
        }
        [Test]
        public void Validate_pMCROfRhoDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testpmcrofrho";
            var detector = new pMCROfRhoDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }
        [Test]
        public void Validate_pMCROfRhoRecessedDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testpmcrofrhorecessed";
            var detector = new pMCROfRhoRecessedDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_ROfAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofangle";
            var detector = new ROfAngleDetector
            {
                Angle = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_ROFxDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testroffx";
            var detector = new ROfFxDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[]
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                SecondMoment = new[]
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne },
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.SecondMoment[0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.SecondMoment[1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_ROfRhoDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofrho";
            var detector = new ROfRhoDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_ROfRhoRecessedDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofrhorecessed";
            var detector = new ROfRhoRecessedDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_TOfAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testtofangle";
            var detector = new TOfAngleDetector
            {
                Angle = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }

        [Test]
        public void Validate_TOfRhoDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testtofrho";
            var detector = new TOfRhoDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] { 1, 2, 3 },
                SecondMoment = new double[] { 4, 5, 6 }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0]);
            Assert.AreEqual(2, detector.Mean[1]);
            Assert.AreEqual(3, detector.Mean[2]);
            Assert.AreEqual(4, detector.SecondMoment[0]);
            Assert.AreEqual(5, detector.SecondMoment[1]);
            Assert.AreEqual(6, detector.SecondMoment[2]);
        }


        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 2D detector.
        /// </summary>

        [Test]
        public void Validate_pMCROFxAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testpmcroffxandTime";
            var detector = new pMCROfFxAndTimeDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 2),
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,]
                {
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                SecondMoment = new[,]
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROFxAndAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testroffxandangle";
            var detector = new ROfFxAndAngleDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,]
                {
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                SecondMoment = new[,]
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROFxAndMaxDepthDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testroffxandMaxDepth";
            var detector = new ROfFxAndMaxDepthDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                MaxDepth = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,]
                {
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                SecondMoment = new[,]
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROFxAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testroffxandTime";
            var detector = new ROfFxAndTimeDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,]
                {
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                SecondMoment = new[,]
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROfRhoAndAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofrhoandangle";
            var detector = new ROfRhoAndAngleDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofrhoandtime";
            var detector = new ROfRhoAndTimeDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROfRhoAndOmegaDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofrhoandomega";
            var detector = new ROfRhoAndOmegaDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Omega = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,]
                {
                    { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                SecondMoment = new[,]
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + 1 * Complex.ImaginaryOne, detector.Mean[0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_ROfXAndYDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofxandy";
            var detector = new ROfXAndYDetector
            {
                X = new DoubleRange(0, 10, 3),
                Y = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_TOfRhoAndAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testtofrhoandz";
            var detector = new TOfRhoAndAngleDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_AOfRhoAndZDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testaofrhoandz";
            var detector = new AOfRhoAndZDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }


        [Test]
        public void Validate_FluenceOfFxAndZDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testfluenceoffxandz";
            var detector = new FluenceOfFxAndZDetector
            {
                Fx = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,]
                {
                    { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                SecondMoment = new[,]
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + Complex.ImaginaryOne, detector.Mean[0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void Validate_FluenceOfRhoAndZDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testfluenceofrhoandz";
            var detector = new FluenceOfRhoAndZDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }

        [Test]
        public void
            Validate_ReflectedMTOfRhoAndSubregionHistDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testreflectedmtofrhoandsubregionhist";
            var detector = new ReflectedMTOfRhoAndSubregionHistDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                MTBins = new DoubleRange(0, 10, 3),
                FractionalMTBins = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } },
                SecondMoment = new double[,] { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } },
                // FractionalMT has dimensions [Rho.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1]=[2,2,3,3]
                FractionalMT = new double[,,,]
                {
                    { { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } } },
                    {
                        { { 19, 20, 21 }, { 22, 23, 24 }, { 25, 26, 27 } },
                        { { 28, 29, 30 }, { 31, 32, 33 }, { 34, 35, 36 } }
                    }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.FractionalMT);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.Mean[2, 0]);
            Assert.AreEqual(8, detector.Mean[2, 1]);
            Assert.AreEqual(9, detector.Mean[2, 2]);
            Assert.AreEqual(10, detector.SecondMoment[0, 0]);
            Assert.AreEqual(11, detector.SecondMoment[0, 1]);
            Assert.AreEqual(12, detector.SecondMoment[0, 2]);
            Assert.AreEqual(13, detector.SecondMoment[1, 0]);
            Assert.AreEqual(14, detector.SecondMoment[1, 1]);
            Assert.AreEqual(15, detector.SecondMoment[1, 2]);
            Assert.AreEqual(16, detector.SecondMoment[2, 0]);
            Assert.AreEqual(17, detector.SecondMoment[2, 1]);
            Assert.AreEqual(18, detector.SecondMoment[2, 2]);

            Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0]);
            Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 1]);
            Assert.AreEqual(3, detector.FractionalMT[0, 0, 0, 2]);
            Assert.AreEqual(4, detector.FractionalMT[0, 0, 1, 0]);
            Assert.AreEqual(5, detector.FractionalMT[0, 0, 1, 1]);
            Assert.AreEqual(6, detector.FractionalMT[0, 0, 1, 2]);
            Assert.AreEqual(7, detector.FractionalMT[0, 0, 2, 0]);
            Assert.AreEqual(8, detector.FractionalMT[0, 0, 2, 1]);
            Assert.AreEqual(9, detector.FractionalMT[0, 0, 2, 2]);
            Assert.AreEqual(10, detector.FractionalMT[0, 1, 0, 0]);
            Assert.AreEqual(11, detector.FractionalMT[0, 1, 0, 1]);
            Assert.AreEqual(12, detector.FractionalMT[0, 1, 0, 2]);
            Assert.AreEqual(13, detector.FractionalMT[0, 1, 1, 0]);
            Assert.AreEqual(14, detector.FractionalMT[0, 1, 1, 1]);
            Assert.AreEqual(15, detector.FractionalMT[0, 1, 1, 2]);
            Assert.AreEqual(16, detector.FractionalMT[0, 1, 2, 0]);
            Assert.AreEqual(17, detector.FractionalMT[0, 1, 2, 1]);
            Assert.AreEqual(18, detector.FractionalMT[0, 1, 2, 2]);
            Assert.AreEqual(19, detector.FractionalMT[1, 0, 0, 0]);
            Assert.AreEqual(20, detector.FractionalMT[1, 0, 0, 1]);
            Assert.AreEqual(21, detector.FractionalMT[1, 0, 0, 2]);
            Assert.AreEqual(22, detector.FractionalMT[1, 0, 1, 0]);
            Assert.AreEqual(23, detector.FractionalMT[1, 0, 1, 1]);
            Assert.AreEqual(24, detector.FractionalMT[1, 0, 1, 2]);
            Assert.AreEqual(25, detector.FractionalMT[1, 0, 2, 0]);
            Assert.AreEqual(26, detector.FractionalMT[1, 0, 2, 1]);
            Assert.AreEqual(27, detector.FractionalMT[1, 0, 2, 2]);
            Assert.AreEqual(28, detector.FractionalMT[1, 1, 0, 0]);
            Assert.AreEqual(29, detector.FractionalMT[1, 1, 0, 1]);
            Assert.AreEqual(30, detector.FractionalMT[1, 1, 0, 2]);
            Assert.AreEqual(31, detector.FractionalMT[1, 1, 1, 0]);
            Assert.AreEqual(32, detector.FractionalMT[1, 1, 1, 1]);
            Assert.AreEqual(33, detector.FractionalMT[1, 1, 1, 2]);
            Assert.AreEqual(34, detector.FractionalMT[1, 1, 2, 0]);
            Assert.AreEqual(35, detector.FractionalMT[1, 1, 2, 1]);
            Assert.AreEqual(36, detector.FractionalMT[1, 1, 2, 2]);
        }

        [Test]
        public void Validate_pMCROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testpmcrofrhoandtime";
            var detector = new pMCROfRhoAndTimeDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 2),
                PerturbedOps = new List<OpticalProperties> { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int> { 1 },
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 2]);
            Assert.AreEqual(4, detector.Mean[1, 0]);
            Assert.AreEqual(5, detector.Mean[1, 1]);
            Assert.AreEqual(6, detector.Mean[1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        }

        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 3D detector.
        /// </summary>
        [Test]
        public void Validate_AOfXAndYAndZ_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testaofxandyandz";
            var detector = new AOfXAndYAndZDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 2),
                Z = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,,] { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } },
                SecondMoment = new double[,,] { { { 13, 14, 15 }, { 16, 17, 18 } }, { { 19, 20, 21 }, { 22, 23, 24 } } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[1, 1, 2]);
            Assert.AreEqual(13, detector.SecondMoment[0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[1, 1, 2]);
        }

        [Test]
        public void Validate_FluenceOfRhoAndZAndOmega_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testfluenceofrhoandzandomega";
            var detector = new FluenceOfRhoAndZAndOmegaDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 2),
                Omega = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new[,,]
                {   {
                        { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                        { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                    },
                    {
                        { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                        { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                    }
                },
                SecondMoment = new[,,]
                {   {
                        { 13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne, 15 + 15 * Complex.ImaginaryOne}, 
                        { 16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne, 18 + 18 * Complex.ImaginaryOne}
                    },
                    {
                        { 19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne, 21 + 21 * Complex.ImaginaryOne },
                        { 22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne, 24 + 24 * Complex.ImaginaryOne }
                    }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + Complex.ImaginaryOne, detector.Mean[0, 0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[0, 1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[0, 1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[0, 1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.Mean[1, 0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.Mean[1, 0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.Mean[1, 0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.Mean[1, 1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.Mean[1, 1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.Mean[1, 1, 2]);
            Assert.AreEqual(13 + 13 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0]);
            Assert.AreEqual(14 + 14 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1]);
            Assert.AreEqual(15 + 15 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 2]);
            Assert.AreEqual(16 + 16 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0]);
            Assert.AreEqual(17 + 17 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1]);
            Assert.AreEqual(18 + 18 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 2]);
            Assert.AreEqual(19 + 19 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0]);
            Assert.AreEqual(20 + 20 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1]);
            Assert.AreEqual(21 + 21 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 2]);
            Assert.AreEqual(22 + 22 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0]);
            Assert.AreEqual(23 + 23 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1]);
            Assert.AreEqual(24 + 24 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 2]);
        }


        [Test]
        public void Validate_FluenceOfRhoAndZAndTime_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testfluenceofrhoandzandtime";
            var detector = new FluenceOfRhoAndZAndTimeDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 100, 2),
                Time = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,,] { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } },
                SecondMoment = new double[,,] { { { 13, 14, 15 }, { 16, 17, 18 } }, { { 19, 20, 21 }, { 22, 23, 24 } } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[1, 1, 2]);
            Assert.AreEqual(13, detector.SecondMoment[0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[1, 1, 2]);
        }

        [Test]
        public void Validate_FluenceOfXAndYAndZ_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testfluenceofxandyandz";
            var detector = new AOfXAndYAndZDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 2),
                Z = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,,] { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } },
                SecondMoment = new double[,,] { { { 13, 14, 15 }, { 16, 17, 18 } }, { { 19, 20, 21 }, { 22, 23, 24 } } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[1, 1, 2]);
            Assert.AreEqual(13, detector.SecondMoment[0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[1, 1, 2]);
        }

        [Test]
        public void Validate_ROfXAndYAndMaxDepth_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofxandyandmaxdepth";
            var detector = new ROfXAndYAndMaxDepthDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 2),
                MaxDepth = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,,] { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } },
                SecondMoment = new double[,,] { { { 13, 14, 15 }, { 16, 17, 18 } }, { { 19, 20, 21 }, { 22, 23, 24 } } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[1, 1, 2]);
            Assert.AreEqual(13, detector.SecondMoment[0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[1, 1, 2]);
        }

        [Test]
        public void Validate_ROfXAndYAndMaxDepthRecessed_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testrofxandyandmaxdepthrecessed";
            var detector = new ROfXAndYAndMaxDepthRecessedDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 2),
                MaxDepth = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,,] { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } },
                SecondMoment = new double[,,] { { { 13, 14, 15 }, { 16, 17, 18 } }, { { 19, 20, 21 }, { 22, 23, 24 } } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[1, 1, 2]);
            Assert.AreEqual(13, detector.SecondMoment[0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[1, 1, 2]);
        }

        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 4D detector.
        /// </summary>
        [Test]
        public void Validate_FluenceOfXAndYAndZAndTime_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testaofxandyandzandtime";
            var detector = new FluenceOfXAndYAndZAndTimeDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 2),
                Z = new DoubleRange(0, 1, 2),
                Time = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,,,]  {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } }, 
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    } },
                SecondMoment = new double[,,,] {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } }, 
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[0, 1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[0, 1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[0, 1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[0, 1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[0, 1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[0, 1, 1, 2]);
            Assert.AreEqual(13, detector.Mean[1, 0, 0, 0]);
            Assert.AreEqual(14, detector.Mean[1, 0, 0, 1]);
            Assert.AreEqual(15, detector.Mean[1, 0, 0, 2]);
            Assert.AreEqual(16, detector.Mean[1, 0, 1, 0]);
            Assert.AreEqual(17, detector.Mean[1, 0, 1, 1]);
            Assert.AreEqual(18, detector.Mean[1, 0, 1, 2]);
            Assert.AreEqual(19, detector.Mean[1, 1, 0, 0]);
            Assert.AreEqual(20, detector.Mean[1, 1, 0, 1]);
            Assert.AreEqual(21, detector.Mean[1, 1, 0, 2]);
            Assert.AreEqual(22, detector.Mean[1, 1, 1, 0]);
            Assert.AreEqual(23, detector.Mean[1, 1, 1, 1]);
            Assert.AreEqual(24, detector.Mean[1, 1, 1, 2]);
            Assert.AreEqual(25, detector.SecondMoment[0, 0, 0, 0]);
            Assert.AreEqual(26, detector.SecondMoment[0, 0, 0, 1]);
            Assert.AreEqual(27, detector.SecondMoment[0, 0, 0, 2]);
            Assert.AreEqual(28, detector.SecondMoment[0, 0, 1, 0]);
            Assert.AreEqual(29, detector.SecondMoment[0, 0, 1, 1]);
            Assert.AreEqual(30, detector.SecondMoment[0, 0, 1, 2]);
            Assert.AreEqual(31, detector.SecondMoment[0, 1, 0, 0]);
            Assert.AreEqual(32, detector.SecondMoment[0, 1, 0, 1]);
            Assert.AreEqual(33, detector.SecondMoment[0, 1, 0, 2]);
            Assert.AreEqual(34, detector.SecondMoment[0, 1, 1, 0]);
            Assert.AreEqual(35, detector.SecondMoment[0, 1, 1, 1]);
            Assert.AreEqual(36, detector.SecondMoment[0, 1, 1, 2]);
            Assert.AreEqual(37, detector.SecondMoment[1, 0, 0, 0]);
            Assert.AreEqual(38, detector.SecondMoment[1, 0, 0, 1]);
            Assert.AreEqual(39, detector.SecondMoment[1, 0, 0, 2]);
            Assert.AreEqual(40, detector.SecondMoment[1, 0, 1, 0]);
            Assert.AreEqual(41, detector.SecondMoment[1, 0, 1, 1]);
            Assert.AreEqual(42, detector.SecondMoment[1, 0, 1, 2]);
            Assert.AreEqual(43, detector.SecondMoment[1, 1, 0, 0]);
            Assert.AreEqual(44, detector.SecondMoment[1, 1, 0, 1]);
            Assert.AreEqual(45, detector.SecondMoment[1, 1, 0, 2]);
            Assert.AreEqual(46, detector.SecondMoment[1, 1, 1, 0]);
            Assert.AreEqual(47, detector.SecondMoment[1, 1, 1, 1]);
            Assert.AreEqual(48, detector.SecondMoment[1, 1, 1, 2]);
        }
    }
}
