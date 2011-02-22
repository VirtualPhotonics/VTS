using System;
using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo.Detectors
{
    [TestFixture]
    public class OutputTests
    {
        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var o = new Output
            {
                Input = new SimulationInput
                {
                    DetectorInput = new DetectorInput
                    {
                        Angle = new DoubleRange(0d, Math.PI, 10)
                    }
                }
            };

            var oCloned = Clone(o);

            Assert.AreEqual(oCloned.Input.DetectorInput.Angle.Start, 0d);
            Assert.AreEqual(oCloned.Input.DetectorInput.Angle.Stop, Math.PI);
            Assert.AreEqual(oCloned.Input.DetectorInput.Angle.Count, 10);
        }


        [Test]
        public void validate_deserialized_class_is_correct_when_using_FileIO()
        {
            new Output { Input = new SimulationInput { N = 10 } }.WriteToXML("testOutput");
            var oCloned = FileIO.ReadFromXML<Output>("testOutput");

            Assert.AreEqual(oCloned.Input.N, 10);
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
