using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class MieScattererTests
    {
        [Test]
        public void verify_MieScatterer_parameters_set_correctly()
        {
            var mieScatterer = new MieScatterer(
                0.5, // particle radius in um
                1.1, // particle refractive index mismatch
                1.4); // medium refractive index
            Assert.AreEqual(mieScatterer.ParticleRadius, 0.5);
            Assert.AreEqual(mieScatterer.ParticleRefractiveIndexMismatch, 1.1);
            Assert.AreEqual(mieScatterer.MediumRefractiveIndexMismatch, 1.4);
        }

        [Test]
        public void verify_BohrenHuffmanMie_method_produces_correct_S11()
        {
            var mieScatterer = new MieScatterer(0.5, 1.1, 1.4);
            mieScatterer.GetMus(720); // wavelength 720
            // to add: check on correct S11 setting
        }

    }
}
