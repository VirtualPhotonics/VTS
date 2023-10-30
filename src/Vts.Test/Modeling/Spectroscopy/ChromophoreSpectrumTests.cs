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
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "ChromophoreSpectrum.txt",
        };


        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void Clear_previously_generated_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// Test that the ChromophoreSpectrum class can be serialized
        /// </summary>
        [Test]
        public void Validate_Serializing_Chromophore_Spectrum()
        {
            var chromophoreSpectrum = CreateChromophoreSpectrum();
            chromophoreSpectrum.WriteToJson("ChromophoreSpectrum.txt");
            Assert.IsTrue(FileIO.FileExists("ChromophoreSpectrum.txt"));
        }

        /// <summary>
        /// Test that the ChromophoreSpectrum class can be deserialized
        /// </summary>
        [Test]
        public void Validate_Deserializing_Chromophore_Spectrum()
        {
            var chromophoreSpectrum = CreateChromophoreSpectrum();
            chromophoreSpectrum.WriteToJson("ChromophoreSpectrum.txt");

            var chromophoreSpectrumRead = FileIO.ReadFromJson<ChromophoreSpectrum>("ChromophoreSpectrum.txt");
            Assert.IsInstanceOf<ChromophoreSpectrum>(chromophoreSpectrumRead);
        }

        private static ChromophoreSpectrum CreateChromophoreSpectrum()
        {
            const string name = "Melanin";
            const AbsorptionCoefficientUnit muaUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            const MolarUnit molarUnit = MolarUnit.None;
            const ChromophoreCoefficientType coefficientType = ChromophoreCoefficientType.FractionalAbsorptionCoefficient;

            // populate list of wavelengths
            var wavelengths = new List<double>
            {
                0.0,
                1.0,
                2.0
            };

            // populate list of values
            var values = new List<double>
            {
                0.1,
                1.1,
                2.1
            };

            return new ChromophoreSpectrum(wavelengths, values, name, coefficientType, muaUnit, molarUnit, WavelengthUnit.Nanometers);
        }
    }
}
