using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO
{
    /// <summary>
    /// Stream finder tests
    /// </summary>
    [TestFixture]
    public class StreamFinderTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "StreamFinderTests_file.txt",
            "resourcefile.txt"
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// Validate writing text to a stream
        /// </summary>
        [Test]
        public void validate_get_file_stream()
        {
            var stream = StreamFinder.GetFileStream("StreamFinderTests_file.txt", FileMode.Create);
            Assert.That(stream, Is.Not.Null);
            stream.Close();
        }

        [Test]
        public void validate_get_file_stream_from_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var stream = StreamFinder.GetFileStreamFromResources("Resources/streamfindertest/resourcefile.txt", assemblyName);
            Assert.That(stream, Is.Not.Null);
        }

    }
}
