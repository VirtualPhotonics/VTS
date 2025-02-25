using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;

namespace Vts.Test.IO
{
    [TestFixture]
    public class FileIOTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "fileiotest.folder",
            "folder1",
            "folder2",
            "folder3",
            "folder4",
            Path.Combine("sourcetest", "subfolder"),
            "sourcetest"
        };

        readonly List<string> listOfTestGeneratedFiles = new List<string>()
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
            "2darray",
            "2darray.txt",
            "floatarray",
            "floatarray.txt",
            "complexarray",
            "complexarray.txt",
            "bytearray",
            "bytearray.txt",
            "ushortarray",
            "ushortarray.txt",
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
                Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.True);
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
            Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.True);
            FileIO.ClearDirectory(folder);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.False);
        }

        [Test]
        public void validate_clone()
        {
            var i = new Position { X = 2, Y = 5, Z = 9 };
            var iCloned = i.Clone();
            Assert.That(i.X, Is.EqualTo(iCloned.X));
            Assert.That(i.Y, Is.EqualTo(iCloned.Y));
            Assert.That(i.Z, Is.EqualTo(iCloned.Z));
        }

        [Test]
        public void validate_copy_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var stream1 = StreamFinder.GetFileStreamFromResources("Resources/streamfindertest/resourcefile.txt", assemblyName);
            var stream2 = StreamFinder.GetFileStream("file5.txt", FileMode.CreateNew);
            Assert.That(stream1, Is.Not.Null);
            FileIO.CopyStream(stream1, stream2);
            Assert.That(stream2, Is.Not.Null);
            Assert.That(stream2, Is.EqualTo(stream1));
            stream1.Close();
            stream2.Close();
        }

        [Test]
        public void validate_create_directory()
        {
            const string folder = "folder2";
            FileIO.CreateDirectory(folder);
            Assert.That(FileIO.DirectoryExists(folder), Is.True);
        }

        [Test]
        public void validate_create_empty_directory()
        {
            const string folder = "fileiotest.folder";
            if (!FileIO.DirectoryExists(folder))
            {
                FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
                Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.True);
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
            Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.True);
            FileIO.CreateEmptyDirectory(folder);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.False);
        }


        [Test]
        public void validate_read_array_from_binary_in_resources_with_size_parameter()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            string dataLocation = "Resources/fileiotest/";
            int size = 100;
            var data = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"ROfRho", assemblyName, size);
            Assert.That(Math.Abs(data[2] - 0.052445) < 0.000001, Is.True);
        }

        [Test]
        public void validate_read_array_from_binary_in_resources_without_parameter_dimensions()
        {
            // ReadArrayFromBinaryInResources without parameter dimensions calls ReadFromJsonInResources
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            string dataLocation = "Resources/fileiotest/";
            var data = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"ROfRho", assemblyName);
            Assert.That(Math.Abs(data[2] - 0.052445) < 0.000001, Is.True);
        }

        [Test] 
        public void validate_read_from_binary_in_resources_custom()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            double ReadMap(BinaryReader b) => b.ReadDouble();
            // the following method has a "yield return" so won't load until accessed
            var arrayRead = FileIO.ReadFromBinaryInResourcesCustom<double>(
                "Resources/fileiotest/ROfRho", assemblyName, ReadMap);
            Assert.That(Math.Abs(arrayRead.Skip(2).Take(1).First() - 0.052445) < 0.000001, Is.True);
        }

        [Test] 
        public void validate_read_from_binary_custom()
        {
            IEnumerable<double> arrayWritten = Enumerable.Range(15, 3).Select(x => (double)x);
            void WriteMap(BinaryWriter b, double s) => b.Write(s);
            FileIO.WriteToBinaryCustom<double>(arrayWritten, "array6", WriteMap);
            double ReadMap(BinaryReader b) => b.ReadDouble();
            // the following method has a "yield return" so won't load until accessed
            var listRead = FileIO.ReadFromBinaryCustom<double>("array6", ReadMap);
            var arrayRead = listRead.Take(3).ToArray();
            Assert.That(arrayRead[1], Is.EqualTo(16));
        }

        [Test]
        public void validate_read_from_json()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/fileiotest/position.txt", "position.txt", assemblyName);
            var pos = FileIO.ReadFromJson<Position>("position.txt");
            Assert.That(pos.X, Is.EqualTo(5));
            Assert.That(pos.Y, Is.EqualTo(10));
            Assert.That(pos.Z, Is.EqualTo(15));
        }

        [Test]
        public void validate_read_from_json_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var pos = FileIO.ReadFromJsonInResources<Position>("Resources/fileiotest/position.txt", assemblyName);
            Assert.That(pos.X, Is.EqualTo(5));
            Assert.That(pos.Y, Is.EqualTo(10));
            Assert.That(pos.Z, Is.EqualTo(15));
        }

        [Test]
        public void validate_read_from_json_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            // create a JSON stream
            var stream = StreamFinder.GetFileStreamFromResources("Resources/fileiotest/position.txt", assemblyName);
            var pos = FileIO.ReadFromJsonStream<Position>(stream);
            Assert.That(pos.X, Is.EqualTo(5));
            Assert.That(pos.Y, Is.EqualTo(10));
            Assert.That(pos.Z, Is.EqualTo(15));
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
            Assert.That(pos.X, Is.EqualTo(2));
            Assert.That(pos.Y, Is.EqualTo(4));
            Assert.That(pos.Z, Is.EqualTo(6));
        }

        [Test]
        public void validate_read_from_xml_in_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var pos = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            Assert.That(pos.X, Is.EqualTo(2));
            Assert.That(pos.Y, Is.EqualTo(4));
            Assert.That(pos.Z, Is.EqualTo(6));
        }


        [Test]
        public void validate_write_json_to_stream()
        {
            var pos = new Position(2, 4, 6);
            var stream = StreamFinder.GetFileStream("file6.txt", FileMode.Create);
            pos.WriteJsonToStream(stream);
            var pos2 = FileIO.ReadFromJson<Position>("file6.txt");
            Assert.That(pos2.X, Is.EqualTo(2));
            Assert.That(pos2.Y, Is.EqualTo(4));
            Assert.That(pos2.Z, Is.EqualTo(6));
        }

        [Test]
        public void validate_write_scalar_value_to_binary_and_read_scalar_value_from_binary()
        {
            // write scalar using Action=WriteMap and validate file exists and non-zero length
            var scalar = 11;
            void WriteMap(BinaryWriter b, int s) => b.Write(s);
            FileIO.WriteScalarValueToBinary<int>(scalar, "scalar", WriteMap);
            Assert.That(FileIO.FileExists("scalar"), Is.True);
            Assert.That(new FileInfo("scalar").Length != 0, Is.True);
            // then read what what written using func ReadMap and validate value
            int ReadMap(BinaryReader b) => b.Read();
            var data = FileIO.ReadScalarValueFromBinary<int>("scalar", ReadMap);
            Assert.That(data, Is.EqualTo(11));
        }

        [Test]
        public void validate_write_to_binary_custom()
        {
            var arrayWritten = Enumerable.Range(7, 3).Select(x => (double) x);          
            void WriteMap(BinaryWriter b, double s) => b.Write(s);
            arrayWritten.WriteToBinaryCustom<double>("array3", WriteMap);
            Assert.That(FileIO.FileExists("array3"), Is.True);
            Assert.That(new FileInfo("array3").Length != 0, Is.True);
        }

        [Test]
        public void validate_write_to_json()
        {
            var pos = new Position(2, 4, 6);
            pos.WriteToJson("file7.txt");
            var pos2 = FileIO.ReadFromJson<Position>("file7.txt");
            Assert.That(pos2.X, Is.EqualTo(2));
            Assert.That(pos2.Y, Is.EqualTo(4));
            Assert.That(pos2.Z, Is.EqualTo(6));
        }

        [Test]
        public void validate_write_to_xml_and_read_from_xml()
        {
            var pos = new Position(2, 4, 6);
            pos.WriteToXML<Position>("file7.xml");
            Assert.That(FileIO.FileExists("file7.xml"), Is.True);
            Assert.That(new FileInfo("file7.xml").Length != 0, Is.True);
            var pos2 = FileIO.ReadFromXML<Position>("file7.xml");
            Assert.That(pos2.X, Is.EqualTo(2));
            Assert.That(pos2.Y, Is.EqualTo(4));
            Assert.That(pos2.Z, Is.EqualTo(6));
        }

        [Test]
        public void validate_write_to_xml_stream()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            var xmlFile = FileIO.ReadFromXMLInResources<Position>("Resources/fileiotest/file7.xml", assemblyName);
            var stream = StreamFinder.GetFileStream("file8.xml", FileMode.Create);
            xmlFile.WriteToXMLStream(stream);
            Assert.That(stream, Is.Not.Null);
            Assert.That(FileIO.FileExists("file8.xml"), Is.True);
            Assert.That(new FileInfo("file8.xml").Length != 0, Is.True);
            stream.Close();
        }

        [Test]
        public void validate_copy_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.fileiotest.embeddedresourcefile.txt", "embeddedresourcefile.txt", name);
            Assert.That(FileIO.FileExists("embeddedresourcefile.txt"), Is.True);
        }

        [Test]
        public void validate_copy_binary_file_from_embedded_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(assemblyName + ".Resources.sourcetest.AOfXAndYAndZ", "AOfXAndYAndZ", name);
            Assert.That(FileIO.FileExists("AOfXAndYAndZ"), Is.True);
        }

        [Test]
        public void validate_copy_file_from_resources()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromResources("Resources/streamfindertest/resourcefile.txt", "resourcefile.txt", assemblyName); 
            Assert.That(FileIO.FileExists("resourcefile.txt"), Is.True);
        }

        [Test]
        public void validate_copy_folder_from_embedded_resources()
        {
            var folder = "fileiotest.folder";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "embeddedresourcefile.txt")), Is.True);
        }

        [Test]
        public void validate_copy_folder_containing_binary_from_embedded_resources()
        {
            var folder = "sourcetest";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ")), Is.True);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ.txt")), Is.True);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "inputAOfXAndYAndZ.txt")), Is.True);
        }

        [Test]
        public void validate_copy_folder_and_sub_folder_from_embedded_resources()
        {
            var folder = "sourcetest";
            FileIO.CopyFolderFromEmbeddedResources(folder, "", Assembly.GetExecutingAssembly().FullName, true);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "subfolder", "noextension")), Is.True);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "subfolder", "textfile.txt")), Is.True);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ")), Is.True);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "AOfXAndYAndZ.txt")), Is.True);
            Assert.That(FileIO.FileExists(Path.Combine(folder, "inputAOfXAndYAndZ.txt")), Is.True);
        }

        [Test]
        public void validate_file_exists()
        {
            const string file = "file1.txt";
            FileIO.WriteToTextFile("Text", file);
            Assert.That(FileIO.FileExists(file), Is.True);
        }

        [Test]
        public void validate_directory_exists()
        {
            const string folder = "folder1";
            FileIO.CreateDirectory(folder);
            Assert.That(FileIO.DirectoryExists(folder), Is.True);
        }

        [Test]
        public void validate_file_delete()
        {
            const string file = "file2.txt";
            FileIO.WriteToTextFile("Text", file);
            FileIO.FileDelete(file);
            Assert.That(FileIO.FileExists(file), Is.False);
        }

        [Test]
        public void validate_directory_delete()
        {
            const string folder = "folder3";
            FileIO.CreateDirectory(folder);
            Assert.That(FileIO.DirectoryExists(folder), Is.True);
            FileIO.DeleteDirectory(folder);
            Assert.That(FileIO.DirectoryExists(folder), Is.False);
        }

        [Test]
        public void validate_write_text_to_file()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            FileIO.WriteToTextFile(myString.ToString(), "file3.txt");
            Assert.That(FileIO.FileExists("file3.txt"), Is.True);
        }

        [Test]
        public void validate_write_text_to_stream()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            Stream stream = StreamFinder.GetFileStream("file4.txt", FileMode.Create);
            FileIO.WriteTextToStream(myString.ToString(), stream);
            Assert.That(stream, Is.Not.Null);
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void Validate_write_double_array_to_binary_and_read_array_from_binary()
        {
            var array = new double[3] { 1.0, 2.0, 3.0 };
            FileIO.WriteArrayToBinary(array, "array1");
            Assert.That(FileIO.FileExists("array1"), Is.True);
            Assert.That(new FileInfo("array1").Length != 0, Is.True);
            Assert.That(FileIO.FileExists("array1.txt"), Is.True);
            var data = (double[])FileIO.ReadArrayFromBinary<double>("array1");
            Assert.That(data[0], Is.EqualTo(1.0));
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_floating_point_array_to_binary_and_read_array_from_binary()
        {
            var array = new float[3] { 1.0F, 2.0F, 3.0F };
            FileIO.WriteArrayToBinary(array, "floatarray", true);
            Assert.That(FileIO.FileExists("floatarray"), Is.True);
            Assert.That(new FileInfo("floatarray").Length != 0, Is.True);
            Assert.That(FileIO.FileExists("floatarray.txt"), Is.True);
            var data = (float[])FileIO.ReadArrayFromBinary<float>("floatarray", 3);
            Assert.That(data[0], Is.EqualTo(1.0F));
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_complex_array_to_binary_and_read_array_from_binary()
        {
            var array = new Complex[2] { new Complex(0.85, 0.0), new Complex(0.3, 0.0) };
            FileIO.WriteArrayToBinary(array, "complexarray", true);
            Assert.That(FileIO.FileExists("complexarray"), Is.True);
            Assert.That(new FileInfo("complexarray").Length != 0, Is.True);
            Assert.That(FileIO.FileExists("complexarray.txt"), Is.True);
            var data = (Complex[])FileIO.ReadArrayFromBinary<Complex>("complexarray", 2);
            Assert.That(new Complex(0.3, 0.0), Is.EqualTo(data[1]));
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_ushort_array_to_binary_and_read_array_from_binary()
        {
            var array = new ushort[2] { 5, 7 };
            FileIO.WriteArrayToBinary(array, "ushortarray", true);
            Assert.That(FileIO.FileExists("ushortarray"), Is.True);
            Assert.That(new FileInfo("ushortarray").Length != 0, Is.True);
            Assert.That(FileIO.FileExists("ushortarray.txt"), Is.True);
            var data = (ushort[])FileIO.ReadArrayFromBinary<ushort>("ushortarray", 2);
            Assert.That(data[1], Is.EqualTo(7));
        }

        /// <summary>
        /// This unit test first verifies WriteArrayToBinary works successfully the
        /// reads back array with ReadArrayFromBinary and checks values
        /// </summary>
        [Test]
        public void validate_write_byte_array_to_binary_and_read_array_from_binary()
        {
            var array = new byte[5] { 1, 0, 0, 1, 0 };
            FileIO.WriteArrayToBinary(array, "bytearray", true);
            Assert.That(FileIO.FileExists("bytearray"), Is.True);
            Assert.That(new FileInfo("bytearray").Length != 0, Is.True);
            Assert.That(FileIO.FileExists("bytearray.txt"), Is.True);
            var data = (byte[])FileIO.ReadArrayFromBinary<byte>("bytearray", 5);
            Assert.That(data[1], Is.EqualTo(0));
        }

        /// <summary>
        /// This unit test verifies that an object is duplicated successfully
        /// using serialization 
        /// </summary>
        [Test]
        public void Validate__object_clone()
        {
            var input = SimulationInputProvider.PointSourceOneLayerTissueROfRhoAndFluenceOfRhoAndZDetectors();
            var inputCopy = input.Clone();
            Assert.That(inputCopy.N, Is.EqualTo(input.N));
            input.N = 10000;
            Assert.AreNotEqual(input.N, inputCopy.N);
        }
    }
}
