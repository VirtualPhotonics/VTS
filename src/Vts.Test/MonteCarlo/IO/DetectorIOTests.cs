using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DetectorIOTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestDetectors = new List<string>
        {
            // 0D detectors
            "testrdiffuse",
            "testrdiffuse_2", // TallySecondMoment is set to true for this detector
            "testtdiffuse",
            "testtdiffuse_2",
            "testatotal",
            "testatotal_2", // TallySecondMoment is set to true for this detector
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
            "testpmcrofrho_2", // TallySecondMoment is set to true for this detector
            // 2D detectors
            "testrofrhoandangle",
            "testrofrhoandangle_2",
            "testrofrhoandtime",
            "testrofrhoandtime_2", // TallySecondMoment is set to true for this detector
            "testrofrhoandomega",
            "testrofrhoandomega_2", // TallySecondMoment is set to true for this detector
            "testrofxandy",
            "testrofxandy_2",
            "testtofrhoandangle",
            "testrofrhoandangle_2", // TallySecondMoment is set to true for this detector
            "testaofrhoandz",
            "testaofrhoandz_2",
            "testfluenceofrhoandz",
            "testfluenceofrhoandz_2",
            "testreflectedmtofrhoandsubregionhist",
            "testreflectedmtofrhoandsubregionhist_2", // TallySecondMoment is set to true for this detector
            "testreflectedmtofrhoandsubregionhist_FractionalMT", // additional output for this detector
            "testpmcrofrhoandtime",
            "testpmcrofrhoandtime_2", // TallySecondMoment is set to true for this detector
            // 3D detectors
            "testfluenceofrhoandzandtime",
            "testfluenceofrhoandzandtime_2", // TallySecondMoment is set to true for this detector
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
        public void Validate_RDiffuseDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrdiffuse";
            IDetectorInput detectorInput = new RDiffuseDetectorInput {TallySecondMoment = true, Name=detectorName};
            var detector = (RDiffuseDetector) detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (RDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, detector.Name);
            Assert.AreEqual(100, detector.Mean);
            Assert.AreEqual(50, detector.SecondMoment); // 0D detectors 2nd moment written to .txt file
        }
        [Test]
        public void Validate_TDiffuseDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testtdiffuse";
            IDetectorInput detectorInput = new TDiffuseDetectorInput {TallySecondMoment = false, Name = detectorName};
            var detector = (TDiffuseDetector) detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, detector.Name);
            Assert.AreEqual(100, detector.Mean); 
            Assert.AreEqual(50, detector.SecondMoment); // 0D detectors 2nd moment written to .txt file

        }
        [Test]
        public void Validate_ATotalDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testatotal";
            IDetectorInput detectorInput = new ATotalDetectorInput {TallySecondMoment = true, Name = detectorName};
            var detector = (ATotalDetector) detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ATotalDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, detector.Name);
            Assert.AreEqual(100, detector.Mean);
            Assert.AreEqual(50, detector.SecondMoment);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 1D detector.
        /// </summary>
        ///         
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
        [Test]
        public void Validate_pMCROfRhoDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testpmcrofrho";
            var detector = new pMCROfRhoDetector
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
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 2D detector.
        /// </summary>
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
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne } },
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
                SecondMoment = new double[,] { { 7, 8, 9}, { 10, 11, 12 }}
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
        public void Validate_ReflectedMTOfRhoAndSubregionHistDetector_deserialized_class_is_correct_when_using_GetBinarySerializers()
        {
            const string detectorName = "testreflectedmtofrhoandsubregionhist";
            var detector = new ReflectedMTOfRhoAndSubregionHistDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                MTBins = new DoubleRange(0, 10, 3),
                FractionalMTBins = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9} },
                SecondMoment = new double[,] { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } },
                // FractionalMT has dimensions [Rho.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1]=[2,2,3,3]
                FractionalMT = new double[,,,] { { { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } } }, { { { 19, 20, 21 }, { 22, 23, 24 }, { 25, 26, 27 } }, { { 28, 29, 30 }, { 31, 32, 33 }, { 34, 35, 36 } } } } 
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment, detector.FractionalMT);

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

            Assert.AreEqual(1, detector.FractionalMT[0, 0, 0 ,0]);
            Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 1]);
            Assert.AreEqual(3,detector.FractionalMT[0, 0, 0, 2]);
            Assert.AreEqual(4,detector.FractionalMT[0, 0, 1, 0]);
            Assert.AreEqual(5,detector.FractionalMT[0, 0, 1, 1]);
            Assert.AreEqual(6,detector.FractionalMT[0, 0, 1, 2]);
            Assert.AreEqual(7,detector.FractionalMT[0, 0, 2, 0]);
            Assert.AreEqual(8, detector.FractionalMT[0, 0, 2, 1]);
            Assert.AreEqual(9, detector.FractionalMT[0, 0, 2, 2]);
            Assert.AreEqual(10, detector.FractionalMT[0, 1, 0, 0]);
            Assert.AreEqual(11,detector.FractionalMT[0, 1, 0, 1]);
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
        public void validate_FluenceOfRhoAndZAndTime_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testfluenceofrhoandzandtime";
            IDetectorInput detectorInput = new FluenceOfRhoAndZAndTimeDetectorInput
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = true,
                Name = detectorName,
            };
            var detector = (FluenceOfRhoAndZAndTimeDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,,] {{{1, 2, 3}, {4, 5, 6}}, {{7, 8, 9}, {10, 11, 12}}};

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (FluenceOfRhoAndZAndTimeDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName,detector.Name);
            Assert.AreEqual(1,detector.Mean[0, 0, 0]);
            Assert.AreEqual(2,detector.Mean[0, 0, 1]);
            Assert.AreEqual(3,detector.Mean[0, 0, 2]);
            Assert.AreEqual(4,detector.Mean[0, 1, 0]);
            Assert.AreEqual(5,detector.Mean[0, 1, 1]);
            Assert.AreEqual(6,detector.Mean[0, 1, 2]);
            Assert.AreEqual(7,detector.Mean[1, 0, 0]);
            Assert.AreEqual(8,detector.Mean[1, 0, 1]);
            Assert.AreEqual(9,detector.Mean[1, 0, 2]);
            Assert.AreEqual(10,detector.Mean[1, 1, 0]);
            Assert.AreEqual(11,detector.Mean[1, 1, 1]);
            Assert.AreEqual(12,detector.Mean[1, 1, 2]);
        }
    }
}
