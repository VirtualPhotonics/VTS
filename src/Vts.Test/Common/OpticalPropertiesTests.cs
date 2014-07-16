using NUnit.Framework;

namespace Vts.Test.Common
{
    [TestFixture]
    public class OpticalPropertiesTests
    {
        [Test]
        public void validate_constructor_assigns_correct_values()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Musp, 2.0);
            Assert.AreEqual(op1.G, 0.9);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Mus-20.0), 10E-6);


            var op2 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.0, n: 1.4);

            Assert.AreEqual(op2.Mua, 0.01);
            Assert.AreEqual(op2.Musp, 2.0);
            Assert.AreEqual(op2.G, 0.0);
            Assert.AreEqual(op2.N, 1.4);
            Assert.Less(System.Math.Abs(op2.Mus - 2.0), 10E-6);
        }
        
        [Test]
        public void validate_changing_g_modifies_mus_correctly_and_keeps_musp_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Musp, 2.0);
            Assert.AreEqual(op1.G, 0.9);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.G = 0.0;

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Musp, 2.0);
            Assert.AreEqual(op1.G, 0.0);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Mus - 2.0), 10E-6);
        }

        [Test]
        public void validate_changing_mus_modifies_musp_correctly_and_keeps_g_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Musp, 2.0);
            Assert.AreEqual(op1.G, 0.9);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.Mus = 2.0;

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Mus, 2.0);
            Assert.AreEqual(op1.G, 0.9);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Musp - 0.2), 10E-6);
        }

        [Test]
        public void validate_changing_musp_modifies_mus_correctly_and_keeps_g_the_same()
        {
            var op1 = new OpticalProperties(mua: 0.01, musp: 2.0, g: 0.9, n: 1.4);

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Musp, 2.0);
            Assert.AreEqual(op1.G, 0.9);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Mus - 20.0), 10E-6);

            op1.Musp = 1.0;

            Assert.AreEqual(op1.Mua, 0.01);
            Assert.AreEqual(op1.Musp, 1.0);
            Assert.AreEqual(op1.G, 0.9);
            Assert.AreEqual(op1.N, 1.4);
            Assert.Less(System.Math.Abs(op1.Mus - 10.0), 10E-6);
        }
    }
}
