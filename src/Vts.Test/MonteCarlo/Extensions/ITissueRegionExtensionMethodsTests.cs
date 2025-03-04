using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class ITissueRegionExtensionMethodsTests
    {
        /// <summary>
        /// Validate method IsAir
        /// </summary>
        [Test]
        public void Validate_IsAir_returns_correct_values()
        {
            // IsAir is true if mua=0 and mus<1e-10 so make musp=1e-12 with g=0.9
            var layer = new LayerTissueRegion(
                new DoubleRange(0, 1, 2),
                new OpticalProperties(0.0, 1e-12, 0.9, 1.0));
           Assert.That(layer.IsAir(), Is.True);
           // set mua != 0.0 to check if IsAir is false
           layer.RegionOP.Mua = 0.01;
           Assert.That(layer.IsAir(), Is.False);
           //// set glass OPs and check if IsAir is false
           //layer.RegionOP.Mua = 0;
           //layer.RegionOP.Musp = 1e-10;
           //layer.RegionOP.G = 1.0;
           //layer.RegionOP.N = 1.5;
           //Assert.That(layer.IsAir(), Is.False);
        }
    }
}

