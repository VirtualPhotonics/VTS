using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Helpers
{
    [TestFixture]
    public class ErrorCalculationTests
    {
        /// <summary>
        /// Validate StandardDeviation provides correct results
        /// </summary>
        [Test]
        public void Validate_StandardDeviation_results()
        {
            var numberOfPhotons = 100;
            var mean = 0.5;
            var secondMoment = 0.25;
            var variance = ErrorCalculation.StandardDeviation(numberOfPhotons, mean, secondMoment);
            Assert.Less(Math.Abs(variance), 0.000001); // check 0 variance
            secondMoment = 0.125;
            variance = ErrorCalculation.StandardDeviation(numberOfPhotons, mean, secondMoment);
            Assert.IsNaN(variance); // check NaN variance
            secondMoment = 0.5;
            variance = ErrorCalculation.StandardDeviation(numberOfPhotons, mean, secondMoment);
            Assert.Less(Math.Abs(variance - 0.05), 0.000001); // check non-zero variance
        }
    }
}

