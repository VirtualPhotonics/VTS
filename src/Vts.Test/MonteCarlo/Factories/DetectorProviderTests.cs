using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for DetectorProvider
    /// </summary>
    [TestFixture]
    public class DetectorProviderTests
    {
        ///// <summary>
        ///// Simulate Usage Of DetectorProvider
        ///// </summary>
        //[Test]
        //public void simulate_usage_of_DetectorProvider()
        //{
        //    var p = new DetectorProvider<SampleDetectorInput, SampleDetector, SampleDetectorOutput>();

        //    var input1 = new SampleDetectorInput { Name = "ROfQ", QRange = new DoubleRange(0, 1, 5)};

        //    var simInput = new SimulationInput();

        //    simInput.DetectorInputs.Add(input1);

        //    p.WriteInputToFile(input1, "sampleinput");

        //    var input2 = p.ReadInputFromFile("sampleinput");

        //    var detector = p.CreateDetector(input2);

        //    var output = p.CreateOutput(detector);

        //    p.WriteOutputToFile(output, "sampleoutput");

        //    p.ReadOutputFromFile("ROfQ", "sampleoutput");
        //}
        /// <summary>
        /// Simulate Usage Of DetectorProvider
        /// </summary>
        [Test]
        public void demonstrate_FancyDetector_creation_via_DetectorFactory()
        {
            var input = new FancyDetectorInput
                {
                    TallyType =  "ROfQ", 
                    Name = "My First Detector", 
                    TallySecondMoment = false,
                    XRange = new DoubleRange(0, 1, 5),
                    YRange = new DoubleRange(0, 1, 5),
                };

            var detector = DetectorFactory.GetDetectors(new[] {input}, null, true);

            var simInput = new SimulationInput { DetectorInputs = new [] { input } };

            var sim = simInput.CreateSimulation();

            var results = sim.Run();

            IDetector detector = null;

            var rOfQ = results.ResultsDictionary.TryGetValue(input.Name, out detector);

            if (rOfQ != null)
            {
                Console.WriteLine(rOfQ);
            }
        }

        [Test]
        public void demonstrate_detector_creation_programmatically()
        {
            var input = new FancyDetectorInput();

            var detector = input.CreateDetector();

            Console.WriteLine(detector);
        }
    }
}
