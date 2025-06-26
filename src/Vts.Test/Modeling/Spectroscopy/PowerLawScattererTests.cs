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

        [Test]
        public void Verify_user_ability_to_specify_lambda0()
        {
            // set up call to GetMusp without lambda0 specified
            var scatterer = new PowerLawScatterer(1.0, 2.0, 3.0, 4.0);
            var musp = scatterer.GetMusp(1000);
            Assert.That(musp, Is.EqualTo(4.0));
            // set up call to GetMusp with lambda0 = 1000 specified
            musp = scatterer.GetMusp(1000, 1000);
            Assert.That(musp, Is.EqualTo(4.0));
            // set up call to GetMusp with another lambda0
            musp = scatterer.GetMusp(1000, 800);
            Assert.That(musp, Is.Not.EqualTo(4.0));

        }
    }
}
