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
        public void validate_RollFilter_returns_correct_values()
        {
            var input = new float[200].InitializeTo(1f);

            var result = Convolution2D.RollFilter(input, 10, 20, 2);

            Assert.IsTrue(result.Skip(2).Take(6).All(f => f == 0f));
            Assert.IsTrue(result.Skip(12).Take(6).All(f => f == 0f));

            Assert.IsTrue(result.Skip(22).Take(6).All(f => f == 1f));
            Assert.IsTrue(result.Skip(32).Take(6).All(f => f == 1f));
            // ...
            Assert.IsTrue(result.Skip(172).Take(6).All(f => f == 1f));

            Assert.IsTrue(result.Skip(182).Take(6).All(f => f == 0f));
            Assert.IsTrue(result.Skip(192).Take(6).All(f => f == 0f));
        }
    }
}
