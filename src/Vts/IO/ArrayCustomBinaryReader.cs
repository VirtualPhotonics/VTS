using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.IO
{
    /// <summary>
    /// Class that implements the interface ICustomBinaryReader to read different types of Array from a binary stream
    /// </summary>
    /// <typeparam name="T">Type of Array to be read</typeparam>
    public class ArrayCustomBinaryReader<T> 
        : ICustomBinaryReader<Array> where T : struct
    {
        private int[] _dims;

        /// <summary>
        /// Constructor for reading a multi-dimensional Array
        /// </summary>
        /// <param name="dims">An array of integers to represent the lengths of a multi-dimensional Array</param>
        public ArrayCustomBinaryReader(int[] dims)
        {
            _dims = dims;
        }

        /// <summary>
        /// Constructor for reading an Array of size length
        /// </summary>
        /// <param name="length">Length of the Array</param>
        public ArrayCustomBinaryReader(int length)
            : this(new[] { length })
        {
        }

        /// <summary>
        /// Read an Array from a binary stream
        /// </summary>
        /// <param name="br">Binary stream reader</param>
        /// <returns>An Array, read from the binary stream</returns>
        public Array ReadFromBinary(BinaryReader br)
        {
            // Initialize the array
            Array dataOut = Array.CreateInstance(typeof(T), _dims);

            var dataType = typeof(T);

            if (dataType.Equals(typeof(double)))
            {
                dataOut.PopulateFromEnumerable(ReadDoubles(br, dataOut.Length));
                return dataOut;
            }

            if (dataType.Equals(typeof(float)))
            {
                dataOut.PopulateFromEnumerable(ReadFloats(br, dataOut.Length));
                return dataOut;
            }

            if (dataType.Equals(typeof(ushort)))
            {
                dataOut.PopulateFromEnumerable(ReadUShorts(br, dataOut.Length));
                return dataOut;
            }

            if (dataType.Equals(typeof(byte)))
            {
                dataOut.PopulateFromEnumerable(ReadBytes(br, dataOut.Length));
                return dataOut;
            }

            if (dataType.Equals(typeof(Complex)))
            {
                dataOut.PopulateFromEnumerable(ReadComplices(br, dataOut.Length));
                return dataOut;
            }

            throw new NotSupportedException("Type of Time is not supported");
        }

        private static IEnumerable<double> ReadDoubles(BinaryReader br, int numberOfElements)
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                yield return br.ReadDouble();
            }
        }

        private static IEnumerable<float> ReadFloats(BinaryReader br, int numberOfElements)
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                yield return br.ReadSingle();
            }
        }

        private static IEnumerable<ushort> ReadUShorts(BinaryReader br, int numberOfElements)
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                yield return br.ReadUInt16();
            }
        }

        private static IEnumerable<byte> ReadBytes(BinaryReader br, int numberOfElements)
        {
            return br.ReadBytes(numberOfElements);
        }

        private static IEnumerable<Complex> ReadComplices(BinaryReader br, int numberOfElements)
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                yield return new Complex(br.ReadDouble(), br.ReadDouble());
            }
        }
    }
}
