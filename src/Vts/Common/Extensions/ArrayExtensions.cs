using System.Collections.Generic;

namespace Vts.Extensions
{
    /// <summary>
    /// Helper group of extension methods for array-based operations
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// method to initialize this Array class with value
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this array class</param>
        /// <param name="value">value to initialize with</param>
        /// <returns>generic array T[]</returns>
        public static T[] InitializeTo<T>(this T[] myArray, T value) where T : struct
        {
            for (int i = 0; i < myArray.Length; i++)
                myArray[i] = value;
            return myArray;
        }
        /// <summary>
        /// method to obtain column from array
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this class array</param>
        /// <param name="column">integer column identifier</param>
        /// <returns>IEnumerable[T] column of array</returns>
        public static IEnumerable<T> Column<T>(this T[,] myArray, int column) where T : struct
        {
            var length = myArray.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                yield return myArray[i, column];
            }
        }
        /// <summary>
        /// method to obtain columns from array
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this class array</param>
        /// <returns>IEnumerable[IEnumerable[T]] providing columns of array</returns>
        public static IEnumerable<IEnumerable<T>> Columns<T>(this T[,] myArray) where T : struct
        {
            var length = myArray.GetLength(1);
            for (int i = 0; i < length; i++)
            {
                yield return myArray.Column(i);
            }
        }
        /// <summary>
        /// method to obtain row from array
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this class array</param>
        /// <param name="row">integer row identifier</param>
        /// <returns>IEnumerable[T] row of array</returns>
        public static IEnumerable<T> Row<T>(this T[,] myArray, int row) where T : struct
        {
            var length = myArray.GetLength(1);
            for (int i = 0; i < length; i++)
            {
                yield return myArray[row, i];
            }
        }
        /// <summary>
        /// method to obtain rows from array
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this class array</param>
        /// <returns>IEnumerable[IEnumerable[T]] providing rows of array</returns>
        public static IEnumerable<IEnumerable<T>> Rows<T>(this T[,] myArray) where T : struct
        {
            var length = myArray.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                yield return myArray.Row(i);
            }
        }

        //public static IEnumerable<double> Average(this double[,] myArray, int axis)
        //{
        //    switch (axis)
        //    {
        //        default:
        //        case 0:
        //            return myArray.Columns().Select(column => column.Average());
        //        case 1:
        //            return myArray.Rows().Select(row => row.Average());
        //    }
        //}

        //public static IEnumerable<double> Sum(this double[,] myArray, int axis)
        //{
        //    switch (axis)
        //    {
        //        default:
        //        case 0:
        //            return myArray.Columns().Select(column => column.Sum());
        //        case 1:
        //            return myArray.Rows().Select(row => row.Sum());
        //    }
        //}
    }
}
