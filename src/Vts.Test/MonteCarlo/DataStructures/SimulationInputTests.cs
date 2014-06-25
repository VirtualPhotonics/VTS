using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SimulationInputTests
    {
        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new SimulationInput { N = 10 };

            var iCloned = i.Clone();

            Assert.AreEqual(iCloned.N, 10);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            new SimulationInput { N = 10 }.ToFile("test.txt");
            var iCloned = SimulationInput.FromFile("test.txt");

            Assert.AreEqual(iCloned.N, 10);
        }

        [Test]
        public void validate_null_detector_input_gets_converted_to_empty_list_correctly()
        {
            var si = new SimulationInput(
                100,
                "results",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiLayerTissueInput(),
                null
                );
            Assert.IsTrue(si.DetectorInputs.Count == 0);
        }
    }
}
