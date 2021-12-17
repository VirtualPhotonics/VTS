using NUnit.Framework;
using System;
using Vts.MonteCarlo;

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
            Assert.IsTrue(Equals(data.LookupTablePhaseFunctionDataType, "Polar"));
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
