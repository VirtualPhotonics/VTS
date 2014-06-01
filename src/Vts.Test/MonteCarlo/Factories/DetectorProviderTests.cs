using System;
using NUnit.Framework;
using Vts.Common;
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
        /// <summary>
        /// Simulate Usage Of DetectorProvider
        /// </summary>
        [Test]
        public void simulate_usage_of_DetectorProvider()
        {
            var p = new DetectorProvider<SampleDetectorInput, SampleDetector, SampleDetectorOutput>();

            var input1 = new SampleDetectorInput { Name = "ROfQ", QRange = new DoubleRange(0, 1, 5)};

            var simInput = new SimulationInput();

            simInput.DetectorInputs.Add(input1);

            p.WriteInputToFile(input1, "sampleinput");

            var input2 = p.ReadInputFromFile("sampleinput");

            var detector = p.CreateDetector(input2);

            var output = p.CreateOutput(detector);

            p.WriteOutputToFile(output, "sampleoutput");

            p.ReadOutputFromFile("ROfQ", "sampleoutput");
        }
    }
}
