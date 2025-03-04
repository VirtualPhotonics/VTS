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
            Assert.That(parameterSweep.Name, Is.EqualTo("mua1"));
            Assert.That(parameterSweep.Range.Start, Is.EqualTo(0.01));
            Assert.That(parameterSweep.Range.Stop, Is.EqualTo(0.05));
            Assert.That(parameterSweep.Range.Delta, Is.EqualTo(0.01));
        }

        [Test]
        public void ParameterSweep_constructor_with_DoubleRange_test()
        {
            var parameterSweep = new ParameterSweep("mua1", new DoubleRange(0.0, 9.0, 10));
            Assert.That(parameterSweep.Name, Is.EqualTo("mua1"));
            Assert.That(parameterSweep.Range.Start, Is.EqualTo(0.0));
            Assert.That(parameterSweep.Range.Stop, Is.EqualTo(9.0));
            Assert.That(parameterSweep.Range.Delta, Is.EqualTo(1.0));
        }

        [Test]
        public void ParameterSweep_constructor_with_double_array_test()
        {
            var parameterSweep = new ParameterSweep("mua2", new[] { 0.1, 0.2, 0.3 });
            Assert.That(parameterSweep.Values[0], Is.EqualTo(0.1));
            Assert.That(parameterSweep.Name, Is.EqualTo("mua2"));
        }
    }
}
