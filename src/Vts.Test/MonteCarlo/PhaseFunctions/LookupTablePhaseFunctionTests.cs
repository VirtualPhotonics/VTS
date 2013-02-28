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
        public void validate_ScatterToThetaAndPhi_modifies_direction_properly()
        {
            var d1 = new Direction(1, 0, 0);
            var rng = new Random();
            var lutData = new PolarLookupTablePhaseFunctionData();
            var phaseFunc = new LookupTablePhaseFunction(rng, lutData);
            phaseFunc.ScatterToThetaAndPhi(d1, Math.PI/2, 0);
            Assert.IsTrue(d1.Equals(new Direction(0,1,0)));
        }
    }
}
