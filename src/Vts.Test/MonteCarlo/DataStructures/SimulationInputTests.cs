using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
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

            var iCloned = Clone(i);

            Assert.AreEqual(iCloned.N, 10);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            new SimulationInput { N = 10 }.ToXMLFile("test");
            var iCloned = SimulationInput.FromXMLFile("test");

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
