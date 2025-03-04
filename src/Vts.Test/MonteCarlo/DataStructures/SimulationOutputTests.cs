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
                    new ROfAngleDetector
                    {
                         Angle = new DoubleRange(0, Math.PI, 10),
                         Name = "testName"
                    }

                };
            var output = new SimulationOutput(new SimulationInput(), detectorList);

            var detector = (ROfAngleDetector)output.ResultsDictionary["testName"];
            var angle = detector.Angle;
            Assert.That(angle.Start, Is.EqualTo(0d));
            Assert.That(angle.Stop, Is.EqualTo(Math.PI));
            Assert.That(angle.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// Test to check that created arrays for surface fiber detectors that allow multiple fibers
        /// to be defined in a single simulation (rather than having to run a separate simulation for each
        /// fiber definition) are correct.
        /// </summary>
        [Test]
        public void Validate_arrays_for_multiple_SurfaceFiber_detectors_are_correct()
        {
            var detectorList =
                new List<IDetector>
                {
                    new SurfaceFiberDetector
                    {
                        Center = new Position(0, 0, 1.0),
                        Radius = 1.0,
                        TallySecondMoment = true,
                        N = 1.0,
                        NA = 1.0,
                        FinalTissueRegionIndex = 1,
                        Mean = 1.0,
                        SecondMoment = 2.0,
                        TallyCount = 3,
                        Name = "SurfaceFiber1"
                    },
                    new SurfaceFiberDetector
                    {
                        Center = new Position(0, 0, 2.0),
                        Radius = 2.0,
                        TallySecondMoment = true,
                        N = 1.2,
                        NA = 1.2,
                        FinalTissueRegionIndex = 2,
                        Mean = 2.0,
                        SecondMoment = 4.0,
                        TallyCount = 6,
                        Name = "SurfaceFiber2"
                    },

                };
            var output = new SimulationOutput(new SimulationInput(), detectorList);

            // verify first detector specification
            var detector = (SurfaceFiberDetector)output.ResultsDictionary["SurfaceFiber1"];
            Assert.AreEqual(1.0, detector.Center.Z);
            Assert.AreEqual(1.0, detector.Radius);
            Assert.AreEqual(1.0, detector.N);
            Assert.AreEqual(1.0, detector.NA);
            Assert.AreEqual(1, detector.FinalTissueRegionIndex);
            Assert.AreEqual(1.0, detector.Mean);
            Assert.AreEqual(2.0, detector.SecondMoment);
            Assert.AreEqual(3, detector.TallyCount);

            // verify second detector specification
            detector = (SurfaceFiberDetector)output.ResultsDictionary["SurfaceFiber2"];
            Assert.AreEqual(2.0, detector.Center.Z);
            Assert.AreEqual(2.0, detector.Radius);
            Assert.AreEqual(1.2, detector.N);
            Assert.AreEqual(1.2, detector.NA);
            Assert.AreEqual(2, detector.FinalTissueRegionIndex);
            Assert.AreEqual(2.0, detector.Mean);
            Assert.AreEqual(4.0, detector.SecondMoment);
            Assert.AreEqual(6, detector.TallyCount);
        }

        /// <summary>
        /// Test to check that created arrays for slanted recessed fiber detectors that allow multiple fibers
        /// to be defined in a single simulation (rather than having to run a separate simulation for each
        /// fiber definition) are correct.
        /// </summary>
        [Test]
        public void Validate_arrays_for_multiple_SlantedRecessedFiber_detectors_are_correct()
        {
            var detectorList =
                new List<IDetector>
                {
                    new SlantedRecessedFiberDetector
                    {
                        Center = new Position(0, 0, 1.0),
                        Radius = 1.0,
                        TallySecondMoment = true,
                        N = 1.0,
                        NA = 1.0,
                        FinalTissueRegionIndex = 1,
                        Mean = 1.0,
                        SecondMoment = 2.0,
                        TallyCount = 3,
                        Name = "SlantedRecessedFiber1"
                    },
                    new SlantedRecessedFiberDetector
                    {
                        Center = new Position(0, 0, 2.0),
                        Radius = 2.0,
                        TallySecondMoment = true,
                        N = 1.2,
                        NA = 1.2,
                        FinalTissueRegionIndex = 2,                        
                        Mean = 2.0,
                        SecondMoment = 4.0,
                        TallyCount = 6,
                        Name = "SlantedRecessedFiber2"
                    },

                };
            var output = new SimulationOutput(new SimulationInput(), detectorList);

            // verify first detector specification
            var detector = (SlantedRecessedFiberDetector)output.ResultsDictionary["SlantedRecessedFiber1"];
            Assert.AreEqual(1.0, detector.Center.Z);
            Assert.AreEqual(1.0, detector.Radius);
            Assert.AreEqual(1.0, detector.N);
            Assert.AreEqual(1.0, detector.NA);
            Assert.AreEqual(1, detector.FinalTissueRegionIndex);
            Assert.AreEqual(1.0, detector.Mean);
            Assert.AreEqual(2.0, detector.SecondMoment);
            Assert.AreEqual(3, detector.TallyCount);

            // verify second detector specification
            detector = (SlantedRecessedFiberDetector)output.ResultsDictionary["SlantedRecessedFiber2"];
            Assert.AreEqual(2.0, detector.Center.Z);
            Assert.AreEqual(2.0, detector.Radius);
            Assert.AreEqual(1.2, detector.N);
            Assert.AreEqual(1.2, detector.NA);
            Assert.AreEqual(2, detector.FinalTissueRegionIndex);
            Assert.AreEqual(2.0, detector.Mean);
            Assert.AreEqual(4.0, detector.SecondMoment);
            Assert.AreEqual(6, detector.TallyCount);
        }

        /// <summary>
        /// Test to check that created arrays for internal surface fiber detectors that allow multiple fibers
        /// to be defined in a single simulation (rather than having to run a separate simulation for each
        /// fiber definition) are correct.
        /// </summary>
        [Test]
        public void Validate_arrays_for_multiple_InternalSurfaceFiber_detectors_are_correct()
        {
            var detectorList =
                new List<IDetector>
                {
                    new InternalSurfaceFiberDetector 
                    {
                        Center = new Position(0, 0, 1.0),
                        Radius = 1.0,
                        TallySecondMoment = true,
                        N = 1.0,
                        NA = 1.0,
                        FinalTissueRegionIndex = 1,
                        InDirectionOfFiberAxis = new Direction(0, 0, -1),
                        Mean = 1.0,
                        SecondMoment = 2.0,
                        TallyCount = 3,
                        Name = "InternalSurfaceFiber1"
                    },
                    new InternalSurfaceFiberDetector
                    {
                        Center = new Position(0, 0, 2.0),
                        Radius = 2.0,
                        TallySecondMoment = true,
                        N = 1.2,
                        NA = 1.2, 
                        FinalTissueRegionIndex = 2,
                        InDirectionOfFiberAxis = new Direction(0, 0, -1),
                        Mean = 2.0,
                        SecondMoment = 4.0,
                        TallyCount = 6,
                        Name = "InternalSurfaceFiber2"
                    },

                };
            var output = new SimulationOutput(new SimulationInput(), detectorList);

            // verify first detector specification
            var detector = (InternalSurfaceFiberDetector)output.ResultsDictionary["InternalSurfaceFiber1"];
            Assert.AreEqual(1.0, detector.Center.Z);
            Assert.AreEqual(1.0, detector.Radius);
            Assert.AreEqual(1.0, detector.N);
            Assert.AreEqual(1.0, detector.NA);
            Assert.AreEqual(1, detector.FinalTissueRegionIndex);
            Assert.AreEqual(1.0, detector.Mean);
            Assert.AreEqual(2.0, detector.SecondMoment);
            Assert.AreEqual(3, detector.TallyCount);

            // verify second detector specification
            detector = (InternalSurfaceFiberDetector)output.ResultsDictionary["InternalSurfaceFiber2"];
            Assert.AreEqual(2.0, detector.Center.Z);
            Assert.AreEqual(2.0, detector.Radius);
            Assert.AreEqual(1.2, detector.N);
            Assert.AreEqual(1.2, detector.NA);
            Assert.AreEqual(2, detector.FinalTissueRegionIndex);
            Assert.AreEqual(2.0, detector.Mean);
            Assert.AreEqual(4.0, detector.SecondMoment);
            Assert.AreEqual(6, detector.TallyCount);
        }

    }
}