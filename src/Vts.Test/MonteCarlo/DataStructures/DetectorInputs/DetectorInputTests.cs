using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DetectorInputTests
    {
        // todo: need to show working with SimulationInput using serialization

        [Test]
        public void validate_class_can_be_serialized_as_part_of_SimulationInput()
        {
            var detectorInput = new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };

            try
            {
                new SimulationInput(
                    10,
                    "",
                    new SimulationOptions(),
                    new DirectionalPointSourceInput(),
                    new MultiLayerTissueInput(),
                    new List<IDetectorInput>
                    {
                        (ROfRhoDetectorInput)detectorInput 
                    }
                ).WriteToXML("test");
            }
            catch(SerializationException se)
            {
                Assert.Fail("SimulationInput class could not be serialized.");
            }

            //var simInputCloned = FileIO.ReadFromXML<SimulationInput>("test");

            //Assert.AreEqual(simInputCloned.DetectorInput.Rho.Start, 10);
        }


        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };
            var iCloned = Clone(i);

            Assert.AreEqual(iCloned.Rho.Start, 10);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            new ROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) }.WriteToXML("test");
            var iCloned = FileIO.ReadFromXML<ROfRhoDetectorInput>("test");

            Assert.AreEqual(iCloned.Rho.Start, 10);
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
