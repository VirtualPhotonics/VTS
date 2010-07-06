using System;
using System.Collections.Generic;
using System.Linq;

namespace Vts.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T singleItem)
        {
            yield return singleItem;
        }

        /// <summary>
        /// 2D array version of Select with x and y indexers
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

        public static IEnumerable<double> To(this double from, double to, double every)
        {
            if (to >= from && every >= 0 || from >= to && every <= 0)
            {
                long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
                var value = from;
                for (long i = 0; i < numElements; i++, value += every)
                {
                    yield return value;
                }
            }
        }
        public static IEnumerable<float> To(this float from, float to, float every)
        {
            if (to >= from && every >= 0 || from >= to && every <= 0)
            {
                long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
                var value = from;
                for (long i = 0; i < numElements; i++, value += every)
                {
                    yield return value;
                }
            }
        }
        public static IEnumerable<long> To(this long from, long to, long every)
        {
            if (to >= from && every >= 0 || from >= to && every <= 0)
            {
                long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
                var value = from;
                for (long i = 0; i < numElements; i++, value += every)
                {
                    yield return value;
                }
            }
        }
        public static IEnumerable<int> To(this int from, int to, int every)
        {
            if (to >= from && every >= 0 || from >= to && every <= 0)
            {
                long numElements = (long)(Math.Floor(Math.Abs((double)(to - from) / every) + 10e-10D) + 1);
                var value = from;
                for (long i = 0; i < numElements; i++, value += every)
                {
                    yield return value;
                }
            }
        }
        public static IEnumerable<int> To(this int from, int to) { return To(from, to, from <=to ? 1 : -1); }
        public static IEnumerable<long> To(this long from, long to) { return To(from, to, from <= to ? 1L : -1L); }
        public static IEnumerable<float> To(this float from, float to) { return To(from, to, from <= to ? 1F : -1F); }
        public static IEnumerable<double> To(this double from, double to) { return To(from, to, from <= to ? 1D : -1D); }

        //public static IEnumerable<Tuple<TFirst, TSecond>> Unzip<TInput, TFirst, TSecond>(this IEnumerable<TInput> input, Func<TInput, Tuple<TFirst, TSecond>> func)
        //{
        //    if (input == null)
        //        throw new ArgumentNullException("input");
        //    return UnzipInternal(input, func);
        //}
        //private static IEnumerable<Tuple<TFirst, TSecond>> UnzipInternal<TInput, TFirst, TSecond>(this IEnumerable<TInput> input, Func<TInput, Tuple<TFirst,TSecond>> func)
        //{
        //    foreach (TInput item in input)
        //        yield return func(item);
        //}

        //public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func)
        //{
        //    if (first == null)
        //        throw new ArgumentNullException("first");

        //    if (second == null)
        //        throw new ArgumentNullException("second");

        //    return ZipInternal(first, second, func);
        //}

        //private static IEnumerable<TResult> ZipInternal<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func)
        //{
        //    var ie1 = first.GetEnumerator();
        //    var ie2 = second.GetEnumerator();

        //    while (ie1.MoveNext() && ie2.MoveNext())
        //        yield return func(ie1.Current, ie2.Current);
        //}

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

        //public static IEnumerable<T> To<T>(this T from, T to, Func<T, T> inc) where T : struct, IComparable<T>
        //{
        //    for (T t = from; t.CompareTo(to) <= 0; t = inc(t))
        //    {
        //        yield return t;
        //    }
        //}
        //public static IEnumerable<int> To(this int from, int to, int every) { return To(from, to, x => x + every); }
        //public static IEnumerable<long> To(this long from, long to, long every) { return To(from, to, x => x + every); }
        //public static IEnumerable<float> To(this float from, float to, float every) { return To(from, to, x => x + every); }
        //public static IEnumerable<double> To(this double from, double to, double every) { return To(from, to, x => x + every); }
        //public static IEnumerable<int> To(this int from, int to) { return To(from, to, x => x + 1); }
        //public static IEnumerable<long> To(this long from, long to) { return To(from, to, x => x + 1L); }
        //public static IEnumerable<float> To(this float from, float to) { return To(from, to, x => x + 1.0F); }
        //public static IEnumerable<double> To(this double from, double to) { return To(from, to, x => x + 1.0D); }
    }
}
