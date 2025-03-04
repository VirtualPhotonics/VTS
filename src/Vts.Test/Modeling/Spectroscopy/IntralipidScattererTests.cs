using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class IntralipidScattererTests
    {
        public IntralipidScatterer _IntralipidScatterer;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _IntralipidScatterer = new IntralipidScatterer();
        }

        [Test]
        public void Test_intralipid_scatterer_default_constructor()
        {
            var scatterer = new IntralipidScatterer();
            Assert.That(scatterer, Is.InstanceOf<IntralipidScatterer>());
            Assert.That(scatterer.VolumeFraction, Is.EqualTo(0.01));
        }

        [Test]
        public void Test_intralipid_scatterer_constructor()
        {
            var scatterer = new IntralipidScatterer(0.02);
            Assert.That(scatterer, Is.InstanceOf<IntralipidScatterer>());
            Assert.That(scatterer.VolumeFraction, Is.EqualTo(0.02));
        }

        [Test]
        public void Test_get_scatterer_type()
        {
            var scatterer = new IntralipidScatterer();
            Assert.That(scatterer.ScattererType, Is.EqualTo(ScatteringType.Intralipid));
        }

        [Test]
        public void Test_set_volume_fraction()
        {
            var scatterer = new IntralipidScatterer(1.1);
            Assert.That(scatterer.VolumeFraction, Is.EqualTo(1));
            scatterer.VolumeFraction = -1;
            Assert.That(scatterer.VolumeFraction, Is.EqualTo(0));
        }

        [Test]
        public void Test_get_g()
        {
            var g = _IntralipidScatterer.GetG(600);
            Assert.That(g, Is.EqualTo(0.752).Within(0.000001));
        }

        [Test]
        public void Test_get_mus()
        {
            var mus = _IntralipidScatterer.GetMus(600);
            Assert.That(mus, Is.EqualTo(5.460981).Within(0.000001));
        }

        [Test]
        public void Test_get_musp()
        {
            var musp = _IntralipidScatterer.GetMusp(600);
            Assert.That(musp, Is.EqualTo(1.354323).Within(0.000001));
        }
    }
}
