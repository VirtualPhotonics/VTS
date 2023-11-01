using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        private ForwardSolverBase _forwardSolverBaseMock;
        private ForwardSolverBase _forwardSolverBaseMockWithSetup;
        private IEnumerable<double> _doubleEnumerable;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _forwardSolverBaseMock = Substitute.ForPartsOf<ForwardSolverBase>();
            _forwardSolverBaseMockWithSetup = Substitute.ForPartsOf<ForwardSolverBase>();
            _forwardSolverBaseMockWithSetup.ROfRhoAndTime(Arg.Any<OpticalProperties>(), Arg.Any<double>(), Arg.Any<double>()).Returns(0.6);
            _doubleEnumerable = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(
                new List<OpticalProperties>
                {
                    new OpticalProperties(0.1, 1, 0.8, 1.4),
                    new OpticalProperties(0.01, 1, 0.8, 1.4)
                }, new List<double>
                {
                    0.1,
                    0.2,
                    0.3
                }, new List<double>
                {
                    0.1,
                    0.2
                });
        }

        [Test]
        public void Test_LoopOverVariables_with_two_values()
        {
            var doubleList =
                _forwardSolverBaseMock.ROfRho(new List<OpticalProperties>
                {
                    new OpticalProperties(0.1, 1, 0.8, 1.4),
                    new OpticalProperties(0.01, 1, 0.8, 1.4)
                }, new List<double>
                {
                    0.1,
                    0.2,
                    0.3
                });
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
            Assert.Throws<NotImplementedException>(() =>
            {
                var arrayList = doubleList.ToArray();
                Assert.IsNull(arrayList);
            });
        }

        [Test]
        public void Test_LoopOverVariables_with_three_values()
        {
            var doubleList =
                _forwardSolverBaseMock.ROfRhoAndTime(new List<OpticalProperties>
                {
                    new OpticalProperties(0.1, 1, 0.8, 1.4),
                    new OpticalProperties(0.01, 1, 0.8, 1.4)
                }, new List<double>
                {
                    0.1,
                    0.2,
                    0.3
                }, new List<double>
                {
                    0.1,
                    0.2
                });
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
            Assert.Throws<NotImplementedException>(() =>
            {
                var arrayList = doubleList.ToArray();
                Assert.IsNull(arrayList);
            });
        }

        [Test]
        public void ToDictionary_returns_dictionary_from_key_value_pairs()
        {
            var keyValuePairList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("one", "first"),
                new KeyValuePair<string, string>("two", "second"),
                new KeyValuePair<string, string>("three", "third")
            };
            var dictionary = keyValuePairList.ToDictionary();
            Assert.IsInstanceOf<Dictionary<string, string>>(dictionary);
            Assert.AreEqual("first", dictionary["one"]);
            Assert.AreEqual("second", dictionary["two"]);
            Assert.AreEqual("third", dictionary["three"]);
        }

        [Test]
        public void To2DArray_returns_a_2_dimensional_array()
        {
            var twoDArray = _doubleEnumerable.To2DArray(2, 4);
            Assert.AreEqual(0.6, twoDArray[0,0]);
        }

        [Test]
        public void Select_returns_enumerable_from_2_dimensional_array()
        {
            var twoDArray = _doubleEnumerable.To2DArray(2, 4);
            var doubleEnumerable = twoDArray.Select((d, i1, i2) => d * i1 * i2);
            doubleEnumerable.ForEach(x => Assert.IsInstanceOf<double>(x));
        }

        [Test]
        public void Zip_first_throws_argument_null_exception()
        {
            IEnumerable<double> doubleEnumerable = null;
            Assert.Throws<ArgumentNullException>(() => doubleEnumerable.Zip<double, double, double, double>(null, null, null));
            Assert.Throws<ArgumentNullException>(() => doubleEnumerable.Zip<double, double, double, double, double>(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => doubleEnumerable.Zip<double, double, double, double, double, double>(null, null, null, null, null));
        }

        [Test]
        public void Zip_second_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double>(null, null, null));
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double>(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double, double>(null, null, null, null, null));
        }

        [Test]
        public void Zip_third_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double>(_doubleEnumerable, null, null));
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double>(_doubleEnumerable, null, null, null));
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double, double>(_doubleEnumerable, null, null, null, null));
        }

        [Test]
        public void Zip_fourth_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double>(_doubleEnumerable, _doubleEnumerable, null, null));
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double, double>(_doubleEnumerable, _doubleEnumerable, null, null, null));
        }

        [Test]
        public void Zip_fifth_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _doubleEnumerable.Zip<double, double, double, double, double, double>(_doubleEnumerable, _doubleEnumerable, _doubleEnumerable, null, null));
        }

        [Test]
        public void Test_Zip_with_four_parameters()
        {
            var result = _doubleEnumerable.Zip(_doubleEnumerable,
                _doubleEnumerable, _doubleEnumerable, (d1, d2, d3, d4) => d1 * d2 * d3);
            result.ForEach(x => Assert.IsInstanceOf<double>(x));
        }

        [Test]
        public void Test_Zip_with_five_parameters()
        {
            var result = _doubleEnumerable.Zip(_doubleEnumerable,
                _doubleEnumerable, _doubleEnumerable, _doubleEnumerable, (d1, d2, d3, d4, d5) => d1 * d2 * d3 * d4);
            result.ForEach(x => Assert.IsInstanceOf<double>(x));
        }

        [Test]
        public void Test_TakeEveryNth_returns_correct_values()
        {
            var range = new DoubleRange(0, 99, 100);
            var skippedValues = range.TakeEveryNth(5);
            var skippedValueArray = skippedValues.ToArray();
            Assert.AreEqual(20, skippedValueArray.Length);
            Assert.AreEqual(0, skippedValueArray[0]);
            Assert.AreEqual(10, skippedValueArray[2]);
            Assert.AreEqual(20, skippedValueArray[4]);
            Assert.AreEqual(85, skippedValueArray[17]);
        }
    }
}
