using System;
using System.Collections.Generic;
using System.Linq;

namespace Vts.Extensions
{
    /// <summary>
    /// Helper group of extension methods for adding LINQ funcitonality
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Extension method to turn any single item into an IEnumerable (i.e. "yield-returns" the item)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="singleItem"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this T singleItem)
        {
            yield return singleItem;
        }

        /// <summary>
        /// Extension method to append a single item to an IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T newItem )
        {
            return Enumerable.Concat(items, newItem.AsEnumerable());
        }

        /// <summary>
        /// Extension method to create a dictionary from key-value pairs
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// 2D array overload of the LINQ Select operator with x and y indexers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="my2DArray"></param>
        /// <param name="myFunc"></param>
        /// <returns></returns>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="myArray"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
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

        ///// <summary>
        ///// (Deprecated) 
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <param name="every"></param>
        ///// <returns></returns>
        //public static IEnumerable<double> To(this double from, double to, double every)
        //{
        //    if (to >= from && every >= 0 || from >= to && every <= 0)
        //    {
        //        long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
        //        var value = from;
        //        for (long i = 0; i < numElements; i++, value += every)
        //        {
        //            yield return value;
        //        }
        //    }
        //}

        //public static IEnumerable<float> To(this float from, float to, float every)
        //{
        //    if (to >= from && every >= 0 || from >= to && every <= 0)
        //    {
        //        long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
        //        var value = from;
        //        for (long i = 0; i < numElements; i++, value += every)
        //        {
        //            yield return value;
        //        }
        //    }
        //}

        //public static IEnumerable<long> To(this long from, long to, long every)
        //{
        //    if (to >= from && every >= 0 || from >= to && every <= 0)
        //    {
        //        long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
        //        var value = from;
        //        for (long i = 0; i < numElements; i++, value += every)
        //        {
        //            yield return value;
        //        }
        //    }
        //}

        //public static IEnumerable<int> To(this int from, int to, int every)
        //{
        //    if (to >= from && every >= 0 || from >= to && every <= 0)
        //    {
        //        long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
        //        var value = from;
        //        for (long i = 0; i < numElements; i++, value += every)
        //        {
        //            yield return value;
        //        }
        //    }
        //}

        //public static IEnumerable<int> To(this int from, int to) { return To(from, to, from <=to ? 1 : -1); }
        //public static IEnumerable<long> To(this long from, long to) { return To(from, to, from <= to ? 1L : -1L); }
        //public static IEnumerable<float> To(this float from, float to) { return To(from, to, from <= to ? 1F : -1F); }
        //public static IEnumerable<double> To(this double from, double to) { return To(from, to, from <= to ? 1D : -1D); }

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
