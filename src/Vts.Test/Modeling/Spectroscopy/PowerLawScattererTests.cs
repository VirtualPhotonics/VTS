using System;
using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class PowerLawScattererTests
    {
        [Test]
        public void Test_power_law_scatterer_constructor()
        {
            var scatterer = new PowerLawScatterer();
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
            Assert.AreEqual(1, scatterer.A);
            Assert.AreEqual(0.1, scatterer.B);
            Assert.AreEqual(0.0, scatterer.C);
            Assert.AreEqual(0.0, scatterer.D);
        }

        [Test]
        public void Test_set_tissue_type_undefined()
        {
            var scatterer = new PowerLawScatterer();
            Assert.Throws<ArgumentOutOfRangeException>(() => scatterer.SetTissueType((TissueType) 100));
        }
    }
}
