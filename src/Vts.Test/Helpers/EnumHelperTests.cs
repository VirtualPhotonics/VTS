using System;
using NUnit.Framework;

namespace Vts.Test.Helpers
{
    [TestFixture]
    internal class EnumHelperTests
    {
        [Test]
        public void Get_names_returns_string_array()
        {
            var enumName = EnumHelper.GetNames<ForwardSolverType>();
            Assert.AreEqual("PointSourceSDA", enumName[0]);
            Assert.AreEqual("DistributedPointSourceSDA", enumName[1]);
            Assert.AreEqual("DistributedGaussianSourceSDA", enumName[2]);
            Assert.AreEqual("DeltaPOne", enumName[3]);
            Assert.AreEqual("MonteCarlo", enumName[4]);
            Assert.AreEqual("Nurbs", enumName[5]);
            Assert.AreEqual("TwoLayerSDA", enumName[6]);
        }

        [Test]
        public void Get_names_throws_exception()
        {
            Assert.Throws<ArgumentException>(() => EnumHelper.GetNames<EnumHelper>());
        }

        [Test]
        public void Get_values_returns_type_array()
        {
            var enumValues = EnumHelper.GetValues<SpatialDomainType>();
            Assert.That(enumValues, Is.InstanceOf<SpatialDomainType[]>());
            Assert.AreEqual(SpatialDomainType.Real, enumValues[0]);
            Assert.AreEqual(SpatialDomainType.SpatialFrequency, enumValues[1]);
        }

        [Test]
        public void Get_values_throws_exception()
        {
            Assert.Throws<ArgumentException>(() => EnumHelper.GetValues<EnumHelper>());
        }

        [Test]
        public void Get_values_returns_enum_array()
        {
            var enumValues = EnumHelper.GetValues(typeof(SpatialDomainType));
            Assert.That(enumValues, Is.InstanceOf<object[]>());
            Assert.AreEqual(SpatialDomainType.Real, enumValues[0]);
            Assert.AreEqual(SpatialDomainType.SpatialFrequency, enumValues[1]);
        }

        [Test]
        public void Get_values_with_type_parameter_throws_exception()
        {
            Assert.Throws<ArgumentException>(() => EnumHelper.GetValues(typeof(EnumHelper)));
        }
    }
}
