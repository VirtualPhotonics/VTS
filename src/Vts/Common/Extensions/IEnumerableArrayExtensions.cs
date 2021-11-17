using System;
using System.Collections.Generic;

namespace Vts.Extensions
{
    /// <summary>
    /// methods enable access to IEnumerableArrays
    /// </summary>
    public static class IEnumerableArrayExtensions
    {
        /// <summary>
        /// method to convert Array to IEnumerable<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this array to convert</param>
        /// <returns>IEnumerable<typeparamref name="T"/></returns>
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
            else if (myArray is T[, , , ,])
            {
                var array = myArray as T[, , , ,];
                int vLength = array.GetLength(4);
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int v = 0; v < vLength; ++v )
                {
                    for (int w = 0; w < wLength; ++w)
                    {
                        for (int z = 0; z < zLength; ++z)
                        {
                            for (int y = 0; y < length; ++y) //for every pixel
                            {
                                for (int x = 0; x < width; ++x)
                                {
                                    yield return array[x, y, z, w, v];
                                }
                            }
                        }
                    }
                }
            }
            else if (myArray is T[, , , , ,])
            {
                var array = myArray as T[, , , , ,];
                int uLength = array.GetLength(5);
                int vLength = array.GetLength(4);
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int u = 0; u < uLength; ++u)
                {
                    for (int v = 0; v < vLength; ++v)
                    {
                        for (int w = 0; w < wLength; ++w)
                        {
                            for (int z = 0; z < zLength; ++z)
                            {
                                for (int y = 0; y < length; ++y) //for every pixel
                                {
                                    for (int x = 0; x < width; ++x)
                                    {
                                        yield return array[x, y, z, w, v, u];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (myArray is T[, , , , , ,])
            {
                var array = myArray as T[, , , , , ,];
                int tLength = array.GetLength(6);
                int uLength = array.GetLength(5);
                int vLength = array.GetLength(4);
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int t = 0; t < tLength; ++t)
                {
                    for (int u = 0; u < uLength; ++u)
                    {
                        for (int v = 0; v < vLength; ++v)
                        {
                            for (int w = 0; w < wLength; ++w)
                            {
                                for (int z = 0; z < zLength; ++z)
                                {
                                    for (int y = 0; y < length; ++y) //for every pixel
                                    {
                                        for (int x = 0; x < width; ++x)
                                        {
                                            yield return array[x, y, z, w, v, u, t];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// method to populate Array from Enumerable
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this array to convert</param>
        /// <param name="enumerable">enumerable to convert</param>
        /// <returns>Array</returns>
        public static Array PopulateFromEnumerable<T>(this Array myArray, IEnumerable<T> enumerable) where T : struct
        {
            var enumerator = enumerable.GetEnumerator();
            myArray.PopulateFromEnumerator(enumerator);
            return myArray;
        }

        /// <summary>
        /// method to populate this array from enumerator
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this array</param>
        /// <param name="enumerator">enumerator to convert from</param>
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
            else if (myArray is T[, , , ,])
            {
                var array = myArray as T[, , , ,];
                int vLength = array.GetLength(4);
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int v = 0; v < vLength; ++v)
                {
                    for (int w = 0; w < wLength; ++w)
                    {
                        for (int z = 0; z < zLength; ++z)
                        {
                            for (int y = 0; y < length; ++y) //for every pixel
                            {
                                for (int x = 0; x < width; ++x)
                                {
                                    if (enumerator.MoveNext())
                                        array[x, y, z, w, v] = enumerator.Current;
                                }
                            }
                        }
                    }
                }
            }
            else if (myArray is T[, , , , ,])
            {
                var array = myArray as T[, , , , ,];
                int uLength = array.GetLength(5);
                int vLength = array.GetLength(4);
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int u = 0; u < uLength; ++u)
                {
                    for (int v = 0; v < vLength; ++v)
                    {
                        for (int w = 0; w < wLength; ++w)
                        {
                            for (int z = 0; z < zLength; ++z)
                            {
                                for (int y = 0; y < length; ++y) //for every pixel
                                {
                                    for (int x = 0; x < width; ++x)
                                    {
                                        if (enumerator.MoveNext())
                                            array[x, y, z, w, v, u] = enumerator.Current;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (myArray is T[, , , , , ,])
            {
                var array = myArray as T[, , , , , ,];
                int tLength = array.GetLength(6);
                int uLength = array.GetLength(5);
                int vLength = array.GetLength(4);
                int wLength = array.GetLength(3);
                int zLength = array.GetLength(2);
                int length = array.GetLength(1);
                int width = array.GetLength(0);
                for (int t = 0; t < tLength; ++t)
                {
                    for (int u = 0; u < uLength; ++u)
                    {
                        for (int v = 0; v < vLength; ++v)
                        {
                            for (int w = 0; w < wLength; ++w)
                            {
                                for (int z = 0; z < zLength; ++z)
                                {
                                    for (int y = 0; y < length; ++y) //for every pixel
                                    {
                                        for (int x = 0; x < width; ++x)
                                        {
                                            if (enumerator.MoveNext())
                                                array[x, y, z, w, v, u, t] = enumerator.Current;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// method to populate array from IEnumerable
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <typeparam name="TArray">generic array</typeparam>
        /// <param name="myArray">this class array</param>
        /// <param name="enumerable">IEnumerable of T</param>
        /// <returns>TArray</returns>
        public static TArray PopulateFromEnumerable2<T, TArray>(this TArray myArray, IEnumerable<T> enumerable) where T : struct
        {
            if(!(myArray is Array))
            {
                throw new ArgumentException("Only arrays can use this method");
            }

            var enumerator = enumerable.GetEnumerator();
            myArray.PopulateFromEnumerator2(enumerator);
            return myArray;
        }

        private static void PopulateFromEnumerator2<T, TArray>(this TArray myArray, IEnumerator<T> enumerator) where T : struct
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
        /// <summary>
        /// method to populate this array with value
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">this array class</param>
        /// <param name="value">value to populate</param>
        /// <returns>generic array T[]</returns>

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

    }
}
