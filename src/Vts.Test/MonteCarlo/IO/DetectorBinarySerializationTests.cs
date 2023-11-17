using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using Vts.Test.Unit.MonteCarlo.Detectors;

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


        #region 3D detectors

        /// <summary>
        /// test to verify that GetBinarySerializers are working correctly for 3D detector.
        /// </summary>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_AOfXAndYAndZDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testaofxandyandz";
            var detector = new AOfXAndYAndZDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_FluenceOfRhoAndZAndOmegaDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testfluenceofrhoandzandomega";
            var detector = new FluenceOfRhoAndZAndOmegaDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 3),
                Omega = new DoubleRange(0, 1, 3),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new[,,] // Rho.Count-1 x Z.Count-1 x Omega.Count: 2x2x3
                {
                    {
                        { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                        { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                    },
                    {
                        { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                        {
                            10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne,
                            12 + 12 * Complex.ImaginaryOne
                        }
                    }
                },
                SecondMoment = new[,,]
                {
                    {
                        {
                            13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne,
                            15 + 15 * Complex.ImaginaryOne
                        },
                        {
                            16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne,
                            18 + 18 * Complex.ImaginaryOne
                        }
                    },
                    {
                        {
                            19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne,
                            21 + 21 * Complex.ImaginaryOne
                        },
                        {
                            22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne,
                            24 + 24 * Complex.ImaginaryOne
                        }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_FluenceOfRhoAndZAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testfluenceofrhoandzandtime";
            var detector = new FluenceOfRhoAndZAndTimeDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 100, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // Rho.Count-1 x Z.Count-1 x Time.Count-1: 2x2x3
                    { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } }, // 4x2x3
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_FluenceOfXAndYAndZDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testfluenceofxandyandz";
            var detector = new FluenceOfXAndYAndZDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // Rho.Count-1 x Z.Count-1 x Z.Count-1: 2x2x3
                    { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } },
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
      Validate_ReflectedDynamicMTOfXAndYAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testreflecteddynamicmtofxandyandsubregionhist";
            var detector = new ReflectedDynamicMTOfXAndYAndSubregionHistDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                MTBins = new DoubleRange(0, 100, 4),
                Z = new DoubleRange(0, 100, 4),
                FractionalMTBins = new DoubleRange(0, 10, 1),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                },
                TotalMTOfZ = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 25, 26, 27 }, { 28, 29, 30 } },
                    { { 31, 32, 33 }, { 34, 35, 36 } }
                },
                TotalMTOfZSecondMoment = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 37, 38, 39 }, { 40, 41, 42 } },
                    { { 43, 44, 45 }, { 46, 47, 48 } }
                },
                DynamicMTOfZ = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 49, 50, 51 }, { 52, 53, 54 } },
                    { { 55, 56, 57 }, { 58, 59, 60 } }
                },
                DynamicMTOfZSecondMoment = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 61, 62, 63 }, { 64, 65, 66 } },
                    { { 67, 68, 69 }, { 70, 71, 72 } }
                },
                FractionalMT = new double[,,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1, FractionalMTBins.Count+1: 2x2x3x2
                {
                    {
                        { 
                            { 1, 2 }, { 3, 4 }, { 5, 6 } 
                        },
                        {
                            { 7, 8 }, { 9, 10 }, { 11, 12 }
                        }

                    },
                    {
                        {
                            { 13, 14 }, { 15, 16 }, { 17, 18 }
                        },
                        {
                            { 19, 20 }, { 21, 22 }, { 23, 24 }
                        }
                    }
                },
                SubregionCollisions = new double[,] // NumSubregions x 2: 3x2
                    { { 25, 26 }, { 27, 28 }, { 29, 30 } }, // 2nd index: 0=static, 1=dynamic

            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.TotalMTOfZ, detector.TotalMTOfZSecondMoment, detector.DynamicMTOfZ, detector.DynamicMTOfZSecondMoment,
                detector.FractionalMT, detector.SubregionCollisions);

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

            Assert.AreEqual(25, detector.TotalMTOfZ[0, 0, 0]);
            Assert.AreEqual(26, detector.TotalMTOfZ[0, 0, 1]);
            Assert.AreEqual(27, detector.TotalMTOfZ[0, 0, 2]);
            Assert.AreEqual(28, detector.TotalMTOfZ[0, 1, 0]);
            Assert.AreEqual(29, detector.TotalMTOfZ[0, 1, 1]);
            Assert.AreEqual(30, detector.TotalMTOfZ[0, 1, 2]);
            Assert.AreEqual(31, detector.TotalMTOfZ[1, 0, 0]);
            Assert.AreEqual(32, detector.TotalMTOfZ[1, 0, 1]);
            Assert.AreEqual(33, detector.TotalMTOfZ[1, 0, 2]);
            Assert.AreEqual(34, detector.TotalMTOfZ[1, 1, 0]);
            Assert.AreEqual(35, detector.TotalMTOfZ[1, 1, 1]);
            Assert.AreEqual(36, detector.TotalMTOfZ[1, 1, 2]);

            Assert.AreEqual(49, detector.DynamicMTOfZ[0, 0, 0]);
            Assert.AreEqual(50, detector.DynamicMTOfZ[0, 0, 1]);
            Assert.AreEqual(51, detector.DynamicMTOfZ[0, 0, 2]);
            Assert.AreEqual(52, detector.DynamicMTOfZ[0, 1, 0]);
            Assert.AreEqual(53, detector.DynamicMTOfZ[0, 1, 1]);
            Assert.AreEqual(54, detector.DynamicMTOfZ[0, 1, 2]);
            Assert.AreEqual(55, detector.DynamicMTOfZ[1, 0, 0]);
            Assert.AreEqual(56, detector.DynamicMTOfZ[1, 0, 1]);
            Assert.AreEqual(57, detector.DynamicMTOfZ[1, 0, 2]);
            Assert.AreEqual(58, detector.DynamicMTOfZ[1, 1, 0]);
            Assert.AreEqual(59, detector.DynamicMTOfZ[1, 1, 1]);
            Assert.AreEqual(60, detector.DynamicMTOfZ[1, 1, 2]);

            Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0]);
            Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 1]);
            Assert.AreEqual(3, detector.FractionalMT[0, 0, 1, 0]);
            Assert.AreEqual(4, detector.FractionalMT[0, 0, 1, 1]);
            Assert.AreEqual(5, detector.FractionalMT[0, 0, 2, 0]);
            Assert.AreEqual(6, detector.FractionalMT[0, 0, 2, 1]);
            Assert.AreEqual(7, detector.FractionalMT[0, 1, 0, 0]);
            Assert.AreEqual(8, detector.FractionalMT[0, 1, 0, 1]);
            Assert.AreEqual(9, detector.FractionalMT[0, 1, 1, 0]);
            Assert.AreEqual(10, detector.FractionalMT[0, 1, 1, 1]);
            Assert.AreEqual(11, detector.FractionalMT[0, 1, 2, 0]);
            Assert.AreEqual(12, detector.FractionalMT[0, 1, 2, 1]);
            Assert.AreEqual(13, detector.FractionalMT[1, 0, 0, 0]);
            Assert.AreEqual(14, detector.FractionalMT[1, 0, 0, 1]);
            Assert.AreEqual(15, detector.FractionalMT[1, 0, 1, 0]);
            Assert.AreEqual(16, detector.FractionalMT[1, 0, 1, 1]);
            Assert.AreEqual(17, detector.FractionalMT[1, 0, 2, 0]);
            Assert.AreEqual(18, detector.FractionalMT[1, 0, 2, 1]);
            Assert.AreEqual(19, detector.FractionalMT[1, 1, 0, 0]);
            Assert.AreEqual(20, detector.FractionalMT[1, 1, 0, 1]);
            Assert.AreEqual(21, detector.FractionalMT[1, 1, 1, 0]);
            Assert.AreEqual(22, detector.FractionalMT[1, 1, 1, 1]);
            Assert.AreEqual(23, detector.FractionalMT[1, 1, 2, 0]);
            Assert.AreEqual(24, detector.FractionalMT[1, 1, 2, 1]);

            Assert.AreEqual(25, detector.SubregionCollisions[0, 0]);
            Assert.AreEqual(26, detector.SubregionCollisions[0, 1]);
            Assert.AreEqual(27, detector.SubregionCollisions[1, 0]);
            Assert.AreEqual(28, detector.SubregionCollisions[1, 1]);
            Assert.AreEqual(29, detector.SubregionCollisions[2, 0]);
            Assert.AreEqual(30, detector.SubregionCollisions[2, 1]);

            if (!tallySecondMoment) return;
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
            Assert.AreEqual(37, detector.TotalMTOfZSecondMoment[0, 0, 0]);
            Assert.AreEqual(38, detector.TotalMTOfZSecondMoment[0, 0, 1]);
            Assert.AreEqual(39, detector.TotalMTOfZSecondMoment[0, 0, 2]);
            Assert.AreEqual(40, detector.TotalMTOfZSecondMoment[0, 1, 0]);
            Assert.AreEqual(41, detector.TotalMTOfZSecondMoment[0, 1, 1]);
            Assert.AreEqual(42, detector.TotalMTOfZSecondMoment[0, 1, 2]);
            Assert.AreEqual(43, detector.TotalMTOfZSecondMoment[1, 0, 0]);
            Assert.AreEqual(44, detector.TotalMTOfZSecondMoment[1, 0, 1]);
            Assert.AreEqual(45, detector.TotalMTOfZSecondMoment[1, 0, 2]);
            Assert.AreEqual(46, detector.TotalMTOfZSecondMoment[1, 1, 0]);
            Assert.AreEqual(47, detector.TotalMTOfZSecondMoment[1, 1, 1]);
            Assert.AreEqual(48, detector.TotalMTOfZSecondMoment[1, 1, 2]);
            Assert.AreEqual(61, detector.DynamicMTOfZSecondMoment[0, 0, 0]);
            Assert.AreEqual(62, detector.DynamicMTOfZSecondMoment[0, 0, 1]);
            Assert.AreEqual(63, detector.DynamicMTOfZSecondMoment[0, 0, 2]);
            Assert.AreEqual(64, detector.DynamicMTOfZSecondMoment[0, 1, 0]);
            Assert.AreEqual(65, detector.DynamicMTOfZSecondMoment[0, 1, 1]);
            Assert.AreEqual(66, detector.DynamicMTOfZSecondMoment[0, 1, 2]);
            Assert.AreEqual(67, detector.DynamicMTOfZSecondMoment[1, 0, 0]);
            Assert.AreEqual(68, detector.DynamicMTOfZSecondMoment[1, 0, 1]);
            Assert.AreEqual(69, detector.DynamicMTOfZSecondMoment[1, 0, 2]);
            Assert.AreEqual(70, detector.DynamicMTOfZSecondMoment[1, 1, 0]);
            Assert.AreEqual(71, detector.DynamicMTOfZSecondMoment[1, 1, 1]);
            Assert.AreEqual(72, detector.DynamicMTOfZSecondMoment[1, 1, 2]);

        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
    Validate_ReflectedMTOfXAndYAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testreflectedmtofxandyandsubregionhist";
            var detector = new ReflectedMTOfXAndYAndSubregionHistDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                MTBins = new DoubleRange(0, 100, 4),
                FractionalMTBins = new DoubleRange(0, 10, 1),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                },
                FractionalMT = new double[,,,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1 x NumRegions x FractionalMTBins.Count+1: 2x2x3x2x2
                {
                    {
                        {
                            {
                                { 1, 2 }, { 3, 4 }, { 5, 6 }
                            },
                            {
                                { 7, 8 }, { 9, 10 }, { 11, 12 }
                            }
                        },
                        {
                            {
                                { 13, 14 }, { 15, 16 }, { 17, 18 }
                            }, 
                            {   
                                { 19, 20 }, { 21, 22 }, { 23, 24 }
                            }
                        }

                    },
                    {
                        {
                            {
                                { 25, 26 }, { 27, 28 }, { 29, 30 }
                            },
                            {
                                { 31, 32 }, { 33, 34 }, { 35, 36 }
                            },
                        },
                        {
                            {
                                { 37, 38 }, { 39, 40 }, { 41, 42 }
                            },
                            {
                                { 43, 44 }, { 45, 46 }, { 47, 48 }
                            }, 
                        }
                    }
                },
            
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.FractionalMT);

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

            Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0, 0]);
            Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 0, 1]);
            Assert.AreEqual(3, detector.FractionalMT[0, 0, 0, 1, 0]);
            Assert.AreEqual(4, detector.FractionalMT[0, 0, 0, 1, 1]);
            Assert.AreEqual(5, detector.FractionalMT[0, 0, 0, 2, 0]);
            Assert.AreEqual(6, detector.FractionalMT[0, 0, 0, 2, 1]);
            Assert.AreEqual(7, detector.FractionalMT[0, 0, 1, 0, 0]);
            Assert.AreEqual(8, detector.FractionalMT[0, 0, 1, 0, 1]);
            Assert.AreEqual(9, detector.FractionalMT[0, 0, 1, 1, 0]);
            Assert.AreEqual(10, detector.FractionalMT[0, 0, 1, 1, 1]);
            Assert.AreEqual(11, detector.FractionalMT[0, 0, 1, 2, 0]);
            Assert.AreEqual(12, detector.FractionalMT[0, 0, 1, 2, 1]);
            Assert.AreEqual(13, detector.FractionalMT[0, 1, 0, 0, 0]);
            Assert.AreEqual(14, detector.FractionalMT[0, 1, 0, 0, 1]);
            Assert.AreEqual(15, detector.FractionalMT[0, 1, 0, 1, 0]);
            Assert.AreEqual(16, detector.FractionalMT[0, 1, 0, 1, 1]);
            Assert.AreEqual(17, detector.FractionalMT[0, 1, 0, 2, 0]);
            Assert.AreEqual(18, detector.FractionalMT[0, 1, 0, 2, 1]);
            Assert.AreEqual(19, detector.FractionalMT[0, 1, 1, 0, 0]);
            Assert.AreEqual(20, detector.FractionalMT[0, 1, 1, 0, 1]);
            Assert.AreEqual(21, detector.FractionalMT[0, 1, 1, 1, 0]);
            Assert.AreEqual(22, detector.FractionalMT[0, 1, 1, 1, 1]);
            Assert.AreEqual(23, detector.FractionalMT[0, 1, 1, 2, 0]);
            Assert.AreEqual(24, detector.FractionalMT[0, 1, 1, 2, 1]);
            Assert.AreEqual(25, detector.FractionalMT[1, 0, 0, 0, 0]);
            Assert.AreEqual(26, detector.FractionalMT[1, 0, 0, 0, 1]);
            Assert.AreEqual(27, detector.FractionalMT[1, 0, 0, 1, 0]);
            Assert.AreEqual(28, detector.FractionalMT[1, 0, 0, 1, 1]);
            Assert.AreEqual(29, detector.FractionalMT[1, 0, 0, 2, 0]);
            Assert.AreEqual(30, detector.FractionalMT[1, 0, 0, 2, 1]);
            Assert.AreEqual(31, detector.FractionalMT[1, 0, 1, 0, 0]);
            Assert.AreEqual(32, detector.FractionalMT[1, 0, 1, 0, 1]);
            Assert.AreEqual(33, detector.FractionalMT[1, 0, 1, 1, 0]);
            Assert.AreEqual(34, detector.FractionalMT[1, 0, 1, 1, 1]);
            Assert.AreEqual(35, detector.FractionalMT[1, 0, 1, 2, 0]);
            Assert.AreEqual(36, detector.FractionalMT[1, 0, 1, 2, 1]);
            Assert.AreEqual(37, detector.FractionalMT[1, 1, 0, 0, 0]);
            Assert.AreEqual(38, detector.FractionalMT[1, 1, 0, 0, 1]);
            Assert.AreEqual(39, detector.FractionalMT[1, 1, 0, 1, 0]);
            Assert.AreEqual(40, detector.FractionalMT[1, 1, 0, 1, 1]);
            Assert.AreEqual(41, detector.FractionalMT[1, 1, 0, 2, 0]);
            Assert.AreEqual(42, detector.FractionalMT[1, 1, 0, 2, 1]);
            Assert.AreEqual(43, detector.FractionalMT[1, 1, 1, 0, 0]);
            Assert.AreEqual(44, detector.FractionalMT[1, 1, 1, 0, 1]);
            Assert.AreEqual(45, detector.FractionalMT[1, 1, 1, 1, 0]);
            Assert.AreEqual(46, detector.FractionalMT[1, 1, 1, 1, 1]);
            Assert.AreEqual(47, detector.FractionalMT[1, 1, 1, 2, 0]);
            Assert.AreEqual(48, detector.FractionalMT[1, 1, 1, 2, 1]);

            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
       Validate_ReflectedTimeOfRhoAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testreflectedtimeofrhoandsubregionhist";
            var detector = new ReflectedTimeOfRhoAndSubregionHistDetector
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 10, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // Rho.Count-1 x NumRegions x Time.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_RadianceOfFxAndZAndAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testradianceoffxandzandangle";
            var detector = new RadianceOfFxAndZAndAngleDetector
            {
                Fx = new DoubleRange(-0, 10, 2),
                Z = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new[,,] // Fx.Count x Z.Count-1 x Angle.Count-1: 2x2x3
                {
                    {
                        { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                        { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                    },
                    {
                        { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                        {
                            10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne,
                            12 + 12 * Complex.ImaginaryOne
                        }
                    }
                },
                SecondMoment = new[,,]
                {
                    {
                        {
                            13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne,
                            15 + 15 * Complex.ImaginaryOne
                        },
                        {
                            16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne,
                            18 + 18 * Complex.ImaginaryOne
                        }
                    },
                    {
                        {
                            19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne,
                            21 + 21 * Complex.ImaginaryOne
                        },
                        {
                            22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne,
                            24 + 24 * Complex.ImaginaryOne
                        }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_RadianceOfRhoAndZAndAngleDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testradianceofrhoandzandangle";
            var detector = new RadianceOfRhoAndZAndAngleDetector
            {
                Rho = new DoubleRange(-0, 10, 3),
                Z = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // Rho.Count-1 x Z.Count-1 x Angle.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_ROfXAndYAndMaxDepthDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandmaxdepth";
            var detector = new ROfXAndYAndMaxDepthDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                MaxDepth = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_ROfXAndYAndMaxDepthRecessedDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandmaxdepthrecessed";
            var detector = new ROfXAndYAndMaxDepthRecessedDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                MaxDepth = new DoubleRange(0, 1, 4),
                ZPlane = -0.1,
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x MaxDepth.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_ROfXAndYAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandtime";
            var detector = new ROfXAndYAndTimeDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x Time.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void Validate_ROfXAndYAndTimeRecessedDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandtimerecessed";
            var detector = new ROfXAndYAndTimeRecessedDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                ZPlane = -0.1,
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x Time.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
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
            if (!tallySecondMoment) return;
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
        [TestCase(true)]
        [TestCase(false)]
        public void
    Validate_TransmittedDynamicMTOfXAndYAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testtransmiteddynamicmtofxandyandsubregionhist";
            var detector = new TransmittedDynamicMTOfXAndYAndSubregionHistDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                MTBins = new DoubleRange(0, 100, 4),
                Z = new DoubleRange(0, 100, 4),
                FractionalMTBins = new DoubleRange(0, 10, 1),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                },
                TotalMTOfZ = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 25, 26, 27 }, { 28, 29, 30 } },
                    { { 31, 32, 33 }, { 34, 35, 36 } }
                },
                TotalMTOfZSecondMoment = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 37, 38, 39 }, { 40, 41, 42 } },
                    { { 43, 44, 45 }, { 46, 47, 48 } }
                },
                DynamicMTOfZ = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 49, 50, 51 }, { 52, 53, 54 } },
                    { { 55, 56, 57 }, { 58, 59, 60 } }
                },
                DynamicMTOfZSecondMoment = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
                {
                    { { 61, 62, 63 }, { 64, 65, 66 } },
                    { { 67, 68, 69 }, { 70, 71, 72 } }
                },
                FractionalMT = new double[,,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1, FractionalMTBins.Count+1: 2x2x3x2
                {
                    {
                        {
                            { 1, 2 }, { 3, 4 }, { 5, 6 }
                        },
                        {
                            { 7, 8 }, { 9, 10 }, { 11, 12 }
                        }

                    },
                    {
                        {
                            { 13, 14 }, { 15, 16 }, { 17, 18 }
                        },
                        {
                            { 19, 20 }, { 21, 22 }, { 23, 24 }
                        }
                    }
                },
                SubregionCollisions = new double[,] // NumSubregions x 2: 3x2
                    { { 25, 26 }, { 27, 28 }, { 29, 30 } }, // 2nd index: 0=static, 1=dynamic

            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.TotalMTOfZ, detector.TotalMTOfZSecondMoment, detector.DynamicMTOfZ, detector.DynamicMTOfZSecondMoment,
                detector.FractionalMT, detector.SubregionCollisions);

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

            Assert.AreEqual(25, detector.TotalMTOfZ[0, 0, 0]);
            Assert.AreEqual(26, detector.TotalMTOfZ[0, 0, 1]);
            Assert.AreEqual(27, detector.TotalMTOfZ[0, 0, 2]);
            Assert.AreEqual(28, detector.TotalMTOfZ[0, 1, 0]);
            Assert.AreEqual(29, detector.TotalMTOfZ[0, 1, 1]);
            Assert.AreEqual(30, detector.TotalMTOfZ[0, 1, 2]);
            Assert.AreEqual(31, detector.TotalMTOfZ[1, 0, 0]);
            Assert.AreEqual(32, detector.TotalMTOfZ[1, 0, 1]);
            Assert.AreEqual(33, detector.TotalMTOfZ[1, 0, 2]);
            Assert.AreEqual(34, detector.TotalMTOfZ[1, 1, 0]);
            Assert.AreEqual(35, detector.TotalMTOfZ[1, 1, 1]);
            Assert.AreEqual(36, detector.TotalMTOfZ[1, 1, 2]);

            Assert.AreEqual(49, detector.DynamicMTOfZ[0, 0, 0]);
            Assert.AreEqual(50, detector.DynamicMTOfZ[0, 0, 1]);
            Assert.AreEqual(51, detector.DynamicMTOfZ[0, 0, 2]);
            Assert.AreEqual(52, detector.DynamicMTOfZ[0, 1, 0]);
            Assert.AreEqual(53, detector.DynamicMTOfZ[0, 1, 1]);
            Assert.AreEqual(54, detector.DynamicMTOfZ[0, 1, 2]);
            Assert.AreEqual(55, detector.DynamicMTOfZ[1, 0, 0]);
            Assert.AreEqual(56, detector.DynamicMTOfZ[1, 0, 1]);
            Assert.AreEqual(57, detector.DynamicMTOfZ[1, 0, 2]);
            Assert.AreEqual(58, detector.DynamicMTOfZ[1, 1, 0]);
            Assert.AreEqual(59, detector.DynamicMTOfZ[1, 1, 1]);
            Assert.AreEqual(60, detector.DynamicMTOfZ[1, 1, 2]);

            Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0]);
            Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 1]);
            Assert.AreEqual(3, detector.FractionalMT[0, 0, 1, 0]);
            Assert.AreEqual(4, detector.FractionalMT[0, 0, 1, 1]);
            Assert.AreEqual(5, detector.FractionalMT[0, 0, 2, 0]);
            Assert.AreEqual(6, detector.FractionalMT[0, 0, 2, 1]);
            Assert.AreEqual(7, detector.FractionalMT[0, 1, 0, 0]);
            Assert.AreEqual(8, detector.FractionalMT[0, 1, 0, 1]);
            Assert.AreEqual(9, detector.FractionalMT[0, 1, 1, 0]);
            Assert.AreEqual(10, detector.FractionalMT[0, 1, 1, 1]);
            Assert.AreEqual(11, detector.FractionalMT[0, 1, 2, 0]);
            Assert.AreEqual(12, detector.FractionalMT[0, 1, 2, 1]);
            Assert.AreEqual(13, detector.FractionalMT[1, 0, 0, 0]);
            Assert.AreEqual(14, detector.FractionalMT[1, 0, 0, 1]);
            Assert.AreEqual(15, detector.FractionalMT[1, 0, 1, 0]);
            Assert.AreEqual(16, detector.FractionalMT[1, 0, 1, 1]);
            Assert.AreEqual(17, detector.FractionalMT[1, 0, 2, 0]);
            Assert.AreEqual(18, detector.FractionalMT[1, 0, 2, 1]);
            Assert.AreEqual(19, detector.FractionalMT[1, 1, 0, 0]);
            Assert.AreEqual(20, detector.FractionalMT[1, 1, 0, 1]);
            Assert.AreEqual(21, detector.FractionalMT[1, 1, 1, 0]);
            Assert.AreEqual(22, detector.FractionalMT[1, 1, 1, 1]);
            Assert.AreEqual(23, detector.FractionalMT[1, 1, 2, 0]);
            Assert.AreEqual(24, detector.FractionalMT[1, 1, 2, 1]);

            Assert.AreEqual(25, detector.SubregionCollisions[0, 0]);
            Assert.AreEqual(26, detector.SubregionCollisions[0, 1]);
            Assert.AreEqual(27, detector.SubregionCollisions[1, 0]);
            Assert.AreEqual(28, detector.SubregionCollisions[1, 1]);
            Assert.AreEqual(29, detector.SubregionCollisions[2, 0]);
            Assert.AreEqual(30, detector.SubregionCollisions[2, 1]);

            if (!tallySecondMoment) return;
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
            Assert.AreEqual(37, detector.TotalMTOfZSecondMoment[0, 0, 0]);
            Assert.AreEqual(38, detector.TotalMTOfZSecondMoment[0, 0, 1]);
            Assert.AreEqual(39, detector.TotalMTOfZSecondMoment[0, 0, 2]);
            Assert.AreEqual(40, detector.TotalMTOfZSecondMoment[0, 1, 0]);
            Assert.AreEqual(41, detector.TotalMTOfZSecondMoment[0, 1, 1]);
            Assert.AreEqual(42, detector.TotalMTOfZSecondMoment[0, 1, 2]);
            Assert.AreEqual(43, detector.TotalMTOfZSecondMoment[1, 0, 0]);
            Assert.AreEqual(44, detector.TotalMTOfZSecondMoment[1, 0, 1]);
            Assert.AreEqual(45, detector.TotalMTOfZSecondMoment[1, 0, 2]);
            Assert.AreEqual(46, detector.TotalMTOfZSecondMoment[1, 1, 0]);
            Assert.AreEqual(47, detector.TotalMTOfZSecondMoment[1, 1, 1]);
            Assert.AreEqual(48, detector.TotalMTOfZSecondMoment[1, 1, 2]);
            Assert.AreEqual(61, detector.DynamicMTOfZSecondMoment[0, 0, 0]);
            Assert.AreEqual(62, detector.DynamicMTOfZSecondMoment[0, 0, 1]);
            Assert.AreEqual(63, detector.DynamicMTOfZSecondMoment[0, 0, 2]);
            Assert.AreEqual(64, detector.DynamicMTOfZSecondMoment[0, 1, 0]);
            Assert.AreEqual(65, detector.DynamicMTOfZSecondMoment[0, 1, 1]);
            Assert.AreEqual(66, detector.DynamicMTOfZSecondMoment[0, 1, 2]);
            Assert.AreEqual(67, detector.DynamicMTOfZSecondMoment[1, 0, 0]);
            Assert.AreEqual(68, detector.DynamicMTOfZSecondMoment[1, 0, 1]);
            Assert.AreEqual(69, detector.DynamicMTOfZSecondMoment[1, 0, 2]);
            Assert.AreEqual(70, detector.DynamicMTOfZSecondMoment[1, 1, 0]);
            Assert.AreEqual(71, detector.DynamicMTOfZSecondMoment[1, 1, 1]);
            Assert.AreEqual(72, detector.DynamicMTOfZSecondMoment[1, 1, 2]);

        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
Validate_TransmittedMTOfXAndYAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testtransmittedmtofxandyandsubregionhist";
            var detector = new TransmittedMTOfXAndYAndSubregionHistDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                MTBins = new DoubleRange(0, 100, 4),
                FractionalMTBins = new DoubleRange(0, 10, 1),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1: 2x2x3
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                SecondMoment = new double[,,]
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                },
                FractionalMT = new double[,,,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1 x NumRegions x FractionalMTBins.Count+1: 2x2x3x2x2
                {
                    {
                        {
                            {
                                { 1, 2 }, { 3, 4 }, { 5, 6 }
                            },
                            {
                                { 7, 8 }, { 9, 10 }, { 11, 12 }
                            }
                        },
                        {
                            {
                                { 13, 14 }, { 15, 16 }, { 17, 18 }
                            },
                            {
                                { 19, 20 }, { 21, 22 }, { 23, 24 }
                            }
                        }

                    },
                    {
                        {
                            {
                                { 25, 26 }, { 27, 28 }, { 29, 30 }
                            },
                            {
                                { 31, 32 }, { 33, 34 }, { 35, 36 }
                            },
                        },
                        {
                            {
                                { 37, 38 }, { 39, 40 }, { 41, 42 }
                            },
                            {
                                { 43, 44 }, { 45, 46 }, { 47, 48 }
                            },
                        }
                    }
                },

            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.FractionalMT);

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

            Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0, 0]);
            Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 0, 1]);
            Assert.AreEqual(3, detector.FractionalMT[0, 0, 0, 1, 0]);
            Assert.AreEqual(4, detector.FractionalMT[0, 0, 0, 1, 1]);
            Assert.AreEqual(5, detector.FractionalMT[0, 0, 0, 2, 0]);
            Assert.AreEqual(6, detector.FractionalMT[0, 0, 0, 2, 1]);
            Assert.AreEqual(7, detector.FractionalMT[0, 0, 1, 0, 0]);
            Assert.AreEqual(8, detector.FractionalMT[0, 0, 1, 0, 1]);
            Assert.AreEqual(9, detector.FractionalMT[0, 0, 1, 1, 0]);
            Assert.AreEqual(10, detector.FractionalMT[0, 0, 1, 1, 1]);
            Assert.AreEqual(11, detector.FractionalMT[0, 0, 1, 2, 0]);
            Assert.AreEqual(12, detector.FractionalMT[0, 0, 1, 2, 1]);
            Assert.AreEqual(13, detector.FractionalMT[0, 1, 0, 0, 0]);
            Assert.AreEqual(14, detector.FractionalMT[0, 1, 0, 0, 1]);
            Assert.AreEqual(15, detector.FractionalMT[0, 1, 0, 1, 0]);
            Assert.AreEqual(16, detector.FractionalMT[0, 1, 0, 1, 1]);
            Assert.AreEqual(17, detector.FractionalMT[0, 1, 0, 2, 0]);
            Assert.AreEqual(18, detector.FractionalMT[0, 1, 0, 2, 1]);
            Assert.AreEqual(19, detector.FractionalMT[0, 1, 1, 0, 0]);
            Assert.AreEqual(20, detector.FractionalMT[0, 1, 1, 0, 1]);
            Assert.AreEqual(21, detector.FractionalMT[0, 1, 1, 1, 0]);
            Assert.AreEqual(22, detector.FractionalMT[0, 1, 1, 1, 1]);
            Assert.AreEqual(23, detector.FractionalMT[0, 1, 1, 2, 0]);
            Assert.AreEqual(24, detector.FractionalMT[0, 1, 1, 2, 1]);
            Assert.AreEqual(25, detector.FractionalMT[1, 0, 0, 0, 0]);
            Assert.AreEqual(26, detector.FractionalMT[1, 0, 0, 0, 1]);
            Assert.AreEqual(27, detector.FractionalMT[1, 0, 0, 1, 0]);
            Assert.AreEqual(28, detector.FractionalMT[1, 0, 0, 1, 1]);
            Assert.AreEqual(29, detector.FractionalMT[1, 0, 0, 2, 0]);
            Assert.AreEqual(30, detector.FractionalMT[1, 0, 0, 2, 1]);
            Assert.AreEqual(31, detector.FractionalMT[1, 0, 1, 0, 0]);
            Assert.AreEqual(32, detector.FractionalMT[1, 0, 1, 0, 1]);
            Assert.AreEqual(33, detector.FractionalMT[1, 0, 1, 1, 0]);
            Assert.AreEqual(34, detector.FractionalMT[1, 0, 1, 1, 1]);
            Assert.AreEqual(35, detector.FractionalMT[1, 0, 1, 2, 0]);
            Assert.AreEqual(36, detector.FractionalMT[1, 0, 1, 2, 1]);
            Assert.AreEqual(37, detector.FractionalMT[1, 1, 0, 0, 0]);
            Assert.AreEqual(38, detector.FractionalMT[1, 1, 0, 0, 1]);
            Assert.AreEqual(39, detector.FractionalMT[1, 1, 0, 1, 0]);
            Assert.AreEqual(40, detector.FractionalMT[1, 1, 0, 1, 1]);
            Assert.AreEqual(41, detector.FractionalMT[1, 1, 0, 2, 0]);
            Assert.AreEqual(42, detector.FractionalMT[1, 1, 0, 2, 1]);
            Assert.AreEqual(43, detector.FractionalMT[1, 1, 1, 0, 0]);
            Assert.AreEqual(44, detector.FractionalMT[1, 1, 1, 0, 1]);
            Assert.AreEqual(45, detector.FractionalMT[1, 1, 1, 1, 0]);
            Assert.AreEqual(46, detector.FractionalMT[1, 1, 1, 1, 1]);
            Assert.AreEqual(47, detector.FractionalMT[1, 1, 1, 2, 0]);
            Assert.AreEqual(48, detector.FractionalMT[1, 1, 1, 2, 1]);

            if (!tallySecondMoment) return;
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

        #endregion

        #region 4D detectors

        /// <summary>
        /// test to verify that GetBinarySerializers are working correctly for 4D detector.
        /// </summary>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_FluenceOfXAndYAndZAndOmegaDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testfluenceofxandyandzandOmega";
            var detector = new FluenceOfXAndYAndZAndOmegaDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Z = new DoubleRange(0, 1, 3),
                Omega = new DoubleRange(0, 1, 3),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new[,,,] // X.Count-1 x Y.Count-1 x Z.Count-1 x Omega.Count: 2x2x2x3
                {
                    {
                        { 
                            { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne }, 
                            { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                        },
                        { 
                            { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne }, 
                            { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                        }
                    },
                    {
                        {
                            { 13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne, 15 + 15 * Complex.ImaginaryOne}, 
                            { 16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne, 18 + 18 * Complex.ImaginaryOne }
                        },
                        {
                            { 19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne, 21 + 21 * Complex.ImaginaryOne }, 
                            { 22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne, 24 + 24 * Complex.ImaginaryOne }
                        }
                    }
                },
                SecondMoment = new[,,,]
                {
                    {
                        {
                            { 25 + 25 * Complex.ImaginaryOne, 26 + 26 * Complex.ImaginaryOne, 27 + 27 * Complex.ImaginaryOne}, 
                            { 28 + 28 * Complex.ImaginaryOne, 29 + 29 * Complex.ImaginaryOne, 30 + 30 * Complex.ImaginaryOne}
                        },
                        {
                            { 31 + 31 * Complex.ImaginaryOne, 32 + 32 * Complex.ImaginaryOne, 33 + 33 * Complex.ImaginaryOne }, 
                            { 34 + 34 * Complex.ImaginaryOne, 35 + 35 * Complex.ImaginaryOne, 36 + 36 * Complex.ImaginaryOne }
                        }
                    },
                    {
                        {
                            { 37 + 37 * Complex.ImaginaryOne, 38 + 38 * Complex.ImaginaryOne, 39 + 39 * Complex.ImaginaryOne }, 
                            { 40 + 40 * Complex.ImaginaryOne, 41 + 41 * Complex.ImaginaryOne, 42 + 42 * Complex.ImaginaryOne }
                        },
                        {
                            { 43 + 43 * Complex.ImaginaryOne, 44 + 44 * Complex.ImaginaryOne, 45 + 45 * Complex.ImaginaryOne }, 
                            { 46 + 46 * Complex.ImaginaryOne, 47 + 47 * Complex.ImaginaryOne, 48 + 48 * Complex.ImaginaryOne }
                        }
                    }
                }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1 + Complex.ImaginaryOne, detector.Mean[0, 0, 0, 0]);
            Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 0, 0, 1]);
            Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 0, 0, 2]);
            Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[0, 0, 1, 0]);
            Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[0, 0, 1, 1]);
            Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[0, 0, 1, 2]);
            Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.Mean[0, 1, 0, 0]);
            Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.Mean[0, 1, 0, 1]);
            Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.Mean[0, 1, 0, 2]);
            Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.Mean[0, 1, 1, 0]);
            Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.Mean[0, 1, 1, 1]);
            Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.Mean[0, 1, 1, 2]);
            Assert.AreEqual(13 + 13 * Complex.ImaginaryOne, detector.Mean[1, 0, 0, 0]);
            Assert.AreEqual(14 + 14 * Complex.ImaginaryOne, detector.Mean[1, 0, 0, 1]);
            Assert.AreEqual(15 + 15 * Complex.ImaginaryOne, detector.Mean[1, 0, 0, 2]);
            Assert.AreEqual(16 + 16 * Complex.ImaginaryOne, detector.Mean[1, 0, 1, 0]);
            Assert.AreEqual(17 + 17 * Complex.ImaginaryOne, detector.Mean[1, 0, 1, 1]);
            Assert.AreEqual(18 + 18 * Complex.ImaginaryOne, detector.Mean[1, 0, 1, 2]);
            Assert.AreEqual(19 + 19 * Complex.ImaginaryOne, detector.Mean[1, 1, 0, 0]);
            Assert.AreEqual(20 + 20 * Complex.ImaginaryOne, detector.Mean[1, 1, 0, 1]);
            Assert.AreEqual(21 + 21 * Complex.ImaginaryOne, detector.Mean[1, 1, 0, 2]);
            Assert.AreEqual(22 + 22 * Complex.ImaginaryOne, detector.Mean[1, 1, 1, 0]);
            Assert.AreEqual(23 + 23 * Complex.ImaginaryOne, detector.Mean[1, 1, 1, 1]);
            Assert.AreEqual(24 + 24 * Complex.ImaginaryOne, detector.Mean[1, 1, 1, 2]);
            if (!tallySecondMoment) return;
            Assert.AreEqual(25 + 25 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0, 0]);
            Assert.AreEqual(26 + 26 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0, 1]);
            Assert.AreEqual(27 + 27 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0, 2]);
            Assert.AreEqual(28 + 28 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1, 0]);
            Assert.AreEqual(29 + 29 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1, 1]);
            Assert.AreEqual(30 + 30 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1, 2]);
            Assert.AreEqual(31 + 31 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0, 0]);
            Assert.AreEqual(32 + 32 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0, 1]);
            Assert.AreEqual(33 + 33 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0, 2]);
            Assert.AreEqual(34 + 34 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1, 0]);
            Assert.AreEqual(35 + 35 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1, 1]);
            Assert.AreEqual(36 + 36 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1, 2]);
            Assert.AreEqual(37 + 37 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0, 0]);
            Assert.AreEqual(38 + 38 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0, 1]);
            Assert.AreEqual(39 + 39 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0, 2]);
            Assert.AreEqual(40 + 40 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1, 0]);
            Assert.AreEqual(41 + 41 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1, 1]);
            Assert.AreEqual(42 + 42 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1, 2]);
            Assert.AreEqual(43 + 43 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0, 0]);
            Assert.AreEqual(44 + 44 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0, 1]);
            Assert.AreEqual(45 + 45 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0, 2]);
            Assert.AreEqual(46 + 46 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1, 0]);
            Assert.AreEqual(47 + 47 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1, 1]);
            Assert.AreEqual(48 + 48 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1, 2]);
        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_FluenceOfXAndYAndZAndTimeDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testfluenceofxandyandzandtime";
            var detector = new FluenceOfXAndYAndZAndTimeDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Z = new DoubleRange(0, 1, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Z.Count-1 x Time.Count-1: 2x2x2x3
                {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                }
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
            if (!tallySecondMoment) return;
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_pMCROfXAndYAndTimeAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testpmcrofxandyandtimeandsubregion";
            var detector = new pMCROfXAndYAndTimeAndSubregionDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 3),
                TallySecondMoment = tallySecondMoment,
                PerturbedOps = new List<OpticalProperties> { new() },
                PerturbedRegionsIndices = new List<int> { 1 },
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Z.Count-1 x Time.Count-1 x NumOfRegions: 2x2x2x3
                {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                },
                ROfXAndY = new double[,] // X.Count-1 x Y.Count-1: 2x2
                    { { 49, 50 }, { 51, 52 } },
                ROfXAndYSecondMoment = new double[,]
                    { { 53, 54 }, { 55, 56 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.ROfXAndY, detector.ROfXAndYSecondMoment);

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
            Assert.AreEqual(49, detector.ROfXAndY[0, 0]);
            Assert.AreEqual(50, detector.ROfXAndY[0, 1]);
            Assert.AreEqual(51, detector.ROfXAndY[1, 0]);
            Assert.AreEqual(52, detector.ROfXAndY[1, 1]);

            if (!tallySecondMoment) return;
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
            Assert.AreEqual(53, detector.ROfXAndYSecondMoment[0, 0]);
            Assert.AreEqual(54, detector.ROfXAndYSecondMoment[0, 1]);
            Assert.AreEqual(55, detector.ROfXAndYSecondMoment[1, 0]);
            Assert.AreEqual(56, detector.ROfXAndYSecondMoment[1, 1]);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_pMCROfXAndYAndTimeAndSubregionRecessedDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testpmcrofxandyandtimeandsubregionrecessed";
            var detector = new pMCROfXAndYAndTimeAndSubregionRecessedDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 3),
                ZPlane = -0.1,
                TallySecondMoment = tallySecondMoment,
                PerturbedOps = new List<OpticalProperties> { new() },
                PerturbedRegionsIndices = new List<int> { 1 },
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Time.Count-1 x NumSubregions: 2x2x2x3
                {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                },
                ROfXAndY = new double[,] // X.Count-1 x Y.Count-1: 2x2
                    { { 49, 50 }, { 51, 52 } },
                ROfXAndYSecondMoment = new double[,]
                    { { 53, 54 }, { 55, 56 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.ROfXAndY, detector.ROfXAndYSecondMoment);

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
            Assert.AreEqual(49, detector.ROfXAndY[0, 0]);
            Assert.AreEqual(50, detector.ROfXAndY[0, 1]);
            Assert.AreEqual(51, detector.ROfXAndY[1, 0]);
            Assert.AreEqual(52, detector.ROfXAndY[1, 1]);

            if (!tallySecondMoment) return;
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
            Assert.AreEqual(53, detector.ROfXAndYSecondMoment[0, 0]);
            Assert.AreEqual(54, detector.ROfXAndYSecondMoment[0, 1]);
            Assert.AreEqual(55, detector.ROfXAndYSecondMoment[1, 0]);
            Assert.AreEqual(56, detector.ROfXAndYSecondMoment[1, 1]);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_ROfXAndYAndThetaAndPhiDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandthetaandphi";
            var detector = new ROfXAndYAndThetaAndPhiDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Theta = new DoubleRange(0, 1, 3),
                Phi = new DoubleRange(0, 1, 4),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Theta.Count-1 x Phi.Count-1: 2x2x2x3
                {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                }
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
            if (!tallySecondMoment) return;
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_ROfXAndYAndTimeAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandtimeandsubregion";
            var detector = new ROfXAndYAndTimeAndSubregionDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 3),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Time.Count-1 x NumOfRegions: 2x2x2x3
                {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                },
                ROfXAndY = new double[,] // X.Count-1 x Y.Count-1: 2x2
                    { { 49, 50 }, { 51, 52 } },
                ROfXAndYSecondMoment = new double[,]
                    { { 53, 54 }, { 55, 56 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.ROfXAndY, detector.ROfXAndYSecondMoment);

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
            Assert.AreEqual(49, detector.ROfXAndY[0, 0]);
            Assert.AreEqual(50, detector.ROfXAndY[0, 1]);
            Assert.AreEqual(51, detector.ROfXAndY[1, 0]);
            Assert.AreEqual(52, detector.ROfXAndY[1, 1]);

            if (!tallySecondMoment) return;
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
            Assert.AreEqual(53, detector.ROfXAndYSecondMoment[0, 0]);
            Assert.AreEqual(54, detector.ROfXAndYSecondMoment[0, 1]);
            Assert.AreEqual(55, detector.ROfXAndYSecondMoment[1, 0]);
            Assert.AreEqual(56, detector.ROfXAndYSecondMoment[1, 1]);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_ROfXAndYAndTimeAndSubregionRecessedDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testrofxandyandtimeandsubregionrecessed";
            var detector = new ROfXAndYAndTimeAndSubregionRecessedDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 3),
                ZPlane = -0.1,
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Time.Count-1 x NumRegions: 2x2x2x3
                { 
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                },
                ROfXAndY = new double[,] // X.Count-1 x Y.Count-1: 2x2
                    { { 49, 50 }, { 51, 52 } },
                ROfXAndYSecondMoment = new double[,]
                    { { 53, 54 }, { 55, 56 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.ROfXAndY, detector.ROfXAndYSecondMoment);

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
            Assert.AreEqual(49, detector.ROfXAndY[0, 0]);
            Assert.AreEqual(50, detector.ROfXAndY[0, 1]);
            Assert.AreEqual(51, detector.ROfXAndY[1, 0]);
            Assert.AreEqual(52, detector.ROfXAndY[1, 1]);

            if (!tallySecondMoment) return;
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
            Assert.AreEqual(53, detector.ROfXAndYSecondMoment[0, 0]);
            Assert.AreEqual(54, detector.ROfXAndYSecondMoment[0, 1]);
            Assert.AreEqual(55, detector.ROfXAndYSecondMoment[1, 0]);
            Assert.AreEqual(56, detector.ROfXAndYSecondMoment[1, 1]);
        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
           Validate_TOfXAndYAndTimeAndSubregionDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testtofxandyandtimeandsubregion";
            var detector = new TOfXAndYAndTimeAndSubregionDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Time = new DoubleRange(0, 1, 3),
                TallySecondMoment = tallySecondMoment,
                Name = detectorName,
                Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Time.Count-1 x NumOfRegions: 2x2x2x3
                {
                    {
                        { { 1, 2, 3 }, { 4, 5, 6 } },
                        { { 7, 8, 9 }, { 10, 11, 12 } }
                    },
                    {
                        { { 13, 14, 15 }, { 16, 17, 18 } },
                        { { 19, 20, 21 }, { 22, 23, 24 } }
                    }
                },
                SecondMoment = new double[,,,]
                {
                    {
                        { { 25, 26, 27 }, { 28, 29, 30 } },
                        { { 31, 32, 33 }, { 34, 35, 36 } }
                    },
                    {
                        { { 37, 38, 39 }, { 40, 41, 42 } },
                        { { 43, 44, 45 }, { 46, 47, 48 } }
                    }
                },
                TOfXAndY = new double[,] // X.Count-1 x Y.Count-1: 2x2
                    { { 49, 50 }, { 51, 52 } },
                TOfXAndYSecondMoment = new double[,]
                    { { 53, 54 }, { 55, 56 } }
            };

            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.TOfXAndY, detector.TOfXAndYSecondMoment);

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
            Assert.AreEqual(49, detector.TOfXAndY[0, 0]);
            Assert.AreEqual(50, detector.TOfXAndY[0, 1]);
            Assert.AreEqual(51, detector.TOfXAndY[1, 0]);
            Assert.AreEqual(52, detector.TOfXAndY[1, 1]);

            if (!tallySecondMoment) return;
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

            Assert.AreEqual(53, detector.TOfXAndYSecondMoment[0, 0]);
            Assert.AreEqual(54, detector.TOfXAndYSecondMoment[0, 1]);
            Assert.AreEqual(55, detector.TOfXAndYSecondMoment[1, 0]);
            Assert.AreEqual(56, detector.TOfXAndYSecondMoment[1, 1]);
        }

        #endregion

        #region 5D detectors

        /// <summary>
        /// test to verify that GetBinarySerializers are working correctly for 5D detector.
        /// </summary>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
     Validate_FluenceOfXAndYAndZAndStartingXAndYDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testfluenceofxandyandzandstartingxandy";
            var detector = new FluenceOfXAndYAndZAndStartingXAndYDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Z = new DoubleRange(0, 1, 3),
                StartingX = new DoubleRange(0, 1, 3),
                StartingY = new DoubleRange(0, 1, 4),
                Name = detectorName,
                TallySecondMoment = tallySecondMoment, 
                Mean = new double[,,,,] // X.Count-1 x Y.Count-1 x Z.Count-1 x Theta.Count-1 x Phi.Count-1: 2x2x2x2x3
                {
                    {
                        {
                            {
                                { 1, 2, 3 },
                                { 4, 5, 6 },
                            },
                            {
                                { 7, 8, 9 },
                                { 10, 11, 12 }
                            },
                        },
                        {
                            {
                                { 13, 14, 15 },
                                { 16, 17, 18 }
                            },
                            {
                                { 19, 20, 21 },
                                { 22, 23, 24 }
                            }
                        },
                    },
                    {
                        {
                            {
                                { 25, 26, 27 },
                                { 28, 29, 30 }
                            },
                            {
                                { 31, 32, 33 },
                                { 34, 35, 36 }
                            }
                        },
                        {
                            {
                                { 37, 38, 39 },
                                { 40, 41, 42 }
                            },
                            {
                                { 43, 44, 45 },
                                { 46, 47, 48 }
                            }
                        }
                    }
                },
                SecondMoment = new double[,,,,]
                {
                    {
                        {
                            {
                                { 1, 2, 3 },
                                { 4, 5, 6 },
                            },
                            {
                                { 7, 8, 9 },
                                { 10, 11, 12 }
                            },
                        },
                        {
                            {
                                { 13, 14, 15 },
                                { 16, 17, 18 }
                            },
                            {
                                { 19, 20, 21 },
                                { 22, 23, 24 }
                            }
                        },
                    },
                    {
                        {
                            {
                                { 25, 26, 27 },
                                { 28, 29, 30 }
                            },
                            {
                                { 31, 32, 33 },
                                { 34, 35, 36 }
                            }
                        },
                        {
                            {
                                { 37, 38, 39 },
                                { 40, 41, 42 }
                            },
                            {
                                { 43, 44, 45 },
                                { 46, 47, 48 }
                            }
                        }
                    }
                },
                StartingXYCount = new double[,] // StartingX.Count-1 x StartingY.Count-1: 2x3
                {
                    { 49, 50, 51 },
                    { 52, 53, 54 }
                }
                
            };
            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
                detector.StartingXYCount);

            Assert.AreEqual(1, detector.Mean[0, 0, 0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 0, 0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 0, 0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 0, 0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[0, 0, 1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[0, 0, 1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[0, 0, 1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[0, 0, 1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[0, 0, 1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[0, 0, 1, 1, 2]);

            Assert.AreEqual(13, detector.Mean[0, 1, 0, 0, 0]);
            Assert.AreEqual(14, detector.Mean[0, 1, 0, 0, 1]);
            Assert.AreEqual(15, detector.Mean[0, 1, 0, 0, 2]);
            Assert.AreEqual(16, detector.Mean[0, 1, 0, 1, 0]);
            Assert.AreEqual(17, detector.Mean[0, 1, 0, 1, 1]);
            Assert.AreEqual(18, detector.Mean[0, 1, 0, 1, 2]);
            Assert.AreEqual(19, detector.Mean[0, 1, 1, 0, 0]);
            Assert.AreEqual(20, detector.Mean[0, 1, 1, 0, 1]);
            Assert.AreEqual(21, detector.Mean[0, 1, 1, 0, 2]);
            Assert.AreEqual(22, detector.Mean[0, 1, 1, 1, 0]);
            Assert.AreEqual(23, detector.Mean[0, 1, 1, 1, 1]);
            Assert.AreEqual(24, detector.Mean[0, 1, 1, 1, 2]);

            Assert.AreEqual(25, detector.Mean[1, 0, 0, 0, 0]);
            Assert.AreEqual(26, detector.Mean[1, 0, 0, 0, 1]);
            Assert.AreEqual(27, detector.Mean[1, 0, 0, 0, 2]);
            Assert.AreEqual(28, detector.Mean[1, 0, 0, 1, 0]);
            Assert.AreEqual(29, detector.Mean[1, 0, 0, 1, 1]);
            Assert.AreEqual(30, detector.Mean[1, 0, 0, 1, 2]);
            Assert.AreEqual(31, detector.Mean[1, 0, 1, 0, 0]);
            Assert.AreEqual(32, detector.Mean[1, 0, 1, 0, 1]);
            Assert.AreEqual(33, detector.Mean[1, 0, 1, 0, 2]);
            Assert.AreEqual(34, detector.Mean[1, 0, 1, 1, 0]);
            Assert.AreEqual(35, detector.Mean[1, 0, 1, 1, 1]);
            Assert.AreEqual(36, detector.Mean[1, 0, 1, 1, 2]);

            Assert.AreEqual(37, detector.Mean[1, 1, 0, 0, 0]);
            Assert.AreEqual(38, detector.Mean[1, 1, 0, 0, 1]);
            Assert.AreEqual(39, detector.Mean[1, 1, 0, 0, 2]);
            Assert.AreEqual(40, detector.Mean[1, 1, 0, 1, 0]);
            Assert.AreEqual(41, detector.Mean[1, 1, 0, 1, 1]);
            Assert.AreEqual(42, detector.Mean[1, 1, 0, 1, 2]);
            Assert.AreEqual(43, detector.Mean[1, 1, 1, 0, 0]);
            Assert.AreEqual(44, detector.Mean[1, 1, 1, 0, 1]);
            Assert.AreEqual(45, detector.Mean[1, 1, 1, 0, 2]);
            Assert.AreEqual(46, detector.Mean[1, 1, 1, 1, 0]);
            Assert.AreEqual(47, detector.Mean[1, 1, 1, 1, 1]);
            Assert.AreEqual(48, detector.Mean[1, 1, 1, 1, 2]);

            Assert.AreEqual(49, detector.StartingXYCount[0, 0]);
            Assert.AreEqual(50, detector.StartingXYCount[0, 1]);
            Assert.AreEqual(51, detector.StartingXYCount[0, 2]);
            Assert.AreEqual(52, detector.StartingXYCount[1, 0]);
            Assert.AreEqual(53, detector.StartingXYCount[1, 1]);
            Assert.AreEqual(54, detector.StartingXYCount[1, 2]);

            if (!tallySecondMoment) return;
            Assert.AreEqual(1, detector.SecondMoment[0, 0, 0, 0, 0]);
            Assert.AreEqual(2, detector.SecondMoment[0, 0, 0, 0, 1]);
            Assert.AreEqual(3, detector.SecondMoment[0, 0, 0, 0, 2]);
            Assert.AreEqual(4, detector.SecondMoment[0, 0, 0, 1, 0]);
            Assert.AreEqual(5, detector.SecondMoment[0, 0, 0, 1, 1]);
            Assert.AreEqual(6, detector.SecondMoment[0, 0, 0, 1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0, 1, 0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 0, 1, 0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 0, 1, 0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[0, 0, 1, 1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[0, 0, 1, 1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[0, 0, 1, 1, 2]);

            Assert.AreEqual(13, detector.SecondMoment[0, 1, 0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 1, 0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 1, 0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[0, 1, 1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[0, 1, 1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[0, 1, 1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[0, 1, 1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[0, 1, 1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[0, 1, 1, 1, 2]);

            Assert.AreEqual(25, detector.SecondMoment[1, 0, 0, 0, 0]);
            Assert.AreEqual(26, detector.SecondMoment[1, 0, 0, 0, 1]);
            Assert.AreEqual(27, detector.SecondMoment[1, 0, 0, 0, 2]);
            Assert.AreEqual(28, detector.SecondMoment[1, 0, 0, 1, 0]);
            Assert.AreEqual(29, detector.SecondMoment[1, 0, 0, 1, 1]);
            Assert.AreEqual(30, detector.SecondMoment[1, 0, 0, 1, 2]);
            Assert.AreEqual(31, detector.SecondMoment[1, 0, 1, 0, 0]);
            Assert.AreEqual(32, detector.SecondMoment[1, 0, 1, 0, 1]);
            Assert.AreEqual(33, detector.SecondMoment[1, 0, 1, 0, 2]);
            Assert.AreEqual(34, detector.SecondMoment[1, 0, 1, 1, 0]);
            Assert.AreEqual(35, detector.SecondMoment[1, 0, 1, 1, 1]);
            Assert.AreEqual(36, detector.SecondMoment[1, 0, 1, 1, 2]);

            Assert.AreEqual(37, detector.SecondMoment[1, 1, 0, 0, 0]);
            Assert.AreEqual(38, detector.SecondMoment[1, 1, 0, 0, 1]);
            Assert.AreEqual(39, detector.SecondMoment[1, 1, 0, 0, 2]);
            Assert.AreEqual(40, detector.SecondMoment[1, 1, 0, 1, 0]);
            Assert.AreEqual(41, detector.SecondMoment[1, 1, 0, 1, 1]);
            Assert.AreEqual(42, detector.SecondMoment[1, 1, 0, 1, 2]);
            Assert.AreEqual(43, detector.SecondMoment[1, 1, 1, 0, 0]);
            Assert.AreEqual(44, detector.SecondMoment[1, 1, 1, 0, 1]);
            Assert.AreEqual(45, detector.SecondMoment[1, 1, 1, 0, 2]);
            Assert.AreEqual(46, detector.SecondMoment[1, 1, 1, 1, 0]);
            Assert.AreEqual(47, detector.SecondMoment[1, 1, 1, 1, 1]);
            Assert.AreEqual(48, detector.SecondMoment[1, 1, 1, 1, 2]);

        }


        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            Validate_RadianceOfXAndYAndZAndThetaAndPhiDetector_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
        {
            const string detectorName = "testradianceofxandyandzandthetaandphi";
            var detector = new RadianceOfXAndYAndZAndThetaAndPhiDetector
            {
                X = new DoubleRange(-10, 10, 3),
                Y = new DoubleRange(-10, 10, 3),
                Z = new DoubleRange(0, 1, 3),
                Theta = new DoubleRange(0, 1, 3),
                Phi = new DoubleRange(0, 1, 4),
                Name = detectorName,
                TallySecondMoment = tallySecondMoment, 
                Mean = new double[,,,,] // X.Count-1 x Y.Count-1 x Z.Count-1 x Theta.Count-1 x Phi.Count-1: 2x2x2x2x3
                {
                    {
                        {
                            {
                                { 1, 2, 3 },
                                { 4, 5, 6 },
                            },
                            {
                                { 7, 8, 9 },
                                { 10, 11, 12 }
                            },
                        },
                        {
                            {
                                { 13, 14, 15 },
                                { 16, 17, 18 }
                            },
                            {
                                { 19, 20, 21 },
                                { 22, 23, 24 }
                            }
                        },
                    },
                    {
                        {
                            {
                                { 25, 26, 27 },
                                { 28, 29, 30 }
                            },
                            {
                                { 31, 32, 33 },
                                { 34, 35, 36 }
                            }
                        },
                        {
                            {
                                { 37, 38, 39 },
                                { 40, 41, 42 }
                            },
                            {
                                { 43, 44, 45 },
                                { 46, 47, 48 }
                            }
                        }
                    }
                },
                SecondMoment = new double[,,,,]
                {
                    {
                        {
                            {
                                { 1, 2, 3 },
                                { 4, 5, 6 },
                            },
                            {
                                { 7, 8, 9 },
                                { 10, 11, 12 }
                            },
                        },
                        {
                            {
                                { 13, 14, 15 },
                                { 16, 17, 18 }
                            },
                            {
                                { 19, 20, 21 },
                                { 22, 23, 24 }
                            }
                        },
                    },
                    {
                        {
                            {
                                { 25, 26, 27 },
                                { 28, 29, 30 }
                            },
                            {
                                { 31, 32, 33 },
                                { 34, 35, 36 }
                            }
                        },
                        {
                            {
                                { 37, 38, 39 },
                                { 40, 41, 42 }
                            },
                            {
                                { 43, 44, 45 },
                                { 46, 47, 48 }
                            }
                        }
                    }
                }
            };
            DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

            Assert.AreEqual(1, detector.Mean[0, 0, 0, 0, 0]);
            Assert.AreEqual(2, detector.Mean[0, 0, 0, 0, 1]);
            Assert.AreEqual(3, detector.Mean[0, 0, 0, 0, 2]);
            Assert.AreEqual(4, detector.Mean[0, 0, 0, 1, 0]);
            Assert.AreEqual(5, detector.Mean[0, 0, 0, 1, 1]);
            Assert.AreEqual(6, detector.Mean[0, 0, 0, 1, 2]);
            Assert.AreEqual(7, detector.Mean[0, 0, 1, 0, 0]);
            Assert.AreEqual(8, detector.Mean[0, 0, 1, 0, 1]);
            Assert.AreEqual(9, detector.Mean[0, 0, 1, 0, 2]);
            Assert.AreEqual(10, detector.Mean[0, 0, 1, 1, 0]);
            Assert.AreEqual(11, detector.Mean[0, 0, 1, 1, 1]);
            Assert.AreEqual(12, detector.Mean[0, 0, 1, 1, 2]);

            Assert.AreEqual(13, detector.Mean[0, 1, 0, 0, 0]);
            Assert.AreEqual(14, detector.Mean[0, 1, 0, 0, 1]);
            Assert.AreEqual(15, detector.Mean[0, 1, 0, 0, 2]);
            Assert.AreEqual(16, detector.Mean[0, 1, 0, 1, 0]);
            Assert.AreEqual(17, detector.Mean[0, 1, 0, 1, 1]);
            Assert.AreEqual(18, detector.Mean[0, 1, 0, 1, 2]);
            Assert.AreEqual(19, detector.Mean[0, 1, 1, 0, 0]);
            Assert.AreEqual(20, detector.Mean[0, 1, 1, 0, 1]);
            Assert.AreEqual(21, detector.Mean[0, 1, 1, 0, 2]);
            Assert.AreEqual(22, detector.Mean[0, 1, 1, 1, 0]);
            Assert.AreEqual(23, detector.Mean[0, 1, 1, 1, 1]);
            Assert.AreEqual(24, detector.Mean[0, 1, 1, 1, 2]);

            Assert.AreEqual(25, detector.Mean[1, 0, 0, 0, 0]);
            Assert.AreEqual(26, detector.Mean[1, 0, 0, 0, 1]);
            Assert.AreEqual(27, detector.Mean[1, 0, 0, 0, 2]);
            Assert.AreEqual(28, detector.Mean[1, 0, 0, 1, 0]);
            Assert.AreEqual(29, detector.Mean[1, 0, 0, 1, 1]);
            Assert.AreEqual(30, detector.Mean[1, 0, 0, 1, 2]);
            Assert.AreEqual(31, detector.Mean[1, 0, 1, 0, 0]);
            Assert.AreEqual(32, detector.Mean[1, 0, 1, 0, 1]);
            Assert.AreEqual(33, detector.Mean[1, 0, 1, 0, 2]);
            Assert.AreEqual(34, detector.Mean[1, 0, 1, 1, 0]);
            Assert.AreEqual(35, detector.Mean[1, 0, 1, 1, 1]);
            Assert.AreEqual(36, detector.Mean[1, 0, 1, 1, 2]);

            Assert.AreEqual(37, detector.Mean[1, 1, 0, 0, 0]);
            Assert.AreEqual(38, detector.Mean[1, 1, 0, 0, 1]);
            Assert.AreEqual(39, detector.Mean[1, 1, 0, 0, 2]);
            Assert.AreEqual(40, detector.Mean[1, 1, 0, 1, 0]);
            Assert.AreEqual(41, detector.Mean[1, 1, 0, 1, 1]);
            Assert.AreEqual(42, detector.Mean[1, 1, 0, 1, 2]);
            Assert.AreEqual(43, detector.Mean[1, 1, 1, 0, 0]);
            Assert.AreEqual(44, detector.Mean[1, 1, 1, 0, 1]);
            Assert.AreEqual(45, detector.Mean[1, 1, 1, 0, 2]);
            Assert.AreEqual(46, detector.Mean[1, 1, 1, 1, 0]);
            Assert.AreEqual(47, detector.Mean[1, 1, 1, 1, 1]);
            Assert.AreEqual(48, detector.Mean[1, 1, 1, 1, 2]);

            if (!tallySecondMoment) return;
            Assert.AreEqual(1, detector.SecondMoment[0, 0, 0, 0, 0]);
            Assert.AreEqual(2, detector.SecondMoment[0, 0, 0, 0, 1]);
            Assert.AreEqual(3, detector.SecondMoment[0, 0, 0, 0, 2]);
            Assert.AreEqual(4, detector.SecondMoment[0, 0, 0, 1, 0]);
            Assert.AreEqual(5, detector.SecondMoment[0, 0, 0, 1, 1]);
            Assert.AreEqual(6, detector.SecondMoment[0, 0, 0, 1, 2]);
            Assert.AreEqual(7, detector.SecondMoment[0, 0, 1, 0, 0]);
            Assert.AreEqual(8, detector.SecondMoment[0, 0, 1, 0, 1]);
            Assert.AreEqual(9, detector.SecondMoment[0, 0, 1, 0, 2]);
            Assert.AreEqual(10, detector.SecondMoment[0, 0, 1, 1, 0]);
            Assert.AreEqual(11, detector.SecondMoment[0, 0, 1, 1, 1]);
            Assert.AreEqual(12, detector.SecondMoment[0, 0, 1, 1, 2]);

            Assert.AreEqual(13, detector.SecondMoment[0, 1, 0, 0, 0]);
            Assert.AreEqual(14, detector.SecondMoment[0, 1, 0, 0, 1]);
            Assert.AreEqual(15, detector.SecondMoment[0, 1, 0, 0, 2]);
            Assert.AreEqual(16, detector.SecondMoment[0, 1, 0, 1, 0]);
            Assert.AreEqual(17, detector.SecondMoment[0, 1, 0, 1, 1]);
            Assert.AreEqual(18, detector.SecondMoment[0, 1, 0, 1, 2]);
            Assert.AreEqual(19, detector.SecondMoment[0, 1, 1, 0, 0]);
            Assert.AreEqual(20, detector.SecondMoment[0, 1, 1, 0, 1]);
            Assert.AreEqual(21, detector.SecondMoment[0, 1, 1, 0, 2]);
            Assert.AreEqual(22, detector.SecondMoment[0, 1, 1, 1, 0]);
            Assert.AreEqual(23, detector.SecondMoment[0, 1, 1, 1, 1]);
            Assert.AreEqual(24, detector.SecondMoment[0, 1, 1, 1, 2]);

            Assert.AreEqual(25, detector.SecondMoment[1, 0, 0, 0, 0]);
            Assert.AreEqual(26, detector.SecondMoment[1, 0, 0, 0, 1]);
            Assert.AreEqual(27, detector.SecondMoment[1, 0, 0, 0, 2]);
            Assert.AreEqual(28, detector.SecondMoment[1, 0, 0, 1, 0]);
            Assert.AreEqual(29, detector.SecondMoment[1, 0, 0, 1, 1]);
            Assert.AreEqual(30, detector.SecondMoment[1, 0, 0, 1, 2]);
            Assert.AreEqual(31, detector.SecondMoment[1, 0, 1, 0, 0]);
            Assert.AreEqual(32, detector.SecondMoment[1, 0, 1, 0, 1]);
            Assert.AreEqual(33, detector.SecondMoment[1, 0, 1, 0, 2]);
            Assert.AreEqual(34, detector.SecondMoment[1, 0, 1, 1, 0]);
            Assert.AreEqual(35, detector.SecondMoment[1, 0, 1, 1, 1]);
            Assert.AreEqual(36, detector.SecondMoment[1, 0, 1, 1, 2]);

            Assert.AreEqual(37, detector.SecondMoment[1, 1, 0, 0, 0]);
            Assert.AreEqual(38, detector.SecondMoment[1, 1, 0, 0, 1]);
            Assert.AreEqual(39, detector.SecondMoment[1, 1, 0, 0, 2]);
            Assert.AreEqual(40, detector.SecondMoment[1, 1, 0, 1, 0]);
            Assert.AreEqual(41, detector.SecondMoment[1, 1, 0, 1, 1]);
            Assert.AreEqual(42, detector.SecondMoment[1, 1, 0, 1, 2]);
            Assert.AreEqual(43, detector.SecondMoment[1, 1, 1, 0, 0]);
            Assert.AreEqual(44, detector.SecondMoment[1, 1, 1, 0, 1]);
            Assert.AreEqual(45, detector.SecondMoment[1, 1, 1, 0, 2]);
            Assert.AreEqual(46, detector.SecondMoment[1, 1, 1, 1, 0]);
            Assert.AreEqual(47, detector.SecondMoment[1, 1, 1, 1, 1]);
            Assert.AreEqual(48, detector.SecondMoment[1, 1, 1, 1, 2]);
        }

        #endregion
    }
}
