using System;
using Meta.Numerics.Functions;
using NUnit.Framework;

namespace Vts.Test.Modeling
{
    [TestFixture]
    public class MetaNumericsZeroOrderBesselFunction
    {
        [Test]
        public void ZeroOrderBesselFunction()
        {
            double minThreshold = 1e-5;
            double[] JzeroZeros = {2.40482,5.5201, 8.6537, 11.7915, 14.9309};

            for (int i = 0; i < JzeroZeros.Length; i++)
			{
			    Assert.Less(Math.Abs(AdvancedMath.BesselJ(0,JzeroZeros[i])), minThreshold,
                    "Error occured at i = {0}",i);
			}   
        }
    }
}