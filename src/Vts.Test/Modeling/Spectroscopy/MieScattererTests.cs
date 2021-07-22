using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class MieScattererTests
    {
        private MieScatterer _mieScatterer;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _mieScatterer = new MieScatterer();
        }

        [Test]
        public void Test_mie_scatterer_default_constructor()
        {
            var scatterer = new MieScatterer();
            Assert.IsInstanceOf<MieScatterer>(scatterer);
            Assert.AreEqual(0.5, scatterer.ParticleRadius);
            Assert.AreEqual(1.4, scatterer.ParticleRefractiveIndexMismatch);
            Assert.AreEqual(1.0, scatterer.MediumRefractiveIndexMismatch);
            Assert.AreEqual(0.01, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_mie_scatterer_constructor()
        {
            var scatterer = new MieScatterer(MieScattererType.Other);
            Assert.IsInstanceOf<MieScatterer>(scatterer);
            Assert.AreEqual(0.5, scatterer.ParticleRadius);
            Assert.AreEqual(1.4, scatterer.ParticleRefractiveIndexMismatch);
            Assert.AreEqual(1.0, scatterer.MediumRefractiveIndexMismatch);
            Assert.AreEqual(0.01, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_mie_scatterer_constructor_parameters()
        {
            var scatterer = new MieScatterer(0.25, 1.8, 0.2, 0.1);
            Assert.IsInstanceOf<MieScatterer>(scatterer);
            Assert.AreEqual(0.25, scatterer.ParticleRadius);
            Assert.AreEqual(1.8, scatterer.ParticleRefractiveIndexMismatch);
            Assert.AreEqual(0.2, scatterer.MediumRefractiveIndexMismatch);
            Assert.AreEqual(0.1, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_get_scatterer_type()
        {
            var scatterer = new MieScatterer();
            Assert.AreEqual(ScatteringType.Mie, scatterer.ScattererType);
        }

        [Test]
        public void Test_set_volume_fraction()
        {
            var scatterer = new MieScatterer {VolumeFraction = 1.1};
            Assert.AreEqual(1, scatterer.VolumeFraction);
            scatterer.VolumeFraction = -1;
            Assert.AreEqual(0, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_get_g()
        {
            var g = _mieScatterer.GetG(600);
            Assert.AreEqual(0.806818, g, 0.000001);
        }

        [Test]
        public void Test_get_mus()
        {
            var mus = _mieScatterer.GetMus(600);
            Assert.AreEqual(60.902469, mus, 0.000001);
        }

        [Test]
        public void Test_get_musp()
        {
            var musp = _mieScatterer.GetMusp(600);
            Assert.AreEqual(11.765246, musp, 0.000001);
        }

        [Test]
        public void verify_MieScatterer_parameters_set_correctly()
        {
            var mieScatterer = new MieScatterer(
                0.5, // particle radius in um
                1.1, // particle refractive index mismatch
                1.4, // medium refractive index
                0.1); // volume fraction
            Assert.AreEqual(mieScatterer.ParticleRadius, 0.5);
            Assert.AreEqual(mieScatterer.ParticleRefractiveIndexMismatch, 1.1);
            Assert.AreEqual(mieScatterer.MediumRefractiveIndexMismatch, 1.4);
        }

        [Test]
        public void verify_BohrenHuffmanMie_method_produces_correct_S11()
        {
            var mieScatterer = new MieScatterer(0.5, 1.1, 1.4, 0.1);
            mieScatterer.GetMus(720); // wavelength 720
            // to add: check on correct S11 setting
        }
    }
}
