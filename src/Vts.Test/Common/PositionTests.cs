using System.IO;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class PositionTests
    {
        [Test]
        public void Test_default_constructor()
        {
            var position = new Position();
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);
            Assert.AreEqual(0, position.Z);
        }

        [Test]
        public void Validate_constructor_assigns_correct_values()
        {
            var p1 = new Position(1, 2, 3);

            Assert.AreEqual(1, p1.X);
            Assert.AreEqual(2, p1.Y);
            Assert.AreEqual(3, p1.Z);
        }

        [Test]
        public void Validate_Equals_returns_true_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);

            Assert.IsTrue(p1.Equals(p2));
        }

        [Test]
        public void Validate_equals_operator_returns_true_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);
            
            Assert.IsTrue(p1 == p2);
        }

        [Test]
        public void Validate_equals_operator_returns_false_for_null_input()
        {
            var p1 = new Position(1, 2, 3);
            Position p2 = null;

            Assert.IsFalse(p1 == p2);
        }

        [Test]
        public void Validate_Equals_returns_false_for_null_input()
        {
            var p1 = new Position(1, 2, 3);
            Position p2 = null;

            Assert.IsFalse(p1.Equals(p2));
        }

        [Test]
        public void Validate_equals_operator_returns_false_for_both_null_inputs()
        {
            Position p1 = null;
            Position p2 = null;

            Assert.IsTrue(p1 == p2);
        }

        [Test]
        public void Validate_Equals_returns_false_for_invalid_input()
        {
            var stringPosition = "position";
            var position = new Position(1, 2, 3);

            Assert.IsFalse(position.Equals(stringPosition));
        }

        [Test]
        public void Validate_not_equals_operator_returns_false_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(2, 4, 6);

            Assert.IsTrue(p1 != p2);
        }

        [Test]
        public void Validate_get_distance()
        {
            var p1 = new Position(3, 4, 0);
            var p2 = new Position(0, 0, 0);
            // if we test on the same plane we can use Pythagorean theorem values
            Assert.AreEqual(5, Position.GetDistance(p1, p2));
            p1 = new Position(4, 7, 10);
            p2 = new Position(1, 3, 10);
            // set a value for z and verify values again
            Assert.AreEqual(5, Position.GetDistance(p1, p2));
            p1 = new Position(4, 5, 10);
            p2 = new Position(1, 1, 5);
            // set different values for z and verify values again
            Assert.AreEqual(7.071067, Position.GetDistance(p1, p2), 0.000001);
        }

        [Test]
        public void Validate_plus_operator_returns_correct_values()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(4, 5, 6);
            var result = p1 + p2;
            Assert.IsTrue(result.Equals(new Position(5, 7, 9)));
        }

        [Test]
        public void Validate_minus_operator_returns_correct_values()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(4, 5, 6);
            var result = p1 - p2;
            Assert.IsTrue(result.Equals(new Position(-3, -3, -3)));
        }

        [Test]
        public void Test_read_binary()
        {
            var position = new Position(1, 2, 3);
            using (var stream = new MemoryStream(new byte[24], true))
            {
                var binaryWriter = new BinaryWriter(stream);
                binaryWriter.Write(position.X);
                binaryWriter.Write(position.Y);
                binaryWriter.Write(position.Z);
                stream.Position = 0;
                var binaryReader = new BinaryReader(stream);
                var position2 = Position.ReadBinary(binaryReader);
                Assert.AreEqual(1, position2.X);
                Assert.AreEqual(2, position2.Y);
                Assert.AreEqual(3, position2.Z);
            }
        }

        [Test]
        public void Test_write_binary()
        {
            var position = new Position(2, 4, 6);
            using (var stream = new MemoryStream(new byte[24], true))
            {
                position.WriteBinary(new BinaryWriter(stream));
                Assert.IsInstanceOf<MemoryStream>(stream);
                stream.Position = 0;
                var binaryReader = new BinaryReader(stream);
                Assert.AreEqual(2, binaryReader.ReadDouble());
                Assert.AreEqual(4, binaryReader.ReadDouble());
                Assert.AreEqual(6, binaryReader.ReadDouble());
            }
        }

        [Test]
        public void Test_clone()
        {
            var position = new Position(1, 3, 5);
            var clonedPosition = position.Clone();
            Assert.AreEqual(position.X, clonedPosition.X);
            Assert.AreEqual(position.Y, clonedPosition.Y);
            Assert.AreEqual(position.Z, clonedPosition.Z);
        }

        [Test]
        public void Test_get_hash_code()
        {
            var position = new Position(1,2,3);
            var hashCode = position.GetHashCode();
            Assert.AreEqual(hashCode, position.GetHashCode());
            var position2 = new Position(1,2,3);
            Assert.AreEqual(hashCode, position2.GetHashCode());
        }
    }
}
