using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public void Validate_deserialized_class_is_correct()
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

    }
}
