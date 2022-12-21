using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    internal class LibraryIOTests
    {
        [Test]
        public void Test_EnsureDllIsLoaded()
        {
            var dllPath = "Vts.dll";
#if NET48
            var location = Assembly.GetExecutingAssembly().Location;
            location = location.Replace("net48", "netstandard2.0");
            dllPath = location.Replace("Vts.Test", "Vts");
#endif
            var assembliesBefore = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyCount = assembliesBefore.Length;
            LibraryIO.EnsureDllIsLoaded(dllPath);
            var assembliesAfter = AppDomain.CurrentDomain.GetAssemblies();
            Assert.Greater(assembliesAfter.Length, assemblyCount);
        }
    }
}
