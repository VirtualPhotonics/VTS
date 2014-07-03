using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;
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
            IDetector detector = new RDiffuseDetector() {TallySecondMoment = true, Name=detectorName, Mean = 100};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (RDiffuseDetector)DetectorIO.ReadDetectorFromFile("RDiffuse", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean, 100);
        }
        [Test]
        public void validate_TDiffuseDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtdiffuse";
            IDetector detector = new TDiffuseDetector() {TallySecondMoment = false, Name = detectorName, Mean = 100};
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TDiffuseDetector)DetectorIO.ReadDetectorFromFile("TDiffuse", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean, 100);
        }
        [Test]
        public void validate_ATotalDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testatotal";
            IDetector detector = new ATotalDetector() {TallySecondMoment = true, Name = detectorName, Mean = 100};
            detector.Initialize(new MultiLayerTissue());
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ATotalDetector)DetectorIO.ReadDetectorFromFile("ATotal", detectorName, "");

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
            IDetector detector = new ROfRhoDetector()
            {
                Rho=new DoubleRange(0, 10, 4), TallySecondMoment = false, Name = detectorName, Mean = new double[] {100, 200, 300}
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoDetector)DetectorIO.ReadDetectorFromFile("ROfRho", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_ROfAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testrofangle";
            IDetector detector = new ROfAngleDetector()
            {
                Angle = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
                Mean = new double[] {100, 200, 300}
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfAngleDetector)DetectorIO.ReadDetectorFromFile("ROfAngle", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_TOfAngleDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtofangle";
            IDetector detector = new TOfAngleDetector()
            {
                Angle = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
                Mean = new double[] {100, 200, 300}
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfAngleDetector)DetectorIO.ReadDetectorFromFile("TOfAngle", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_TOfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testtofrho";
            IDetector detector = new TOfRhoDetector()
            {
                Rho = new DoubleRange(0, 10, 4),
                TallySecondMoment = false,
                Name = detectorName,
                Mean = new double[] {100, 200, 300}
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfRhoDetector)DetectorIO.ReadDetectorFromFile("TOfRho", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0], 100);
            Assert.AreEqual(dcloned.Mean[1], 200);
            Assert.AreEqual(dcloned.Mean[2], 300);
        }
        [Test]
        public void validate_pMCMuaMusROfRhoDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testpmcrofrho";
            IDetector detector = new pMCROfRhoDetector()
            {
                Rho = new DoubleRange(0, 10, 4),
                PerturbedOps = new List<OpticalProperties>() {new OpticalProperties()},
                PerturbedRegionsIndices = new List<int>() {1},
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[] {100, 200, 300}
            };
            detector.Initialize(new MultiLayerTissue());
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (pMCROfRhoDetector)DetectorIO.ReadDetectorFromFile("pMCROfRho", detectorName, "");

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
            IDetector detector = new ROfRhoAndTimeDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFile("ROfRhoAndTime", detectorName, "");

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
            IDetector detector = new ROfRhoAndAngleDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndAngleDetector)DetectorIO.ReadDetectorFromFile("ROfRhoAndAngle", detectorName, "");

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
            IDetector detector = new TOfRhoAndAngleDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Angle = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (TOfRhoAndAngleDetector)DetectorIO.ReadDetectorFromFile("TOfRhoAndAngle", detectorName, "");

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
            IDetector detector = new AOfRhoAndZDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = true,
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };
            detector.Initialize(new MultiLayerTissue());

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (AOfRhoAndZDetector)DetectorIO.ReadDetectorFromFile("AOfRhoAndZ", detectorName, "");

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
            IDetector detector = new ROfXAndYDetector()
            {
                X = new DoubleRange(0, 10, 3),
                Y = new DoubleRange(0, 1, 4),
                TallySecondMoment = false,
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfXAndYDetector)DetectorIO.ReadDetectorFromFile("ROfXAndY", detectorName, "");

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
            var tissue = new MultiLayerTissue();
            IDetector detector = new FluenceOfRhoAndZDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };
            detector.Initialize(tissue);

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (FluenceOfRhoAndZDetector)DetectorIO.ReadDetectorFromFile("FluenceOfRhoAndZ", detectorName, "");

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
            IDetector detector = new ROfRhoAndOmegaDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Omega = new DoubleRange(0, 1, 4),
                TallySecondMoment = true, // tally Second Moment
                Name = detectorName,
                Mean = new Complex[,] { { 1 + Complex.ImaginaryOne * 1, 2 + Complex.ImaginaryOne * 2, 3 + Complex.ImaginaryOne * 3, 4 + Complex.ImaginaryOne * 4},
                                        { 5 + Complex.ImaginaryOne * 5, 6 + Complex.ImaginaryOne * 6, 7 + Complex.ImaginaryOne * 7, 8 + Complex.ImaginaryOne * 8} }
     
            };
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ROfRhoAndOmegaDetector)DetectorIO.ReadDetectorFromFile("ROfRhoAndOmega", detectorName, "");

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
            IDetector detector = new ReflectedMTOfRhoAndSubregionHistDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                MTBins = new DoubleRange(0, 10, 3),
                FractionalMTBins = new DoubleRange(0, 1, 11),
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] {{1, 2}, {3, 4}}
            };
            detector.Initialize(new MultiLayerTissue());

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (ReflectedMTOfRhoAndSubregionHistDetector)DetectorIO.ReadDetectorFromFile("ReflectedMTOfRhoAndSubregionHist", detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
            Assert.AreEqual(dcloned.Mean[0, 0], 1);
            Assert.AreEqual(dcloned.Mean[0, 1], 2);
            Assert.AreEqual(dcloned.Mean[1, 0], 3);
            Assert.AreEqual(dcloned.Mean[1, 1], 4);

        }
        [Test]
        public void validate_pMCMuaMusROfRhoAndTimeDetector_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "testpmcmuamusrofrhoandtime";
            IDetector detector = new pMCROfRhoAndTimeDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                PerturbedOps = new List<OpticalProperties>() {new OpticalProperties()},
                PerturbedRegionsIndices = new List<int>() {1},
                TallySecondMoment = true, // tally SecondMoment
                Name = detectorName,
                Mean = new double[,] {{1, 2, 3}, {4, 5, 6}}
            };
            detector.Initialize(new MultiLayerTissue());
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (pMCROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFile("pMCROfRhoAndTime", detectorName, "");

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
            var tissue = new MultiLayerTissue();
            IDetector detector = new FluenceOfRhoAndZAndTimeDetector()
            {
                Rho = new DoubleRange(0, 10, 3),
                Z = new DoubleRange(0, 10, 3),
                Time = new DoubleRange(0, 1, 4),
                TallySecondMoment = true,
                Name = detectorName,
                Mean = new double[,,] {{{1, 2, 3}, {4, 5, 6}}, {{7, 8, 9}, {10, 11, 12}}}
            };
            detector.Initialize(tissue);

            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = (FluenceOfRhoAndZAndTimeDetector)DetectorIO.ReadDetectorFromFile("FluenceOfRhoAndZAndTime", detectorName, "");

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
