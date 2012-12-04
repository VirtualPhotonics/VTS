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
        //[Test]
        //public void validate_constructor_assigns_correct_values()
        //{
        //    var d1 = new Direction(1, 2, 3);

        //    Assert.IsTrue(
        //        d1.Ux == 1.0 &&
        //        d1.Uy == 2.0 &&
        //        d1.Uz == 3.0
        //   );
        //}

        /// <summary>
        /// Tests whether ScatterToNewDirection returns correct value
        /// </summary>
        [Test]
        public void validate_ScatterToSpecificDirection_returns_correct_value()
        {
            var d1 = new Direction(1, 0, 0);
            var rng = new Random();
            var lutData = new PolarLookupTablePhaseFunctionData();
            var phaseFunc = new LookupTablePhaseFunction(rng, lutData);
            phaseFunc.ScatterToSpecificDirection(d1, Math.PI/2, 0);
            Assert.IsTrue(d1.Equals(new Direction(0,1,0)));
        }
    }
}
