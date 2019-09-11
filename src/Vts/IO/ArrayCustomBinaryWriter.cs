using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.Extensions;

namespace Vts.IO
{
    /// <summary>
    /// Class that implements the interface ICustomBinaryWriter to write different types of Array to a binary stream
    /// </summary>
    public class ArrayCustomBinaryWriter//<T>
        : ICustomBinaryWriter<Array> //where T : struct
    {
        /// <summary>
        /// Write Array to a binary stream
        /// </summary>
        /// <param name="bw">The binary stream in which to write the data</param>
        /// <param name="input">The Array to write</param>
        public void WriteToBinary(BinaryWriter bw, Array input)
        {
            var length = input.GetLength(0);
            int count = 0;
            var array1 = input.ToEnumerable<double>();
            foreach (var val in array1)
            {
                count++;
            }
            if (length == count && array1 is IEnumerable<double>)
            {
                (array1 as IEnumerable<double>).ForEach(bw.Write);
                return;
            }
            var array2 = input.ToEnumerable<float>();
            count = 0;
            foreach (var val in array2)
            {
                count++;
            }
            if (length == count && array2 is IEnumerable<float>)
            {
                (array2 as IEnumerable<float>).ForEach(bw.Write);
                return;
            }
            var array3 = input.ToEnumerable<Complex>();
            count = 0;
            foreach (var val in array3)
            {
                count++;
            }
            if (length == count && array3 is IEnumerable<Complex>)
            {
                (array3 as IEnumerable<Complex>).ForEach(c => { bw.Write(c.Real); bw.Write(c.Imaginary); });
                return;
            }
            var array4 = input.ToEnumerable<ushort>();
            count = 0;
            foreach (var val in array4)
            {
                count++;
            }
            if (length == count && array4 is IEnumerable<ushort>)
            {
                (array4 as IEnumerable<ushort>).ForEach(bw.Write);
                return;
            }
            var array5 = input.ToEnumerable<byte>();
            count = 0;
            foreach (var val in array5)
            {
                count++;
            }
            if (length == count && array5 is IEnumerable<byte>)
            {
                (array5 as IEnumerable<byte>).ForEach(bw.Write);
                return;
            }
        }
    }
}
