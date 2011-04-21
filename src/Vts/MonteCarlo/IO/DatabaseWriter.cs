using System;
using Vts.IO;

namespace Vts.MonteCarlo.IO
{
    public class DatabaseWriter<TDatabase, TElement> : CustomBinaryStreamWriter<TElement>
        where TDatabase : Database<TElement>, new()
    {
        public DatabaseWriter(string filename, TDatabase database, ICustomBinaryWriter<TElement> binaryWriter)
            : base(filename, binaryWriter)
        {
            //Database = new TDatabase();
            Database = database;

            // this specifies any action to take at the end of the file writing
            PostWriteAction = delegate
            {
                // note: Count will be calculated at the end, not captured at instantiation
                Func<long> currentCount = () => Count;
                Database.NumberOfElements = currentCount();
                Database.WriteToXML(filename + ".xml");
            };
        }

        ///// <summary>
        ///// Creates an instance of DatabaseWriter, where the database instantiation is handled internally
        ///// </summary>
        ///// <param name="filename"></param>
        ///// <param name="binaryWriter"></param>
        //public DatabaseWriter(string filename, ICustomBinaryWriter<TElement> binaryWriter)
        //    : this(filename, new TDatabase(), binaryWriter)
        //{
        //}

        /// <summary>
        /// Publicly exposed database for assigning custom parameters (todo: evaluate utility)
        /// </summary>
        public TDatabase Database { get; private set; }
    }
}
