using System;
using System.IO;

namespace Vts.IO
{
    /// <summary>
    /// class to write detector arrays to/from binary files
    /// </summary>
    public class BinaryArraySerializer
    {
        /// <summary>
        /// name of array
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// file tag string
        /// </summary>
        public string FileTag { get; set; }
        /// <summary>
        /// method to write data
        /// </summary>
        public Action<BinaryWriter> WriteData { get; set; }
        /// <summary>
        /// method to read data
        /// </summary>
        public Action<BinaryReader> ReadData { get; set; }
    }
}
