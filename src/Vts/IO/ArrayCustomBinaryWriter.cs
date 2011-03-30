using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using Vts.Extensions;
using System.Collections;

namespace Vts.IO
{
    public class ArrayCustomBinaryWriter<T>
        : ICustomBinaryWriter<Array> where T : struct
    {
        public void WriteToBinary(BinaryWriter bw, Array input)
        {
            IEnumerable<T> array = input.ToEnumerable<T>();

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

            if (array is IEnumerable<Complex>)
            {
                (array as IEnumerable<Complex>).ForEach(c => { bw.Write(c.Real); bw.Write(c.Imaginary); });
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
