using System.Collections.Generic;

namespace Vts.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] InitializeTo<T>(this T[] myArray, T value) where T : struct
        {
            for (int i = 0; i < myArray.Length; i++)
                myArray[i] = value;
            return myArray;
        }

        public static IEnumerable<T> Column<T>(this T[,] myArray, int column) where T : struct
        {
            var length = myArray.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                yield return myArray[i, column];
            }
        }

        public static IEnumerable<IEnumerable<T>> Columns<T>(this T[,] myArray) where T : struct
        {
            var length = myArray.GetLength(1);
            for (int i = 0; i < length; i++)
            {
                yield return myArray.Column(i);
            }
        }

        public static IEnumerable<T> Row<T>(this T[,] myArray, int row) where T : struct
        {
            var length = myArray.GetLength(1);
            for (int i = 0; i < length; i++)
            {
                yield return myArray[row, i];
            }
        }

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
