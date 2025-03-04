using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Extensions;

namespace Vts.Test.Extensions
{
    [TestFixture] 
    public class ArrayExtensionsTests
    {
        [Test]
        public void validate_Column_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Column(0);

            Assert.That(IEnumerablesHaveEqualMembers(result, new double[] { 1, 4 }), Is.True);
        }

        [Test]
        public void validate_Columns_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Columns().Last();

            Assert.That(IEnumerablesHaveEqualMembers(result.ToArray(), new double[] { 3, 6 }), Is.True);
        }

        [Test]
        public void validate_Row_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Row(0);

            Assert.That(IEnumerablesHaveEqualMembers(result, new double[] { 1, 2, 3 }), Is.True);
        }

        [Test]
        public void validate_Rows_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Rows().Last();

            Assert.That(IEnumerablesHaveEqualMembers(result.ToArray(), new double[] { 4, 5, 6 }), Is.True);
        }

        [Test]
        public void Verify_CreateArray()
        {
            var originalArray = new int[3, 2, 2] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            var newArray = originalArray.CreateArray();
            Assert.That(newArray.Rank, Is.EqualTo(3));

            var singleArray = new int[3] { 11, 12, 13 };
            var newSingleArray = singleArray.CreateArray();
            Assert.That(newSingleArray.Rank, Is.EqualTo(1));
        }

        private static bool IEnumerablesHaveEqualMembers<T>(IEnumerable<T> myList, IEnumerable<T> comparingList)
        {
            return !Enumerable.Zip(myList, comparingList, (left, right) => left.Equals(right)).Contains(false);
        }
    }

}
