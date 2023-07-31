using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    public class FolderCleanupTests
    {
        [Test]
        public void Test_directory_cleanup()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string directory;
            for (var i = 1; i < 4; i++)
            {
                directory = $"directory_cleanup_{i}";
                Directory.CreateDirectory(directory);
                Assert.IsTrue(Directory.Exists(directory));
            }
            FolderCleanup.DeleteDirectoryContaining(currentPath, "directory_cleanup");
            for (var i = 1; i < 4; i++)
            {
                directory = $"directory_cleanup_{i}";
                Assert.IsFalse(Directory.Exists(directory));
            }
        }

        [Test]
        public void Test_file_cleanup()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string file;
            for (var i = 1; i < 4; i++)
            {
                file = $"file_cleanup_{i}.txt";
                FileIO.WriteToTextFile("Text", file);
                Assert.IsTrue(File.Exists(file));
            }
            FolderCleanup.DeleteFileContaining(currentPath, "file_cleanup");
            for (var i = 1; i < 4; i++)
            {
                file = $"file_cleanup_{i}.txt";
                Assert.IsFalse(File.Exists(file));
            }
        }
    }
}

