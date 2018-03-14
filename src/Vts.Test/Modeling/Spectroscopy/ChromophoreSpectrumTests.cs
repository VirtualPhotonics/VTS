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
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "ChromophoreSpectrum.txt",
        };

        /// <summary>
        /// Runs before every unit test after the TestFixtureSetup
        /// </summary>
        [SetUp]
        public void setup()
        {
        }

        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [TestFixtureSetUp]
        [TestFixtureTearDown]
        public void clear_previously_generated_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// Test that the ChromophoreSpectrum class can be serialized
        /// </summary>
        [Test]
        public void validate_Serializing_Chromophore_Spectrum()
        {
            ChromophoreSpectrum chromophoreSpectrum = CreateChromophoreSpectrum();
            chromophoreSpectrum.WriteToJson("ChromophoreSpectrum.txt");
            Assert.IsTrue(FileIO.FileExists("ChromophoreSpectrum.txt"));
        }

        /// <summary>
        /// Test that the ChromophoreSpectrum class can be deserialized
        /// </summary>
        [Test]
        public void validate_Deserializing_Chromophore_Spectrum()
        {
            ChromophoreSpectrum chromophoreSpectrum = CreateChromophoreSpectrum();
            chromophoreSpectrum.WriteToJson("ChromophoreSpectrum.txt");

            var chromophoreSpectrumRead = FileIO.ReadFromJson<ChromophoreSpectrum>("ChromophoreSpectrum.txt");
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
