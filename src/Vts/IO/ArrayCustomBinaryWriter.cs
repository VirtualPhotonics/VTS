using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.Extensions;

namespace Vts.IO
{
    /// <summary>
    /// Class that implements the interface ICustomBinaryWriter to write different types of Array to a binary stream supported types are double, float, ushort, Complex and byte
    /// </summary>
    public class ArrayCustomBinaryWriter
        : ICustomBinaryWriter<Array> 
    {
        /// <summary>
        /// Write Array to a binary stream
        /// </summary>
        /// <param name="bw">The binary stream in which to write the data</param>
        /// <param name="input">The Array to write</param>
        public void WriteToBinary(BinaryWriter bw, Array input)
        {
            Type valueType = input.GetType();
            var expectedType = typeof(double);
            if (expectedType.IsAssignableFrom(valueType.GetElementType()))
            {
                var array = input.ToEnumerable<double>();
                array.ForEach(bw.Write);
                return;
            }
            expectedType = typeof(float);
            if (expectedType.IsAssignableFrom(valueType.GetElementType()))
            {
                var array = input.ToEnumerable<float>();
                array.ForEach(bw.Write);
                return;
            }
            expectedType = typeof(ushort);
            if (expectedType.IsAssignableFrom(valueType.GetElementType()))
            {
                var array = input.ToEnumerable<ushort>();
                array.ForEach(bw.Write);
                return;
            }
            expectedType = typeof(byte);
            if (expectedType.IsAssignableFrom(valueType.GetElementType()))
            {
                var array = input.ToEnumerable<byte>();
                array.ForEach(bw.Write);
                return;
            }
            expectedType = typeof(Complex);
            if (expectedType.IsAssignableFrom(valueType.GetElementType()))
            {
                var array = input.ToEnumerable<Complex>();
                array.ForEach(c => { bw.Write(c.Real); bw.Write(c.Imaginary); });
            }
        }
    }
}
