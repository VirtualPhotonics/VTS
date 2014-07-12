using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

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
        public void demonstrate_basic_mc_creation()
        {
            var detectorInput = new ROfRhoDetectorInput
                {
                    TallyType =  "ROfRho", 
                    Name = "My First Detector", 
                    TallySecondMoment = false,
                    Rho = new DoubleRange(0, 1, 5),
                };

            var simInput = new SimulationInput { DetectorInputs = new[] { detectorInput } };

            var sim = simInput.CreateSimulation();

            var results = sim.Run();

            IDetector detector;

            var rOfRhoDetector = results.ResultsDictionary.TryGetValue(detectorInput.Name, out detector);

            Assert.NotNull(rOfRhoDetector);
        }

        /// <summary>
        /// Simulate Usage Of DetectorProvider
        /// </summary>
        [Test]
        public void demonstrate_fluent_mc_creation()
        {
            var rOfRhoDetector = new SimulationInput
                {
                    DetectorInputs = new[]
                    {
                        new ROfRhoDetectorInput
                        {
                            TallyType = "ROfRho",
                            Name = "My First Detector",
                            TallySecondMoment = false,
                            Rho = new DoubleRange(0, 1, 5),
                        }
                    }
                }
                .CreateSimulation()
                .Run()
                .GetDetector("My First Detector");

            Assert.NotNull(rOfRhoDetector);
        }
    }
}
