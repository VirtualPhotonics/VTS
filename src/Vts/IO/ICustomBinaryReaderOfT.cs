using System.IO;

namespace Vts.IO
{
    /// <summary>
    /// General interface to read a specified type from binary
    /// </summary>
    /// <typeparam name="T">Type to read</typeparam>
    public interface ICustomBinaryReader<out T>
    {
        /// <summary>
        /// Reads the specified type
        /// </summary>
        /// <param name="br">The binary stream</param>
        /// <returns>The type to read</returns>
        T ReadFromBinary(BinaryReader br);
    }
}
