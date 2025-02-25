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
            Assert.That(skin[0].Name, Is.EqualTo("Hb"));
            Assert.That(skin[0].Concentration, Is.EqualTo(28.4));
            Assert.That(skin[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_breast_pre_menopause()
        {
            var breast = TissueProvider.CreateAbsorbers(TissueType.BreastPreMenopause);
            Assert.That(breast[0].Name, Is.EqualTo("Hb"));
            Assert.That(breast[0].Concentration, Is.EqualTo(6.9));
            Assert.That(breast[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_breast_post_menopause()
        {
            var breast = TissueProvider.CreateAbsorbers(TissueType.BreastPostMenopause);
            Assert.That(breast[0].Name, Is.EqualTo("Hb"));
            Assert.That(breast[0].Concentration, Is.EqualTo(3.75));
            Assert.That(breast[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_brain_white_matter()
        {
            var brain = TissueProvider.CreateAbsorbers(TissueType.BrainWhiteMatter);
            Assert.That(brain[0].Name, Is.EqualTo("Hb"));
            Assert.That(brain[0].Concentration, Is.EqualTo(24));
            Assert.That(brain[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_brain_gray_matter()
        {
            var brain = TissueProvider.CreateAbsorbers(TissueType.BrainGrayMatter);
            Assert.That(brain[0].Name, Is.EqualTo("Hb"));
            Assert.That(brain[0].Concentration, Is.EqualTo(24));
            Assert.That(brain[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_liver()
        {
            var liver = TissueProvider.CreateAbsorbers(TissueType.Liver);
            Assert.That(liver[0].Name, Is.EqualTo("Hb"));
            Assert.That(liver[0].Concentration, Is.EqualTo(66));
            Assert.That(liver[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_intralipid_phantom()
        {
            var intalipid = TissueProvider.CreateAbsorbers(TissueType.IntralipidPhantom);
            Assert.That(intalipid[0].Name, Is.EqualTo("Nigrosin"));
            Assert.That(intalipid[0].Concentration, Is.EqualTo(0.01));
            Assert.That(intalipid[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_polystyrene_sphere_phantom()
        {
            var phantom = TissueProvider.CreateAbsorbers(TissueType.PolystyreneSpherePhantom);
            Assert.That(phantom[0].Name, Is.EqualTo("Nigrosin"));
            Assert.That(phantom[0].Concentration, Is.EqualTo(0.01));
            Assert.That(phantom[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
        }

        [Test]
        public void Test_tissue_provider_custom()
        {
            var custom = TissueProvider.CreateAbsorbers(TissueType.Custom);
            Assert.That(custom[0].Name, Is.EqualTo("Hb"));
            Assert.That(custom[0].Concentration, Is.EqualTo(20));
            Assert.That(custom[0].ChromophoreCoefficientType, Is.EqualTo(ChromophoreCoefficientType.MolarAbsorptionCoefficient));
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
