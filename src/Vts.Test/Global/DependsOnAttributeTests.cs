using NUnit.Framework;

namespace Vts.Test
{
    [TestFixture]
    internal class DependsOnAttributeTests
    {
        [Test]
        public void Test_DependsOnAttribute()
        {
            var parameters = new[] { "one", "two", "three" };
            var dependsOnAttribute = new DependsOnAttribute(parameters);
            Assert.That(dependsOnAttribute.Properties[0], Is.EqualTo(parameters[0]));
            Assert.That(dependsOnAttribute.Properties[1], Is.EqualTo(parameters[1]));
            Assert.That(dependsOnAttribute.Properties[2], Is.EqualTo(parameters[2]));
        }
    }
}
