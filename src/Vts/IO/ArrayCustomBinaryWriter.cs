using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.IO
{
    /// <summary>
    /// Class that implements the interface ICustomBinaryWriter to write different types of Array to a binary stream
    /// </summary>
    /// <typeparam name="T">Type of Array to be written</typeparam>
    public class ArrayCustomBinaryWriter<T>
        : ICustomBinaryWriter<Array> where T : struct
    {
        /// <summary>
        /// Write Array to a binary stream
        /// </summary>
        /// <param name="bw">The binary stream in which to write the data</param>
        /// <param name="input">The Array to write</param>
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
