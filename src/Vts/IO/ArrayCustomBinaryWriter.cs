using System;
using System.Collections.Generic;
using System.IO;
using Vts.Extensions;

namespace Vts.IO
{
    public class ArrayCustomBinaryWriter<T>
        : ICustomBinaryWriter<Array> where T : struct
    {
        public void WriteToBinary(BinaryWriter bw, Array array)
        {
            if (array is IEnumerable<float>)
            {
                (array as IEnumerable<float>).ForEach(bw.Write);
                return;
            }

            if (array is IEnumerable<double>)
            {
                (array as IEnumerable<double>).ForEach(bw.Write);
                return;
            }

            if (array is IEnumerable<ushort>)
            {
                (array as IEnumerable<ushort>).ForEach(bw.Write);
                return;
            }

            if (array is IEnumerable<byte>)
            {
                (array as IEnumerable<byte>).ForEach(bw.Write);
                return;
            }
        }
    }
}
