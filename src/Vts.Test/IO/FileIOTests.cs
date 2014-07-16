using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    public class FileIOTests
    {
        /// <summary>
        /// clear all previously generated folders and files
        /// </summary>
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
            if(FileIO.FileExists("file1.txt"))
            {
                FileIO.FileDelete("file1.txt");
            }
            if (FileIO.FileExists("file2.txt"))
            {
                FileIO.FileDelete("file2.txt");
            }
            if (FileIO.FileExists("file3.txt"))
            {
                FileIO.FileDelete("file3.txt");
            }
            if (FileIO.FileExists("file4.txt"))
            {
                FileIO.FileDelete("file4.txt");
            }
            if (FileIO.FileExists("array1")) // binary array
            {
                FileIO.FileDelete("array1");
            }
            if (FileIO.FileExists("array1.txt")) // metadata
            {
                FileIO.FileDelete("array1.txt");
            }
        }

        [Test]
        public void validate_copy_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.embeddedresourcefile.txt", "embeddededresourcefile.txt", name);
            Assert.IsTrue(FileIO.FileExists("embeddededresourcefile.txt"));
        }

        [Test]
        public void validate_copy_file_from_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/resourcefile.txt", "resourcefile.txt", assemblyName); 
            Assert.IsTrue(FileIO.FileExists("resourcefile.txt"));
        }

        [Test]
        public void validate_copy_folder_from_embedded_resources()
        {
            var folder = "folder";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }

        [Test]
        public void validate_file_exists()
        {
            FileIO.WriteToTextFile("Text", "file1.txt");
            Assert.IsTrue(FileIO.FileExists("file1.txt"));
        }

        [Test]
        public void validate_file_delete()
        {
            FileIO.WriteToTextFile("Text", "file2.txt");
            FileIO.FileDelete("file2.txt");
            Assert.IsFalse(FileIO.FileExists("file2.txt"));
        }

        [Test]
        public void validate_save_text_to_file()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            FileIO.WriteToTextFile(myString.ToString(), "file3.txt");
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_write_text_to_stream()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            Stream stream = StreamFinder.GetFileStream("file4.txt", FileMode.Create);
            FileIO.WriteTextToStream(myString.ToString(), stream);
            Assert.IsNotNull(stream);
        }

        [Test]
        public void validate_write_array_to_binary()
        {
            double[] array = new double[3] {1, 2, 3};
            FileIO.WriteArrayToBinary<double>(array, "array1", true);
            Assert.IsTrue(FileIO.FileExists("array1"));
            Assert.IsTrue(FileIO.FileExists("array1.txt"));
        }
    }
}
