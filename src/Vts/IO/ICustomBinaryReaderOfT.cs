using System.IO;

namespace Vts.IO
{
    public interface ICustomBinaryReader<T>
    {
        T ReadFromBinary(BinaryReader br);
    }
}
