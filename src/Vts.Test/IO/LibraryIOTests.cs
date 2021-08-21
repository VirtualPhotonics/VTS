using System;
using System.Reflection;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    internal class LibraryIOTests
    {
        [Test]
        public void Test_EnsureDllIsLoaded()
        {
            var assembliesBefore = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyCount = assembliesBefore.Length;
            LibraryIO.EnsureDllIsLoaded("Vts.dll");
            var assembliesAfter = AppDomain.CurrentDomain.GetAssemblies();
            Assert.Greater(assembliesAfter.Length, assemblyCount);
        }
    }
}
