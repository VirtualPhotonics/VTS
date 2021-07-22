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
        public void validate_IsAir_returns_correct_values()
        {
            // IsAir is true if mua=0 and mus<=1e-10 so make musp=1e-12 with g=0.9
            var layer = new LayerTissueRegion(
                new DoubleRange(0, 1, 2),
                new OpticalProperties(0.0, 1e-12, 0.9, 1.4));
           Assert.IsTrue(layer.IsAir());
           // set mua != 0.0 to check if IsAir is false
           layer.RegionOP.Mua = 0.01;
           Assert.IsFalse(layer.IsAir());
        }
    }
}

