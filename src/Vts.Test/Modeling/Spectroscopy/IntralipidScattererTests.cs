using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.IsInstanceOf<IntralipidScatterer>(scatterer);
            Assert.AreEqual(0.01, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_intralipid_scatterer_constructor()
        {
            var scatterer = new IntralipidScatterer(0.02);
            Assert.IsInstanceOf<IntralipidScatterer>(scatterer);
            Assert.AreEqual(0.02, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_get_scatterer_type()
        {
            var scatterer = new IntralipidScatterer();
            Assert.AreEqual(ScatteringType.Intralipid, scatterer.ScattererType);
        }

        [Test]
        public void Test_set_volume_fraction()
        {
            var scatterer = new IntralipidScatterer(1.1);
            Assert.AreEqual(1, scatterer.VolumeFraction);
            scatterer.VolumeFraction = -1;
            Assert.AreEqual(0, scatterer.VolumeFraction);
        }

        [Test]
        public void Test_get_g()
        {
            var g = _IntralipidScatterer.GetG(600);
            Assert.AreEqual(0.752, g, 0.000001);
        }

        [Test]
        public void Test_get_mus()
        {
            var mus = _IntralipidScatterer.GetMus(600);
            Assert.AreEqual(5.460981, mus, 0.000001);
        }

        [Test]
        public void Test_get_musp()
        {
            var musp = _IntralipidScatterer.GetMusp(600);
            Assert.AreEqual(1.354323, musp, 0.000001);
        }
    }
}
