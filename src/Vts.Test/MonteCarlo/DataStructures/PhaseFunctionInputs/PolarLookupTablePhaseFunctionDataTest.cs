using NUnit.Framework;
using System;
using System.Linq;
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
        /// Tests to validate if last value of CDF is 1.
        /// </summary>
        [Test]
        public void validate_CDF()
        {
            var d1 = new Direction(1, 0, 0);
            var rng = new Random();
            var lutData = new PolarLookupTablePhaseFunctionData();
            Assert.IsTrue(lutData.LutCdf[lutData.LutCdf.Length - 1] == 1);
        }
        /// <summary>
        /// Tests to validate if integral of PDF is CDF.
        /// </summary>
        [Test]
        public void validate_PDF()
        {
            var lutData = new PolarLookupTablePhaseFunctionData();
            var testCdf = Integration.IntegrateTrapezoidRuleForTwoLists(lutData.LutAngles.ToList(), lutData.LutPdf.ToList(), 0, Math.PI);
        }
    }
}
