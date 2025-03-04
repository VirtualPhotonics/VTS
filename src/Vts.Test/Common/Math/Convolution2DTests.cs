using System.Linq;
using NUnit.Framework;
using Vts.Common.Math;
using Vts.Extensions;

namespace Vts.Test.Common.Math
{
    [TestFixture]
    public class Convolution2DTests
    {
        [Test]
        public void Validate_RollFilter_returns_correct_values()
        {
            var input = new float[200].InitializeTo(1f);

            var result = Convolution2D.RollFilter(input, 10, 20, 2);

            Assert.That(result.Skip(2).Take(6).All(f => f == 0f), Is.True);
            Assert.That(result.Skip(12).Take(6).All(f => f == 0f), Is.True);

            Assert.That(result.Skip(22).Take(6).All(f => f == 1f), Is.True);
            Assert.That(result.Skip(32).Take(6).All(f => f == 1f), Is.True);
            // ...
            Assert.That(result.Skip(172).Take(6).All(f => f == 1f), Is.True);

            Assert.That(result.Skip(182).Take(6).All(f => f == 0f), Is.True);
            Assert.That(result.Skip(192).Take(6).All(f => f == 0f), Is.True);
        }

        [Ignore("Need to get a raw speckle image as an int array")]
        [Test]
        public void Validate_LsiRoll_populates_values_correctly()
        {
            var raw = new int[200];
            var speckleContrast = new float[2];
            var speckleFlowIndex = new float[2];
            var rollRow = new int[2];
            var rollColumn = new int[2];
            var rollRowSquared = new int[2];
            var rollColumnSquared = new int[2];

            Convolution2D.LsiRoll(raw, speckleContrast, speckleFlowIndex, rollRow, rollColumn, rollRowSquared, rollColumnSquared, 10, 20, 2, 5);
            Assert.That(rollRow[0], Is.EqualTo(0));
        }
    }
}
