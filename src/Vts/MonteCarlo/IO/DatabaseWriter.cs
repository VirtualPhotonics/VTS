using System;
using Vts.IO;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// This is a base class to PhotonDatabaseWriter and CollisionInfoDatabaseWriter.
    /// It controls writing to a database of generic-type.
    /// </summary>
    /// <typeparam name="TDatabase">Database of generic type Time</typeparam>
    /// <typeparam name="TElement">Generic database element</typeparam>
    public class DatabaseWriter<TDatabase, TElement> : CustomBinaryStreamWriter<TElement>
        where TDatabase : Database<TElement>, new()
    {
        /// <summary>
        /// writes database to file
        /// </summary>
        /// <param name="filename">name of file to write to</param>
        /// <param name="database">database to be written</param>
        /// <param name="binaryWriter">ICustomBinaryWriter</param>
        public DatabaseWriter(string filename, TDatabase database, ICustomBinaryWriter<TElement> binaryWriter)
            : base(filename, binaryWriter)
        {
            // set property
            Database = database;

            // this specifies any action to take at the end of the file writing
            PostWriteAction = delegate
            {
                // note: Count will be calculated at the end, not captured at instantiation
                Func<long> currentCount = () => Count;
                Database.NumberOfElements = currentCount();
                Database.WriteToJson(filename + ".txt");
            };
        }

        /// <summary>
        /// Publicly exposed database for assigning custom parameters 
        /// </summary>
        public TDatabase Database { get; private set; }
    }
}
