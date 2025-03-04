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
            Assert.That(unit, Is.EqualTo(WavelengthUnit.Nanometers));
        }

        [Test]
        public void Test_get_wavelength_unit_um()
        {
            var unit = SpectralConverter.getWavelengthUnit("um");
            Assert.That(unit, Is.EqualTo(WavelengthUnit.Micrometers));
        }

        [Test]
        public void Test_get_wavelength_unit_m()
        {
            var unit = SpectralConverter.getWavelengthUnit("m");
            Assert.That(unit, Is.EqualTo(WavelengthUnit.Meters));
        }

        [Test]
        public void Test_get_wavelength_unit_inv_m()
        {
            var unit = SpectralConverter.getWavelengthUnit("1/m");
            Assert.That(unit, Is.EqualTo(WavelengthUnit.InverseMeters));
        }

        [Test]
        public void Test_get_wavelength_unit_inv_cm()
        {
            var unit = SpectralConverter.getWavelengthUnit("1/cm");
            Assert.That(unit, Is.EqualTo(WavelengthUnit.InverseCentimeters));
        }

        [Test]
        public void Test_get_wavelength_unit_invalid()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getWavelengthUnit("yu"));
        }

        [Test]
        public void Test_get_wavelength_unit_undefined()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getWavelengthUnit(""));
        }
        [Test]
        public void Test_get_wavelength_unit_nanometers()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.Nanometers);
            Assert.That(unit, Is.EqualTo("nm"));
        }

        [Test]
        public void Test_get_wavelength_unit_micrometers()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.Micrometers);
            Assert.That(unit, Is.EqualTo("um"));
        }

        [Test]
        public void Test_get_wavelength_unit_meters()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.Meters);
            Assert.That(unit, Is.EqualTo("m"));
        }

        [Test]
        public void Test_get_wavelength_unit_inv_meters()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.InverseMeters);
            Assert.That(unit, Is.EqualTo("1/m"));
        }

        [Test]
        public void Test_get_wavelength_unit_inv_centimeters()
        {
            var unit = SpectralConverter.getWavelengthUnit(WavelengthUnit.InverseCentimeters);
            Assert.That(unit, Is.EqualTo("1/cm"));
        }

        [Test]
        public void Test_get_wavelength_unit_invalid_enum()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getWavelengthUnit((WavelengthUnit)100));
        }

        [Test]
        public void Test_convert_wavelength_nm_returns_self()
        {
            const double wavelength = 500;
            Assert.That(wavelength.ConvertWavelength(WavelengthUnit.Nanometers), Is.EqualTo(wavelength));
        }

        [Test]
        public void Test_convert_wavelength_um_to_nm()
        {
            const double wavelength = 0.5;
            Assert.That(wavelength.ConvertWavelength(WavelengthUnit.Micrometers), Is.EqualTo(500));
        }

        [Test]
        public void Test_convert_wavelength_m_to_nm()
        {
            const double wavelength = 0.0000005;
            Assert.That(wavelength.ConvertWavelength(WavelengthUnit.Meters), Is.EqualTo(500));
        }

        [Test]
        public void Test_convert_wavelength_inv_m_to_nm()
        {
            const double wavelength = 2000000;
            Assert.That(wavelength.ConvertWavelength(WavelengthUnit.InverseMeters), Is.EqualTo(500));
        }

        [Test]
        public void Test_convert_wavelength_inv_cm_to_nm()
        {
            const double wavelength = 20000;
            Assert.That(wavelength.ConvertWavelength(WavelengthUnit.InverseCentimeters), Is.EqualTo(500));
        }

        [Test]
        public void Test_convert_coefficient_inv_mm_returns_self()
        {
            const double coefficient = 0.001;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters), Is.EqualTo(coefficient));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm()
        {
            const double coefficient = 1000;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters), Is.EqualTo(1));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm()
        {
            const double coefficient = 10;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters), Is.EqualTo(1));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm()
        {
            const double coefficient = 0.001;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers), Is.EqualTo(1));
        }
        
        //Tests with micro molar coefficient applied
        [Test]
        public void Test_convert_coefficient_inv_mm_with_molar_uM_returns_self()
        {
            const double coefficient = 0.001;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.MicroMolar), Is.EqualTo(coefficient));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm_with_molar_uM()
        {
            const double coefficient = 1000;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.MicroMolar), Is.EqualTo(1));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm_with_molar_uM()
        {
            const double coefficient = 10;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.MicroMolar), Is.EqualTo(1));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_molar_uM()
        {
            const double coefficient = 0.001;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.MicroMolar), Is.EqualTo(1));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_nano_molar()
        {
            const double coefficient = 10;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.NanoMolar), Is.EqualTo(1000));
        }

        //Tests with millimolar coefficient applied
        [Test]
        public void Test_convert_coefficient_inv_mm_with_molar_mM()
        {
            const double coefficient = 1;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.MilliMolar), Is.EqualTo(0.001));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm_with_molar_mM()
        {
            const double coefficient = 1000;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.MilliMolar), Is.EqualTo(0.001));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm_with_molar_mM()
        {
            const double coefficient = 10;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.MilliMolar), Is.EqualTo(0.001));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_molar_mM()
        {
            const double coefficient = 0.001;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.MilliMolar), Is.EqualTo(0.001));
        }

        //Tests with molar coefficient applied
        [Test]
        public void Test_convert_coefficient_inv_mm_with_molar_M()
        {
            const double coefficient = 1;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMillimeters, MolarUnit.Molar), Is.EqualTo(0.000001));
        }

        [Test]
        public void Test_convert_coefficient_inv_m_to_inv_mm_with_molar_M()
        {
            const double coefficient = 1000;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMeters, MolarUnit.Molar), Is.EqualTo(0.000001));
        }

        [Test]
        public void Test_convert_coefficient_inv_cm_to_inv_mm_with_molar_M()
        {
            const double coefficient = 10;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseCentimeters, MolarUnit.Molar), Is.EqualTo(0.000001));
        }

        [Test]
        public void Test_convert_coefficient_inv_um_to_inv_mm_with_molar_M()
        {
            const double coefficient = 0.001;
            Assert.That(coefficient.ConvertCoefficient(AbsorptionCoefficientUnit.InverseMicrometers, MolarUnit.Molar), Is.EqualTo(0.000001));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_mm()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/mm");
            Assert.That(unit, Is.EqualTo(AbsorptionCoefficientUnit.InverseMillimeters));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_cm()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/cm");
            Assert.That(unit, Is.EqualTo(AbsorptionCoefficientUnit.InverseCentimeters));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_m()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/m");
            Assert.That(unit, Is.EqualTo(AbsorptionCoefficientUnit.InverseMeters));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_um()
        {
            var unit = SpectralConverter.getAbsorptionCoefficientUnit("1/um");
            Assert.That(unit, Is.EqualTo(AbsorptionCoefficientUnit.InverseMicrometers));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_undefined()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getAbsorptionCoefficientUnit(""));
        }

        [Test]
        public void Test_get_absorption_coefficient_unit_invalid()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getAbsorptionCoefficientUnit("1/xx"));

        }

        [Test]
        public void Test_get_molar_unit_inv_cm_M()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*M");
            Assert.That(unit, Is.EqualTo(MolarUnit.Molar));
        }

        [Test]
        public void Test_get_molar_unit_inv_cm_mM()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*mM");
            Assert.That(unit, Is.EqualTo(MolarUnit.MilliMolar));
        }

        [Test]
        public void Test_get_molar_unit_inv_cm_uM()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*uM");
            Assert.That(unit, Is.EqualTo(MolarUnit.MicroMolar));
        }

        [Test]
        public void Test_get_molar_unit_inv_cm_nM()
        {
            var unit = SpectralConverter.getMolarUnit("1/cm*nM");
            Assert.That(unit, Is.EqualTo(MolarUnit.NanoMolar));
        }

        [Test]
        public void Test_get_molar_unit_none()
        {
            var unit = SpectralConverter.getMolarUnit("1/x");
            Assert.That(unit, Is.EqualTo(MolarUnit.None));
        }

        [Test]
        public void Test_get_molar_unit_none_invalid()
        {
            var unit = SpectralConverter.getMolarUnit("1/x*x");
            Assert.That(unit, Is.EqualTo(MolarUnit.None));
        }

        [Test]
        public void Test_get_molar_unit_invalid()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getMolarUnit("4"));
        }

        [Test]
        public void Test_get_spectral_unit_molar_inv_cm()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.Molar, AbsorptionCoefficientUnit.InverseCentimeters);
            Assert.That(unit, Is.EqualTo("1/(cm*M)"));
        }

        [Test]
        public void Test_get_spectral_unit_micromolar_inv_m()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.MicroMolar, AbsorptionCoefficientUnit.InverseMeters);
            Assert.That(unit, Is.EqualTo("1/(m*uM)"));
        }

        [Test]
        public void Test_get_spectral_unit_millimolar_inv_mm()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.MilliMolar, AbsorptionCoefficientUnit.InverseMillimeters);
            Assert.That(unit, Is.EqualTo("1/(mm*mM)"));
        }

        [Test]
        public void Test_get_spectral_unit_nanomolar_inv_um()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.NanoMolar, AbsorptionCoefficientUnit.InverseMicrometers);
            Assert.That(unit, Is.EqualTo("1/(um*nM)"));
        }

        [Test]
        public void Test_get_spectral_unit_molar_none_inv_um()
        {
            var unit = SpectralConverter.getSpectralUnit(MolarUnit.None, AbsorptionCoefficientUnit.InverseMicrometers);
            Assert.That(unit, Is.EqualTo("1/um"));
        }

        [Test]
        public void Test_get_spectral_unit_unknown_absorption_coefficient()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getSpectralUnit(MolarUnit.NanoMolar, (AbsorptionCoefficientUnit)100));
        }

        [Test]
        public void Test_get_spectral_unit_unknown_molar_unit()
        {
            Assert.Throws<ArgumentException>(() => SpectralConverter.getSpectralUnit((MolarUnit)100, AbsorptionCoefficientUnit.InverseMicrometers));
        }
    }
}
