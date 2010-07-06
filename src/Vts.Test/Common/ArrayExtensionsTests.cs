using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Extensions;

namespace Vts.Test.Common
{
    [TestFixture] 
    public class ArrayExtensionsTests
    {
        [Test]
        public void validate_Column_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Column(0);

            Assert.IsTrue(IEnumerablesHaveEqualMembers(result, new double[] { 1, 4 }));
        }

        [Test]
        public void validate_Columns_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Columns().Last();

            Assert.IsTrue(IEnumerablesHaveEqualMembers(result.ToArray(), new double[] { 3, 6 }));
        }

        [Test]
        public void validate_Row_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Row(0);

            Assert.IsTrue(IEnumerablesHaveEqualMembers(result, new double[] { 1, 2, 3 }));
        }

        [Test]
        public void validate_Rows_returns_correct_values()
        {
            var a = new double[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };

            var result = a.Rows().Last();

            Assert.IsTrue(IEnumerablesHaveEqualMembers(result.ToArray(), new double[] { 4, 5, 6 }));
        }

        private static bool IEnumerablesHaveEqualMembers<T>(IEnumerable<T> myList, IEnumerable<T> comparingList)
        {
            return !myList.Zip(comparingList, (left, right) => left.Equals(right)).Contains(false);
        }
    }

}
