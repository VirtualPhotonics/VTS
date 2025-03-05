﻿using NUnit.Framework;
using System;
using Vts.MonteCarlo;

namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    [TestFixture]
    public class LookupTablePhaseFunctionInputTests
    {
        /// <summary>
        /// Test to see if default constructor works properly.
        /// </summary>
        [Test]
        public void validate_LookupTablePhaseFunctionInput_default_constructor()
        {
            var data = new LookupTablePhaseFunctionInput();
            PolarLookupTablePhaseFunctionData temp = new PolarLookupTablePhaseFunctionData();
            temp.LutAngles = new[] {0, Math.PI};
            temp.LutPdf  = new[] {1.0, 0.0};
            temp.LutCdf = new[] {1.0, 0.0};
            data.RegionPhaseFunctionData.Equals(temp);
        }
        /// <summary>
        /// Test to see if constructor works.
        /// </summary>
        [Test]
        public void validate_LookuptablePhaseFunctionInput_constructor()
        {
            var data = new PolarLookupTablePhaseFunctionData();
            var input = new LookupTablePhaseFunctionInput(data);
            Assert.That(input.PhaseFunctionType.Equals("LookupTable"), Is.True);
            Assert.That(input.RegionPhaseFunctionData.Equals(data), Is.True);
        }
    }
}