using System.Threading;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class StopwatchTests
    {
        [Test]
        public void validate_StartNew_counts_correctly()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Thread.Sleep(100);
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 90);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 110);
        }
    }
}
