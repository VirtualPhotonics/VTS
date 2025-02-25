using NUnit.Framework;

namespace Vts.Test
{
    [TestFixture]
    public class OpticalPropertiesTests
    {
        [Test]
        public void Validate_constructor_with_array()
        {
            var opticalProperties = new OpticalProperties(new[] { 0.1, 0.001, 0.8, 1.4 });
            Assert.That(opticalProperties.Mua, Is.EqualTo(0.1));
            Assert.That(opticalProperties.Musp, Is.EqualTo(0.001));
            Assert.That(opticalProperties.G, Is.EqualTo(0.8));
            Assert.That(opticalProperties.N, Is.EqualTo(1.4));
            Assert.That(opticalProperties.Mus, Is.EqualTo(0.005).Within(0.00001));
        }

        [Test]
        public void Validate_constructor_assigns_correct_values()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Musp, Is.EqualTo(2.0));
            Assert.That(op1.G, Is.EqualTo(0.9));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);


            var op2 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.0, n: 1.4);

            Assert.That(op2.Mua, Is.EqualTo(0.01));
            Assert.That(op2.Musp, Is.EqualTo(2.0));
            Assert.That(op2.G, Is.EqualTo(0.0));
            Assert.That(op2.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op2.Mus - 2.0), 10E-6);
        }

        [Test]
        public void Validate_changing_g_modifies_mus_correctly_and_keeps_musp_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Musp, Is.EqualTo(2.0));
            Assert.That(op1.G, Is.EqualTo(0.9));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.G = 0.0;

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Musp, Is.EqualTo(2.0));
            Assert.That(op1.G, Is.EqualTo(0.0));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Mus - 2.0), 10E-6);
        }

        [Test]
        public void Validate_changing_mus_modifies_musp_correctly_and_keeps_g_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Musp, Is.EqualTo(2.0));
            Assert.That(op1.G, Is.EqualTo(0.9));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.Mus = 2.0;

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Mus, Is.EqualTo(2.0));
            Assert.That(op1.G, Is.EqualTo(0.9));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Musp - 0.2), 10E-6);
        }

        [Test]
        public void Validate_changing_musp_modifies_mus_correctly_and_keeps_g_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Musp, Is.EqualTo(2.0));
            Assert.That(op1.G, Is.EqualTo(0.9));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.Musp = 1.0;

            Assert.That(op1.Mua, Is.EqualTo(0.01));
            Assert.That(op1.Musp, Is.EqualTo(1.0));
            Assert.That(op1.G, Is.EqualTo(0.9));
            Assert.That(op1.N, Is.EqualTo(1.4));
            Assert.Less(System.Math.Abs(op1.Mus - 10.0), 10E-6);
        }

        [Test]
        public void Validate_to_string()
        {
            var opticalProperties = new OpticalProperties();
            Assert.That(opticalProperties.ToString(), Is.EqualTo("μa=0.01 μs'=1 g=0.8 n=1.4"));
        }
    }
}
