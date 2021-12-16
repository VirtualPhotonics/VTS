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
            Assert.IsFalse(knownTypes.ContainsKey("OpticalProperties"));
            KnownTypes.Add(typeof(OpticalProperties));
            knownTypes = KnownTypes.CurrentKnownTypes;
            Assert.IsTrue(knownTypes.ContainsKey("Vts.OpticalProperties"));
            KnownTypes.Add(typeof(Position));
            knownTypes = KnownTypes.CurrentKnownTypes;
            Assert.IsTrue(knownTypes.ContainsKey("Vts.Common.Position"));
        }
    }
}
