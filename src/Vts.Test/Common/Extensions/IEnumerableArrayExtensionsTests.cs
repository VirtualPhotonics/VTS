using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vts.Extensions;

namespace Vts.Test.Common.Extensions
{
    [TestFixture]
    internal class IEnumerableArrayExtensionsTests
    {
        [Test]
        public void ToEnumerable_array_type_returns_enumerable()
        {
            Array array = new Array[2];
            var result = array.ToEnumerable<int>();
            Assert.IsInstanceOf<IEnumerable<int>>(result);
        }

        [Test]
        public void ToEnumerable_array_returns_enumerable()
        {
            Array array = new Complex[2] { new Complex(4.0, 0.0), new Complex(1.3, 0.85) }; 
            var result = array.ToEnumerable<Complex>();
            Assert.IsInstanceOf<IEnumerable<Complex>>(result);
        }

        [Test]
        public void ToEnumerable_multi_array_2_returns_enumerable()
        {
            Array array = new double[3, 2] { { 1.5, 6.2 }, { 5.1, 6.7 }, { 4.0, 3.2 } };
            var result = array.ToEnumerable<double>();
            Assert.IsInstanceOf<IEnumerable<double>>(result);
        }

        [Test]
        public void ToEnumerable_multi_array_3_returns_enumerable()
        {
            Array array = new ushort[3, 2, 2] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            var result = array.ToEnumerable<ushort>();
            Assert.IsInstanceOf<IEnumerable<ushort>>(result);
        }

        [Test]
        public void ToEnumerable_multi_array_4_returns_enumerable()
        {
            var uShortArray = new ushort[3, 2, 2, 2]
            {
                {
                    {
                        {1, 0}, {2, 1}
                    },
                    {
                        {3, 5}, {4, 2}
                    }
                },
                {
                    {
                        {2, 6}, {2, 2}
                    },
                    {
                        {0, 0}, {9, 1}
                    }
                },
                {
                    {
                        {6, 4}, {3, 1}
                    },
                    {
                        {3, 0}, {7, 1}
                    }
                }
            };

            var uShortEnumerable = uShortArray.ToEnumerable<ushort>();
            Assert.IsInstanceOf<IEnumerable<ushort>>(uShortEnumerable);
            uShortEnumerable.ForEach(x => Assert.IsInstanceOf<ushort>(x));
        }
    }
}
