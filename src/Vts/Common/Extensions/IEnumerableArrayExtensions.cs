using System;
using System.Collections.Generic;

namespace Vts.Extensions
{
    public static class IEnumerableArrayExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this Array myArray) where T : struct
        {
            if (myArray is Array[][])
            {
                var array = myArray as Array[][];
                foreach (var item in array)
                {
                    foreach (var subItem in item)
                    {
                        foreach (var subSubItem in subItem.ToEnumerable<T>())
                        {
                            yield return subSubItem;
                        }
                    }
                }
            }
            else if (myArray is Array[])
            {
                var array = myArray as Array[];
                foreach (var item in array)
                {
                    foreach (var subItem in item.ToEnumerable<T>())
                    {
                        yield return subItem;
                    }
                }
            }
            else if (myArray is T[])
            {
                var array = myArray as T[];
                for (int i = 0; i < array.Length; i++)
                {
                    yield return array[i];
                }
            }
            else if (myArray is T[,])
            {
                var array = myArray as T[,];
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int y = 0; y < length; ++y) //for every pixel
                {
                    for (int x = 0; x < width; ++x)
                    {
                        yield return array[x, y];
                    }
                }
            }
            else if (myArray is T[, ,])
            {
                var array = myArray as T[, ,];
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int z = 0; z < zLength; ++z)
                {
                    for (int y = 0; y < length; ++y) //for every pixel
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            yield return array[x, y, z];
                        }
                    }
                }
            }
            else if (myArray is T[, , ,])
            {
                var array = myArray as T[, , ,];
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int w = 0; w < wLength; ++w)
                {
                    for (int z = 0; z < zLength; ++z)
                    {
                        for (int y = 0; y < length; ++y) //for every pixel
                        {
                            for (int x = 0; x < width; ++x)
                            {
                                yield return array[x, y, z, w];
                            }
                        }
                    }
                }
            }
        }

        public static void PopulateFromEnumerable<T>(this Array myArray, IEnumerable<T> enumerable) where T : struct
        {
            var enumerator = enumerable.GetEnumerator();
            myArray.PopulateFromEnumerator(enumerator);
        }

        private static void PopulateFromEnumerator<T>(this Array myArray, IEnumerator<T> enumerator) where T : struct
        {
            if (myArray is Array[][])
            {
                var array = myArray as Array[][];
                foreach (var item in array)
                {
                    foreach (var subItem in item)
                    {
                        subItem.PopulateFromEnumerator(enumerator);
                    }
                }
            }
            else if (myArray is Array[])
            {
                var array = myArray as Array[];
                foreach (var item in array)
                {
                    item.PopulateFromEnumerator(enumerator);
                }
            }
            else if (myArray is T[])
            {
                var array = myArray as T[];
                for (int i = 0; i < array.Length; i++)
                {
                    if (enumerator.MoveNext())
                        array[i] = enumerator.Current;
                }
            }
            else if (myArray is T[,])
            {
                var array = myArray as T[,];
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int y = 0; y < length; ++y) //for every pixel
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (enumerator.MoveNext())
                            array[x, y] = enumerator.Current;
                    }
                }
            }
            else if (myArray is T[, ,])
            {
                var array = myArray as T[, ,];
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int z = 0; z < zLength; ++z)
                {
                    for (int y = 0; y < length; ++y) //for every pixel
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            if (enumerator.MoveNext())
                                array[x, y, z] = enumerator.Current;
                        }
                    }
                }
            }
            else if (myArray is T[, , ,])
            {
                var array = myArray as T[, , ,];
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int w = 0; w < wLength; ++w)
                {
                    for (int z = 0; z < zLength; ++z)
                    {
                        for (int y = 0; y < length; ++y) //for every pixel
                        {
                            for (int x = 0; x < width; ++x)
                            {
                                if (enumerator.MoveNext())
                                    array[x, y, z, w] = enumerator.Current;
                            }
                        }
                    }
                }
            }
        }


        public static T[] PopulateWithValue<T>(this T[] myArray, T value) where T : struct
        {
            myArray.PopulateFromEnumerable(ConstantToEnumerable(value));
            return myArray;
        }

        private static IEnumerable<T> ConstantToEnumerable<T>(T item)
        {
            while (true)
            {
                yield return item;
            }
        }


        //// Sets up custom enumerators for each type of object
        //public static IEnumerable<T> AsEnumerable<T>(this T[] myArray)
        //{
        //    for (int x = 0; x < myArray.Length; ++x)
        //    {
        //        yield return myArray[x];
        //    }
        //}
        //public static IEnumerable<T> AsEnumerable<T>(this T[,] myArray)
        //{
        //    int length = myArray.GetLength(1);
        //    int width = myArray.GetLength(0);
        //    for (int y = 0; y < length; ++y) //for every pixel
        //    {
        //        for (int x = 0; x < width; ++x)
        //        {
        //            yield return myArray[x, y];
        //        }
        //    }
        //}
        //public static IEnumerable<T> AsEnumerable<T>(this T[, ,] myArray)
        //{
        //    int zLength = myArray.GetLength(2);
        //    int length = myArray.GetLength(1);
        //    int width = myArray.GetLength(0);
        //    for (int z = 0; z < zLength; ++z)
        //    {
        //        for (int y = 0; y < length; ++y) //for every pixel
        //        {
        //            for (int x = 0; x < width; ++x)
        //            {
        //                yield return myArray[x, y, z];
        //            }
        //        }
        //    }
        //}
        //public static IEnumerable<T> AsEnumerable<T>(this T[, , ,] myArray)
        //{
        //    int wLength = myArray.GetLength(3);
        //    int zLength = myArray.GetLength(2);
        //    int length = myArray.GetLength(1);
        //    int width = myArray.GetLength(0);
        //    for (int w = 0; w < wLength; ++w)
        //    {
        //        for (int z = 0; z < zLength; ++z)
        //        {
        //            for (int y = 0; y < length; ++y) //for every pixel
        //            {
        //                for (int x = 0; x < width; ++x)
        //                {
        //                    yield return myArray[x, y, z, w];
        //                }
        //            }
        //        }
        //    }
        //}
        //public static IEnumerable<T> AsEnumerable<T>(this T[][] myArray)
        //{
        //    foreach (T[] item in myArray) //for every frequency
        //    {
        //        foreach (T subItem in item)
        //        {
        //            yield return subItem;
        //        }
        //    }
        //}
        //public static IEnumerable<T> AsEnumerable<T>(this T[][,] myArray)
        //{
        //    foreach (T[,] item in myArray) //for every frequency
        //    {
        //        foreach (T subItem in item.AsEnumerable<T>())
        //        {
        //            yield return subItem;
        //        }
        //    }
        //}
        //public static IEnumerable<T> AsEnumerable<T>(this T[][][,] myArray)
        //{
        //    foreach (T[][,] item in myArray)
        //    {
        //        foreach (T[,] subItem in item)
        //        {
        //            foreach (T subSubItem in subItem.AsEnumerable<T>())
        //            {
        //                yield return subSubItem;
        //            }
        //        }
        //    }
        //}
    }
}
