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
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="singleItem">The single item to turn into IEnumerable</param>
        /// <returns>An IEnumerable of type T</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T singleItem)
        {
            yield return singleItem;
        }

        /// <summary>
        /// Extension method to append a single item to an IEnumerable
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="items">The list of existing items</param>
        /// <param name="newItem">The item to be added</param>
        /// <returns>An IEnumerable of type T with concatenated list</returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T newItem )
        {
            return items.Concat(newItem.AsEnumerable());
        }

        /// <summary>
        /// Extension method to create a dictionary from key-value pairs
        /// </summary>
        /// <typeparam name="TKey">The dictionary key</typeparam>
        /// <typeparam name="TValue">The dictionary value</typeparam>
        /// <param name="keyValuePairs">The key-value pairs used to create the dictionary</param>
        /// <returns>An IDictionary with key-value pairs</returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// 2D array overload of the LINQ Select operator with x and y indexers
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="my2DArray">The 2D array of type T</param>
        /// <param name="myFunc">The linq select function</param>
        /// <returns>An IEnumerable of type TResult</returns>
        public static IEnumerable<TResult> Select<T, TResult>(
            this T[,] my2DArray,
            Func<T, int, int, TResult> myFunc)
        {
            var width = my2DArray.GetLength(0);
            var height = my2DArray.GetLength(1);

            return
                from yi in Enumerable.Range(0, height)
                from xi in Enumerable.Range(0, width)
                select myFunc(my2DArray[xi, yi], xi, yi);
        }

        /// <summary>
        /// Helper extension method that returns every nth element of the enumerable, starting at the specified skip index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">the values being filtered</param>
        /// <param name="n">number of values to jump forward at a time</param>
        /// <param name="skip">number of values to initially skip</param>
        /// <returns></returns>
        public static IEnumerable<T> TakeEveryNth<T>(this IEnumerable<T> values, int n, int skip = 0) =>
                values.Where((_, i) => (i - skip) % n == 0);

        /// <summary>
        /// Method for applying an action to any enumerable sequence. 
        /// This is the analog of Select, but with no output (only "side effects").
        /// This overload takes in an Action with an additional int parameter that 
        /// provides the index of the sequence.
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="items">The list of items</param>
        /// <param name="action">The action to apply to list</param>
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
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="items">The list of items</param>
        /// <param name="action">The action to apply to list</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in items)
            {
                action(item, i);
                i++;
            }
        }

        /// <summary>
        /// Converts an IEnumerable into a 2D array given a specified width and height
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="myArray">The IEnumerable type T</param>
        /// <param name="width">The integer representing the width of the array</param>
        /// <param name="height">The integer representing the height of the array</param>
        /// <returns>A 2D array of type T</returns>
        public static T[,] To2DArray<T>(this IEnumerable<T> myArray, int width, int height)
        {
            var outputArray = new T[width, height];
            var enumerable = myArray.GetEnumerator();
            for (var yi = 0; yi < height; yi++)
            {
                for (var xi = 0; xi < width; xi++)
                {
                    enumerable.MoveNext();
                    outputArray[xi, yi] = enumerable.Current;
                }
            }
            return outputArray;
        }

        /// <summary>
        /// Method to zip together values using supplied function
        /// </summary>
        /// <typeparam name="TFirst">The type of 1st list</typeparam>
        /// <typeparam name="TSecond">The type of 2nd list</typeparam>
        /// <typeparam name="TThird">The type of 3rd list</typeparam>
        /// <typeparam name="TResult">The type of the resulting list</typeparam>
        /// <param name="first">The IEnumerable of 1st values</param>
        /// <param name="second">The IEnumerable of 2nd values</param>
        /// <param name="third">IEnumerable of 3rd values</param>
        /// <param name="func">The function to zip together the values</param>
        /// <returns>An IEnumerable of type TResult of the zipped values</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> func)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            if (third == null)
                throw new ArgumentNullException(nameof(third));

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
        /// Method to zip together values using supplied function
        /// </summary>
        /// <typeparam name="TFirst">The type of 1st list</typeparam>
        /// <typeparam name="TSecond">The type of 2nd list</typeparam>
        /// <typeparam name="TThird">The type of 3rd list</typeparam>
        /// <typeparam name="TFourth">The type of 4th list</typeparam>
        /// <typeparam name="TResult">The resulting list type</typeparam>
        /// <param name="first">The IEnumerable of 1st values</param>
        /// <param name="second">The IEnumerable of 2nd values</param>
        /// <param name="third">The IEnumerable of 3rd values</param>
        /// <param name="fourth">The IEnumerable of 4th values</param>
        /// <param name="func">The function to zip together the values</param>
        /// <returns>An IEnumerable of type TResult of the zipped values</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, Func<TFirst, TSecond, TThird, TFourth, TResult> func)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            if (third == null)
                throw new ArgumentNullException(nameof(third));

            if (fourth == null)
                throw new ArgumentNullException(nameof(fourth));


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
        /// Method to zip together values using supplied function
        /// </summary>
        /// <typeparam name="TFirst">The type of 1st list</typeparam>
        /// <typeparam name="TSecond">The type of 2nd list</typeparam>
        /// <typeparam name="TThird">The type of 3rd list</typeparam>
        /// <typeparam name="TFourth">The type of 4th list</typeparam>
        /// <typeparam name="TFifth">The type of 5th list</typeparam>
        /// <typeparam name="TResult">The resulting list type</typeparam>
        /// <param name="first">The IEnumerable of 1st values</param>
        /// <param name="second">The IEnumerable of 2nd values</param>
        /// <param name="third">The IEnumerable of 3rd values</param>
        /// <param name="fourth">The IEnumerable of 4th values</param>
        /// <param name="fifth">The IEnumerable of 5th values</param>
        /// <param name="func">The function to zip together the values</param>
        /// <returns>An IEnumerable of type TResult of the zipped values</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, IEnumerable<TFifth> fifth, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> func)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            if (third == null)
                throw new ArgumentNullException(nameof(third));

            if (fourth == null)
                throw new ArgumentNullException(nameof(fourth));

            if (fifth == null)
                throw new ArgumentNullException(nameof(fifth));

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
        /// Extension method to loop over 3D set of values and apply a function
        /// </summary>
        /// <typeparam name="T1">The type of first dimension</typeparam>
        /// <typeparam name="T2">The type of second dimension</typeparam>
        /// <typeparam name="T3">The type of third dimension</typeparam>
        /// <typeparam name="TReturn">The resulting list type</typeparam>
        /// <param name="myFunc">The function to be evaluated</param>
        /// <param name="firstValues">The first dimension values</param>
        /// <param name="secondValues">The second dimension values</param>
        /// <param name="thirdValues">The third dimension values</param>
        /// <returns>An IEnumerable of type TResult</returns>
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
        /// Extension method to loop over 2D set of values and apply a function
        /// </summary>
        /// <typeparam name="T1">The type of first dimension</typeparam>
        /// <typeparam name="T2">The type of second dimension</typeparam>
        /// <typeparam name="TReturn">The resulting list type</typeparam>
        /// <param name="myFunc">The function to be evaluated</param>
        /// <param name="firstValues">The first dimension values</param>
        /// <param name="secondValues">The second dimension values</param>
        /// <returns>An IEnumerable of type TReturn</returns>
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
