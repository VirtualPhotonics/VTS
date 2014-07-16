using NUnit.Framework;
using Vts.Common.Math;

namespace Vts.Test.Common.Math
{
    [TestFixture]
    public class InterpolationTests
    {
        [Test]
        public void validate_interp1_returns_correct_values()
        {
            var x = new double[10] { -5, -4, -3, -2, -1, 1, 2, 3, 4, 5};
            var y = new double[10];
            double result;
            for (int i = 0; i < x.GetLength(0); i++)
            {
                y[i] = x[i] * x[i] - 2;
            }
            // check that xi below range of x returns y value at first x
            result = Interpolation.interp1(x, y, -6);
            Assert.AreEqual(result, y[0]);
            // check that xi beyond range of x returns y value at last x
            result = Interpolation.interp1(x, y, 6);
            Assert.AreEqual(result, y[9]);
            // check that xi between two y's with same value gives same value
            result = Interpolation.interp1(x, y, 0);
            Assert.AreEqual(result, y[4]);
            // check that xi between two y's of different sign gives 0 crossing value
            result = Interpolation.interp1(x, y, -2.0 + 2.0/3.0);
            Assert.Less(System.Math.Abs(result), 1e-10);
        }
    }
}
