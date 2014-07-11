using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class pMCDetectorInputTests
    {
        // todo: need to show working with SimulationInput using serialization

        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var i = new pMCROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) };
            var iCloned = Clone(i);

            Assert.AreEqual(iCloned.Rho.Start, 10);
        }

        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            new pMCROfRhoDetectorInput() { Rho = new DoubleRange(10, 20, 51) }.WriteToJson("test");
            var iCloned = FileIO.ReadFromJson<pMCROfRhoDetectorInput>("test");

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
