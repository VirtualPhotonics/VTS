using System;
using System.Collections.Generic;
using System.Linq;

namespace Vts.Extensions
{
    /// <summary>
    /// Helper group of extension methods for adding LINQ functionality
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Extension method to turn any single item into an IEnumerable (i.e. "yield-returns" the item)
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="singleItem"></param>
        /// <returns>IEnumerable of generic type</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T singleItem)
        {
            yield return singleItem;
        }

        /// <summary>
        /// Extension method to append a single item to an IEnumerable
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="items">list of existing items</param>
        /// <param name="newItem">item to be added</param>
        /// <returns>IEnumerable of generic type with concated list</returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T newItem )
        {
            return Enumerable.Concat(items, newItem.AsEnumerable());
        }

        /// <summary>
        /// Extension method to create a dictionary from key-value pairs
        /// </summary>
        /// <typeparam name="TKey">key of dictionary</typeparam>
        /// <typeparam name="TValue">value of dictionary</typeparam>
        /// <param name="keyValuePairs">key-value pairs used to create dictionary</param>
        /// <returns>IDictionary with key-value pairs</returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// 2D array overload of the LINQ Select operator with x and y indexers
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <typeparam name="TResult">type of result</typeparam>
        /// <param name="my2DArray">2D array of type T</param>
        /// <param name="myFunc">linq select function</param>
        /// <returns>IEnumerable of TResult</returns>
        public static IEnumerable<TResult> Select<T, TResult>(
            this T[,] my2DArray,
            Func<T, int, int, TResult> myFunc)
        {
            int width = my2DArray.GetLength(0);
            int height = my2DArray.GetLength(1);

            return
                from yi in Enumerable.Range(0, height)
                from xi in Enumerable.Range(0, width)
                select myFunc(my2DArray[xi, yi], xi, yi);
        }

        /// <summary>
        /// Method for applying an action to any enumerable sequence. 
        /// This is the analog of Select, but with no output (only "side effects").
        /// This overload takes in an Action with an additional int parameter that 
        /// provides the index of the sequence.
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="items">list of items</param>
        /// <param name="action">action to apply to list</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Method for applying an action to any enumerable sequence. 
        /// This is the analog of Select, but with no output (only "side effects").
        /// This overload takes in an Action with an additional int parameter that 
        /// provides the index of the sequence.
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="items">list of items</param>
        /// <param name="action">action to apply to list</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            int i = 0;
            foreach (var item in items)
            {
                action(item, i);
                i++;
            }
        }

        /// <summary>
        /// Converts an IEnumerable into a 2D array given a specified width and height
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="myArray">IEnumerable of generic type</param>
        /// <param name="width">integer representing width of array</param>
        /// <param name="height">integer representing height of array</param>
        /// <returns>2D array of generic type</returns>
        public static T[,] To2DArray<T>(this IEnumerable<T> myArray, int width, int height)
        {
            T[,] outputArray = new T[width, height];
            var enumerable = myArray.GetEnumerator();
            for (int yi = 0; yi < height; yi++)
            {
                for (int xi = 0; xi < width; xi++)
                {
                    enumerable.MoveNext();
                    outputArray[xi, yi] = enumerable.Current;
                }
            }
            return outputArray;
        }

        /// <summary>
        /// method to zip together values using supplied function
        /// </summary>
        /// <typeparam name="TFirst">type of 1st list</typeparam>
        /// <typeparam name="TSecond">type of 2nd list</typeparam>
        /// <typeparam name="TThird">type of 3rd list</typeparam>
        /// <typeparam name="TResult">resulting list</typeparam>
        /// <param name="first">IEnumerable of 1st values</param>
        /// <param name="second">IEnumerable of 1st values</param>
        /// <param name="third">IEnumerable of 1st values</param>
        /// <param name="func">IEnumerable of 1st values</param>
        /// <returns>IEnumerable of TResult</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> func)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            if (second == null)
                throw new ArgumentNullException("second");

            if (third == null)
                throw new ArgumentNullException("third");

            return ZipInternal(first, second, third, func);
        }

        private static IEnumerable<TResult> ZipInternal<TFirst, TSecond, TThird, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> func)
        {
            var ie1 = first.GetEnumerator();
            var ie2 = second.GetEnumerator();
            var ie3 = third.GetEnumerator();

            while (ie1.MoveNext() && ie2.MoveNext() && ie3.MoveNext())
                yield return func(ie1.Current, ie2.Current, ie3.Current);
        }
        /// <summary>
        /// method to zip together values using supplied function
        /// </summary>
        /// <typeparam name="TFirst">type of 1st list</typeparam>
        /// <typeparam name="TSecond">type of 2nd list</typeparam>
        /// <typeparam name="TThird">type of 3rd list</typeparam>
        /// <typeparam name="TFourth">type of 4th list</typeparam>
        /// <typeparam name="TResult">resulting list</typeparam>
        /// <param name="first">IEnumerable of 1st values</param>
        /// <param name="second">IEnumerable of 1st values</param>
        /// <param name="third">IEnumerable of 1st values</param>
        /// <param name="fourth">IEnumerable of 1st values</param>
        /// <param name="func">IEnumerable of 1st values</param>
        /// <returns>IEnumerable to zipped result</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, Func<TFirst, TSecond, TThird, TFourth, TResult> func)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            if (second == null)
                throw new ArgumentNullException("second");

            if (third == null)
                throw new ArgumentNullException("third");

            if (fourth == null)
                throw new ArgumentNullException("fourth");


            return ZipInternal(first, second, third, fourth, func);
        }

        private static IEnumerable<TResult> ZipInternal<TFirst, TSecond, TThird, TFourth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, Func<TFirst, TSecond, TThird, TFourth, TResult> func)
        {
            var ie1 = first.GetEnumerator();
            var ie2 = second.GetEnumerator();
            var ie3 = third.GetEnumerator();
            var ie4 = fourth.GetEnumerator();

            while (ie1.MoveNext() && ie2.MoveNext() && ie3.MoveNext() && ie4.MoveNext())
                yield return func(ie1.Current, ie2.Current, ie3.Current, ie4.Current);
        }
        /// <summary>
        /// method to zip together values using supplied function
        /// </summary>
        /// <typeparam name="TFirst">type of 1st list</typeparam>
        /// <typeparam name="TSecond">type of 2nd list</typeparam>
        /// <typeparam name="TThird">type of 3rd list</typeparam>
        /// <typeparam name="TFourth">type of 4th list</typeparam>
        /// <typeparam name="TFifth">type of 5th list</typeparam>
        /// <typeparam name="TResult">resulting list</typeparam>
        /// <param name="first">IEnumerable of 1st values</param>
        /// <param name="second">IEnumerable of 1st values</param>
        /// <param name="third">IEnumerable of 1st values</param>
        /// <param name="fourth">IEnumerable of 1st values</param>
        /// <param name="fifth">IEnumerable of 1st values</param>
        /// <param name="func">IEnumerable of 1st values</param>
        /// <returns>IEnumerable of TResult</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, IEnumerable<TFifth> fifth, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> func)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            if (second == null)
                throw new ArgumentNullException("second");

            if (third == null)
                throw new ArgumentNullException("third");

            if (fourth == null)
                throw new ArgumentNullException("fourth");

            if (fifth == null)
                throw new ArgumentNullException("fifth");

            return ZipInternal(first, second, third, fourth, fifth, func);
        }

        private static IEnumerable<TResult> ZipInternal<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, IEnumerable<TFifth> fifth, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> func)
        {
            var ie1 = first.GetEnumerator();
            var ie2 = second.GetEnumerator();
            var ie3 = third.GetEnumerator();
            var ie4 = fourth.GetEnumerator();
            var ie5 = fifth.GetEnumerator();

            while (ie1.MoveNext() && ie2.MoveNext() && ie3.MoveNext() && ie4.MoveNext() && ie5.MoveNext())
                yield return func(ie1.Current, ie2.Current, ie3.Current, ie4.Current, ie5.Current);
        }
        /// <summary>
        /// extension method to loop over 3D set of values and apply a function
        /// </summary>
        /// <typeparam name="T1">type of first dimension</typeparam>
        /// <typeparam name="T2">type of second dimension</typeparam>
        /// <typeparam name="T3">type of third dimension</typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="myFunc">function to be evaluated</param>
        /// <param name="firstValues">first dimension values</param>
        /// <param name="secondValues">second dimension values</param>
        /// <param name="thirdValues">third dimension values</param>
        /// <returns>IEnumerable of TResult</returns>
        public static IEnumerable<TReturn> LoopOverVariables<T1, T2, T3, TReturn>(
            this Func<T1, T2, T3, TReturn> myFunc,
            IEnumerable<T1> firstValues,
            IEnumerable<T2> secondValues,
            IEnumerable<T3> thirdValues)
        {
            foreach (var v1 in firstValues)
            {
                foreach (var v2 in secondValues)
                {
                    foreach (var v3 in thirdValues)
                    {
                        yield return myFunc(v1, v2, v3);
                    }
                }
            }
        }
        /// <summary>
        /// extension method to loop over 2D set of values and apply a function
        /// </summary>
        /// <typeparam name="T1">type of first dimension</typeparam>
        /// <typeparam name="T2">type of second dimension</typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="myFunc">function to be evaluated</param>
        /// <param name="firstValues">first dimension values</param>
        /// <param name="secondValues">second dimension values</param>
        /// <returns>IEnumerable of TReturn</returns>
        public static IEnumerable<TReturn> LoopOverVariables<T1, T2, TReturn>(
            this Func<T1, T2, TReturn> myFunc,
            IEnumerable<T1> firstValues,
            IEnumerable<T2> secondValues)
        {
            foreach (var v1 in firstValues)
            {
                foreach (var v2 in secondValues)
                {
                    yield return myFunc(v1, v2);
                }
            }
        }
    }
}
