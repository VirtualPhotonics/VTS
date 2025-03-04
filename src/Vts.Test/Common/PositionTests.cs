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
            Assert.That(position.X, Is.EqualTo(0));
            Assert.That(position.Y, Is.EqualTo(0));
            Assert.That(position.Z, Is.EqualTo(0));
        }

        [Test]
        public void Validate_constructor_assigns_correct_values()
        {
            var p1 = new Position(1, 2, 3);

            Assert.That(p1.X, Is.EqualTo(1));
            Assert.That(p1.Y, Is.EqualTo(2));
            Assert.That(p1.Z, Is.EqualTo(3));
        }

        [Test]
        public void Validate_Equals_returns_true_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);

            Assert.That(p1.Equals(p2), Is.True);
        }

        [Test]
        public void Validate_equals_operator_returns_true_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(1, 2, 3);
            
            Assert.That(p1 == p2, Is.True);
        }

        [Test]
        public void Validate_equals_operator_returns_false_for_null_input()
        {
            var p1 = new Position(1, 2, 3);
            Position p2 = null;

            Assert.That(p1 == p2, Is.False);
        }

        [Test]
        public void Validate_Equals_returns_false_for_null_input()
        {
            var p1 = new Position(1, 2, 3);
            Position p2 = null;

            Assert.That(p1.Equals(p2), Is.False);
        }

        [Test]
        public void Validate_equals_operator_returns_false_for_both_null_inputs()
        {
            Position p1 = null;
            Position p2 = null;

            Assert.That(p1 == p2, Is.True);
        }

        [Test]
        public void Validate_Equals_returns_false_for_invalid_input()
        {
            var stringPosition = "position";
            var position = new Position(1, 2, 3);

            Assert.That(position.Equals(stringPosition), Is.False);
        }

        [Test]
        public void Validate_not_equals_operator_returns_false_for_same_inputs()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(2, 4, 6);

            Assert.That(p1 != p2, Is.True);
        }

        [Test]
        public void Validate_get_distance()
        {
            var p1 = new Position(3, 4, 0);
            var p2 = new Position(0, 0, 0);
            // if we test on the same plane we can use Pythagorean theorem values
            Assert.That(Position.GetDistance(p1, p2), Is.EqualTo(5));
            p1 = new Position(4, 7, 10);
            p2 = new Position(1, 3, 10);
            // set a value for z and verify values again
            Assert.That(Position.GetDistance(p1, p2), Is.EqualTo(5));
            p1 = new Position(4, 5, 10);
            p2 = new Position(1, 1, 5);
            // set different values for z and verify values again
            Assert.That(Position.GetDistance(p1, p2), Is.EqualTo(7.071067).Within(0.000001));
        }

        [Test]
        public void Validate_plus_operator_returns_correct_values()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(4, 5, 6);
            var result = p1 + p2;
            Assert.That(result.Equals(new Position(5, 7, 9)), Is.True);
        }

        [Test]
        public void Validate_minus_operator_returns_correct_values()
        {
            var p1 = new Position(1, 2, 3);
            var p2 = new Position(4, 5, 6);
            var result = p1 - p2;
            Assert.That(result.Equals(new Position(-3, -3, -3)), Is.True);
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
                Assert.That(position2.X, Is.EqualTo(1));
                Assert.That(position2.Y, Is.EqualTo(2));
                Assert.That(position2.Z, Is.EqualTo(3));
            }
        }

        [Test]
        public void Test_write_binary()
        {
            var position = new Position(2, 4, 6);
            using (var stream = new MemoryStream(new byte[24], true))
            {
                position.WriteBinary(new BinaryWriter(stream));
                Assert.That(stream, Is.InstanceOf<MemoryStream>());
                stream.Position = 0;
                var binaryReader = new BinaryReader(stream);
                Assert.That(binaryReader.ReadDouble(), Is.EqualTo(2));
                Assert.That(binaryReader.ReadDouble(), Is.EqualTo(4));
                Assert.That(binaryReader.ReadDouble(), Is.EqualTo(6));
            }
        }

        [Test]
        public void Test_clone()
        {
            var position = new Position(1, 3, 5);
            var clonedPosition = position.Clone();
            Assert.That(clonedPosition.X, Is.EqualTo(position.X));
            Assert.That(clonedPosition.Y, Is.EqualTo(position.Y));
            Assert.That(clonedPosition.Z, Is.EqualTo(position.Z));
        }

        [Test]
        public void Test_get_hash_code()
        {
            var position = new Position(1,2,3);
            var hashCode = position.GetHashCode();
            Assert.That(position.GetHashCode(), Is.EqualTo(hashCode));
            var position2 = new Position(1,2,3);
            Assert.That(position2.GetHashCode(), Is.EqualTo(hashCode));
        }
    }
}
