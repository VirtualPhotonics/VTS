using NUnit.Framework;
using Meta.Numerics.Statistics;
using System;
using Vts.Common;
using Vts.Common.Math;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhaseFunctions;
using Vts.MonteCarlo.PhaseFunctionInputs;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class PolarLookupTablePhaseFunctionDataTests
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            var data = new PolarLookupTablePhaseFunctionData();
            Assert.IsTrue(data.Name.Equals("PolarLookuptablePhaseFunctionData"));
        }
        /// <summary>
        /// Test setting the member variables.
        /// </summary>
        [Test]
        public void validate_setting_member_vars()
        {
            var data = new PolarLookupTablePhaseFunctionData();
            data.LutAngles = new[] { 0.0, Math.PI/2, Math.PI };
            Assert.IsTrue(data.LutAngles.Equals(new[] {0.0, Math.PI/2, Math.PI});
        }
    }
}
