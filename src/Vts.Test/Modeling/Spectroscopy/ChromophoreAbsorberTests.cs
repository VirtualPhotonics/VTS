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
            Assert.AreEqual("vol. frac.", concentrationUnits);
        }

        [Test]
        public void Test_to_string()
        {
            var name = _chromophoreAbsorber.ToString();
            Assert.AreEqual("H2O", name);
        }
    }
}
