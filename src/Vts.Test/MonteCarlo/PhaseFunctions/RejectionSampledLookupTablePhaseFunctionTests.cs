/*using NUnit.Framework;
using System;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    [TestFixture]
    public class RejectionSasmpledLookuptablePhaseFunctionTests
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
            phaseFunc.ScatterToNewDirection(d1);
            Assert.IsTrue(d1.Equals(new Direction(0, 1, 0)));
        }
    }
}
*/