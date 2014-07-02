using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class OutputTests
    {
        /// <summary>
        /// Test to check that deserialized ROfAngleDetector is correct
        /// </summary>
        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var detectorList =
                new List<IDetector> 
                {
                    new ROfAngleDetector(
                        new DoubleRange(0, Math.PI, 10), 
                        true, // tally SecondMoment
                        "testName")
                };
            var output = new SimulationOutput(new SimulationInput(), detectorList);

            var detector = (ROfAngleDetector)output.ResultsDictionary["testName"];
            var angle = detector.Angle;
            Assert.AreEqual(angle.Start, 0d);
            Assert.AreEqual(angle.Stop, Math.PI);
            Assert.AreEqual(angle.Count, 10);
        }
        /// <summary>
        /// Test to check that addition of "1" to detector name is successful when
        /// two detectors with same name are added to ResultsDictionary
        /// </summary>
        [Test]
        public void validate_whether_two_detectors_with_same_name_are_added_to_ResultsDictionary_correctly()
        {
            var detectorList =
                new List<IDetector> 
                {
                    new ROfRhoDetector(new DoubleRange(0, 10, 10), false, "testName"),
                    new ROfRhoDetector(new DoubleRange(0, 20, 20), false, "testName")
                };
            SimulationOutput output = new SimulationOutput(new SimulationInput(), detectorList);
            var detector = (ROfRhoDetector)output.ResultsDictionary["testName"];
            var rho = detector.Rho;
            Assert.AreEqual(rho.Start, 0d);
            Assert.AreEqual(rho.Stop, 10);
            Assert.AreEqual(rho.Count, 10);
            var detector1 = (ROfRhoDetector)output.ResultsDictionary["testName1"];
            var rho1 = detector1.Rho;
            Assert.AreEqual(rho1.Start, 0d);
            Assert.AreEqual(rho1.Stop, 20);
            Assert.AreEqual(rho1.Count, 20);
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
