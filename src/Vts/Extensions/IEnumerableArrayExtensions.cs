using System;
using System.Collections.Generic;
using System.Linq;

namespace Vts.Extensions
{
    /// <summary>
    /// methods enable access to IEnumerableArrays
    /// </summary>
    public static class IEnumerableArrayExtensions
    {
        /// <summary>
        /// Extension method to convert Array to IEnumerable<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The array to convert</param>
        /// <returns>An IEnumerable of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> ToEnumerable<T>(this Array myArray) where T : struct
        {
            switch (myArray)
            {
                case Array[][] arrays:
                    {
                        foreach (var item in arrays)
                        {
                            foreach (var subItem in item)
                            {
                                foreach (var subSubItem in subItem.ToEnumerable<T>())
                                {
                                    yield return subSubItem;
                                }
                            }
                        }

                        break;
                    }
                case Array[] arrays:
                    {
                        foreach (var item in arrays)
                        {
                            foreach (var subItem in item.ToEnumerable<T>())
                            {
                                yield return subItem;
                            }
                        }

                        break;
                    }
                case T[] array:
                    {
                        foreach (var t in array)
                        {
                            yield return t;
                        }

                        break;
                    }
                case T[,] array:
                    {
                        var length = array.GetLength(1);
                        var width = array.GetLength(0);
                        for (var y = 0; y < length; ++y) //for every pixel
                        {
                            for (var x = 0; x < width; ++x)
                            {
                                yield return array[x, y];
                            }
                        }

                        break;
                    }
                case T[,,] array:
                    {
                        var zLength = array.GetLength(2);
                        var length = array.GetLength(1);
                        var width = array.GetLength(0);
                        for (var z = 0; z < zLength; ++z)
                        {
                            for (var y = 0; y < length; ++y) //for every pixel
                            {
                                for (var x = 0; x < width; ++x)
                                {
                                    yield return array[x, y, z];
                                }
                            }
                        }

                        break;
                    }
                case T[,,,] array:
                    {
                        var wLength = array.GetLength(3);
                        var zLength = array.GetLength(2);
                        var length = array.GetLength(1);
                        var width = array.GetLength(0);
                        for (var w = 0; w < wLength; ++w)
                        {
                            for (var z = 0; z < zLength; ++z)
                            {
                                for (var y = 0; y < length; ++y) //for every pixel
                                {
                                    for (var x = 0; x < width; ++x)
                                    {
                                        yield return array[x, y, z, w];
                                    }
                                }
                            }
                        }

                        break;
                    }
                case T[,,,,] array:
                    {
                        var vLength = array.GetLength(4);
                        var wLength = array.GetLength(3);
                        var zLength = array.GetLength(2);
                        var length = array.GetLength(1);
                        var width = array.GetLength(0);
                        for (var v = 0; v < vLength; ++v)
                        {
                            for (var w = 0; w < wLength; ++w)
                            {
                                for (var z = 0; z < zLength; ++z)
                                {
                                    for (var y = 0; y < length; ++y) //for every pixel
                                    {
                                        for (var x = 0; x < width; ++x)
                                        {
                                            yield return array[x, y, z, w, v];
                                            Console.WriteLine($"{x} {y} {z} {w} {v}");
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                case T[,,,,,] array:
                    {
                        var uLength = array.GetLength(5);
                        var vLength = array.GetLength(4);
                        var wLength = array.GetLength(3);
                        var zLength = array.GetLength(2);
                        var length = array.GetLength(1);
                        var width = array.GetLength(0);
                        for (var u = 0; u < uLength; ++u)
                        {
                            for (var v = 0; v < vLength; ++v)
                            {
                                for (var w = 0; w < wLength; ++w)
                                {
                                    for (var z = 0; z < zLength; ++z)
                                    {
                                        for (var y = 0; y < length; ++y) //for every pixel
                                        {
                                            for (var x = 0; x < width; ++x)
                                            {
                                                yield return array[x, y, z, w, v, u];
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                case T[,,,,,,] array:
                    {
                        var tLength = array.GetLength(6);
                        var uLength = array.GetLength(5);
                        var vLength = array.GetLength(4);
                        var wLength = array.GetLength(3);
                        var zLength = array.GetLength(2);
                        var length = array.GetLength(1);
                        var width = array.GetLength(0);
                        for (var t = 0; t < tLength; ++t)
                        {
                            for (var u = 0; u < uLength; ++u)
                            {
                                for (var v = 0; v < vLength; ++v)
                                {
                                    for (var w = 0; w < wLength; ++w)
                                    {
                                        for (var z = 0; z < zLength; ++z)
                                        {
                                            for (var y = 0; y < length; ++y) //for every pixel
                                            {
                                                for (var x = 0; x < width; ++x)
                                                {
                                                    yield return array[x, y, z, w, v, u, t];
                                                    Console.WriteLine($"{x} {y} {z} {w} {v} {u} {t}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Extension method to convert any multi-dimensional Array to IEnumerable<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="myArray">The array to convert</param>
        /// <returns>An IEnumerable of type <typeparamref name="T"/></returns>
        internal static IEnumerable<T> ToEnumerable2<T>(this Array myArray) where T : struct
        {
            var rank = myArray.Rank;
            var dimensions = new int[rank];

            for (var i = 0; i < rank; ++i)
            {
                // get the dimensions in reverse order
                dimensions[i] = myArray.GetLength(rank - i - 1);
            }

            var results = new int[rank];
            var position = new int[rank];
            Array.Clear(position, 0, position.Length);

            return IterateThroughArray<T>(dimensions, results, position, myArray);
        }

        private static IEnumerable<T> IterateThroughArray<T>(IReadOnlyList<int> dimensions, IList<int> results, IList<int> positions, Array array) where T : struct
        {
            var rank = dimensions.Count;
            if (rank == 1)
            {
                for (var i = 0; i < dimensions[0]; i++)
                {
                    var position = positions.IndexOf(1);
                    results[results.Count - 1 - position] = i;
                    var indices = results.Select((_, j) => results[results.Count - 1 - j]).ToList();
                    var arrayValue = (T)array.GetValue(indices.ToArray());
                    yield return arrayValue;
                    var message = indices.Aggregate("", (current, idx) => $"{current} {idx} ");
                    Console.WriteLine($"{message} - {arrayValue}");
                }
            }
            else
            {
                var currentPosition = results.Count - dimensions.Count;
                if (positions.All(value => value != 1))
                {
                    positions[currentPosition] = 1;
                }
                for (var j = 0; j < dimensions[0]; j++)
                {
                    results[currentPosition] = j;
                    var values = IterateThroughArray<T>(dimensions.Skip(1).ToList(), results, positions, array);
                    foreach (var value in values)
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Extension method to populate Array from Enumerable
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The array to convert</param>
        /// <param name="enumerable">The enumerable to convert</param>
        /// <returns>An Array</returns>
        public static Array PopulateFromEnumerable<T>(this Array myArray, IEnumerable<T> enumerable) where T : struct
        {
            var enumerator = enumerable.GetEnumerator();
            myArray.PopulateFromEnumerator(enumerator);
            return myArray;
        }

        /// <summary>
        /// Extension method to populate this array from enumerator
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The array to populate</param>
        /// <param name="enumerator">The enumerator to convert</param>
        private static void PopulateFromEnumerator<T>(this Array myArray, IEnumerator<T> enumerator) where T : struct
        {
            switch (myArray)
            {
                case Array[][] arrays:
                {
                    foreach (var item in arrays)
                    {
                        foreach (var subItem in item)
                        {
                            subItem.PopulateFromEnumerator(enumerator);
                        }
                    }

                    break;
                }
                case Array[] arrays:
                {
                    foreach (var item in arrays)
                    {
                        item.PopulateFromEnumerator(enumerator);
                    }

                    break;
                }
                case T[] array:
                {
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (enumerator.MoveNext())
                            array[i] = enumerator.Current;
                    }

                    break;
                }
                case T[,] array:
                {
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var y = 0; y < length; ++y) //for every pixel
                    {
                        for (var x = 0; x < width; ++x)
                        {
                            if (enumerator.MoveNext())
                                array[x, y] = enumerator.Current;
                        }
                    }

                    break;
                }
                case T[,,] array:
                {
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var z = 0; z < zLength; ++z)
                    {
                        for (var y = 0; y < length; ++y) //for every pixel
                        {
                            for (var x = 0; x < width; ++x)
                            {
                                if (enumerator.MoveNext())
                                    array[x, y, z] = enumerator.Current;
                            }
                        }
                    }

                    break;
                }
                case T[,,,] array:
                {
                    var wLength = array.GetLength(3);
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var w = 0; w < wLength; ++w)
                    {
                        for (var z = 0; z < zLength; ++z)
                        {
                            for (var y = 0; y < length; ++y) //for every pixel
                            {
                                for (var x = 0; x < width; ++x)
                                {
                                    if (enumerator.MoveNext())
                                        array[x, y, z, w] = enumerator.Current;
                                }
                            }
                        }
                    }

                    break;
                }
                case T[,,,,] array:
                {
                    var vLength = array.GetLength(4);
                    var wLength = array.GetLength(3);
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var v = 0; v < vLength; ++v)
                    {
                        for (var w = 0; w < wLength; ++w)
                        {
                            for (var z = 0; z < zLength; ++z)
                            {
                                for (var y = 0; y < length; ++y) //for every pixel
                                {
                                    for (var x = 0; x < width; ++x)
                                    {
                                        if (enumerator.MoveNext())
                                            array[x, y, z, w, v] = enumerator.Current;
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
                case T[,,,,,] array:
                {
                    var uLength = array.GetLength(5);
                    var vLength = array.GetLength(4);
                    var wLength = array.GetLength(3);
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var u = 0; u < uLength; ++u)
                    {
                        for (var v = 0; v < vLength; ++v)
                        {
                            for (var w = 0; w < wLength; ++w)
                            {
                                for (var z = 0; z < zLength; ++z)
                                {
                                    for (var y = 0; y < length; ++y) //for every pixel
                                    {
                                        for (var x = 0; x < width; ++x)
                                        {
                                            if (enumerator.MoveNext())
                                                array[x, y, z, w, v, u] = enumerator.Current;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
                case T[,,,,,,] array:
                {
                    var tLength = array.GetLength(6);
                    var uLength = array.GetLength(5);
                    var vLength = array.GetLength(4);
                    var wLength = array.GetLength(3);
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var t = 0; t < tLength; ++t)
                    {
                        for (var u = 0; u < uLength; ++u)
                        {
                            for (var v = 0; v < vLength; ++v)
                            {
                                for (var w = 0; w < wLength; ++w)
                                {
                                    for (var z = 0; z < zLength; ++z)
                                    {
                                        for (var y = 0; y < length; ++y) //for every pixel
                                        {
                                            for (var x = 0; x < width; ++x)
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

                    break;
                }
            }
        }

        /// <summary>
        /// Extension method to populate array from IEnumerable
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <typeparam name="TArray">The array type</typeparam>
        /// <param name="myArray">The array of type TArray</param>
        /// <param name="enumerable">The IEnumerable of type T</param>
        /// <returns>Aa array of type TArray</returns>
        public static TArray PopulateFromEnumerable2<T, TArray>(this TArray myArray, IEnumerable<T> enumerable) where T : struct
        {
            if (!(myArray is Array))
            {
                throw new ArgumentException("Only arrays can use this method");
            }

            var enumerator = enumerable.GetEnumerator();
            myArray.PopulateFromEnumerator2(enumerator);
            return myArray;
        }

        private static void PopulateFromEnumerator2<T, TArray>(this TArray myArray, IEnumerator<T> enumerator) where T : struct
        {
            switch (myArray)
            {
                case Array[][] arrays:
                {
                    foreach (var item in arrays)
                    {
                        foreach (var subItem in item)
                        {
                            subItem.PopulateFromEnumerator(enumerator);
                        }
                    }

                    break;
                }
                case Array[] arrays:
                {
                    foreach (var item in arrays)
                    {
                        item.PopulateFromEnumerator(enumerator);
                    }

                    break;
                }
                case T[] array:
                {
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (enumerator.MoveNext())
                            array[i] = enumerator.Current;
                    }

                    break;
                }
                case T[,] array:
                {
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var y = 0; y < length; ++y) //for every pixel
                    {
                        for (var x = 0; x < width; ++x)
                        {
                            if (enumerator.MoveNext())
                                array[x, y] = enumerator.Current;
                        }
                    }

                    break;
                }
                case T[,,] array:
                {
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var z = 0; z < zLength; ++z)
                    {
                        for (var y = 0; y < length; ++y) //for every pixel
                        {
                            for (var x = 0; x < width; ++x)
                            {
                                if (enumerator.MoveNext())
                                    array[x, y, z] = enumerator.Current;
                            }
                        }
                    }

                    break;
                }
                case T[,,,] array:
                {
                    var wLength = array.GetLength(3);
                    var zLength = array.GetLength(2);
                    var length = array.GetLength(1);
                    var width = array.GetLength(0);
                    for (var w = 0; w < wLength; ++w)
                    {
                        for (var z = 0; z < zLength; ++z)
                        {
                            for (var y = 0; y < length; ++y) //for every pixel
                            {
                                for (var x = 0; x < width; ++x)
                                {
                                    if (enumerator.MoveNext())
                                        array[x, y, z, w] = enumerator.Current;
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Extension method to populate array with value
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The array to populate</param>
        /// <param name="value">The value to populate</param>
        /// <returns>A generic array T[]</returns>

        public static T[] PopulateWithValue<T>(this T[] myArray, T value) where T : struct
        {
            myArray.PopulateFromEnumerable(ConstantToEnumerable(value));
            return myArray;
        }

        private static IEnumerable<T> ConstantToEnumerable<T>(T item, int max = (int)1E+9)
        {
            // item is an enumerator so yield return of item will move to the next value
            var i = 0;
            while (true)
            {
                // add a max parameter as a fail safe
                if (++i > max) throw new InvalidOperationException("Limit our enumerable to 1,000,000,000");
                yield return item;
            }
        }

    }
}
