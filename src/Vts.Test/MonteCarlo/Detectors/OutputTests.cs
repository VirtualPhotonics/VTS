using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
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
                    DetectorInputs = new List<IDetectorInput>
                    {
                        new ROfAngleDetectorInput(new DoubleRange(0d, Math.PI, 10))
                    }
                }
            };

            var oCloned = Clone(o);
            var angle = ((ROfAngleDetectorInput)oCloned.Input.DetectorInputs.
                Where(d => d.TallyType == TallyType.ROfAngle).First()).Angle;
            Assert.AreEqual(angle.Start, 0d);
            Assert.AreEqual(angle.Stop, Math.PI);
            Assert.AreEqual(angle.Count, 10);
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
