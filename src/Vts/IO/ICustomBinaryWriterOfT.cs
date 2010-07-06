using System.IO;

namespace Vts.IO
{
    public interface ICustomBinaryWriter<T>
    {
        void WriteToBinary(BinaryWriter bw, T item);
    }
}
