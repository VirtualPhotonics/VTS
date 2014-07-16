using System;
using Vts.IO;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// This controls the reading of a database of generic-type.
    /// </summary>
    /// <typeparam name="TDatabase">Database of generic type Time</typeparam>
    /// <typeparam name="TElement">Generic database element</typeparam>
    public class DatabaseReader<TDatabase, TElement> where TDatabase : Database<TElement>
    {
        private Func<TDatabase, ICustomBinaryReader<TElement>> _binaryReaderCreator;

        /// <summary>
        /// Creates an instance of DatabaseReader with a map to create a binary reader. Use this overload
        /// if you need database-specific information in order to deserialize (e.g. number of subregions, etc)
        /// </summary>
        /// <param name="binaryReaderCreator">ICustomBonaryReader&lt;TElement&gt;</param>
        public DatabaseReader(Func<TDatabase, ICustomBinaryReader<TElement>> binaryReaderCreator)
        {
            _binaryReaderCreator = binaryReaderCreator;
        }

        /// <summary>
        /// Creates an instance of DatabaseReader with a simple binary reader. Use this overload
        /// if there is no database-specific information necessary for creating the reader
        /// </summary>
        /// <param name="binaryReader">ICustomBinaryReader&lt;TElement&gt;</param>
        public DatabaseReader(ICustomBinaryReader<TElement> binaryReader)
            : this(db => binaryReader)
        {
        }

        /// <summary>
        /// Creates a database of generic type Time from an XML file
        /// </summary>
        /// <param name="fileName">Name of the XML file to be read</param>
        /// <returns>a database of generic type Time</returns>
        public TDatabase FromFile(string fileName)
        {
            var database = FileIO.ReadFromJson<TDatabase>(fileName + ".txt");

            var binaryReader = _binaryReaderCreator(database);

            var dataPoints = FileIO.ReadFromBinaryCustom<TElement>(fileName, binaryReader.ReadFromBinary);

            database.SetDataPoints(dataPoints);

            return database;
        }
        /// <summary>
        /// Creates a database of generic type Time from a file in resources
        /// </summary>
        /// <param name="fileName">Name of the XML file to be read</param>
        /// <param name="projectName">Project name for the location of resources</param>
        /// <returns>a database of generic type Time</returns>
        public TDatabase FromFileInResources(string fileName, string projectName)
        {
            var database = FileIO.ReadFromJsonInResources<TDatabase>(fileName + ".txt", projectName);

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
