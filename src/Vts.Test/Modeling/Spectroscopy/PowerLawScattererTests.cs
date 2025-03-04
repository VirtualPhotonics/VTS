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
            Assert.That(scatterer, Is.InstanceOf<PowerLawScatterer>());
            Assert.That(scatterer.A, Is.EqualTo(1));
            Assert.That(scatterer.B, Is.EqualTo(0.1));
            Assert.That(scatterer.C, Is.EqualTo(0.0));
            Assert.That(scatterer.D, Is.EqualTo(0.0));
        }

        [Test]
        public void Test_set_tissue_type_undefined()
        {
            var scatterer = new PowerLawScatterer();
            Assert.Throws<ArgumentOutOfRangeException>(() => scatterer.SetTissueType((TissueType) 100));
        }
    }
}
