using System;
using System.Linq;
using NUnit.Framework;
using Vts.Common.Math;

namespace Vts.Test.Common.Math
{
    [TestFixture]
    public class InterpolationTests
    {
        [Test]
        public void Validate_interp1_returns_correct_values()
        {
            var x = new double[] { -5, -4, -3, -2, -1, 1, 2, 3, 4, 5 };
            var y = new double[10];
            for (var i = 0; i < x.GetLength(0); i++)
            {
                y[i] = x[i] * x[i] - 2;
            }
            // check that xi below range of x returns y value at first x
            var result = Interpolation.interp1(x, y, -6);
            Assert.That(y[0], Is.EqualTo(result));
            // check that xi beyond range of x returns y value at last x
            result = Interpolation.interp1(x, y, 6);
            Assert.That(y[9], Is.EqualTo(result));
            // check that xi between two y's with same value gives same value
            result = Interpolation.interp1(x, y, 0);
            Assert.That(y[4], Is.EqualTo(result));
            // check that xi between two y's of different sign gives 0 crossing value
            result = Interpolation.interp1(x, y, -2.0 + 2.0 / 3.0);
            Assert.That(System.Math.Abs(result), Is.LessThan(1e-10));
        }

        [Test]
        public void Validate_interp1_multi_value_returns_correct_values()
        {
            var x = new double[] { -5, -4, -3, -2, -1, 1, 2, 3, 4, 5 };
            var y = new double[10];
            for (var i = 0; i < x.GetLength(0); i++)
            {
                y[i] = x[i] * x[i] - 2;
            }
            var xis = new double[] { -6, 6, 0 };

            var result = Interpolation.interp1(x, y, xis);
            var resultArray = result as double[] ?? result.ToArray();
            Assert.That( y[0], Is.EqualTo(resultArray[0]));
            Assert.That( y[9], Is.EqualTo(resultArray[1]));
            Assert.That( y[4], Is.EqualTo(resultArray[2]));
        }

        [Test]
        public void Validate_interp1_multi_dimensional_array_returns_correct_values()
        {
            var x = new double[] { 0, 2, 4, 5 };
            var y = new double[,] { { -4, -2, 2, 4 }, { -4, -2, 2, 4 }, { -6, -3, 3, 6} };
            
            var result = Interpolation.interp1(x, y, 0, 1, 2);
            Assert.That(y[2, 0], Is.EqualTo(result));
        }

        [Test]
        public void Validate_interp1_multi_dimensional_array_fixed_returns_correct_values()
        {
            var x = new double[] { 0, 2, 4, 5 };
            var y = new double[,] { { -4, -2, 2, 4 }, { -4, -2, 2, 4 }, { -6, -3, 3, 6 }, { 1, 2, 3, 4 } };

            var result = Interpolation.interp1(x, y, 0, 0, 0);
            Assert.That(y[0, 0], Is.EqualTo(result));
        }

        [Test]
        public void Validate_interp1_multi_value_multi_dimensional_array_returns_correct_values()
        {
            var x = new double[] { 0, 2, 4, 5 };
            var y = new double[,] { { -4, -2, 2, 4 }, { -4, -2, 2, 4 }, { -6, -3, 3, 6 }, { 1, 2, 3, 4 } };
            var xis = new double[] { 0, 2, 4 };

            var result = Interpolation.interp1(x, y, xis, 1, 2);
            var resultArray = result as double[] ?? result.ToArray();

            Assert.That(y[2, 0], Is.EqualTo(resultArray[0]));
            Assert.That(y[2, 1], Is.EqualTo(resultArray[1]));
            Assert.That(y[2, 2], Is.EqualTo(resultArray[2]));
        }

        [Test]
        public void Validate_interp1_float_returns_correct_values()
        {
            var x = new[] { -5.0F, -4.0F, -3.0F, -2.0F, -1.0F, 1.0F, 2.0F, 3.0F, 4.0F, 5.0F };
            var y = new float[10];
            for (var i = 0; i < x.GetLength(0); i++)
            {
                y[i] = x[i] * x[i] - 2;
            }

            // check that xi below range of x returns Nan
            var result = Interpolation.interp1(x, y, -6F);
            Assert.That(result, Is.EqualTo(float.NaN));
            // check that xi beyond range of x returns NaN
            result = Interpolation.interp1(x, y, 6F);
            Assert.That(result, Is.EqualTo(float.NaN));
            // check that xi between two y's with same value gives same value
            result = Interpolation.interp1(x, y, 0F);
            Assert.That(y[4], Is.EqualTo(result));
        }

        [Test]
        public void Validate_interp1_multi_value_float_returns_correct_values()
        {
            var x = new[] { -5.0F, -4.0F, -3.0F, -2.0F, -1.0F, 1.0F, 2.0F, 3.0F, 4.0F, 5.0F };
            var y = new float[10];
            for (var i = 0; i < x.GetLength(0); i++)
            {
                y[i] = x[i] * x[i] - 2;
            }

            var xis = new[] { -6F, 6F, 0F };

            var result = Interpolation.interp1(x, y, xis);
            var resultArray = result as float[] ?? result.ToArray();
            Assert.That(resultArray[0], Is.EqualTo(float.NaN));
            Assert.That(resultArray[1], Is.EqualTo(float.NaN));
            Assert.That(resultArray[2], Is.EqualTo(y[4]));
        }

        [Test]
        public void Validate_interp1_float_multi_dimensional_array_returns_correct_values()
        {
            var x = new[] { 0F, 2F, 4F, 5F };
            var y = new[,] { { -4F, -2F, 2F, 4F }, { -4F, -2F, 2F, 4F }, { -6F, -3F, 3F, 6F } };

            var result = Interpolation.interp1(x, y, 0, 1, 2);
            Assert.That(y[2, 0], Is.EqualTo(result));
        }

        [Test]
        public void Validate_interp1_float_multi_dimensional_array_fixed_returns_correct_values()
        {
            var x = new[] { 0F, 2F, 4F, 5F };
            var y = new[,] { { -4F, -2F, 2F, 4F }, { -4F, -2F, 2F, 4F }, { -6F, -3F, 3F, 6F }, { 1F, 2F, 3F, 4F } };

            var result = Interpolation.interp1(x, y, 0, 0, 0);
            Assert.That(y[0, 0], Is.EqualTo(result));
        }

        [Test]
        public void Validate_interp1_float_multi_value_multi_dimensional_array_returns_correct_values()
        {
            var x = new[] { 0F, 2F, 4F, 5F };
            var y = new[,] { { -4F, -2F, 2F, 4F }, { -4F, -2F, 2F, 4F }, { -6F, -3F, 3F, 6F } };
            var xis = new[] { 0F, 2F, 4F };
            
            var result = Interpolation.interp1(x, y, xis, 1, 2);
            var resultArray = result as float[] ?? result.ToArray();

            Assert.That(y[2, 0], Is.EqualTo(resultArray[0]));
            Assert.That(y[2, 1], Is.EqualTo(resultArray[1]));
            Assert.That(y[2, 2], Is.EqualTo(resultArray[2]));
        }

        [Test]
        public void Test_interp1_throws_argument_exception()
        {
            var x = new double[] { -5, -4, -3, -2, -1, 1, 2, 3, 4, 5 };
            var y = new double[] { -6, -5, -4, -3, -2, -1, 1, 2, 3, 4, 5, 6 };
            // check that x and y of different lengths throws an exception
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    Interpolation.interp1(x, y, -6);
                }
                catch (Exception e)
                {
                    Assert.That(e.Message, Is.EqualTo("Error in interp1: arrays x and y are not the same size!"));
                    throw;
                }
            });
        }

        [Test]
        public void Test_interp2_returns_correct_values()
        {
            var x = new double[] { 1, 2, 3, 4 };
            var y = new double[4];
            for (var i = 0; i < x.GetLength(0); i++)
            {
                y[i] = x[i] * 2;
            }
            var f = new double[,] { { 1, 2, 3, 4 }, { 1, 2, 3, 4 }, { 1, 2, 3, 4 }, { 1, 2, 3, 4 } };
            
            var result = Interpolation.interp2(x, y, f, 6, 11);
            Assert.That(result, Is.EqualTo(4));
            result = Interpolation.interp2(x, y, f, 0, 2);
            Assert.That(result, Is.EqualTo(2));
            result = Interpolation.interp2(x, y, f, 2, 11);
            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public void Test_interp2_throws_argument_exception()
        {
            var x = new double[] { -5, -4, -3, -2, -1, 1, 2, 3, 4, 5 };
            var y = new double[] { -6, -5, -4, -3, -2, -1, 1, 2, 3, 4, 5, 6 };
            var f = new double[,] { { -6, 8 }, { 3, 6 } };
            // check that x and y of different lengths throws an exception
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    Interpolation.interp2(x, y, f, 4, 5);
                }
                catch (Exception e)
                {
                    Assert.That(e.Message, Is.EqualTo("Error in interp2: arrays x, y and f dimensions do not agree!"));
                    throw;
                }
            });
        }
    }
}
