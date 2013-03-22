using NUnit.Framework;
using System;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhaseFunctions;
using Vts.MonteCarlo.PhaseFunctionInputs;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class LookupTablePhaseFunctionTests
    {
        /// <summary>
        /// Tests whether ScatterToNewDirection returns correct value
        /// </summary>
        [Test]
        public void validate_ScattersToNewDirection_samples_properly()
        {
            //want to do a KS test to see if sample comes from pdf.
            var d1 = new Direction(1, 0, 0);
            var rng = new Random();
            var lutData = new PolarLookupTablePhaseFunctionData();
            var phaseFunc = new LookupTablePhaseFunction(rng, lutData);
            phaseFunc.ScatterToNewTheta(d1);
            Assert.IsTrue(d1.Equals(new Direction(0,1,0)));
        }
        public void validate_Scatter()
        {
            var d1 = new Direction(1, 0, 0);
            var rng = new Random();
            var lutData = new PolarLookupTablePhaseFunctionData();
            var phaseFunc = new LookupTablePhaseFunction(rng, lutData);
            phaseFunc.Scatter(d1, Math.PI / 6, Math.PI);
            Assert.AreEqual(d1.Ux, -Math.Sqrt(3)/2);
            Assert.AreEqual(d1.Uy, 0.0);
            Assert.AreEqual(d1.Uz, 0.5);
        }
    }
}