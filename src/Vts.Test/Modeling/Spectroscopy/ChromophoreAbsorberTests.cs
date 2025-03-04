using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class ChromophoreAbsorberTests
    {
        private ChromophoreAbsorber _chromophoreAbsorber;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _chromophoreAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.1);
        }

        [Test]
        public void Test_concentration_units()
        {
            var concentrationUnits = _chromophoreAbsorber.ConcentrationUnits;
            Assert.That(concentrationUnits, Is.EqualTo("vol. frac."));
        }

        [Test]
        public void Test_to_string()
        {
            var name = _chromophoreAbsorber.ToString();
            Assert.That(name, Is.EqualTo("H2O"));
        }
    }
}
