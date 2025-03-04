using NUnit.Framework;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    internal class KnownTypesTests
    {
        [Test]
        public void Test_known_types()
        {
            var knownTypes = KnownTypes.CurrentKnownTypes;
            Assert.That(knownTypes.ContainsKey("OpticalProperties"), Is.False);
            KnownTypes.Add(typeof(OpticalProperties));
            knownTypes = KnownTypes.CurrentKnownTypes;
            Assert.That(knownTypes.ContainsKey("Vts.OpticalProperties"), Is.True);
            KnownTypes.Add(typeof(Position));
            knownTypes = KnownTypes.CurrentKnownTypes;
            Assert.That(knownTypes.ContainsKey("Vts.Common.Position"), Is.True);
        }
    }
}
