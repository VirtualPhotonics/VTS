using System;
using Meta.Numerics.Functions;
using NUnit.Framework;

namespace Vts.Test.Modeling
{
    [TestFixture]
    public class MetaNumericsZeroOrderBesselFunctionTests
    {
        [Test]
        public void ZeroOrderBesselFunction()
        {
            double minThreshold = 1e-5;
            double[] JzeroZeros = {2.40482,5.5201, 8.6537, 11.7915, 14.9309};

            for (int i = 0; i < JzeroZeros.Length; i++)
			{
			    Assert.That(Math.Abs(AdvancedMath.BesselJ(0,JzeroZeros[i])), Is.LessThan(minThreshold), $"Error occured at i = {i}");
			}   
        }
    }
}