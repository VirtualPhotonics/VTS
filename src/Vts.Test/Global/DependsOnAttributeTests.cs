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
            Assert.AreEqual(parameters[0], dependsOnAttribute.Properties[0]);
            Assert.AreEqual(parameters[1], dependsOnAttribute.Properties[1]);
            Assert.AreEqual(parameters[2], dependsOnAttribute.Properties[2]);
        }
    }
}
