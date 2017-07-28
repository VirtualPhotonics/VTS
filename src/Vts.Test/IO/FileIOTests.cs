using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using Vts.Common;
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
            FileIO.FileDelete("file1.txt");
            FileIO.FileDelete("file2.txt");
            FileIO.FileDelete("file3.txt");
            FileIO.FileDelete("file4.txt");
            FileIO.FileDelete("file5.txt");
            FileIO.FileDelete("array1");
            FileIO.FileDelete("array1.txt");
            FileIO.DeleteDirectory("folder1");
            FileIO.DeleteDirectory("folder2");
            FileIO.DeleteDirectory("folder3");
            FileIO.DeleteDirectory("folder4");
        }

        [Test]
        public void validate_clear_directory()
        {
            const string folder = "folder";
            if (!FileIO.DirectoryExists(folder))
            {
                FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
                Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            }
            else
            {
                if (!FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")))
                {
                    var name = Assembly.GetExecutingAssembly().FullName;
                    var assemblyName = new AssemblyName(name).Name;
                    FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.embeddedresourcefile.txt", Path.Combine(folder, "embeddedresourcefile.txt"), name);
                }
            }
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            FileIO.ClearDirectory(folder);
            Assert.IsFalse(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }

        [Test]
        public void validate_clone()
        {
            var i = new Position { X = 2, Y = 5, Z = 9 };
            var iCloned = i.Clone();
            Assert.AreEqual(iCloned.X, i.X);
            Assert.AreEqual(iCloned.Y, i.Y);
            Assert.AreEqual(iCloned.Z, i.Z);
        }

        [Test]
        public void validate_copy_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var stream1 = StreamFinder.GetFileStreamFromResources("Resources/resourcefile.txt", assemblyName);
            var stream2 = StreamFinder.GetFileStream("file5.txt", FileMode.CreateNew);
            Assert.IsNotNull(stream1);
            FileIO.CopyStream(stream1, stream2);
            Assert.IsNotNull(stream2);
            Assert.AreEqual(stream1, stream2);
        }

        [Test]
        public void validate_create_directory()
        {
            const string folder = "folder2";
            FileIO.CreateDirectory(folder);
            Assert.IsTrue(FileIO.DirectoryExists(folder));
        }

        [Test]
        public void validate_create_empty_directory()
        {
            const string folder = "folder";
            if (!FileIO.DirectoryExists(folder))
            {
                FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
                Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            }
            else
            {
                if (!FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")))
                {
                    var name = Assembly.GetExecutingAssembly().FullName;
                    var assemblyName = new AssemblyName(name).Name;
                    FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.embeddedresourcefile.txt", "embeddededresourcefile.txt", name);
                }
            }
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            FileIO.CreateEmptyDirectory(folder);
            Assert.IsFalse(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }

        [Test]
        public void validate_read_array_from_binary()
        {

        }

        [Test]
        public void validate_read_array_from_binary_in_resources()
        {

        }

        [Test]
        public void validate_read_from_binary()
        {

        }

        [Test]
        public void validate_read_from_binary_custom()
        {

        }

        [Test]
        public void validate_read_from_binary_in_resources()
        {

        }

        [Test]
        public void validate_read_from_binary_in_resources_custom()
        {

        }

        [Test]
        public void validate_read_from_binary_stream()
        {

        }

        [Test]
        public void validate_read_from_json()
        {

        }

        [Test]
        public void validate_read_from_json_in_resources()
        {

        }

        [Test]
        public void validate_read_from_json_stream()
        {

        }

        [Test]
        public void validate_read_from_stream()
        {

        }

        [Test]
        public void validate_read_from_xml()
        {

        }

        [Test]
        public void validate_read_from_xml_in_resources()
        {

        }

        [Test]
        public void validate_read_scalar_value_from_binary()
        {

        }

        [Test]
        public void validate_read_stream_from_binary_custom()
        {

        }

        [Test]
        public void validate_write_json_to_stream()
        {

        }

        [Test]
        public void validate_write_scalar_value_to_binary()
        {

        }

        [Test]
        public void validate_write_to_binary()
        {

        }

        [Test]
        public void validate_write_to_binary_custom()
        {

        }

        [Test]
        public void validate_write_to_binary_stream()
        {

        }

        [Test]
        public void validate_write_to_json()
        {

        }

        [Test]
        public void validate_write_to_xml()
        {

        }

        [Test]
        public void validate_write_to_xml_stream()
        {

        }

        [Test]
        public void validate_zip_files()
        {

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
            const string file = "file1.txt";
            FileIO.WriteToTextFile("Text", file);
            Assert.IsTrue(FileIO.FileExists(file));
        }

        [Test]
        public void validate_directory_exists()
        {
            const string folder = "folder1";
            FileIO.CreateDirectory(folder);
            Assert.IsTrue(FileIO.DirectoryExists(folder));
        }

        [Test]
        public void validate_file_delete()
        {
            const string file = "file2.txt";
            FileIO.WriteToTextFile("Text", file);
            FileIO.FileDelete(file);
            Assert.IsFalse(FileIO.FileExists(file));
        }

        [Test]
        public void validate_directory_delete()
        {
            const string folder = "folder3";
            FileIO.CreateDirectory(folder);
            Assert.IsTrue(FileIO.DirectoryExists(folder));
            FileIO.DeleteDirectory(folder);
            Assert.IsFalse(FileIO.DirectoryExists(folder));
        }

        [Test]
        public void validate_write_text_to_file()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            FileIO.WriteToTextFile(myString.ToString(), "file3.txt");
            Assert.IsTrue(FileIO.FileExists("file3.txt"));
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
