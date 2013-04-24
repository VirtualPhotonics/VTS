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
    /// <summary>
    /// Test the class PolarLookupTablePhaseFunctionData.
    /// </summary>
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
            Assert.IsTrue(Equals(data.Name, "PolarLookupTablePhaseFunctionData"));
        }
        /// <summary>
        /// Test setting the member variables.
        /// </summary>
        [Test]
        public void validate_setting_member_vars()
        {
            var data = new PolarLookupTablePhaseFunctionData();
            data.LutAngles = new[] { 0.0, Math.PI/2, Math.PI };
            Assert.AreEqual(data.LutAngles[0], 0.0);
            Assert.AreEqual(data.LutAngles[1], Math.PI/2);
            Assert.AreEqual(data.LutAngles[2], Math.PI);
        }
    }
}
