using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DetectorIOTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestDetectors = new()
        {
            // 0D detectors
            "testrdiffuse",
            "testrdiffuse_2", // TallySecondMoment is set to true for this detector
            "testtdiffuse",
            "testatotal",
            "testatotal_2", // TallySecondMoment is set to true for this detector
            // 1D detectors
            "testrofangle",
            "testrofrho",
            "testtofangle",
            "testtofrho",
            "testpmcrofrho",
            "testpmcrofrho_2", // TallySecondMoment is set to true for this detector
            // 2D detectors
            "testrofrhoandangle",
            "testrofrhoandtime",
            "testrofrhoandtime_2", // TallySecondMoment is set to true for this detector
            "testrofrhoandomega",
            "testrofrhoandomega_2", // TallySecondMoment is set to true for this detector
            "testrofxandy",
            "testtofrhoandangle",
            "testrofrhoandangle_2", // TallySecondMoment is set to true for this detector
            "testaofrhoandz",
            "testfluenceofrhoandz",
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
            IDetectorInput detectorInput = new RDiffuseDetectorInput() { TallySecondMoment = true, Name = detectorName };
            var detector = (RDiffuseDetector)detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (RDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean);
            Assert.AreEqual(50, dcloned.SecondMoment); // 0D detectors 2nd moment written to .txt file
        }
        [Test]
        public void Validate_TDiffuseDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testtdiffuse";
            IDetectorInput detectorInput = new TDiffuseDetectorInput() { TallySecondMoment = false, Name = detectorName };
            var detector = (TDiffuseDetector)detectorInput.CreateDetector();
            detector.Mean = 100;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean);
        }
        [Test]
        public void Validate_ATotalDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testatotal";
            IDetectorInput detectorInput = new ATotalDetectorInput() { TallySecondMoment = true, Name = detectorName };
            var detector = (ATotalDetector)detectorInput.CreateDetector();
            detector.Mean = 100;
            detector.SecondMoment = 50;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ATotalDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean);
            Assert.AreEqual(50, dcloned.SecondMoment);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 1D detector.
        /// </summary>
        ///         
        [Test]
        public void Validate_ROfAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrofangle";
            IDetectorInput detectorInput = new ROfAngleDetectorInput()
            {
                Angle = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (ROfAngleDetector)detectorInput.CreateDetector();
            detector.Mean = new double[] { 100, 200, 300 };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean[0]);
            Assert.AreEqual(200, dcloned.Mean[1]);
            Assert.AreEqual(300, dcloned.Mean[2]);
        }
        [Test]
        public void Validate_ROfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrofrho";
            IDetectorInput detectorInput = new ROfRhoDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName
            };
            var detector = (ROfRhoDetector)detectorInput.CreateDetector();
            detector.Mean = new double[] { 100, 200, 300 };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean[0]);
            Assert.AreEqual(200, dcloned.Mean[1]);
            Assert.AreEqual(300, dcloned.Mean[2]);
        }
        [Test]
        public void Validate_TOfAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testtofangle";
            IDetectorInput detectorInput = new TOfAngleDetectorInput()
            {
                Angle = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (TOfAngleDetector)detectorInput.CreateDetector();
            detector.Mean = new double[] { 100, 200, 300 };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean[0]);
            Assert.AreEqual(200, dcloned.Mean[1]);
            Assert.AreEqual(300, dcloned.Mean[2]);
        }
        [Test]
        public void Validate_TOfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testtofrho";
            IDetectorInput detectorInput = new TOfRhoDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (TOfRhoDetector)detectorInput.CreateDetector();
            detector.Mean = new double[] { 100, 200, 300 };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfRhoDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean[0]);
            Assert.AreEqual(200, dcloned.Mean[1]);
            Assert.AreEqual(300, dcloned.Mean[2]);
        }
        [Test]
        public void Validate_pMCROfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testpmcrofrho";
            IDetectorInput detectorInput = new pMCROfRhoDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 4),
                PerturbedOps = new List<OpticalProperties>() { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int>() { 1 },
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (pMCROfRhoDetector)detectorInput.CreateDetector();
            detector.Mean = new double[] { 100, 200, 300 };
            detector.SecondMoment = new double[] { 50, 150, 250 };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (pMCROfRhoDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");
            // ckh: not sure how I would read in 2nd moment data in detector + "_2"

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(100, dcloned.Mean[0]);
            Assert.AreEqual(200, dcloned.Mean[1]);
            Assert.AreEqual(300, dcloned.Mean[2]);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 2D detector.
        /// </summary>
        [Test]
        public void Validate_ROfRhoAndAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrofrhoandangle";
            IDetectorInput detectorInput = new ROfRhoAndAngleDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (ROfRhoAndAngleDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        [Test]
        public void Validate_ROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrofrhoandtime";
            IDetectorInput detectorInput = new ROfRhoAndTimeDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (ROfRhoAndTimeDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        [Test]
        public void Validate_ROfRhoAndOmegaDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrofrhoandomega";
            IDetectorInput detectorInput = new ROfRhoAndOmegaDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Omega = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally Second Moment
                Name = detectorName,
            };
            var detector = (ROfRhoAndOmegaDetector)detectorInput.CreateDetector();
            detector.Mean = new Complex[,]
            {
                {
                    1 + Complex.ImaginaryOne*1, 2 + Complex.ImaginaryOne*2, 3 + Complex.ImaginaryOne*3,
                    4 + Complex.ImaginaryOne*4
                },
                {
                    5 + Complex.ImaginaryOne*5, 6 + Complex.ImaginaryOne*6, 7 + Complex.ImaginaryOne*7,
                    8 + Complex.ImaginaryOne*8
                }
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndOmegaDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual( 1 + Complex.ImaginaryOne * 1, dcloned.Mean[0, 0]);
            Assert.AreEqual( 2 + Complex.ImaginaryOne * 2, dcloned.Mean[0, 1]);
            Assert.AreEqual( 3 + Complex.ImaginaryOne * 3, dcloned.Mean[0, 2]);
            Assert.AreEqual( 4 + Complex.ImaginaryOne * 4, dcloned.Mean[0, 3]);
            Assert.AreEqual( 5 + Complex.ImaginaryOne * 5, dcloned.Mean[1, 0]);
            Assert.AreEqual( 6 + Complex.ImaginaryOne * 6, dcloned.Mean[1, 1]);
            Assert.AreEqual( 7 + Complex.ImaginaryOne * 7, dcloned.Mean[1, 2]);
            Assert.AreEqual( 8 + Complex.ImaginaryOne * 8, dcloned.Mean[1, 3]);
        }
        [Test]
        public void Validate_ROfXAndYDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testrofxandy";
            IDetectorInput detectorInput = new ROfXAndYDetectorInput()
            {
                X = new DoubleRange(0, 10, 3),
                Y = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (ROfXAndYDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfXAndYDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        [Test]
        public void Validate_TOfRhoAndAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testtofrhoandangle";
            IDetectorInput detectorInput = new TOfRhoAndAngleDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (TOfRhoAndAngleDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfRhoAndAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        [Test]
        public void Validate_AOfRhoAndZDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testaofrhoandz";
            IDetectorInput detectorInput = new AOfRhoAndZDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (AOfRhoAndZDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (AOfRhoAndZDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        [Test]
        public void Validate_FluenceOfRhoAndZDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testfluenceofrhoandz";
            IDetectorInput detectorInput = new FluenceOfRhoAndZDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = false, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (FluenceOfRhoAndZDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (FluenceOfRhoAndZDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        [Test]
        public void Validate_ReflectedMTOfRhoAndSubregionHistDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testreflectedmtofrhoandsubregionhist";
            var tissue = new MultiLayerTissue(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.7, 1.33)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                }
            );
            IDetectorInput detectorInput = new ReflectedMTOfRhoAndSubregionHistDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                MTBins = new DoubleRange(0, 10, 3),
                FractionalMTBins = new DoubleRange(0, 1, 2),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (ReflectedMTOfRhoAndSubregionHistDetector)detectorInput.CreateDetector();
            // need to initialize detector so that NumSubregions gets set
            detector.Initialize(tissue, null);
            // Mean has dimensions [Rho.Count - 1, MTBins.Count - 1]
            detector.Mean = new double[,] { { 1, 2 }, { 3, 4 } };
            // FractionalMT has dimensions [Rho.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count + 1]=[2,2,3,3]
            detector.FractionalMT = new double[,,,] { { { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } } }, { { { 19, 20, 21 }, { 22, 23, 24 }, { 25, 26, 27 } }, { { 28, 29, 30 }, { 31, 32, 33 }, { 34, 35, 36 } } } };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ReflectedMTOfRhoAndSubregionHistDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[1, 0]);
            Assert.AreEqual(4, dcloned.Mean[1, 1]);

            Assert.AreEqual(1, dcloned.FractionalMT[0, 0, 0, 0]);
            Assert.AreEqual(2, dcloned.FractionalMT[0, 0, 0, 1]);
            Assert.AreEqual(3, dcloned.FractionalMT[0, 0, 0, 2]);
            Assert.AreEqual(4, dcloned.FractionalMT[0, 0, 1, 0]);
            Assert.AreEqual(5, dcloned.FractionalMT[0, 0, 1, 1]);
            Assert.AreEqual(6, dcloned.FractionalMT[0, 0, 1, 2]);
            Assert.AreEqual(7, dcloned.FractionalMT[0, 0, 2, 0]);
            Assert.AreEqual(8, dcloned.FractionalMT[0, 0, 2, 1]);
            Assert.AreEqual(9, dcloned.FractionalMT[0, 0, 2, 2]);
            Assert.AreEqual(10, dcloned.FractionalMT[0, 1, 0, 0]);
            Assert.AreEqual(11, dcloned.FractionalMT[0, 1, 0, 1]);
            Assert.AreEqual(12, dcloned.FractionalMT[0, 1, 0, 2]);
            Assert.AreEqual(13, dcloned.FractionalMT[0, 1, 1, 0]);
            Assert.AreEqual(14, dcloned.FractionalMT[0, 1, 1, 1]);
            Assert.AreEqual(15, dcloned.FractionalMT[0, 1, 1, 2]);
            Assert.AreEqual(16, dcloned.FractionalMT[0, 1, 2, 0]);
            Assert.AreEqual(17, dcloned.FractionalMT[0, 1, 2, 1]);
            Assert.AreEqual(18, dcloned.FractionalMT[0, 1, 2, 2]);
            Assert.AreEqual(19, dcloned.FractionalMT[1, 0, 0, 0]);
            Assert.AreEqual(20, dcloned.FractionalMT[1, 0, 0, 1]);
            Assert.AreEqual(21, dcloned.FractionalMT[1, 0, 0, 2]);
            Assert.AreEqual(22, dcloned.FractionalMT[1, 0, 1, 0]);
            Assert.AreEqual(23, dcloned.FractionalMT[1, 0, 1, 1]);
            Assert.AreEqual(24, dcloned.FractionalMT[1, 0, 1, 2]);
            Assert.AreEqual(25, dcloned.FractionalMT[1, 0, 2, 0]);
            Assert.AreEqual(26, dcloned.FractionalMT[1, 0, 2, 1]);
            Assert.AreEqual(27, dcloned.FractionalMT[1, 0, 2, 2]);
            Assert.AreEqual(28, dcloned.FractionalMT[1, 1, 0, 0]);
            Assert.AreEqual(29, dcloned.FractionalMT[1, 1, 0, 1]);
            Assert.AreEqual(30, dcloned.FractionalMT[1, 1, 0, 2]);
            Assert.AreEqual(31, dcloned.FractionalMT[1, 1, 1, 0]);
            Assert.AreEqual(32, dcloned.FractionalMT[1, 1, 1, 1]);
            Assert.AreEqual(33, dcloned.FractionalMT[1, 1, 1, 2]);
            Assert.AreEqual(34, dcloned.FractionalMT[1, 1, 2, 0]);
            Assert.AreEqual(35, dcloned.FractionalMT[1, 1, 2, 1]);
            Assert.AreEqual(36, dcloned.FractionalMT[1, 1, 2, 2]);
        }
        [Test]
        public void Validate_pMCROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testpmcrofrhoandtime";
            IDetectorInput detectorInput = new pMCROfRhoAndTimeDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                PerturbedOps = new List<OpticalProperties>() { new OpticalProperties() },
                PerturbedRegionsIndices = new List<int>() { 1 },
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (pMCROfRhoAndTimeDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (pMCROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 2]);
            Assert.AreEqual(4, dcloned.Mean[1, 0]);
            Assert.AreEqual(5, dcloned.Mean[1, 1]);
            Assert.AreEqual(6, dcloned.Mean[1, 2]);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 3D detector.
        /// </summary>
        [Test]
        public void Validate_FluenceOfRhoAndZAndTime_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            const string detectorName = "testfluenceofrhoandzandtime";
            IDetectorInput detectorInput = new FluenceOfRhoAndZAndTimeDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = true,
                Name = detectorName,
            };
            var detector = (FluenceOfRhoAndZAndTimeDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,,] { { { 1, 2, 3 }, { 4, 5, 6 } }, { { 7, 8, 9 }, { 10, 11, 12 } } };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (FluenceOfRhoAndZAndTimeDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(detectorName, dcloned.Name);
            Assert.AreEqual(1, dcloned.Mean[0, 0, 0]);
            Assert.AreEqual(2, dcloned.Mean[0, 0, 1]);
            Assert.AreEqual(3, dcloned.Mean[0, 0, 2]);
            Assert.AreEqual(4, dcloned.Mean[0, 1, 0]);
            Assert.AreEqual(5, dcloned.Mean[0, 1, 1]);
            Assert.AreEqual(6, dcloned.Mean[0, 1, 2]);
            Assert.AreEqual(7, dcloned.Mean[1, 0, 0]);
            Assert.AreEqual(8, dcloned.Mean[1, 0, 1]);
            Assert.AreEqual(9, dcloned.Mean[1, 0, 2]);
            Assert.AreEqual(10, dcloned.Mean[1, 1, 0]);
            Assert.AreEqual(11, dcloned.Mean[1, 1, 1]);
            Assert.AreEqual(12, dcloned.Mean[1, 1, 2]);
        }
    }
}
