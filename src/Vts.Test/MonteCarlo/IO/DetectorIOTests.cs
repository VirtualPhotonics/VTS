using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DetectorIOTests
    {
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 1D detector.
        /// </summary>
        [Test]
        public void validate_1D_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "test1D";
            IDetector detector = new ROfRhoDetector(new DoubleRange(0, 10, 11), detectorName);
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = DetectorIO.ReadDetectorFromFile(TallyType.ROfRho, detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
        }        
        /// <summary>
        /// test to verify that DetectorIO.WriteDetectorToFile and DetectorIO.ReadDetectorToFile
        /// are working correctly for 2D detector.
        /// </summary>
        [Test]
        public void validate_2D_deserialized_class_is_correct_when_using_WriteReadDetectorToFile()
        {
            string detectorName = "test2D";
            IDetector detector = new ROfRhoAndTimeDetector(
                new DoubleRange(0, 10, 11), 
                new DoubleRange(0, 1, 101),
                new MultiLayerTissue(),
                detectorName);
            DetectorIO.WriteDetectorToFile(detector, "");
            var dcloned = DetectorIO.ReadDetectorFromFile(TallyType.ROfRhoAndTime, detectorName, "");

            Assert.AreEqual(dcloned.Name, detectorName);
        }
        

        private static T Clone<T>(T myObject)
        {
            using (MemoryStream ms = new MemoryStream(1024))
            {
                var dcs = new DataContractSerializer(typeof(T));
                dcs.WriteObject(ms, myObject);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }
    }
}
