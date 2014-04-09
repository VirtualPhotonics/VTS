using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    /// <summary>
    /// Test for the ChromophoreSpectrum class
    /// </summary>
    [TestFixture]
    public class ChromophoreSpectrumTests
    {
        /// <summary>
        /// Runs before every unit test after the TestFixtureSetup
        /// </summary>
        [SetUp]
        public void clear_folders_and_files()
        {
            if (FileIO.FileExists("ChromophoreSpectrum.xml"))
            {
                FileIO.FileDelete("ChromophoreSpectrum.xml");
            }
        }

        /// <summary>
        /// Test that the ChromophoreSpectrum class can be serialized
        /// </summary>
        [Test]
        public void validate_Serializing_Chromophore_Spectrum()
        {
            ChromophoreSpectrum chromophoreSpectrum = CreateChromophoreSpectrum();
            chromophoreSpectrum.WriteToXML("ChromophoreSpectrum.xml");
            Assert.IsTrue(FileIO.FileExists("ChromophoreSpectrum.xml"));
        }

        /// <summary>
        /// Test that the ChromophoreSpectrum class can be deserialized
        /// </summary>
        [Test]
        public void validate_Deserializing_Chromophore_Spectrum()
        {
            ChromophoreSpectrum chromophoreSpectrum = CreateChromophoreSpectrum();
            chromophoreSpectrum.WriteToXML("ChromophoreSpectrum.xml");

            var chromophoreSpectrumRead = FileIO.ReadFromXML<ChromophoreSpectrum>("ChromophoreSpectrum.xml");
            Assert.IsInstanceOf<ChromophoreSpectrum>(chromophoreSpectrumRead);
        }

        private ChromophoreSpectrum CreateChromophoreSpectrum()
        {
            string name = "Melanin";
            AbsorptionCoefficientUnit muaUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            MolarUnit molarUnit = MolarUnit.None;
            ChromophoreCoefficientType coeffType = ChromophoreCoefficientType.FractionalAbsorptionCoefficient;

            // populate list of wavelengths
            List<double> wavelengths = new List<double>();
            wavelengths.Add(0.0);
            wavelengths.Add(1.0);
            wavelengths.Add(2.0);

            // populate list of values
            List<double> values = new List<double>();
            values.Add(0.1);
            values.Add(1.1);
            values.Add(2.1);

            return new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnit, molarUnit, WavelengthUnit.Nanometers);
        }
    }
}
