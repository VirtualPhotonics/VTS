using NUnit.Framework;
using System.IO;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class DirectionTests
    {
        private Direction _direction;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _direction = new Direction();
        }

        [Test]
        public void Validate_default_constructor()
        {
            Assert.AreEqual(0.0, _direction.Ux);
            Assert.AreEqual(0.0, _direction.Uy);
            Assert.AreEqual(1.0, _direction.Uz);
        }

        [Test]
        public void Validate_set_direction_values()
        {
            var direction = new Direction {Ux = 1.1, Uy = 2.2, Uz = 3.3};
            Assert.AreEqual(1.1, direction.Ux);
            Assert.AreEqual(2.2, direction.Uy);
            Assert.AreEqual(3.3, direction.Uz);
        }

        [Test]
        public void Test_equals_operator_null_value()
        {
            Direction d1 = null;
            var d2 = new Direction(1, 2, 3);
            Assert.IsFalse(d1 == d2);
        }

        [Test]
        public void Test_equals_operator_both_null_value()
        {
            Direction d1 = null;
            Direction d2 = null;
            Assert.IsTrue(d1 == d2);
        }

        [Test]
        public void Test_not_equals_operator()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(4, 5, 6);
            Assert.IsTrue(d1 != d2);
        }

        [Test]
        public void Test_read_binary()
        {
            var direction = new Direction(0.1, 0.2, 0.3);
            using (var stream = new MemoryStream(new byte[24], true))
            {
                var binaryWriter = new BinaryWriter(stream);
                binaryWriter.Write(direction.Ux);
                binaryWriter.Write(direction.Uy);
                binaryWriter.Write(direction.Uz);
                stream.Position = 0;
                var binaryReader = new BinaryReader(stream);
                var direction2 = Direction.ReadBinary(binaryReader);
                Assert.AreEqual(0.1, direction2.Ux);
                Assert.AreEqual(0.2, direction2.Uy);
                Assert.AreEqual(0.3, direction2.Uz);
            }
        }

        [Test]
        public void Test_write_binary()
        {
            var direction = new Direction(1.1, 2.2, 3.3);
            using (var stream = new MemoryStream(new byte[24], true))
            {
                direction.WriteBinary(new BinaryWriter(stream));
                Assert.IsInstanceOf<MemoryStream>(stream);
                stream.Position = 0;
                var binaryReader = new BinaryReader(stream);
                Assert.AreEqual(1.1, binaryReader.ReadDouble());
                Assert.AreEqual(2.2, binaryReader.ReadDouble());
                Assert.AreEqual(3.3, binaryReader.ReadDouble());
            }
        }

        [Test]
        public void Validate_constructor_assigns_correct_values()
        {
            var d1 = new Direction(1, 2, 3);

            Assert.AreEqual(1.0, d1.Ux);
            Assert.AreEqual(2.0, d1.Uy);
            Assert.AreEqual(3.0, d1.Uz);
        }

        [Test]
        public void Validate_Equals_returns_true_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);

            Assert.IsTrue(d1.Equals(d2));
        }

        [Test]
        public void Validate_equals_operator_returns_true_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);
            
            Assert.IsTrue(d1 == d2);
        }

        [Test]
        public void Validate_equals_returns_false()
        {
            const string s1 = "invalid object";
            Assert.IsFalse(_direction.Equals(s1));
        }

        [Test]
        public void Validate_NotEqualsOperator_returns_false_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);

            Assert.IsFalse(d1 != d2);
        }

        /// <summary>
        /// Tests whether GetDistance returns correct value
        /// </summary>
        [Test]
        public void Validate_GetDotProduct_returns_correct_value()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(2, 2, 2);
            var result = Direction.GetDotProduct(d1, d2);
            Assert.AreEqual(12, result);
        }

        [Test]
        public void Validate_along_positive_x_axis()
        {
            var direction = Direction.AlongPositiveXAxis;
            Assert.AreEqual(1.0, direction.Ux);
            Assert.AreEqual(0.0, direction.Uy);
            Assert.AreEqual(0.0, direction.Uz);
        }

        [Test]
        public void Validate_along_positive_y_axis()
        {
            var direction = Direction.AlongPositiveYAxis;
            Assert.AreEqual(0.0, direction.Ux);
            Assert.AreEqual(1.0, direction.Uy);
            Assert.AreEqual(0.0, direction.Uz);
        }

        [Test]
        public void Validate_along_positive_z_axis()
        {
            var direction = Direction.AlongPositiveZAxis;
            Assert.AreEqual(0.0, direction.Ux);
            Assert.AreEqual(0.0, direction.Uy);
            Assert.AreEqual(1.0, direction.Uz);
        }

        [Test]
        public void Validate_along_negative_x_axis()
        {
            var direction = Direction.AlongNegativeXAxis;
            Assert.AreEqual(-1.0, direction.Ux);
            Assert.AreEqual(0.0, direction.Uy);
            Assert.AreEqual(0.0, direction.Uz);
        }

        [Test]
        public void Validate_along_negative_y_axis()
        {
            var direction = Direction.AlongNegativeYAxis;
            Assert.AreEqual(0.0, direction.Ux);
            Assert.AreEqual(-1.0, direction.Uy);
            Assert.AreEqual(0.0, direction.Uz);
        }

        [Test]
        public void Validate_along_negative_z_axis()
        {
            var direction = Direction.AlongNegativeZAxis;
            Assert.AreEqual(0.0, direction.Ux);
            Assert.AreEqual(0.0, direction.Uy);
            Assert.AreEqual(-1.0, direction.Uz);
        }

        [Test]
        public void Test_clone()
        {
            var direction = _direction.Clone();
            Assert.AreEqual(_direction.Ux, direction.Ux);
            Assert.AreEqual(_direction.Uy, direction.Uy);
            Assert.AreEqual(_direction.Uz, direction.Uz);
        }

        [Test]
        public void Test_get_hash_code()
        {
            var position = new Position();
            var hashCode = position.GetHashCode();
            Assert.AreEqual(hashCode, position.GetHashCode());
            var position2 = new Position();
            Assert.AreNotEqual(hashCode, position2.GetHashCode());
        }
    }
}
