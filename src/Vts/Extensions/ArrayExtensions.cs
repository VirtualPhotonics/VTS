using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vts.Extensions
{
    /// <summary>
    /// The <see cref="Extensions"/> namespace contains the classes with the extension methods for the Virtual Tissue Simulator
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Helper group of extension methods for array-based operations
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Extension method to initialize this Array class with value
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The array class</param>
        /// <param name="value">The value with which to initialize</param>
        /// <returns>A generic array T[]</returns>
        public static T[] InitializeTo<T>(this T[] myArray, T value) where T : struct
        {
            for (var i = 0; i < myArray.Length; i++)
                myArray[i] = value;
            return myArray;
        }

        /// <summary>
        /// Extension method to obtain column from array
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The class array</param>
        /// <param name="column">The integer column identifier</param>
        /// <returns>An IEnumerable[T] column of array</returns>
        public static IEnumerable<T> Column<T>(this T[,] myArray, int column) where T : struct
        {
            var length = myArray.GetLength(0);
            for (var i = 0; i < length; i++)
            {
                yield return myArray[i, column];
            }
        }

        /// <summary>
        /// Extension method to obtain columns from array
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The class array</param>
        /// <returns>An IEnumerable[IEnumerable[T]] providing columns of array</returns>
        public static IEnumerable<IEnumerable<T>> Columns<T>(this T[,] myArray) where T : struct
        {
            var length = myArray.GetLength(1);
            for (var i = 0; i < length; i++)
            {
                yield return myArray.Column(i);
            }
        }

        /// <summary>
        /// Extension method to obtain row from array
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The class array</param>
        /// <param name="row">The integer row identifier</param>
        /// <returns>An IEnumerable[T] row of array</returns>
        public static IEnumerable<T> Row<T>(this T[,] myArray, int row) where T : struct
        {
            var length = myArray.GetLength(1);
            for (var i = 0; i < length; i++)
            {
                yield return myArray[row, i];
            }
        }

        /// <summary>
        /// Extension method to obtain rows from array
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The class array</param>
        /// <returns>An IEnumerable[IEnumerable[T]] providing rows of array</returns>
        public static IEnumerable<IEnumerable<T>> Rows<T>(this T[,] myArray) where T : struct
        {
            var length = myArray.GetLength(0);
            for (var i = 0; i < length; i++)
            {
                yield return myArray.Row(i);
            }
        }

        /// <summary>
        /// Creates a new array with the same dimensions as the original array
        /// </summary>
        /// <param name="array">The multi-dimensional array</param>
        /// <returns>A new array with the same dimensions as the original</returns>
        internal static Array CreateArray(this Array array)
        {
            // Gets the lengths and lower bounds of the input array
            var lowerBounds = new int[array.Rank];
            var lengths = new int[array.Rank];
            for (var i = 0; i < array.Rank; i++)
            {
                lowerBounds[i] = array.GetLowerBound(i);
                lengths[i] = array.GetLength(i);
            }

            var elementType = array.GetType().GetElementType();  // Gets the type of the elements in the input array
            return Array.CreateInstance(elementType, lengths, lowerBounds);    // Returns the new array
        }
    }
}
