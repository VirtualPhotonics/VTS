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
            Assert.That(enumName[0], Is.EqualTo("PointSourceSDA"));
            Assert.That(enumName[1], Is.EqualTo("DistributedPointSourceSDA"));
            Assert.That(enumName[2], Is.EqualTo("DistributedGaussianSourceSDA"));
            Assert.That(enumName[3], Is.EqualTo("DeltaPOne"));
            Assert.That(enumName[4], Is.EqualTo("MonteCarlo"));
            Assert.That(enumName[5], Is.EqualTo("Nurbs"));
            Assert.That(enumName[6], Is.EqualTo("TwoLayerSDA"));
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
            Assert.That(enumValues[0], Is.EqualTo(SpatialDomainType.Real));
            Assert.That(enumValues[1], Is.EqualTo(SpatialDomainType.SpatialFrequency));
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
            Assert.That(enumValues[0], Is.EqualTo(SpatialDomainType.Real));
            Assert.That(enumValues[1], Is.EqualTo(SpatialDomainType.SpatialFrequency));
        }

        [Test]
        public void Get_values_with_type_parameter_throws_exception()
        {
            Assert.Throws<ArgumentException>(() => EnumHelper.GetValues(typeof(EnumHelper)));
        }
    }
}
