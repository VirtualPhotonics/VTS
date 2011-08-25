using System;
using Vts.IO;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// This controls the reading of a database of generic-type.
    /// </summary>
    /// <typeparam name="TDatabase">Database of generic type T</typeparam>
    /// <typeparam name="TElement">Generic database element</typeparam>
    public class DatabaseReader<TDatabase, TElement> where TDatabase : Database<TElement>
    {
        private Func<TDatabase, ICustomBinaryReader<TElement>> _binaryReaderCreator;

        /// <summary>
        /// Creates an instance of DatabaseReader with a map to create a binary reader. Use this overload
        /// if you need database-specific information in order to deserialize (e.g. number of subregions, etc)
        /// </summary>
        /// <param name="binaryReaderCreator"></param>
        public DatabaseReader(Func<TDatabase, ICustomBinaryReader<TElement>> binaryReaderCreator)
        {
            _binaryReaderCreator = binaryReaderCreator;
        }

        /// <summary>
        /// Creates an instance of DatabaseReader with a simple binary reader. Use this overload
        /// if there is no database-specific information necessary for creating the reader
        /// </summary>
        /// <param name="binaryReader"></param>
        public DatabaseReader(ICustomBinaryReader<TElement> binaryReader)
            : this(db => binaryReader)
        {
        }

        public TDatabase FromFile(string fileName)
        {
            var database = FileIO.ReadFromXML<TDatabase>(fileName + ".xml");

            var binaryReader = _binaryReaderCreator(database);

            var dataPoints = FileIO.ReadFromBinaryCustom<TElement>(fileName, binaryReader.ReadFromBinary);

            database.SetDataPoints(dataPoints);

            return database;
        }

        public TDatabase FromFileInResources(string fileName, string projectName)
        {
            var database = FileIO.ReadFromXMLInResources<TDatabase>(fileName + ".xml", projectName);

            var binaryReader = _binaryReaderCreator(database);

            var dataPoints = FileIO.ReadFromBinaryInResourcesCustom<TElement>(
                fileName,
                projectName,
                binaryReader.ReadFromBinary);

            database.SetDataPoints(dataPoints);

            return database;
        }
    }
}
