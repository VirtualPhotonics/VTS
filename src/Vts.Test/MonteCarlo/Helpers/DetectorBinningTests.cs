using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Helpers
{
    [TestFixture]
    public class DetectorBinningTests
    {
        /// <summary>
        /// Validate WhichBin provides correct results.  If value before Start or beyond Stop,
        /// bin is first or last respectively
        /// </summary>
        [Test]
        public void validate_WhichBin_results()
        {
            var range = new DoubleRange(0, 10, 11);
            // test overload WhichBin(double value, int numberOfBins, double binSize, double binStart)
            var bin = DetectorBinning.WhichBin(5.5, range.Count - 1, range.Delta, range.Start);
            Assert.IsTrue(bin == 5);
            // test value beyond Stop
            bin = DetectorBinning.WhichBin(10.5, range.Count, range.Delta, range.Start);
            Assert.IsTrue(bin == 10);
            // test overload WhichBin(double value, double binSize, double[] binCenters)
            var binCenters = new double[] {0.5, 1.5, 2.5, 3.5, 4.5, 5.5, 6.5, 7.5, 8.5, 9.5};
            bin = DetectorBinning.WhichBin(5.5, range.Delta, binCenters);
            Assert.IsTrue(bin == 5);
            // test value beyond Stop -> this overload does not put in last bin
            bin = DetectorBinning.WhichBin(10.5, range.Delta, binCenters);
            Assert.IsTrue(bin == -1);
            // test overload WhichBin(double value, double[] binStops)
            var binStops = new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            bin = DetectorBinning.WhichBin(5.5, binStops);
            Assert.IsTrue(bin == 5);
            // test value beyond Stop -> this overload does not put in last bin
            bin = DetectorBinning.WhichBin(10.5, binStops);
            Assert.IsTrue(bin == -1);
        }

        /// <summary>
        /// Validate WhichBinExclusive results.  With this method, if value is beyond Start
        /// of Stop, no bin is selected.
        /// </summary>
        [Test]
        public void validate_WhichBinExclusive_results()
        {
            var range = new DoubleRange(0, 10, 11);
            var bin = DetectorBinning.WhichBinExclusive(5.5, range.Count - 1, range.Delta, range.Start);
            Assert.IsTrue(bin == 5);
            // test value beyond Stop
            bin = DetectorBinning.WhichBinExclusive(10.5, range.Count - 1, range.Delta, range.Start);
            Assert.IsTrue(bin == -1);
        }

        /// <summary>
        /// Validate GetTimeDelay results.
        /// </summary>
        [Test]
        public void validate_GetTimeDelay_results()
        {
            var pathLength = 10; // mm
            var n = 1.4; // refractive index mismatch
            var timeDelay = DetectorBinning.GetTimeDelay(pathLength, n);
            Assert.IsTrue(Math.Abs(timeDelay - 0.046698) < 0.000001);
        }
        /// <summary>
        /// Validate GetRho results.
        /// </summary>
        [Test]
        public void validate_GetRho_results()
        {
            var x = 5;
            var y = 5;
            var rho = DetectorBinning.GetRho(x, y);
            Assert.IsTrue(Math.Abs(rho - 7.07106) < 0.00001);
            y = 0;
            rho = DetectorBinning.GetRho(x, y);
            Assert.IsTrue(Math.Abs(rho - 5.0) < 0.000001);
        }
    }
}

