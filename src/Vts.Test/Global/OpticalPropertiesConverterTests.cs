using System;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test
{
    [TestFixture]
    public class OpticalPropertiesConverterTests
    {
        private OpticalProperties _opticalProperties;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _opticalProperties = new OpticalProperties(0.01, 1.0, 0.8, 1.4);
        }

        [Test]
        public void Test_can_convert()
        {
            Assert.That(new OpticalPropertiesConverter().CanConvert(_opticalProperties.GetType()), Is.True);
        }

        [Test]
        public void Test_read_json()
        {
            var jsonSerialized = _opticalProperties.WriteToJson();
            var opticalPropertiesDeserialized = jsonSerialized.ReadFromJson<OpticalProperties>();
            Assert.That(opticalPropertiesDeserialized, Is.InstanceOf<OpticalProperties>());
            Assert.AreEqual(0.01, opticalPropertiesDeserialized.Mua);
            Assert.AreEqual(1.0, opticalPropertiesDeserialized.Musp);
            Assert.AreEqual(0.8, opticalPropertiesDeserialized.G);
            Assert.AreEqual(1.4, opticalPropertiesDeserialized.N);
        }

        [Test]
        public void Test_can_write()
        {
            Assert.That(new OpticalPropertiesConverter().CanWrite, Is.False);
        }

        [Test]
        public void Test_write_json_throws_not_implemented_error()
        {
            Assert.Throws<NotImplementedException>(() => new OpticalPropertiesConverter().WriteJson(null, null, null));
        }
    }
}
