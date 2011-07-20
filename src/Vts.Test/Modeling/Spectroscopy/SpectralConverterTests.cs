using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class SpectralConverterTests
    {
        [Test]
        public void verify_convert_wavelength_nm_returns_self()
        {
            double wavelength = 500;
            Assert.AreEqual(wavelength, wavelength.ConvertWavelength(WavelengthUnit.Nanometers));
        }

        [Test]
        public void verify_convert_wavelength_um_to_nm()
        {
            double wavelength = 0.5;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.Micrometers));
        }

        [Test]
        public void verify_convert_wavelength_m_to_nm()
        {
            double wavelength = 0.0000005;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.Meters));
        }

        [Test]
        public void verify_convert_wavelength_inv_m_to_nm()
        {
            double wavelength = 2000000;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.InverseMeters));
        }

        [Test]
        public void verify_convert_wavelength_inv_cm_to_nm()
        {
            double wavelength = 20000;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.InverseCentimeters));
        }

        [Test]
        public void verify_convert_coefficient_inv_mm_returns_self()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters));
        }

        [Test]
        public void test_convert_coefficient_inv_m_to_inv_mm()
        {
            double coefficient = 1000;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters), 1);
        }

        [Test]
        public void test_convert_coefficient_inv_cm_to_inv_mm()
        {
            double coefficient = 10;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters), 1);
        }

        [Test]
        public void test_convert_coefficient_inv_um_to_inv_mm()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers), 1);
        }
        
        //Tests with micro molar coefficient applied
        [Test]
        public void verify_convert_coefficient_inv_mm_with_molar_uM_returns_self()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.MicroMolar));
        }

        [Test]
        public void test_convert_coefficient_inv_m_to_inv_mm_with_molar_uM()
        {
            double coefficient = 1000;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.MicroMolar), 1);
        }

        [Test]
        public void test_convert_coefficient_inv_cm_to_inv_mm_with_molar_uM()
        {
            double coefficient = 10;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.MicroMolar), 1);
        }

        [Test]
        public void test_convert_coefficient_inv_um_to_inv_mm_with_molar_uM()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.MicroMolar), 1);
        }

        //Tests with millimolar coefficient applied
        [Test]
        public void verify_convert_coefficient_inv_mm_with_molar_mM()
        {
            double coefficient = 1;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.MilliMolar), 0.001);
        }

        [Test]
        public void test_convert_coefficient_inv_m_to_inv_mm_with_molar_mM()
        {
            double coefficient = 1000;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.MilliMolar), 0.001);
        }

        [Test]
        public void test_convert_coefficient_inv_cm_to_inv_mm_with_molar_mM()
        {
            double coefficient = 10;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.MilliMolar), 0.001);
        }

        [Test]
        public void test_convert_coefficient_inv_um_to_inv_mm_with_molar_mM()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.MilliMolar), 0.001);
        }

        //Tests with molar coefficient applied
        [Test]
        public void verify_convert_coefficient_inv_mm_with_molar_M()
        {
            double coefficient = 1;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.Molar), 0.000001);
        }

        [Test]
        public void test_convert_coefficient_inv_m_to_inv_mm_with_molar_M()
        {
            double coefficient = 1000;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.Molar), 0.000001);
        }

        [Test]
        public void test_convert_coefficient_inv_cm_to_inv_mm_with_molar_M()
        {
            double coefficient = 10;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.Molar), 0.000001);
        }

        [Test]
        public void test_convert_coefficient_inv_um_to_inv_mm_with_molar_M()
        {
            double coefficient = 0.001;
            Assert.AreEqual(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.Molar), 0.000001);
        }
    }
}
