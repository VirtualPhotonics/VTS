using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.DataStructures
{
    [TestFixture]
    public class InputTests
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
            new SimulationInput { N = 10 }.ToFile("test");
            var iCloned = SimulationInput.FromFile("test");

            Assert.AreEqual(iCloned.N, 10);
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
