using System;
using System.Reflection;
using Vts.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// Implements an entity representing a stream of data of a type, T
    /// </summary>
    /// <typeparam name="T">The "atomic" data type resident in the database</typeparam>
    /// <typeparam name="TSelfReferenceType">A helper type - should match the derived class' type</typeparam>
    public abstract class Database<T, TSelfReferenceType> where TSelfReferenceType : Database<T, TSelfReferenceType>
    {
        // note: we're using a self-referencing type as per the following post-comment by Rob Leclerc:
        // http://stackoverflow.com/questions/2081612/net-determine-the-type-of-this-class-in-its-static-method
        // the second argument is just to help this base class figure stuff out, and should be identical to the derived class' type
        // e.g. MyDataPointDatabase : Database<MyDataPoint, MyDataPointDatabase>

        private static readonly Func<string, TSelfReferenceType> _fileReadMethod;
        private static readonly Func<string, string, TSelfReferenceType> _resourcesReadMethod;

        static Database()
        {
            //_type = MethodBase.GetCurrentMethod().DeclaringType;
            var type = typeof(TSelfReferenceType);

            MethodInfo fileReadMethod = typeof(FileIO).GetMethod("ReadFromXML");
            MethodInfo genericFileReadMethod = fileReadMethod.MakeGenericMethod(new Type[] { type });
            _fileReadMethod = fileName => (TSelfReferenceType)(genericFileReadMethod.Invoke(null, new[] { fileName }));

            MethodInfo resourcesReadMethod = typeof(FileIO).GetMethod("ReadFromXMLInResources");
            MethodInfo resourcesReadMethodGeneric = resourcesReadMethod.MakeGenericMethod(new Type[] { type });
            _resourcesReadMethod = (fileName, projectName) => (TSelfReferenceType)(resourcesReadMethodGeneric.Invoke(null, new[] { fileName }));
        }

        [IgnoreDataMember]
        public IEnumerable<T> DataPoints { get; set; }

        public static TSelfReferenceType FromFile(string fileName)
        {
            if (BinaryReader == null)
                throw new NullReferenceException("BinaryReader is null. It must be set in the database class constructor.");

            var database = _fileReadMethod(fileName + ".xml");

            database.DataPoints = FileIO.ReadFromBinaryCustom<T>(
                fileName,
                BinaryReader.ReadFromBinary);

            return database;
        }

        public static TSelfReferenceType FromFileInResources(string fileName, string projectName)
        {
            if (BinaryReader == null)
                throw new NullReferenceException("BinaryReader is null. It must be set in the database class constructor.");

            var database = _resourcesReadMethod(fileName + ".xml", projectName);

            database.DataPoints = FileIO.ReadFromBinaryInResourcesCustom<T>(
                fileName,
                projectName,
                BinaryReader.ReadFromBinary);

            return database;
        }

        /// <summary>
        ///  This needs to be set by the derived classes!
        /// </summary>
        [IgnoreDataMember]
        public static ICustomBinaryReader<T> BinaryReader { get; set; }
    }
}
