using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class SpectralConverterTest
    {
        [Test]
        public void verify_convert_wavelength_nm_returns_self()
        {
            double wavelength = 400;
            Assert.AreEqual(wavelength, wavelength.ConvertWavelength(Wavelength_Unit.Nanometers));
        }

        [Test]
        public void verify_convert_coefficient_inv_mm_returns_self()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient, coefficient.ConvertCoefficient(Coefficient_Unit.InverseMillimeters, Molar_Unit.MicroMolar));
        }
    }
}
