using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class TissueTests
    {
        private Tissue _tissue;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            const double n = 1.4;

            _tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue",
                n);
        }

        [Test]
        public void Test_tissue_constructor()
        {
            Assert.That(_tissue.N, Is.EqualTo(1.4));
            Assert.That(_tissue.Absorbers[0].Concentration, Is.EqualTo(66));
            Assert.That(_tissue.Absorbers[1].Concentration, Is.EqualTo(124));
            Assert.That(_tissue.Absorbers[2].Concentration, Is.EqualTo(0.02));
            Assert.That(_tissue.Absorbers[3].Concentration, Is.EqualTo(0.87));
        }

        [Test]
        public void Test_tissue_constructor_tissue_type()
        {
            var tissue = new Tissue(TissueType.Liver);
            Assert.That(tissue.TissueType, Is.EqualTo(TissueType.Liver));
            Assert.That(tissue.Name, Is.EqualTo("Liver"));
            Assert.That(tissue.N, Is.EqualTo(1.4));
            Assert.That(tissue.Absorbers[0].Name, Is.EqualTo("Hb"));
            Assert.That(tissue.Absorbers[0].Concentration, Is.EqualTo(66));
            Assert.That(tissue.Absorbers[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
            Assert.IsInstanceOf<PowerLawScatterer>(tissue.Scatterer);
            Assert.That(tissue.ScattererType, Is.EqualTo(ScatteringType.PowerLaw));
        }

        [Test]
        public void Test_get_mua()
        {
            var mua = _tissue.GetMua(1000);
            Assert.That(mua, Is.EqualTo(0.067854).Within(0.000001));
        }

        [Test]
        public void Test_get_musp()
        {
            var musp = _tissue.GetMusp(1000);
            Assert.That(musp, Is.EqualTo(0.839999).Within(0.000001));
        }

        [Test]
        public void Test_get_mus()
        {
            var mus = _tissue.GetMus(1000);
            Assert.That(mus, Is.EqualTo(4.2).Within(0.000001));
        }

        [Test]
        public void Test_get_g()
        {
            var g = _tissue.GetG(1000);
            Assert.That(g, Is.EqualTo(0.8));
        }

        [Test]
        public void Test_to_string()
        {
            Assert.That(_tissue.ToString(), Is.EqualTo("test_tissue"));
        }

        [Test]
        public void Test_get_optical_properties()
        {
            var opticalProperties = _tissue.GetOpticalProperties(1000);
            Assert.That(opticalProperties.N, Is.EqualTo(1.4));
            Assert.That(opticalProperties.Mua, Is.EqualTo(0.067854).Within(0.000001));
            Assert.That(opticalProperties.Mus, Is.EqualTo(4.2).Within(0.000001));
            Assert.That(opticalProperties.Musp, Is.EqualTo(0.84).Within(0.000001));
            Assert.That(opticalProperties.G, Is.EqualTo(0.8));
        }

        [Test]
        public void Test_get_optical_properties_wavelength_array()
        {
            var opticalPropertyArray = _tissue.GetOpticalProperties(new double[] {
                600, 700, 1000
            });
            Assert.That(opticalPropertyArray[0].N, Is.EqualTo(1.4));
            Assert.That(opticalPropertyArray[0].Mua, Is.EqualTo(0.314619).Within(0.000001));
            Assert.That(opticalPropertyArray[0].Mus, Is.EqualTo(5.562449).Within(0.000001));
            Assert.That(opticalPropertyArray[0].Musp, Is.EqualTo(1.112489).Within(0.000001));
            Assert.That(opticalPropertyArray[0].G, Is.EqualTo(0.8));
            Assert.That(opticalPropertyArray[1].N, Is.EqualTo(1.4));
            Assert.That(opticalPropertyArray[1].Mua, Is.EqualTo(0.036097).Within(0.000001));
            Assert.That(opticalPropertyArray[1].Mus, Is.EqualTo(5.110287).Within(0.000001));
            Assert.That(opticalPropertyArray[1].Musp, Is.EqualTo(1.022057).Within(0.000001));
            Assert.That(opticalPropertyArray[1].G, Is.EqualTo(0.8));
            Assert.That(opticalPropertyArray[2].N, Is.EqualTo(1.4));
            Assert.That(opticalPropertyArray[2].Mua, Is.EqualTo(0.067854).Within(0.000001));
            Assert.That(opticalPropertyArray[2].Mus, Is.EqualTo(4.2).Within(0.000001));
            Assert.That(opticalPropertyArray[2].Musp, Is.EqualTo(0.84).Within(0.000001));
            Assert.That(opticalPropertyArray[2].G, Is.EqualTo(0.8));
        }
    }
}
