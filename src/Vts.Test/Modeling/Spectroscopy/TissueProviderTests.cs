using System;
using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class TissueProviderTests
    {
        [Test]
        public void Test_tissue_provider_skin()
        {
            var skin = TissueProvider.CreateAbsorbers(TissueType.Skin);
            Assert.AreEqual("Hb", skin[0].Name);
            Assert.AreEqual(28.4, skin[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, skin[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_breast_pre_menopause()
        {
            var breast = TissueProvider.CreateAbsorbers(TissueType.BreastPreMenopause);
            Assert.AreEqual("Hb", breast[0].Name);
            Assert.AreEqual(6.9, breast[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, breast[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_breast_post_menopause()
        {
            var breast = TissueProvider.CreateAbsorbers(TissueType.BreastPostMenopause);
            Assert.AreEqual("Hb", breast[0].Name);
            Assert.AreEqual(3.75, breast[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, breast[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_brain_white_matter()
        {
            var brain = TissueProvider.CreateAbsorbers(TissueType.BrainWhiteMatter);
            Assert.AreEqual("Hb", brain[0].Name);
            Assert.AreEqual(24, brain[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, brain[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_brain_gray_matter()
        {
            var brain = TissueProvider.CreateAbsorbers(TissueType.BrainGrayMatter);
            Assert.AreEqual("Hb", brain[0].Name);
            Assert.AreEqual(24, brain[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, brain[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_liver()
        {
            var liver = TissueProvider.CreateAbsorbers(TissueType.Liver);
            Assert.AreEqual("Hb", liver[0].Name);
            Assert.AreEqual(66, liver[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, liver[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_intralipid_phantom()
        {
            var intalipid = TissueProvider.CreateAbsorbers(TissueType.IntralipidPhantom);
            Assert.AreEqual("Nigrosin", intalipid[0].Name);
            Assert.AreEqual(0.01, intalipid[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, intalipid[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_polystyrene_sphere_phantom()
        {
            var phantom = TissueProvider.CreateAbsorbers(TissueType.PolystyreneSpherePhantom);
            Assert.AreEqual("Nigrosin", phantom[0].Name);
            Assert.AreEqual(0.01, phantom[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, phantom[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_tissue_provider_custom()
        {
            var custom = TissueProvider.CreateAbsorbers(TissueType.Custom);
            Assert.AreEqual("Hb", custom[0].Name);
            Assert.AreEqual(20, custom[0].Concentration);
            Assert.AreEqual(ChromophoreCoefficientType.MolarAbsorptionCoefficient, custom[0].ChromophoreCoefficientType);
        }

        [Test]
        public void Test_create_scatterer_skin()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.Skin);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_liver()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.Liver);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_brain_white_matter()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.BrainWhiteMatter);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_brain_gray_matter()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.BrainGrayMatter);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_breast_pre_menopause()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.BreastPreMenopause);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_breast_post_menopause()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.BreastPostMenopause);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_custom()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.Custom);
            Assert.IsInstanceOf<PowerLawScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_intralipid_phantom()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.IntralipidPhantom);
            Assert.IsInstanceOf<IntralipidScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_polystyrene_sphere_phantom()
        {
            var scatterer = TissueProvider.CreateScatterer(TissueType.PolystyreneSpherePhantom);
            Assert.IsInstanceOf<MieScatterer>(scatterer);
        }

        [Test]
        public void Test_create_scatterer_out_of_range()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TissueProvider.CreateScatterer((TissueType)10));
        }
    }
}
