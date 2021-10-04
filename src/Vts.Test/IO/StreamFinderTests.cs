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
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file4.txt",
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
            var myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            var stream = StreamFinder.GetFileStream("file4.txt", FileMode.Create);
            Assert.IsNotNull(stream);
        }

        [Test]
        public void validate_get_file_stream_from_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var stream = StreamFinder.GetFileStreamFromResources("Resources/streamfindertest/resourcefile.txt", assemblyName);
            Assert.IsNotNull(stream);
        }

    }
}
