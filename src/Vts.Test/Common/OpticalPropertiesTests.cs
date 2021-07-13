using NUnit.Framework;

namespace Vts.Test.Common
{
    [TestFixture]
    public class OpticalPropertiesTests
    {
        [Test]
        public void Validate_constructor_with_array()
        {
            var opticalProperties = new OpticalProperties(new[] {0.1, 0.001, 0.8, 1.4});
            Assert.AreEqual(0.1, opticalProperties.Mua);
            Assert.AreEqual(0.001, opticalProperties.Musp);
            Assert.AreEqual(0.8, opticalProperties.G);
            Assert.AreEqual(1.4, opticalProperties.N);
            Assert.AreEqual(0.005, opticalProperties.Mus, 0.00001);
        }

        [Test]
        public void Validate_constructor_assigns_correct_values()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(2.0, op1.Musp);
            Assert.AreEqual(0.9, op1.G );
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Mus-20.0), 10E-6);


            var op2 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.0, n: 1.4);

            Assert.AreEqual(0.01, op2.Mua);
            Assert.AreEqual(2.0, op2.Musp);
            Assert.AreEqual(0.0, op2.G);
            Assert.AreEqual(1.4, op2.N);
            Assert.Less(System.Math.Abs(op2.Mus - 2.0), 10E-6);
        }
        
        [Test]
        public void Validate_changing_g_modifies_mus_correctly_and_keeps_musp_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(2.0, op1.Musp);
            Assert.AreEqual(0.9, op1.G);
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.G = 0.0;

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(2.0, op1.Musp);
            Assert.AreEqual(0.0, op1.G);
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Mus - 2.0), 10E-6);
        }

        [Test]
        public void Validate_changing_mus_modifies_musp_correctly_and_keeps_g_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(2.0, op1.Musp);
            Assert.AreEqual(0.9, op1.G);
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.Mus = 2.0;

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(2.0, op1.Mus);
            Assert.AreEqual(0.9, op1.G);
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Musp - 0.2), 10E-6);
        }

        [Test]
        public void Validate_changing_musp_modifies_mus_correctly_and_keeps_g_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(2.0, op1.Musp);
            Assert.AreEqual(0.9, op1.G);
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.Musp = 1.0;

            Assert.AreEqual(0.01, op1.Mua);
            Assert.AreEqual(1.0, op1.Musp);
            Assert.AreEqual(0.9, op1.G);
            Assert.AreEqual(1.4, op1.N);
            Assert.Less(System.Math.Abs(op1.Mus - 10.0), 10E-6);
        }

        [Test]
        public void Validate_to_string()
        {
            var opticalProperties = new OpticalProperties();
            Assert.AreEqual("μa=0.01 μs'=1 g=0.8 n=1.4", opticalProperties.ToString());
        }
    }
}
