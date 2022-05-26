using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SimulationOutputTests
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
                    new ROfAngleDetector()
                    {
                         Angle = new DoubleRange(0, Math.PI, 10), 
                         Name = "testName"
                    }
                       
                };
            var output = new SimulationOutput(new SimulationInput(), detectorList);

            var detector = (ROfAngleDetector)output.ResultsDictionary["testName"];
            var angle = detector.Angle;
            Assert.AreEqual(0d, angle.Start);
            Assert.AreEqual(Math.PI, angle.Stop);
            Assert.AreEqual(10, angle.Count);
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
                    new ROfRhoDetector() { Rho = new DoubleRange(0, 10, 10), TallySecondMoment = false, Name = "testName"},
                    new ROfRhoDetector() { Rho = new DoubleRange(0, 20, 20), TallySecondMoment = false, Name = "testName"}
                };
            SimulationOutput output = new SimulationOutput(new SimulationInput(), detectorList);
            var detector = (ROfRhoDetector)output.ResultsDictionary["testName"];
            var rho = detector.Rho;
            Assert.AreEqual(0d, rho.Start);
            Assert.AreEqual(10, rho.Stop);
            Assert.AreEqual(10, rho.Count);
            var detector1 = (ROfRhoDetector)output.ResultsDictionary["testName1"];
            var rho1 = detector1.Rho;
            Assert.AreEqual(0d, rho1.Start);
            Assert.AreEqual(20, rho1.Stop);
            Assert.AreEqual(20, rho1.Count);
        }

    }
}
