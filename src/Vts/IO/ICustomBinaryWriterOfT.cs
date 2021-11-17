using System.IO;

namespace Vts.IO
{
    /// <summary>
    /// General interface to write a specified type to binary
    /// </summary>
    /// <typeparam name="T">Type to write</typeparam>
    public interface ICustomBinaryWriter<in T>
    {
        /// <summary>
        /// Writes the specified type
        /// </summary>
        /// <param name="bw">Binary stream to which to write</param>
        /// <param name="item">The type to write</param>
        void WriteToBinary(BinaryWriter bw, T item);
    }
}
