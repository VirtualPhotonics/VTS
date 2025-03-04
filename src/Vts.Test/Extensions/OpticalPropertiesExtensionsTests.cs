using NUnit.Framework;
using Vts.Extensions;

namespace Vts.Test.Extensions
{
    [TestFixture]
    public class OpticalPropertiesExtensionsTests 
    {
        [Test]
        public void validate_GetScatterLength_returns_correct_values()
        {
            var op = new OpticalProperties(0.1, 1.0, 0.9, 1.4); // note mus = 1.0/(1-0.9) = 10
            double result = op.GetScatterLength(AbsorptionWeightingType.Analog);
            Assert.That(System.Math.Abs(result - 10.1), Is.LessThan(1e-6));
            result = op.GetScatterLength(AbsorptionWeightingType.Discrete);
            Assert.That(System.Math.Abs(result - 10.1), Is.LessThan(1e-6));
            result = op.GetScatterLength(AbsorptionWeightingType.Continuous);
            Assert.That(System.Math.Abs(result - 10.0), Is.LessThan(1e-6));
        }

    }

}
