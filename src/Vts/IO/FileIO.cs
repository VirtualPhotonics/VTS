//#define SILVERLIGHT_SAVE_TO_LOCAL_FILESTREAM_WITH_SAVEFILEDIALOG

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Vts.Extensions;
using Vts.MonteCarlo;

#if !SILVERLIGHT
using System.Runtime.Serialization.Formatters.Binary;
#endif
using Vts.MonteCarlo.Detectors;


namespace Vts.IO
{
    /// <summary>
    /// This class includes methods for saving and loading XML and binary data
    /// It uses custom iterators for saving (unfortunately, nothing as useful for reading value types)
    /// Currently, float, double and ushort are supported (and they're processed in that order)
    /// </summary>
    public static class FileIO
    {
        public static T Clone<T>(this T myObject)
        {
            using (MemoryStream ms = new MemoryStream(1024))
            {
                var dcs = new DataContractSerializer(typeof(T));
                dcs.WriteObject(ms, myObject);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }

        public static void WriteToStream<T>(this T myObject, Stream stream)
        {
            var dcs = new DataContractSerializer(typeof(T), KnownTypes.CurrentKnownTypes.Values);
            dcs.WriteObject(stream, myObject);
        }

        public static T ReadFromStream<T>(Stream stream)
        {
            var dcs = new DataContractSerializer(typeof(T), KnownTypes.CurrentKnownTypes.Values);
            return (T)dcs.ReadObject(stream);
        }

        public static void WriteToXML<T>(this T myObject, string filename)
        {
            using (Stream stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            {
                //new DataContractSerializer(typeof(T)).WriteObject(stream, myObject);
                myObject.WriteToStream(stream);
            }
        }

        public static T ReadFromXML<T>(String filename)
        {
            using (Stream stream = StreamFinder.GetFileStream(filename, FileMode.Open))
            {
                return ReadFromStream<T>(stream);
                //return (T)new DataContractSerializer(typeof(T)).ReadObject(stream);
            }
        }

        public static T ReadFromXMLInResources<T>(string fileName, string projectName)
        {
            using (Stream stream = StreamFinder.GetFileStreamFromResources(fileName, projectName))
            {
                return ReadFromStream<T>(stream);
                //return (T)new DataContractSerializer(typeof(T)).ReadObject(stream);
            }
        }

        public static void WriteScalarValueToBinary<T>(T dataIN, string filename, Action<BinaryWriter, T> writeMap)
        {
            // Create a file to write binary data 
            using (Stream s = StreamFinder.GetFileStream(filename, FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    writeMap(bw, dataIN);
                }
            }
        }

        public static T ReadScalarValueFromBinary<T>(string filename, Func<BinaryReader, T> readMap)
        {
            // Create a file to write binary data 
            using (Stream s = StreamFinder.GetFileStream(filename, FileMode.OpenOrCreate))
            {
                using (BinaryReader br = new BinaryReader(s))
                {
                    return readMap(br);
                }
            }
        }

        /// <summary>
        /// Writes an array to a binary file and optionally accompanying .xml file 
        /// (to store array dimensions) if includeMetaData = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataIN"></param>
        /// <param name="filename"></param>
        /// <param name="includeMetaData"</param>
        public static void WriteArrayToBinary<T>(Array dataIN, string filename, bool includeMetaData) where T : struct
        {
            // Write XML file to describe the contents of the binary file
            if (includeMetaData)
            {
                new MetaData(dataIN).WriteToXML(filename + ".xml");
            }
            // Create a file to write binary data 
            using (Stream s = StreamFinder.GetFileStream(filename, FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter<T>().WriteToBinary(bw, dataIN);
                    //WriteArrayToBinaryInternal(bw, dataIN.ToEnumerable<T>());
                }
            }
        }

        /// <summary>
        /// Writes an array to a binary file, as well as an accompanying .xml file to store array dimensions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataIN"></param>
        /// <param name="filename"></param>
        public static void WriteArrayToBinary<T>(Array dataIN, string filename) where T : struct
        {
            WriteArrayToBinary<T>(dataIN, filename, true);
        }

        /// <summary>
        /// Reads array from a binary file, using the accompanying .xml file to specify dimensions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Array ReadArrayFromBinary<T>(string filename) where T : struct
        {
            MetaData dataInfo = ReadFromXML<MetaData>(filename + ".xml");

            return ReadArrayFromBinary<T>(filename, dataInfo.dims);
        }

        /// <summary>
        /// Reads array from a binary file using explicitly-set dimensions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="dims"></param>
        /// <returns></returns>
        public static Array ReadArrayFromBinary<T>(string filename, params int[] dims) where T : struct
        {
            using (Stream s = StreamFinder.GetFileStream(filename, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(s))
                {
                    //Array dataOut = Array.CreateInstance(typeof(T), dims);

                    return new ArrayCustomBinaryReader<T>(dims).ReadFromBinary(br);
                    //ReadArrayFromBinaryInternal<T>(br, ref dataOut);

                    //return dataOut;
                }
            }
        }

        /// <summary>
        /// Reads array from a binary file in resources, using the accompanying .xml file to specify dimensions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public static Array ReadArrayFromBinaryInResources<T>(string filename, string projectname) where T : struct
        {
            // Read XML file that describes the contents of the binary file
            MetaData dataInfo = ReadFromXMLInResources<MetaData>(filename + ".xml", projectname);

            // call the overload (below) which explicitly specifies the array dimensions
            return ReadArrayFromBinaryInResources<T>(filename, projectname, dataInfo.dims);
        }

        /// <summary>
        /// Reads array from a binary file in resources using explicitly-set dimensions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="projectname"></param>
        /// <param name="dims"></param>
        /// <returns></returns>
        public static Array ReadArrayFromBinaryInResources<T>(string filename, string projectname, params int[] dims) where T : struct
        {
            using (Stream stream = StreamFinder.GetFileStreamFromResources(filename, projectname))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    // Initialize the array
                    //Array dataOut = Array.CreateInstance(typeof(T), dims);
                    // Fill with data
                    //ReadArrayFromBinaryInternal<T>(br, ref dataOut);

                    return new ArrayCustomBinaryReader<T>(dims).ReadFromBinary(br);
                }
            }
        }

        //#region Write/ReadArrayToBinary Helpers

        //private static void WriteArrayToBinaryInternal<T>(BinaryWriter bw, IEnumerable<T> array) where T : struct
        //{
        //    if (array is IEnumerable<float>)
        //    {
        //        (array as IEnumerable<float>).ForEach(bw.Write);
        //    }
        //    else if (array is IEnumerable<double>)
        //    {
        //        (array as IEnumerable<double>).ForEach(bw.Write);
        //    }
        //    else if (array is IEnumerable<ushort>)
        //    {
        //        (array as IEnumerable<ushort>).ForEach(bw.Write);
        //    }
        //    else if (array is IEnumerable<byte>)
        //    {
        //        (array as IEnumerable<byte>).ForEach(bw.Write);
        //    }
        //}

        //private static void ReadArrayFromBinaryInternal<T>(BinaryReader br, ref Array myArray) where T : struct
        //{
        //    var dataType = typeof (T);

        //    if (dataType == typeof(double))
        //    {
        //        myArray.PopulateFromEnumerable(ReadDoubles(br, myArray.Length));
        //    }
        //    else if (dataType == typeof(float))
        //    {
        //        myArray.PopulateFromEnumerable(ReadFloats(br, myArray.Length));
        //    }
        //    else if (dataType == typeof(ushort))
        //    {
        //        myArray.PopulateFromEnumerable(ReadUShorts(br, myArray.Length));
        //    }
        //    else if (dataType == typeof(byte))
        //    {
        //        myArray.PopulateFromEnumerable(ReadBytes(br, myArray.Length));
        //    }
        //}

        //private static IEnumerable<double> ReadDoubles(BinaryReader br, int numberOfElements)
        //{
        //    for (int i = 0; i < numberOfElements; i++)
        //    {
        //        yield return br.ReadDouble();
        //    }
        //}
        //private static IEnumerable<float> ReadFloats(BinaryReader br, int numberOfElements)
        //{
        //    for (int i = 0; i < numberOfElements; i++)
        //    {
        //        yield return br.ReadSingle();
        //    }
        //}
        //private static IEnumerable<ushort> ReadUShorts(BinaryReader br, int numberOfElements)
        //{
        //    for (int i = 0; i < numberOfElements; i++)
        //    {
        //        yield return br.ReadUInt16();
        //    }
        //}
        //private static IEnumerable<byte> ReadBytes(BinaryReader br, int numberOfElements)
        //{
        //    return br.ReadBytes(numberOfElements);
        //    //for (int i = 0; i < numberOfElements; i++)
        //    //{
        //    //    yield return br.re.ReadByte();
        //    //}
        //}

        //#endregion

        public static void WriteToBinaryCustom<T>(this IEnumerable<T> data, string fileName, Func<BinaryWriter, T> writerMap)
        {
            // todo: convert to "push" method with System.Observable in Rx Extensions (write upon appearance of new datum)
            using (Stream s = StreamFinder.GetFileStream(fileName, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    data.ForEach(d => writerMap(bw));
                }
            }
        }

        public static IEnumerable<T> ReadFromBinaryCustom<T>(string fileName, Func<BinaryReader, T> readerMap)
        {
            using (Stream s = StreamFinder.GetFileStream(fileName, FileMode.Open))
            {
                foreach (var item in ReadStreamFromBinaryCustom<T>(s, readerMap))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> ReadFromBinaryInResourcesCustom<T>(string fileName, string projectName, Func<BinaryReader, T> readerMap)
        {
            using (Stream s = StreamFinder.GetFileStreamFromResources(fileName, projectName))
            {
                foreach (var item in ReadStreamFromBinaryCustom<T>(s, readerMap))
                {
                    yield return item;
                }
            }
        }

        // both versions of ReadArrayFromBinary<T> call this method to actually read the data
        private static IEnumerable<T> ReadStreamFromBinaryCustom<T>(Stream s, Func<BinaryReader, T> readerMap)
        {
            using (BinaryReader br = new BinaryReader(s))
            {
                while (s.Position < s.Length)
                {
                    yield return readerMap(br);
                }
            }
        }

        #region Platform-Specific Methods

#if SILVERLIGHT // stuff that currently only works on the Silverlight/CoreCLR platform
#else // stuff that currently only works on the .NET desktop/CLR platform
        // todo: investigate Silverlight Binary serializer: http://whydoidoit.com/silverlight-serializer/
        public static T ReadFromBinary<T>(string filename)
        {
            using (Stream stream = StreamFinder.GetFileStream(filename, FileMode.Open))
            {
                return ReadFromBinaryStream<T>(stream);
            }
        }

        public static T ReadFromBinaryInResources<T>(string filename, string projectName)
        {
            using (Stream stream = StreamFinder.GetFileStreamFromResources(filename, projectName))
            {
                return ReadFromBinaryStream<T>(stream);
            }
        }

        public static void WriteToBinary<T>(this T myObject, string filename)
        {
            using (Stream stream = StreamFinder.GetFileStream(filename, FileMode.Create))
            {
                WriteToBinaryStream<T>(myObject, stream);
            }
        }

        public static T ReadFromBinaryStream<T>(Stream s)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                return (T)formatter.Deserialize(s);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
        }

        public static void WriteToBinaryStream<T>(T myObject, Stream s)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(s, myObject);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
        }
#endif
        #endregion

    }
}