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
    /// <typeparam name="T">Type of Array to be written</typeparam>
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
            //IEnumerable<T> array = input.ToEnumerable<T>();
            object array = null;
            // testing whether this class really needs to be generic, or if we can test like this...
            var array1 = input.ToEnumerable<float>();
            var array2 = input.ToEnumerable<double>();
            var array3 = input.ToEnumerable<Complex>();
            var array4 = input.ToEnumerable<ushort>();
            var array5 = input.ToEnumerable<byte>();
           
            if (array1 != null) {Console.WriteLine(array1); array = array1;}
            if (array2 != null) {Console.WriteLine(array2); array = array2;}
            if (array3 != null) {Console.WriteLine(array3); array = array3;}
            if (array4 != null) {Console.WriteLine(array4); array = array4;}
            if (array5 != null) {Console.WriteLine(array5); array = array5;}

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
