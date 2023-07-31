using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Vts.Extensions;


namespace Vts.IO
{
    /// <summary>
    /// This class includes methods for saving and loading JSON text and binary data
    /// It uses custom iterators for saving (unfortunately, nothing as useful for reading value types)
    /// Currently, float, double and ushort are supported (and they're processed in that order)
    /// </summary>
    public static class FileIO
    {
        /// <summary>
        /// Static method to check if a file exists
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>A Boolean indicating whether file exists or not</returns>
        public static bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        /// <summary>
        /// Static method to check if a directory exists
        /// </summary>
        /// <param name="folder">Name of the directory</param>
        /// <returns>A Boolean indicating whether directory exists or not</returns>
        public static bool DirectoryExists(string folder)
        {
            return Directory.Exists(folder);
        }

        /// <summary>
        /// Static method to delete a file
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        public static void FileDelete(string fileName)
        {
            if(File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Static method to delete a directory
        /// </summary>
        /// <param name="folder">Name of the directory to delete</param>
        public static void DeleteDirectory(string folder)
        {
            if (!Directory.Exists(folder)) return;
            ClearDirectory(folder);
            Directory.Delete(folder);
        }

        /// <summary>
        /// Static method to clone an object
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="myObject">The object to clone</param>
        /// <returns>A clone of the object</returns>
        public static T Clone<T>(this T myObject)
        {
            var serialized = myObject.WriteToJson();
            return serialized.ReadFromJson<T>();
        }

        /// <summary>
        /// Copies one stream to the other
        /// </summary>
        /// <param name="input">Input stream to copy</param>
        /// <param name="output">Output copied stream</param>
        /// <remarks>See http://stackoverflow.com/questions/230128/best-way-to-copy-between-two-stream-instances-c </remarks>
        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Platform-agnostic directory creation
        /// </summary>
        /// <param name="folderPath">Path for new directory</param>
        public static void CreateDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }        
        
        /// <summary>
        /// Platform-agnostic directory
        /// </summary>
        /// <param name="folderPath">Path for new directory</param>
        public static void ClearDirectory(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Platform-agnostic call to create an empty directory (deleting files if they exist)
        /// </summary>
        /// <param name="folderPath">Path for new directory</param>
        public static void CreateEmptyDirectory(string folderPath)
        {
            FileIO.ClearDirectory(folderPath);
            FileIO.CreateDirectory(folderPath);
        }

        /// <summary>
        /// Static method to write an object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be written</typeparam>
        /// <param name="myObject">Object to be written</param>
        /// <param name="stream">Stream to which to write the object</param>
        public static void WriteToXMLStream<T>(this T myObject, Stream stream)
        {
            var dcs = new DataContractSerializer(typeof(T), KnownTypes.CurrentKnownTypes.Values);
            dcs.WriteObject(stream, myObject);
        }
        
        /// <summary>
        /// Static method to write an object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be written</typeparam>
        /// <param name="myObject">Object to be written</param>
        /// <param name="stream">Stream to which to write the object</param>
        public static void WriteJsonToStream<T>(this T myObject, Stream stream)
        {
            var serializedJson = myObject.WriteToJson();
            WriteTextToStream(serializedJson, stream);
        }

        /// <summary>
        /// Static method to read a specified type from a stream
        /// </summary>
        /// <typeparam name="T">Type of the data to be read</typeparam>
        /// <param name="stream">Stream from which to read</param>
        /// <returns>The data as the specified type</returns>
        public static T ReadFromStream<T>(Stream stream)
        {
            var dcs = new DataContractSerializer(typeof(T), KnownTypes.CurrentKnownTypes.Values);
            return (T)dcs.ReadObject(stream);
        }

        /// <summary>
        /// Static method to read a specified type from a stream
        /// </summary>
        /// <typeparam name="T">Type of the data to be read</typeparam>
        /// <param name="stream">Stream from which to read</param>
        /// <returns>The data as the specified type</returns>
        public static T ReadFromJsonStream<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var serializedJson = sr.ReadToEnd();
                var myObject = serializedJson.ReadFromJson<T>();
                return myObject;
            }
        }

        /// <summary>
        /// Writes the string to a stream
        /// </summary>
        /// <param name="text">Text to write to the file</param>
        /// <param name="stream">Name of the stream</param>
        public static void WriteTextToStream(string text, Stream stream)
        {
            using (var outStream = new StreamWriter(stream))
            {
                outStream.Write(text);
            }
        }

        /// <summary>
        /// Writes the string to a text file
        /// </summary>
        /// <param name="text">Text to write to the file</param>
        /// <param name="filename">Name of the text file to write </param>
        public static void WriteToTextFile(string text, string filename)
        {
            var stream = StreamFinder.GetFileStream(filename, FileMode.Create);
            using (var outFile = new StreamWriter(stream))
            {
                outFile.Write(text);
            }
        }

        /// <summary>
        /// Writes data of a specified type to an XML file
        /// </summary>
        /// <typeparam name="T">Type of the data to be written</typeparam>
        /// <param name="myObject">Object to be written</param>
        /// <param name="filename">Name of the XML file to write</param>
        public static void WriteToXML<T>(this T myObject, string filename)
        {
            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            {
                myObject.WriteToXMLStream(stream);
            }
        }

        /// <summary>
        /// Writes data of a specified type to an JSON file
        /// </summary>
        /// <typeparam name="T">Type of the data to be written</typeparam>
        /// <param name="myObject">Object to be written</param>
        /// <param name="filename">Name of the JSON file to write</param>
        public static void WriteToJson<T>(this T myObject, string filename)
        {
            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            {
                myObject.WriteJsonToStream(stream);
            }
        }

        /// <summary>
        /// Reads data of a specified type from an XML file
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="filename">Name of the XML file to be read</param>
        /// <returns>The data as the specified type</returns>
        public static T ReadFromXML<T>(string filename)
        {
            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Open))
            {
                return ReadFromStream<T>(stream);
            }
        }

        /// <summary>
        /// Reads data of a specified type from a JSON file
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="filename">Name of the JSON file to be read</param>
        /// <returns>The data as the specified type</returns>
        public static T ReadFromJson<T>(string filename)
        {
            using (var stream = StreamFinder.GetFileStream(filename, FileMode.Open))
            {
                return ReadFromJsonStream<T>(stream);
            }
        }

        /// <summary>
        /// Copy a file from resources to an external location
        /// </summary>
        /// <example>FileIO.CopyFileFromResources("Resources/resourcesfile.txt", Path.Combine(resultsFolder, "resourcefile.txt"), "Vts.Desktop.Test");</example>
        /// <param name="sourceFileName">Path and filename of the file in resources</param>
        /// <param name="destinationFileName">Path and filename of the destination location</param>
        /// <param name="projectName">The name of the project where the file in resources is located</param>
        public static void CopyFileFromResources(string sourceFileName, string destinationFileName, string projectName)
        {
            using (var stream = StreamFinder.GetFileStreamFromResources(sourceFileName, projectName))
            {
                var emptyStream = StreamFinder.GetFileStream(destinationFileName, FileMode.Create);
                stream.CopyTo(emptyStream);
                emptyStream.Close();
            }
        }

        /// <summary>
        /// Copy a file from embedded resources in the project assembly and 
        /// copies to an external location
        /// </summary>
        /// <param name="sourceFileName">Path and filename of the file in resources</param>
        /// <param name="destinationFileName">Path and filename of the destination location</param>
        /// <param name="projectName">The name of the project where the file is located</param>
        public static void CopyFileFromEmbeddedResources(string sourceFileName, string destinationFileName, string projectName)
        {
            var currentAssembly = Assembly.Load(projectName);
            Stream emptyStream;
            Stream stream;
            using (stream = currentAssembly.GetManifestResourceStream(sourceFileName))
            {
                emptyStream = StreamFinder.GetFileStream(destinationFileName, FileMode.Create);
                if (stream != null)
                {
                    stream.CopyTo(emptyStream);
                }
            }
            emptyStream.Close();
        }

        /// <summary>
        /// Copy a folder and its contents to an external location.
        /// Due to the file and folder delimiters being a dot, there are 
        /// some assumptions with this method. The file extension must be 
        /// only 3 characters and a file without an extension must have a 
        /// name with more than 3 characters.
        /// </summary>
        /// <param name="folderName">Name of the folder to copy</param>
        /// <param name="destinationFolder">Name of the destination folder to copy the folder</param>
        /// <param name="projectName">Name of the project where the file is located</param>
        /// <param name="includeFolder">Boolean value to determine whether to include the containing folder</param>
        /// <returns>Returns a list of the copied files</returns>
        public static List<string> CopyFolderFromEmbeddedResources(string folderName, string destinationFolder, string projectName, bool includeFolder)
        {
            var fileList = new List<string>();
            var currentAssembly = Assembly.Load(projectName);
            var listAssemblies = currentAssembly.GetManifestResourceNames();
            foreach (var i in listAssemblies)
            {
                // check to see if folder name is in the name - add the dots so it will check the whole folder name
                if (!i.Contains($".{folderName}.")) continue;
                var destinationFileName = "";
                var destination = "";
                var startOfFolderIndex = i.IndexOf($".{folderName}.", StringComparison.Ordinal) + folderName.Length + 2; // includes the dots so add 2
                var possibleFileName = i.Substring(startOfFolderIndex); // possible filename but it could also be a sub folder and filename
                if (!possibleFileName.Contains(".")) // file in the root of the folder with no extension
                {
                    destinationFileName = possibleFileName;
                }
                else
                {
                    // get the filename extension
                    var ext = i.Substring(i.LastIndexOf(".", StringComparison.Ordinal));
                    // get the length of the filename without the extension
                    var filenameLength = (i.Length - startOfFolderIndex) - (i.Length - i.LastIndexOf(".", StringComparison.Ordinal));
                    var folderToLastDot = i.Substring(startOfFolderIndex, filenameLength);
                    if (ext.Length > 4) // extensions are usually dot + 3 chars 
                    {
                        // if the extension is longer assume it's a filename not an extension
                        //destinationFileName = ext.Substring(1) // get the name after the dot -> not used
                        ext = "";
                        folderToLastDot = possibleFileName;
                    }
                    // get the filename if there are more folders
                    var filename = folderToLastDot;
                    if (folderToLastDot.Contains("."))
                    {
                        filename = folderToLastDot.Substring(folderToLastDot.LastIndexOf(".", StringComparison.Ordinal) + 1);
                        var folders = folderToLastDot.Substring(0, folderToLastDot.Length - (folderToLastDot.Length - folderToLastDot.LastIndexOf(".", StringComparison.Ordinal)));
                        var folderList = folders.Split('.');
                        foreach (var folder in folderList)
                        {
                            destination = Path.Combine(destination, folder);
                        }
                    }
                    destinationFileName = filename + ext;
                }
                if (includeFolder)
                {
                    destination = Path.Combine(folderName, destination);
                }
                var sourceFileName = i;
                CreateDirectory(Path.Combine(destinationFolder, destination));
                CopyFileFromEmbeddedResources(sourceFileName, Path.Combine(destinationFolder, destination, destinationFileName), projectName);
                fileList.Add(Path.Combine(destination, destinationFileName));
            }
            return fileList;
        }

        /// <summary>
        /// Reads data of a specified type from an XML file in resources
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="fileName">Name of the XML file to be read</param>
        /// <param name="projectName">Project name for the location of resources</param>
        /// <returns>The data as the specified type</returns>
        public static T ReadFromXMLInResources<T>(string fileName, string projectName)
        {
            using (var stream = StreamFinder.GetFileStreamFromResources(fileName, projectName))
            {
                return ReadFromStream<T>(stream);
            }
        }

        /// <summary>
        /// Reads data of a specified type from an JSON file in resources
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="fileName">Name of the JSON file to be read</param>
        /// <param name="projectName">Project name for the location of resources</param>
        /// <returns>The data as the specified type</returns>
        public static T ReadFromJsonInResources<T>(string fileName, string projectName)
        {
            using (var stream = StreamFinder.GetFileStreamFromResources(fileName, projectName))
            {
                return ReadFromJsonStream<T>(stream);
            }
        }

        /// <summary>
        /// Writes a scalar value to a binary file
        /// </summary>
        /// <typeparam name="T">Type of the data to be written</typeparam>
        /// <param name="dataIn">Data to be written</param>
        /// <param name="filename">Name of the binary file to write</param>
        /// <param name="writeMap">Action used to write binary</param>
        public static void WriteScalarValueToBinary<T>(T dataIn, string filename, Action<BinaryWriter, T> writeMap)
        {
            // Create a file to write binary data 
            using (var s = StreamFinder.GetFileStream(filename, FileMode.OpenOrCreate))
            {
                using (var bw = new BinaryWriter(s))
                {
                    writeMap(bw, dataIn);
                }
            }
        }

        /// <summary>
        /// Reads a scalar value from a binary file
        /// </summary>
        /// <typeparam name="T">Type of data to be read</typeparam>
        /// <param name="filename">Name of the binary file</param>
        /// <param name="readMap">function used to read binary</param>
        /// <returns>Generic type T</returns>
        public static T ReadScalarValueFromBinary<T>(string filename, Func<BinaryReader, T> readMap)
        {
            // Create a file to write binary data 
            using (var s = StreamFinder.GetFileStream(filename, FileMode.OpenOrCreate))
            {
                using (var br = new BinaryReader(s))
                {
                    return readMap(br);
                }
            }
        }

        /// <summary>
        /// Writes an array to a binary file and optionally accompanying .xml file 
        /// (to store array dimensions) if includeMetaData = true
        /// </summary>
        /// <param name="dataIN">Array to be written</param>
        /// <param name="filename">Name of the file where the data is written</param>
        /// <param name="includeMetaData">Boolean to determine whether to include meta data, if set to true, an accompanying XML file will be created with the same name</param>
        public static void WriteArrayToBinary(Array dataIN, string filename, bool includeMetaData)
        {
            // Write XML file to describe the contents of the binary file
            if (includeMetaData)
            {
                new MetaData(dataIN).WriteToJson(filename + ".txt");
            }
            // Create a file to write binary data 
            using (var s = StreamFinder.GetFileStream(filename, FileMode.OpenOrCreate))
            {
                using (var bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, dataIN);
                }
            }
        }

        /// <summary>
        /// Writes an array to a binary file, as well as an accompanying .txt (JSON) file to store array dimensions
        /// </summary>
        /// <param name="dataIN">Array to be written</param>
        /// <param name="filename">Name of the file to which the array is written</param>
        public static void WriteArrayToBinary(Array dataIN, string filename)
        {
            WriteArrayToBinary(dataIN, filename, true);
        }

        /// <summary>
        /// Reads array from a binary file, using the accompanying .txt file to specify dimensions
        /// </summary>
        /// <typeparam name="T">Type of the array being read</typeparam>
        /// <param name="filename">Name of the file from which to read the array</param>
        /// <returns>Array from the file</returns>
        public static Array ReadArrayFromBinary<T>(string filename) where T : struct
        {
            var dataInfo = ReadFromJson<MetaData>(filename + ".txt");

            return ReadArrayFromBinary<T>(filename, dataInfo.dims);
        }

        /// <summary>
        /// Reads array from a binary file using explicitly-set dimensions
        /// </summary>
        /// <typeparam name="T">Type of the array being read</typeparam>
        /// <param name="filename">Name of the file from which to read the array</param>
        /// <param name="dims">Dimensions of the array</param>
        /// <returns>Array from the file</returns>
        public static Array ReadArrayFromBinary<T>(string filename, params int[] dims) where T : struct
        {
            using (var s = StreamFinder.GetFileStream(filename, FileMode.Open))
            {
                using (var br = new BinaryReader(s))
                {
                    return new ArrayCustomBinaryReader<T>(dims).ReadFromBinary(br);
                }
            }
        }

        /// <summary>
        /// Reads array from a binary file in resources, using the accompanying .txt (JSON) file to specify dimensions
        /// </summary>
        /// <typeparam name="T">Type of the array being read</typeparam>
        /// <param name="filename">Name of the JSON file containing the meta data</param>
        /// <param name="projectName">Project name for the location of resources</param>
        /// <returns>Array from the file</returns>
        public static Array ReadArrayFromBinaryInResources<T>(string filename, string projectName) where T : struct
        {
            // Read JSON text file that describes the contents of the binary file
            var dataInfo = ReadFromJsonInResources<MetaData>(filename + ".txt", projectName);

            // call the overload (below) which explicitly specifies the array dimensions
            return ReadArrayFromBinaryInResources<T>(filename, projectName, dataInfo.dims);
        }

        /// <summary>
        /// Reads array from a binary file in resources using explicitly-set dimensions
        /// </summary>
        /// <typeparam name="T">Type of the array being read</typeparam>
        /// <param name="filename">Name of the JSON (text) file containing the meta data</param>
        /// <param name="projectName">Project name for the location of resources</param>
        /// <param name="dims">Dimensions of the array</param>
        /// <returns>Array from the file</returns>
        public static Array ReadArrayFromBinaryInResources<T>(string filename, string projectName, params int[] dims) where T : struct
        {
            using (var stream = StreamFinder.GetFileStreamFromResources(filename, projectName))
            {
                using (var br = new BinaryReader(stream))
                {
                    return new ArrayCustomBinaryReader<T>(dims).ReadFromBinary(br);
                }
            }
        }


        /// <summary>
        /// Write to binary file using an Action
        /// </summary>
        /// <typeparam name="T">The type to be written as binary</typeparam>
        /// <param name="data">Data to be written</param>
        /// <param name="fileName">Name of the binary file to write</param>
        /// <param name="writerMap">Action of BinaryWriter and T</param>
        public static void WriteToBinaryCustom<T>(this IEnumerable<T> data, string fileName, Action<BinaryWriter, T> writerMap)
        {
            // convert to "push" method with System.Observable in Rx Extensions (write upon appearance of new datum)?
            using (var s = StreamFinder.GetFileStream(fileName, FileMode.Create))
            {
                using (var bw = new BinaryWriter(s))
                {
                    data.ForEach(d => writerMap(bw, d));
                }
            }
        }

        /// <summary>
        /// Read from binary file using reader map
        /// </summary>
        /// <typeparam name="T">Type to be read from binary</typeparam>
        /// <param name="fileName">Name of the binary file to read</param>
        /// <param name="readerMap">Function of BinaryReader and generic type</param>
        /// <returns>IEnumerable of generic type T</returns>
        public static IEnumerable<T> ReadFromBinaryCustom<T>(string fileName, Func<BinaryReader, T> readerMap)
        {
            using (var s = StreamFinder.GetFileStream(fileName, FileMode.Open))
            {
                foreach (var item in ReadStreamFromBinaryCustom<T>(s, readerMap))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Read from binary file in Resources 
        /// </summary>
        /// <typeparam name="T">Type to be read from binary in resources</typeparam>
        /// <param name="fileName">Name of the binary file to read</param>
        /// <param name="projectName">Project name where resources is located</param>
        /// <param name="readerMap">Function of BinaryReader and T</param>
        /// <returns>IEnumerable of generic type T</returns>
        public static IEnumerable<T> ReadFromBinaryInResourcesCustom<T>(string fileName, string projectName, Func<BinaryReader, T> readerMap)
        {
            using (var s = StreamFinder.GetFileStreamFromResources(fileName, projectName))
            {
                foreach (var item in ReadStreamFromBinaryCustom<T>(s, readerMap))
                {
                    yield return item;
                }
            }
        }

        // both versions of ReadArrayFromBinary<T> call this method to actually read the data - is this still true?
        /// <summary>
        /// Read of binary file using reader map
        /// </summary>
        /// <typeparam name="T">Type to be read from binary</typeparam>
        /// <param name="s">The binary stream</param>
        /// <param name="readerMap">Function of BinaryReader and T</param>
        /// <returns>IEnumerable of T</returns>
        private static IEnumerable<T> ReadStreamFromBinaryCustom<T>(Stream s, Func<BinaryReader, T> readerMap)
        {
            using (var br = new BinaryReader(s))
            {
                while (s.Position < s.Length)
                {
                    yield return readerMap(br);
                }
            }
        }
    }
}