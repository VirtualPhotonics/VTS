using System.Collections.Generic;
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
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestFiles = new List<string>()
        {
            "test", // file that captures screen output of MC simulation
        };

        [TestFixtureSetUp]
        public void clear_previously_generated_files()
        {
            foreach (var file in listOfTestFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
        /// <summary>
        /// clear all newly generated files
        /// </summary>
        [TestFixtureTearDown]
        public void clear_newly_generated_files()
        {
            // delete any newly generated files
            foreach (var file in listOfTestFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
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
