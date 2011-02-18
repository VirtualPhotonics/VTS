using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class DirectionTests
    {
        [Test]
        public void validate_constructor_assigns_correct_values()
        {
            var d1 = new Direction(1, 2, 3);

            Assert.IsTrue(
                d1.Ux == 1.0 &&
                d1.Uy == 2.0 &&
                d1.Uz == 3.0
           );
        }

        [Test]
        public void validate_Equals_returns_true_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);

            Assert.IsTrue(d1.Equals(d2));
        }

        [Test]
        public void validate_EqualsOperator_returns_true_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);
            
            Assert.IsTrue(d1 == d2);
        }

        [Test]
        public void validate_NotEqualsOperator_returns_false_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);

            Assert.IsFalse(d1 != d2);
        }

        /// <summary>
        /// Tests whether GetDistance returns correct value
        /// </summary>
        [Test]
        public void validate_GetDotProduct_returns_correct_value()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(2, 2, 2);
            var result = Direction.GetDotProduct(d1, d2);
            Assert.IsTrue(result == 12);
        }
    }
}
