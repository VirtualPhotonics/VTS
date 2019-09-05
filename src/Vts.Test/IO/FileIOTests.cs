using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "fileiotest.folder",
            "folder1",
            "folder2",
            "folder3",
            "folder4",
            "sourcetest"
        };
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file1.txt",
            "file2.txt",
            "file3.txt",
            "file4.txt",
            "file5.txt",
            "file6.txt",
            "file7.txt",
            "file7.xml",
            "file8.xml",
            "array1",
            "array1.txt",
            "array2",
            "array3",
            "array4",
            "array5",
            "array6",
            "scalar",
            "embeddedresourcefile.txt",
            "resourcefile.txt",
            "AOfXAndYAndZ",
            "position.txt"
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
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }

        [Test]
        public void validate_clear_directory()
        {
            const string folder = "fileiotest.folder";
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
            var stream1 = StreamFinder.GetFileStreamFromResources("Resources/streamfindertest/resourcefile.txt", assemblyName);
            var stream2 = StreamFinder.GetFileStream("file5.txt", FileMode.CreateNew);
            Assert.IsNotNull(stream1);
            FileIO.CopyStream(stream1, stream2);
            Assert.IsNotNull(stream2);
            Assert.AreEqual(stream1, stream2);
            stream1.Close();
            stream2.Close();
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
            const string folder = "fileiotest.folder";
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
                    FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.fileiotest.embeddedresourcefile.txt", "embeddedresourcefile.txt", name);
                }
            }
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
            FileIO.CreateEmptyDirectory(folder);
            Assert.IsFalse(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }


        [Test]
        public void validate_read_array_from_binary_in_resources_with_size_parameter()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            string dataLocation = "Resources/fileiotest/";
            int size = 100;
            double[] data = new double[100];
            data = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"ROfRho", assemblyName, size);
            Assert.IsTrue(Math.Abs(data[2] - 0.052445) < 0.000001);
        }

        [Test]
        [Ignore("This test needs to be added")]
        public void validate_read_array_from_binary_in_resources_without_parameter_dimensions()
        {
            // ReadArrayFromBinaryInResources without parameter dimensions calls ReadFromJsonInResources
            // which does not set dims and so next line in method fails
            //var name = Assembly.GetExecutingAssembly().FullName;
            //var assemblyName = new AssemblyName(name).Name;
            //string dataLocation = "Resources/sourcetest/";
            //double[,,] data = new double[4, 1, 3];
            //// CH: since the following method does not instantiate return, need to know size prior to calling it
            //data = (double[,,])FileIO.ReadArrayFromBinaryInResources<double>
            //    (dataLocation + @"AOfXAndYAndZ", assemblyName);
            //Assert.IsTrue(Math.Abs(data[0,0,0]) < 0.000001);
        }

        [Test]
        public void validate_read_from_binary_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            double data;
            data = (double)FileIO.ReadFromBinaryInResources<double>(
                "Resources/fileiotest/binarydbl", assemblyName);
            Assert.AreEqual(data, 10);
        }

        [Test] // CH: need help

        [Ignore("This test needs to be added")]
        public void validate_read_from_binary_in_resources_custom()
        {
            //var name = Assembly.GetExecutingAssembly().FullName;
            //var assemblyName = new AssemblyName(name).Name;
            //double ReadMap(BinaryReader b) => b.Read();
            //// the following method has a "yield return"
            //var listRead = FileIO.ReadFromBinaryInResourcesCustom<double>(
            //    "Resources/fileiotest/binarydbl", assemblyName, ReadMap);
            //Assert.AreEqual(listRead.First(), 0); // should be 10
        }


        [Test] // CH: need help
        [Ignore("This test needs to be added")]
        public void validate_read_from_binary_custom()
        {
            //var name = Assembly.GetExecutingAssembly().FullName;
            //var assemblyName = new AssemblyName(name).Name;
            //int size = 100;
            //// read file from resources and write it so that can be read in
            //double data;
            //data = (double)FileIO.ReadFromBinaryInResources<double>(
            //    "Resources/fileiotest/binarydbl", assemblyName);
            //FileIO.WriteToBinary<double>(data, "array6");
            //double ReadMap(BinaryReader b) => b.Read();
            //var listRead = FileIO.ReadFromBinaryCustom<double>("array6", ReadMap);
            //Assert.AreEqual(listRead.First(), 0); // should be 10
        }

        [Test]
        public void validate_read_from_binary_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            int size = 100;
            var arrayWritten = new double[size];
            // read file from resources and write it so that can be read in
            arrayWritten = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                ("Resources/fileiotest/ROfRho", assemblyName, size);
            FileIO.WriteToBinary<double[]>(arrayWritten, "array5");
            double[] arrayRead = new double[100];
            using (Stream stream = StreamFinder.GetFileStream("array5", FileMode.Open))
            {
                arrayRead = FileIO.ReadFromBinaryStream<double[]>(stream);
            }
            Assert.IsTrue(Math.Abs(arrayRead[2] - 0.052445) < 0.000001);
        }

        [Test]
        public void validate_read_from_json()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/fileiotest/position.txt", "position.txt", assemblyName);
            var pos = FileIO.ReadFromJson<Position>("position.txt");
            Assert.AreEqual(pos.X, 5);
            Assert.AreEqual(pos.Y, 10);
            Assert.AreEqual(pos.Z, 15);
        }

        [Test]
        public void validate_read_from_json_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var pos = FileIO.ReadFromJsonInResources<Position>("Resources/fileiotest/position.txt", assemblyName);
            Assert.AreEqual(pos.X, 5);
            Assert.AreEqual(pos.Y, 10);
            Assert.AreEqual(pos.Z, 15);
        }

        [Test]
        public void validate_read_from_json_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            // create a JSON stream
            var stream = StreamFinder.GetFileStreamFromResources("Resources/fileiotest/position.txt", assemblyName);
            var pos = FileIO.ReadFromJsonStream<Position>(stream);
            Assert.AreEqual(pos.X, 5);
            Assert.AreEqual(pos.Y, 10);
            Assert.AreEqual(pos.Z, 15);
            stream.Close();
        }

        [Test]
        public void validate_read_from_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            Position pos;
            // read file from resources and write it so that can be read in
            var xml = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            FileIO.WriteToXML<Position>(xml, "file7.xml");
            using (Stream stream = StreamFinder.GetFileStream("file7.xml", FileMode.Open))
            {
                pos = FileIO.ReadFromStream<Position>(stream);
            }
            Assert.AreEqual(pos.X, 2);
            Assert.AreEqual(pos.Y, 4);
            Assert.AreEqual(pos.Z, 6);
        }

        [Test]
        public void validate_read_from_xml_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var pos = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            Assert.AreEqual(pos.X, 2);
            Assert.AreEqual(pos.Y, 4);
            Assert.AreEqual(pos.Z, 6);
        }


        [Test]
        public void validate_write_json_to_stream()
        {
            var pos = new Position(2, 4, 6);
            Stream stream = StreamFinder.GetFileStream("file6.txt", FileMode.Create);
            FileIO.WriteJsonToStream(pos, stream);
            var pos2 = FileIO.ReadFromJson<Position>("file6.txt");
            Assert.AreEqual(pos2.X, 2.0);
            Assert.AreEqual(pos2.Y, 4.0);
            Assert.AreEqual(pos2.Z, 6.0);
        }

        [Test]
        public void validate_write_scalar_value_to_binary_and_read_scalar_value_from_binary()
        {
            // write scalar using Action=WriteMap and validate file exists and non-zero length
            var scalar = 11;
            void WriteMap(BinaryWriter b, int s) => b.Write(s);
            FileIO.WriteScalarValueToBinary<int>(scalar, "scalar", WriteMap);
            Assert.IsTrue(FileIO.FileExists("scalar"));
            Assert.IsTrue(new FileInfo("scalar").Length != 0);
            // then read what what written using func ReadMap and validate value
            int ReadMap(BinaryReader b) => b.Read();
            var data = FileIO.ReadScalarValueFromBinary<int>("scalar", ReadMap);
            Assert.AreEqual(data, 11);
        }

        [Test]
        public void validate_write_to_binary_and_read_from_binary()
        {
            double[] array = new double[3] { 4.0, 5.0, 6.0 };
            FileIO.WriteToBinary<double[]>(array, "array2");
            Assert.IsTrue(FileIO.FileExists("array2"));
            Assert.IsTrue(new FileInfo("array2").Length != 0);
            double[] data = new double[3];
            data = FileIO.ReadFromBinary<double[]>("array2");
            Assert.AreEqual(data[0], 4.0);
        }

        [Test]
        public void validate_write_to_binary_custom()
        {
            IEnumerable<double> arrayWritten = Enumerable.Range(7, 3).Select(x => (double) x);          
            void WriteMap(BinaryWriter b, double s) => b.Write(s);
            FileIO.WriteToBinaryCustom<double>(arrayWritten, "array3", WriteMap);
            Assert.IsTrue(FileIO.FileExists("array3"));
            Assert.IsTrue(new FileInfo("array3").Length != 0);
        }

        [Test]
        public void validate_write_to_binary_stream_and_read_from_binary_stream()
        {
            // first create stream, write array, validate written and close stream
            double[] array = new double[3] { 10, 11, 12 };
            Stream streamWrite = StreamFinder.GetFileStream("array4", FileMode.Create);
            FileIO.WriteToBinaryStream(array, streamWrite);
            Assert.IsNotNull(streamWrite);
            Assert.IsTrue(FileIO.FileExists("array4"));
            Assert.IsTrue(new FileInfo("array4").Length != 0);
            streamWrite.Close();
            // then open stream, read array, validate values and close stream
            Stream streamRead = StreamFinder.GetFileStream("array4", FileMode.Open);
            double[] data = new double[3];
            data = FileIO.ReadFromBinaryStream<double[]>(streamRead);
            Assert.AreEqual(data[0], 10);
            streamRead.Close();
        }

        [Test]
        public void validate_write_to_json()
        {
            var pos = new Position(2, 4, 6);
            FileIO.WriteToJson(pos, "file7.txt");
            var pos2 = FileIO.ReadFromJson<Position>("file7.txt");
            Assert.AreEqual(pos2.X, 2.0);
            Assert.AreEqual(pos2.Y, 4.0);
            Assert.AreEqual(pos2.Z, 6.0);
        }

        [Test]
        public void validate_write_to_xml_and_read_from_xml()
        {
            var pos = new Position(2, 4, 6);
            FileIO.WriteToXML<Position>(pos, "file7.xml");
            Assert.IsTrue(FileIO.FileExists("file7.xml"));
            Assert.IsTrue(new FileInfo("file7.xml").Length != 0);
            var pos2 = FileIO.ReadFromXML<Position>("file7.xml");
            Assert.AreEqual(pos2.X, 2.0);
            Assert.AreEqual(pos2.Y, 4.0);
            Assert.AreEqual(pos2.Z, 6.0);
        }

        [Test]
        public void validate_write_to_xml_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var xmlFile = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            Stream stream = StreamFinder.GetFileStream("file8.xml", FileMode.Create);
            FileIO.WriteToXMLStream(xmlFile, stream);
            Assert.IsNotNull(stream);
            Assert.IsTrue(FileIO.FileExists("file8.xml"));
            Assert.IsTrue(new FileInfo("file8.xml").Length != 0);
            stream.Close();
        }

        [Test]
        public void validate_copy_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.fileiotest.embeddedresourcefile.txt", "embeddedresourcefile.txt", name);
            Assert.IsTrue(FileIO.FileExists("embeddedresourcefile.txt"));
        }

        [Test]
        public void validate_copy_binary_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.sourcetest.AOfXAndYAndZ", "AOfXAndYAndZ", name);
            Assert.IsTrue(FileIO.FileExists("AOfXAndYAndZ"));
        }

        [Test]
        public void validate_copy_file_from_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/streamfindertest/resourcefile.txt", "resourcefile.txt", assemblyName); 
            Assert.IsTrue(FileIO.FileExists("resourcefile.txt"));
        }

        [Test]
        public void validate_copy_folder_from_embedded_resources()
        {
            var folder = "fileiotest.folder";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")));
        }

        [Test]
        public void validate_copy_folder_containing_binary_from_embedded_resources()
        {
            var folder = "sourcetest";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ")));
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ.txt")));
            Assert.IsTrue(FileIO.FileExists(Path.Combine(folder, "input.txt")));
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

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        [Ignore("This test needs to be added")]
        public void validate_write_array_to_binary_and_read_array_from_binary()
        {
            // if the commented out code in ArrayCustomBinaryWriter is used, the following works
            // however Vts.ReportForward/InverseSolver.Desktop does not build
            // decided to revert code and comment out this test
            //double[] array = new double[3] { 1.0, 2.0, 3.0 };
            //FileIO.WriteArrayToBinary(array, "array1", true);
            //Assert.IsTrue(FileIO.FileExists("array1"));
            //Assert.IsTrue(new FileInfo("array1").Length != 0);
            //Assert.IsTrue(FileIO.FileExists("array1.txt"));
            //double[] data = new double[3];
            //data = (double[])FileIO.ReadArrayFromBinary<double>("array1", 3);
            //Assert.AreEqual(data[0], 1.0);
        }
    }
}
