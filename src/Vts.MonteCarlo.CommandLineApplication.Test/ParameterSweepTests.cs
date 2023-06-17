using NUnit.Framework;
using Vts.Common;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    internal class ParameterSweepTests
    {
        [Test]
        public void ParameterSweep_default_constructor_test()
        {
            var parameterSweep = new ParameterSweep();
            Assert.AreEqual("mua1", parameterSweep.Name);
            Assert.AreEqual(0.01, parameterSweep.Range.Start);
            Assert.AreEqual(0.05, parameterSweep.Range.Stop);
            Assert.AreEqual(0.01, parameterSweep.Range.Delta);
        }

        [Test]
        public void ParameterSweep_constructor_with_DoubleRange_test()
        {
            var parameterSweep = new ParameterSweep("mua1", new DoubleRange(0.0, 9.0, 10));
            Assert.AreEqual("mua1", parameterSweep.Name);
            Assert.AreEqual(0.0, parameterSweep.Range.Start);
            Assert.AreEqual(9.0, parameterSweep.Range.Stop);
            Assert.AreEqual(1.0, parameterSweep.Range.Delta);
        }

        [Test]
        public void ParameterSweep_constructor_with_double_array_test()
        {
            var parameterSweep = new ParameterSweep("mua2", new [] { 0.1, 0.2, 0.3 });
            Assert.AreEqual(0.1, parameterSweep.Values[0]);
            Assert.AreEqual("mua2", parameterSweep.Name);
        }
    }
}
