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
            Assert.That(Equals(data.LookupTablePhaseFunctionDataType, "Polar"), Is.True);
        }
        /// <summary>
        /// Test setting the member variables.
        /// </summary>
        [Test]
        public void validate_setting_member_vars()
        {
            var data = new PolarLookupTablePhaseFunctionData();
            data.LutAngles = new[] { 0.0, Math.PI/2, Math.PI };
            Assert.That(data.LutAngles[0], Is.EqualTo(0.0));
            Assert.That(data.LutAngles[1], Is.EqualTo(Math.PI/2));
            Assert.That(data.LutAngles[2], Is.EqualTo(Math.PI));
        }
    }
}
