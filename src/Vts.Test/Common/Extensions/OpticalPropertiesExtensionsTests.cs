using NUnit.Framework;
using Vts.Extensions;

namespace Vts.Test.Common
{
    [TestFixture]
    public class OpticalPropertiesExtensionsTests 
    {
        [Test]
        public void validate_GetScatterLength_returns_correct_values()
        {
            var op = new OpticalProperties(0.1, 1.0, 0.9, 1.4); // note mus = 1.0/(1-0.9) = 10
            double result = op.GetScatterLength(AbsorptionWeightingType.Analog);
            Assert.Less(System.Math.Abs(result - 10.1), 1e-6);
            result = op.GetScatterLength(AbsorptionWeightingType.Discrete);
            Assert.Less(System.Math.Abs(result - 10.1), 1e-6);
            result = op.GetScatterLength(AbsorptionWeightingType.Continuous);
            Assert.Less(System.Math.Abs(result - 10.0), 1e-6);
        }

    }

}
