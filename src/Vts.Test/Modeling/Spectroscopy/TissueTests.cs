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
            Assert.AreEqual(1.4, _tissue.N);
            Assert.AreEqual(66, _tissue.Absorbers[0].Concentration);
            Assert.AreEqual(124, _tissue.Absorbers[1].Concentration);
            Assert.AreEqual(0.02, _tissue.Absorbers[2].Concentration);
            Assert.AreEqual(0.87, _tissue.Absorbers[3].Concentration);
        }

        [Test]
        public void Test_tissue_constructor_tissue_type()
        {
            var tissue = new Tissue(TissueType.Liver);
            Assert.AreEqual(TissueType.Liver, tissue.TissueType);
            Assert.AreEqual("Liver", tissue.Name);
            Assert.AreEqual(1.4, tissue.N);
            Assert.AreEqual("Hb", tissue.Absorbers[0].Name);
            Assert.AreEqual(66, tissue.Absorbers[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, tissue.Absorbers[0].ChromophoreCoefficientType);
            Assert.IsInstanceOf<PowerLawScatterer>(tissue.Scatterer);
            Assert.AreEqual(ScatteringType.PowerLaw, tissue.ScattererType);
        }

        [Test]
        public void Test_get_mua()
        {
            var mua = _tissue.GetMua(1000);
            Assert.AreEqual(0.067854, mua, 0.000001);
        }

        [Test]
        public void Test_get_musp()
        {
            var musp = _tissue.GetMusp(1000);
            Assert.AreEqual(0.839999, musp, 0.000001);
        }

        [Test]
        public void Test_get_mus()
        {
            var mus = _tissue.GetMus(1000);
            Assert.AreEqual(4.2, mus, 0.000001);
        }

        [Test]
        public void Test_get_g()
        {
            var g = _tissue.GetG(1000);
            Assert.AreEqual(0.8, g);
        }

        [Test]
        public void Test_to_string()
        {
            Assert.AreEqual("test_tissue", _tissue.ToString());
        }

        [Test]
        public void Test_get_optical_properties()
        {
            var opticalProperties = _tissue.GetOpticalProperties(1000);
            Assert.AreEqual(1.4, opticalProperties.N);
            Assert.AreEqual(0.067854, opticalProperties.Mua, 0.000001);
            Assert.AreEqual(4.2, opticalProperties.Mus, 0.000001);
            Assert.AreEqual(0.84, opticalProperties.Musp, 0.000001);
            Assert.AreEqual(0.8, opticalProperties.G);
        }

        [Test]
        public void Test_get_optical_properties_wavelength_array()
        {
            var opticalPropertyArray = _tissue.GetOpticalProperties(new double[] {
                600, 700, 1000
            });
            Assert.AreEqual(1.4, opticalPropertyArray[0].N);
            Assert.AreEqual(0.314619, opticalPropertyArray[0].Mua, 0.000001);
            Assert.AreEqual(5.562449, opticalPropertyArray[0].Mus, 0.000001);
            Assert.AreEqual(1.112489, opticalPropertyArray[0].Musp, 0.000001);
            Assert.AreEqual(0.8, opticalPropertyArray[0].G);
            Assert.AreEqual(1.4, opticalPropertyArray[1].N);
            Assert.AreEqual(0.036097, opticalPropertyArray[1].Mua, 0.000001);
            Assert.AreEqual(5.110287, opticalPropertyArray[1].Mus, 0.000001);
            Assert.AreEqual(1.022057, opticalPropertyArray[1].Musp, 0.000001);
            Assert.AreEqual(0.8, opticalPropertyArray[1].G);
            Assert.AreEqual(1.4, opticalPropertyArray[2].N);
            Assert.AreEqual(0.067854, opticalPropertyArray[2].Mua, 0.000001);
            Assert.AreEqual(4.2, opticalPropertyArray[2].Mus, 0.000001);
            Assert.AreEqual(0.84, opticalPropertyArray[2].Musp, 0.000001);
            Assert.AreEqual(0.8, opticalPropertyArray[2].G);
        }
    }
}
