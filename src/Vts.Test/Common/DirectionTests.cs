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
            Assert.That(_direction.Ux, Is.EqualTo(0.0));
            Assert.That(_direction.Uy, Is.EqualTo(0.0));
            Assert.That(_direction.Uz, Is.EqualTo(1.0));
        }

        [Test]
        public void Validate_set_direction_values()
        {
            var direction = new Direction {Ux = 1.1, Uy = 2.2, Uz = 3.3};
            Assert.That(direction.Ux, Is.EqualTo(1.1));
            Assert.That(direction.Uy, Is.EqualTo(2.2));
            Assert.That(direction.Uz, Is.EqualTo(3.3));
        }

        [Test]
        public void Test_equals_operator_null_value()
        {
            Direction d1 = null;
            var d2 = new Direction(1, 2, 3);
            Assert.That(d1 == d2, Is.False);
        }

        [Test]
        public void Test_equals_operator_both_null_value()
        {
            Direction d1 = null;
            Direction d2 = null;
            Assert.That(d1 == d2, Is.True);
        }

        [Test]
        public void Test_not_equals_operator()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(4, 5, 6);
            Assert.That(d1 != d2, Is.True);
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
                Assert.That(direction2.Ux, Is.EqualTo(0.1));
                Assert.That(direction2.Uy, Is.EqualTo(0.2));
                Assert.That(direction2.Uz, Is.EqualTo(0.3));
            }
        }

        [Test]
        public void Test_write_binary()
        {
            var direction = new Direction(1.1, 2.2, 3.3);
            using (var stream = new MemoryStream(new byte[24], true))
            {
                direction.WriteBinary(new BinaryWriter(stream));
                Assert.That(stream, Is.InstanceOf<MemoryStream>());
                stream.Position = 0;
                var binaryReader = new BinaryReader(stream);
                Assert.That(binaryReader.ReadDouble(), Is.EqualTo(1.1));
                Assert.That(binaryReader.ReadDouble(), Is.EqualTo(2.2));
                Assert.That(binaryReader.ReadDouble(), Is.EqualTo(3.3));
            }
        }

        [Test]
        public void Validate_constructor_assigns_correct_values()
        {
            var d1 = new Direction(1, 2, 3);

            Assert.That(d1.Ux, Is.EqualTo(1.0));
            Assert.That(d1.Uy, Is.EqualTo(2.0));
            Assert.That(d1.Uz, Is.EqualTo(3.0));
        }

        [Test]
        public void Validate_Equals_returns_true_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);

            Assert.That(d1.Equals(d2), Is.True);
        }

        [Test]
        public void Validate_equals_operator_returns_true_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);
            
            Assert.That(d1 == d2, Is.True);
        }

        [Test]
        public void Validate_equals_returns_false()
        {
            const string s1 = "invalid object";
            Assert.That(_direction.Equals(s1), Is.False);
        }

        [Test]
        public void Validate_NotEqualsOperator_returns_false_for_same_inputs()
        {
            var d1 = new Direction(1, 2, 3);
            var d2 = new Direction(1, 2, 3);

            Assert.That(d1 != d2, Is.False);
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
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void Validate_along_positive_x_axis()
        {
            var direction = Direction.AlongPositiveXAxis;
            Assert.That(direction.Ux, Is.EqualTo(1.0));
            Assert.That(direction.Uy, Is.EqualTo(0.0));
            Assert.That(direction.Uz, Is.EqualTo(0.0));
        }

        [Test]
        public void Validate_along_positive_y_axis()
        {
            var direction = Direction.AlongPositiveYAxis;
            Assert.That(direction.Ux, Is.EqualTo(0.0));
            Assert.That(direction.Uy, Is.EqualTo(1.0));
            Assert.That(direction.Uz, Is.EqualTo(0.0));
        }

        [Test]
        public void Validate_along_positive_z_axis()
        {
            var direction = Direction.AlongPositiveZAxis;
            Assert.That(direction.Ux, Is.EqualTo(0.0));
            Assert.That(direction.Uy, Is.EqualTo(0.0));
            Assert.That(direction.Uz, Is.EqualTo(1.0));
        }

        [Test]
        public void Validate_along_negative_x_axis()
        {
            var direction = Direction.AlongNegativeXAxis;
            Assert.That(direction.Ux, Is.EqualTo(-1.0));
            Assert.That(direction.Uy, Is.EqualTo(0.0));
            Assert.That(direction.Uz, Is.EqualTo(0.0));
        }

        [Test]
        public void Validate_along_negative_y_axis()
        {
            var direction = Direction.AlongNegativeYAxis;
            Assert.That(direction.Ux, Is.EqualTo(0.0));
            Assert.That(direction.Uy, Is.EqualTo(-1.0));
            Assert.That(direction.Uz, Is.EqualTo(0.0));
        }

        [Test]
        public void Validate_along_negative_z_axis()
        {
            var direction = Direction.AlongNegativeZAxis;
            Assert.That(direction.Ux, Is.EqualTo(0.0));
            Assert.That(direction.Uy, Is.EqualTo(0.0));
            Assert.That(direction.Uz, Is.EqualTo(-1.0));
        }

        [Test]
        public void Test_clone()
        {
            var direction = _direction.Clone();
            Assert.That(direction.Ux, Is.EqualTo(_direction.Ux));
            Assert.That(direction.Uy, Is.EqualTo(_direction.Uy));
            Assert.That(direction.Uz, Is.EqualTo(_direction.Uz));
        }

        [Test]
        public void Test_get_hash_code()
        {
            var direction = new Direction();
            var hashCode = direction.GetHashCode();
            Assert.That(direction.GetHashCode(), Is.EqualTo(hashCode));
            var direction2 = new Direction();
            Assert.That(direction2.GetHashCode(), Is.EqualTo(hashCode));
        }
    }
}
