using System;
using NUnit.Framework;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class SpectralConverterTests
    {
        [Test]
        public void Test_get_wavelength_unit_nm()
        {
            var unit = SpectralConverter.getWavelengthUnit("nm");
            Assert.AreEqual(WavelengthUnit.Nanometers, unit);
        }

        [Test]
        public void Test_get_wavelength_unit_um()
        {
            var unit = SpectralConverter.getWavelengthUnit("um");
            Assert.AreEqual(WavelengthUnit.Micrometers, unit);
        }

        [Test]
        public void Test_get_wavelength_unit_m()
        {
            var unit = SpectralConverter.getWavelengthUnit("m");
            Assert.AreEqual(WavelengthUnit.Meters, unit);
        }

        [Test]
        public void Test_get_wavelength_unit_inv_m()
        {
            var unit = SpectralConverter.getWavelengthUnit("1/m");
            Assert.AreEqual(WavelengthUnit.InverseMeters, unit);
        }

        [Test]
        public void Test_get_wavelength_unit_inv_cm()
        {
            var unit = SpectralConverter.getWavelengthUnit("1/cm");
            Assert.AreEqual(WavelengthUnit.InverseCentimeters, unit);
        }

        [Test]
        public void Test_get_wavelength_unit_invalid()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getWavelengthUnit("yu"));
        }

        [Test]
        public void Test_get_wavelength_unit_undefined()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getWavelengthUnit(""));
        }
        [Test]
        public void Test_get_wavelength_unit_nanometers()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.Nanometers);
            Assert.AreEqual("nm", unit);
        }

        [Test]
        public void Test_get_wavelength_unit_micrometers()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.Micrometers);
            Assert.AreEqual("um", unit);
        }

        [Test]
        public void Test_get_wavelength_unit_meters()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.Meters);
            Assert.AreEqual("m", unit);
        }

        [Test]
        public void Test_get_wavelength_unit_inv_meters()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.InverseMeters);
            Assert.AreEqual("1/m", unit);
        }

        [Test]
        public void Test_get_wavelength_unit_inv_centimeters()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.InverseCentimeters);
            Assert.AreEqual("1/cm", unit);
        }

        [Test]
        public void Test_get_wavelength_unit_invalid_enum()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getWavelengthUnit((WavelengthUnit)100));
        }

        [Test]
        public void Test_convert_wavelength_nm_returns_self()
        {
            const double wavelength = 500;
            Assert.AreEqual(wavelength, wavelength.ConvertWavelength(WavelengthUnit.Nanometers));
        }

        [Test]
        public void Test_convert_wavelength_um_to_nm()
        {
            const double wavelength = 0.5;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.Micrometers));
        }

        [Test]
        public void Test_convert_wavelength_m_to_nm()
        {
            const double wavelength = 0.0000005;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.Meters));
        }

        [Test]
        public void Test_convert_wavelength_inv_m_to_nm()
        {
            const double wavelength = 2000000;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.InverseMeters));
        }

        [Test]
        public void Test_convert_wavelength_inv_cm_to_nm()
        {
            const double wavelength = 20000;
            Assert.AreEqual(500, wavelength.ConvertWavelength(WavelengthUnit.InverseCentimeters));
        }

        [Test]
        public void Test_convert_coefficient_inv_mm_returns_self()
        {
            const double coefficient = 0.001;
            Assert.AreEqual(coefficient, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm()
        {
            const double coefficient = 1000;
            Assert.AreEqual(1, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm()
        {
            const double coefficient = 10;
            Assert.AreEqual(1, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm()
        {
            const double coefficient = 0.001;
            Assert.AreEqual(1, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers));
        }
        
        //Tests with micro molar coefficient applied
        [Test]
        public void Test_convert_coefficient_inv_mm_with_molar_uM_returns_self()
        {
            const double coefficient = 0.001;
            Assert.AreEqual(coefficient, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.MicroMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm_with_molar_uM()
        {
            const double coefficient = 1000;
            Assert.AreEqual(1,coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.MicroMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm_with_molar_uM()
        {
            const double coefficient = 10;
            Assert.AreEqual(1, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.MicroMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_molar_uM()
        {
            const double coefficient = 0.001;
            Assert.AreEqual(1, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.MicroMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_nano_molar()
        {
            const double coefficient = 10;
            Assert.AreEqual(1000, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.NanoMolar));
        }

        //Tests with millimolar coefficient applied
        [Test]
        public void Test_convert_coefficient_inv_mm_with_molar_mM()
        {
            const double coefficient = 1;
            Assert.AreEqual(0.001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.MilliMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm_with_molar_mM()
        {
            const double coefficient = 1000;
            Assert.AreEqual(0.001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.MilliMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm_with_molar_mM()
        {
            const double coefficient = 10;
            Assert.AreEqual(0.001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.MilliMolar));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_molar_mM()
        {
            const double coefficient = 0.001;
            Assert.AreEqual(0.001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.MilliMolar));
        }

        //Tests with molar coefficient applied
        [Test]
        public void Test_convert_coefficient_inv_mm_with_molar_M()
        {
            const double coefficient = 1;
            Assert.AreEqual(0.000001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.Molar));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm_with_molar_M()
        {
            const double coefficient = 1000;
            Assert.AreEqual(0.000001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.Molar));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm_with_molar_M()
        {
            const double coefficient = 10;
            Assert.AreEqual(0.000001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.Molar));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_molar_M()
        {
            const double coefficient = 0.001;
            Assert.AreEqual(0.000001, coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.Molar));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_mm()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/mm");
            Assert.AreEqual(AbsorptionCoefficientUnit.InverseMillimeters, unit);
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_cm()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/cm");
            Assert.AreEqual(AbsorptionCoefficientUnit.InverseCentimeters, unit);
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_m()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/m");
            Assert.AreEqual(AbsorptionCoefficientUnit.InverseMeters, unit);
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_um()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/um");
            Assert.AreEqual(AbsorptionCoefficientUnit.InverseMicrometers, unit);
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_undefined()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getAbsorptionCoefficientUnit(""));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_invalid()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getAbsorptionCoefficientUnit("1/xx"));

        }

        [Test]
        public void Test_get_molar_unit_inv_cm_M()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*M");
            Assert.AreEqual(MolarUnit.Molar, unit);
        }

        [Test]
        public void Test_get_molar_unit_inv_cm_mM()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*mM");
            Assert.AreEqual(MolarUnit.MilliMolar, unit);
        }

        [Test]
        public void Test_get_molar_unit_inv_cm_uM()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*uM");
            Assert.AreEqual(MolarUnit.MicroMolar, unit);
        }

        [Test]
        public void Test_get_molar_unit_inv_cm_nM()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*nM");
            Assert.AreEqual(MolarUnit.NanoMolar, unit);
        }

        [Test]
        public void Test_get_molar_unit_none()
        {
            var unit = SpectralConverter.getMolarUnit("1/x");
            Assert.AreEqual(MolarUnit.None, unit);
        }

        [Test]
        public void Test_get_molar_unit_none_invalid()
        {
            var unit = SpectralConverter.getMolarUnit("1/x*x");
            Assert.AreEqual(MolarUnit.None, unit);
        }

        [Test]
        public void Test_get_molar_unit_invalid()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getMolarUnit("4"));
        }

        [Test]
        public void Test_get_spectral_unit_molar_inv_cm()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.Molar, AbsorptionCoefficientUnit.InverseCentimeters);
            Assert.AreEqual("1/(cm*M)", unit);
        }

        [Test]
        public void Test_get_spectral_unit_micromolar_inv_m()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.MicroMolar, AbsorptionCoefficientUnit.InverseMeters);
            Assert.AreEqual("1/(m*uM)", unit);
        }

        [Test]
        public void Test_get_spectral_unit_millimolar_inv_mm()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.MilliMolar, AbsorptionCoefficientUnit.InverseMillimeters);
            Assert.AreEqual("1/(mm*mM)", unit);
        }

        [Test]
        public void Test_get_spectral_unit_nanomolar_inv_um()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.NanoMolar, AbsorptionCoefficientUnit.InverseMicrometers);
            Assert.AreEqual("1/(um*nM)", unit);
        }

        [Test]
        public void Test_get_spectral_unit_molar_none_inv_um()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.None, AbsorptionCoefficientUnit.InverseMicrometers);
            Assert.AreEqual("1/um", unit);
        }

        [Test]
        public void Test_get_spectral_unit_unknown_absorption_coefficient()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getSpectralUnit(MolarUnit.NanoMolar, (AbsorptionCoefficientUnit)100));
        }

        [Test]
        public void Test_get_spectral_unit_unknown_molar_unit()
        {
            Assert.Throws<Exception>(() => SpectralConverter.getSpectralUnit((MolarUnit)100, AbsorptionCoefficientUnit.InverseMicrometers));
        }
    }
}
