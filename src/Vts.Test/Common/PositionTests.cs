using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class PositionTests
    {
        [Test]
        public void validate_constructor_assigns_correct_values()
        {
            var p1 = new Position(1, 2, 3);

            Assert.IsTrue(
                p1.X == 1.0 &&
                p1.Y == 2.0 &&
                p1.Z == 3.0
           );
        }

        [Test]
        public void validate_Equals_returns_true_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);

            Assert.IsTrue(p1.Equals(p2));
        }

        [Test]
        public void validate_EqualsOperator_returns_true_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);
            
            Assert.IsTrue(p1 == p2);
        }

        [Test]
        public void validate_NotEqualsOperator_returns_false_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);

            Assert.IsFalse(p1 != p2);
        }

        [Test]
        public void validate_PlusOperator_returns_correct_values()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(4, 5, 6);
            var result = p1 + p2;
            Assert.IsTrue(result.Equals(new Position(5, 7, 9)));
        }

        [Test]
        public void validate_MinusOperator_returns_correct_values()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(4, 5, 6);
            var result = p1 - p2;
            Assert.IsTrue(result.Equals(new Position(-3, -3, -3)));
        }
    }
}
