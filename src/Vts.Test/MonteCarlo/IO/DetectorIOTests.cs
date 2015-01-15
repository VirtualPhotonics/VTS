using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
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
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 0D detectors.
        /// </summary>
        [Test]
        public void validate_RDiffuseDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrdiffuse";
            IDetectorInput detectorInput = new RDiffuseDetectorInput() {TallySecondMoment = true, Name=detectorName};
            var detector = (RDiffuseDetector) detectorInput.CreateDetector();
            detector.Mean = 100;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (RDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean, 100);
        }
        [Test]
        public void validate_TDiffuseDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtdiffuse";
            IDetectorInput detectorInput = new TDiffuseDetectorInput() {TallySecondMoment = false, Name = detectorName};
            var detector = (TDiffuseDetector) detectorInput.CreateDetector();
            detector.Mean = 100;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TDiffuseDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean, 100);
        }
        [Test]
        public void validate_ATotalDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testatotal";
            IDetectorInput detectorInput = new ATotalDetectorInput() {TallySecondMoment = true, Name = detectorName};
            var detector = (ATotalDetector) detectorInput.CreateDetector();
            detector.Mean = 100;
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ATotalDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean, 100);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 1D detector.
        /// </summary>
        [Test]
        public void validate_ROfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofrho";
            IDetectorInput detectorInput = new ROfRhoDetectorInput()
            {
                Rho=new DoubleRange(0, 10, 4), TallySecondMoment = false, Name = detectorName
            };
            var detector = (ROfRhoDetector) detectorInput.CreateDetector();
            detector.Mean = new double[] {100, 200, 300};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_ROfAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofangle";
            IDetectorInput detectorInput = new ROfAngleDetectorInput()
            {
                Angle = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (ROfAngleDetector) detectorInput.CreateDetector();
            detector.Mean = new double[] {100, 200, 300};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_TOfAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtofangle";
            IDetectorInput detectorInput = new TOfAngleDetectorInput()
            {
                Angle = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (TOfAngleDetector) detectorInput.CreateDetector();
            detector.Mean = new double[] {100, 200, 300};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_TOfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtofrho";
            IDetectorInput detectorInput = new TOfRhoDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (TOfRhoDetector) detectorInput.CreateDetector();
            detector.Mean = new double[] {100, 200, 300};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfRhoDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_pMCROfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testpmcrofrho";
            IDetectorInput detectorInput = new pMCROfRhoDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 4),
                PerturbedOps = new List<OpticalProperties>() {new OpticalProperties()},
                PerturbedRegionsIndices = new List<int>() {1},
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (pMCROfRhoDetector) detectorInput.CreateDetector();
            detector.Mean = new double[] {100, 200, 300};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (pMCROfRhoDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 2D detector.
        /// </summary>
        [Test]
        public void validate_ROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofrhoandtime";
            IDetectorInput detectorInput = new ROfRhoAndTimeDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (ROfRhoAndTimeDetector) detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        [Test]
        public void validate_ROfRhoAndAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofrhoandangle";
            IDetectorInput detectorInput = new ROfRhoAndAngleDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (ROfRhoAndAngleDetector) detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        [Test]
        public void validate_TOfRhoAndAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtofrhoandangle";
            IDetectorInput detectorInput = new TOfRhoAndAngleDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (TOfRhoAndAngleDetector) detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfRhoAndAngleDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        [Test]
        public void validate_AOfRhoAndZDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testaofrhoandz";
            IDetectorInput detectorInput = new AOfRhoAndZDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (AOfRhoAndZDetector) detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (AOfRhoAndZDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        [Test]
        public void validate_ROfXAndYDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofxandy";
            IDetectorInput detectorInput = new ROfXAndYDetectorInput()
            {
                X = new DoubleRange(0, 10, 3),
                Y = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
            };
            var detector = (ROfXAndYDetector) detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfXAndYDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        [Test]
        public void validate_FluenceOfRhoAndZDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testfluenceofrhoandz";
            IDetectorInput detectorInput = new FluenceOfRhoAndZDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = false, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (FluenceOfRhoAndZDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (FluenceOfRhoAndZDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        [Test]
        public void validate_ROfRhoAndOmegaDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofrhoandomega";
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

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1 + Complex.ImaginaryOne * 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2 + Complex.ImaginaryOne * 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3 + Complex.ImaginaryOne * 3);
            Assert.AreEqual(dcloned.Mean[0, 3], 4 + Complex.ImaginaryOne * 4);
            Assert.AreEqual(dcloned.Mean[1, 0], 5 + Complex.ImaginaryOne * 5);
            Assert.AreEqual(dcloned.Mean[1, 1], 6 + Complex.ImaginaryOne * 6);
            Assert.AreEqual(dcloned.Mean[1, 2], 7 + Complex.ImaginaryOne * 7);
            Assert.AreEqual(dcloned.Mean[1, 3], 8 + Complex.ImaginaryOne * 8);
        }
        [Test]
        public void validate_ReflectedMTOfRhoAndSubregionHistDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testReflectedMTOfRhoAndSubregionHist";
            var tissue = new MultiLayerTissue(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 1.0), // upper layer 1mm
                        new OpticalProperties(0.01, 1.0, 0.7, 1.33)), // Tyler's data
                    new LayerTissueRegion(
                        new DoubleRange(1.0, 100.0),
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
                FractionalMTBins = new DoubleRange(0, 1, 3),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
            };
            var detector = (ReflectedMTOfRhoAndSubregionHistDetector)detectorInput.CreateDetector();
            // need to initialize detector so that NumSubregions gets set
            detector.Initialize(tissue);
            // Mean has dimensions [Rho.Count - 1, MTBins.Count - 1]
            detector.Mean = new double[,] {{1, 2}, {3, 4}};
            // FractionalMT has dimensions [Rho.Count - 1, MTBins.Count - 1, NumSubregions, FractionalMTBins.Count - 1]=[2,2,4,2]
            detector.FractionalMT = new double[,,,] {{{{1,2},{3,4},{5,6},{7,8}},{{9,10},{11,12},{13,14},{15,16}}}, {{{17,18},{19,20},{21,22},{23,24}},{{25,26},{27,28},{29,30},{31,32}}}};

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ReflectedMTOfRhoAndSubregionHistDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[1, 0], 3);
            Assert.AreEqual(dcloned.Mean[1, 1], 4);

            Assert.AreEqual(dcloned.FractionalMT[0, 0, 0 ,0], 1);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 0, 1], 2);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 1, 0], 3);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 1, 1], 4);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 2, 0], 5);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 2, 1], 6);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 3, 0], 7);
            Assert.AreEqual(dcloned.FractionalMT[0, 0, 3, 1], 8);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 0, 0], 9);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 0, 1], 10);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 1, 0], 11);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 1, 1], 12);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 2, 0], 13);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 2, 1], 14);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 3, 0], 15);
            Assert.AreEqual(dcloned.FractionalMT[0, 1, 3, 1], 16);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 0, 0], 17);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 0, 1], 18);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 1, 0], 19);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 1, 1], 20);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 2, 0], 21);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 2, 1], 22);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 3, 0], 23);
            Assert.AreEqual(dcloned.FractionalMT[1, 0, 3, 1], 24);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 0, 0], 25);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 0, 1], 26);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 1, 0], 27);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 1, 1], 28);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 2, 0], 29);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 2, 1], 30);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 3, 0], 31);
            Assert.AreEqual(dcloned.FractionalMT[1, 1, 3, 1], 32);
        }
        [Test]
        public void validate_pMCROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testpmcmuamusrofrhoandtime";
            IDetectorInput detectorInput = new pMCROfRhoAndTimeDetectorInput()
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                PerturbedOps = new List<OpticalProperties>() {new OpticalProperties()},
                PerturbedRegionsIndices = new List<int>() {1},
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,               
            };
            var detector = (pMCROfRhoAndTimeDetector)detectorInput.CreateDetector();
            detector.Mean = new double[,] {{1, 2, 3}, {4, 5, 6}};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (pMCROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFile(detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 2], 3);
            Assert.AreEqual(dcloned.Mean[1, 0], 4);
            Assert.AreEqual(dcloned.Mean[1, 1], 5);
            Assert.AreEqual(dcloned.Mean[1, 2], 6);
        }
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 3D detector.
        /// </summary>
        [Test]
        public void validate_FluenceOfRhoAndZAndTime_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testfluenceofrhoandzandtime";
            IDetectorInput detectorInput = new FluenceOfRhoAndZAndTimeDetectorInput()
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

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 0, 1], 2);
            Assert.AreEqual(dcloned.Mean[0, 0, 2], 3);
            Assert.AreEqual(dcloned.Mean[0, 1, 0], 4);
            Assert.AreEqual(dcloned.Mean[0, 1, 1], 5);
            Assert.AreEqual(dcloned.Mean[0, 1, 2], 6);
            Assert.AreEqual(dcloned.Mean[1, 0, 0], 7);
            Assert.AreEqual(dcloned.Mean[1, 0, 1], 8);
            Assert.AreEqual(dcloned.Mean[1, 0, 2], 9);
            Assert.AreEqual(dcloned.Mean[1, 1, 0], 10);
            Assert.AreEqual(dcloned.Mean[1, 1, 1], 11);
            Assert.AreEqual(dcloned.Mean[1, 1, 2], 12);
        }
        //private static Time Clone<Time>(Time myObject)
        //{
        //    using (MemoryStream ms = new MemoryStream(1024))
        //    {
        //        var dcs = new DataContractSerializer(typeof(Time));
        //        dcs.WriteObject(ms, myObject);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        return (Time)dcs.ReadObject(ms);
        //    }
        //}
    }
}
