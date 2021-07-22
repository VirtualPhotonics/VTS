using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Vts.IO;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.Spectroscopy
{
    /// <summary>
    /// Tests for spectral database reading and writing
    /// </summary>
    [TestFixture]
    public class SpectralDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new List<string>()
        {
            "SpectralDictionary.txt",
            "dictionary.json",
            "dictionary2.json",
            "absorber-Fat.txt",
            "absorber-Hb.txt",
            "absorber-HbO2.txt",
            "absorber-H2O.txt",
            "absorber-Melanin.txt",
            "absorber-Nigrosin.txt"
        };

        private ChromophoreSpectrumDictionary _chromophoreSpectrumDictionary;

        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        public void One_time_setup()
        {
            Clear_folders_and_files();
            _chromophoreSpectrumDictionary = CreateDictionary();
            SpectralDatabase.SaveDatabaseToFile(_chromophoreSpectrumDictionary, "dictionary.json");
        }

        [OneTimeTearDown]
        public void One_time_tear_down()
        {
            Clear_folders_and_files();
        }

        private void Clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        [Test]
        public void Test_get_spectrum_value()
        {
            var spectrum = SpectralDatabase.GetSpectrumValue("H2O", 600);
            Assert.AreEqual(0.000301, spectrum, 0.000001);
        }

        [Test]
        public void Test_save_database_to_file()
        {
            Assert.IsTrue(FileIO.FileExists("dictionary.json"));
        }

        [Test]
        public void Test_get_spectrum_value_returns_0()
        {
            var spectrum = SpectralDatabase.GetSpectrumValue("", 600);
            Assert.AreEqual(0, spectrum);
        }

        /// <summary>
        /// Validate loading the default spectral database
        /// </summary>
        [Test]
        public void Validate_loading_spectral_database_from_file_in_resources()
        {
            var testDictionary = SpectralDatabase.GetDefaultDatabaseFromFileInResources();
            Assert.IsNotNull(testDictionary);
        }

        /// <summary>
        /// Validate the serialization of the spectral database
        /// </summary>
        [Test]
        public void Validate_serializing_spectral_database()
        {
            _chromophoreSpectrumDictionary.WriteToJson("SpectralDictionary.txt");
            Assert.IsTrue(FileIO.FileExists("SpectralDictionary.txt"));
        }

        /// <summary>
        /// Validate the deserialization of the spectral database
        /// </summary>
        [Test]
        public void Test_get_database_from_file()
        {
            var chromophoreSpectrumDictionary = SpectralDatabase.GetDatabaseFromFile("dictionary.json");
            Assert.IsNotNull(chromophoreSpectrumDictionary);
            Assert.IsInstanceOf<ChromophoreSpectrumDictionary>(chromophoreSpectrumDictionary);
        }

        /// <summary>
        /// Validate the data after serializing and deserializing the database
        /// </summary>
        [Test]
        public void Validate_serialized_data()
        {
            var comparisonDictionary = CreateDictionary();
            var chromophoreSpectrumDictionary = SpectralDatabase.GetDatabaseFromFile("dictionary.json");
            Assert.AreEqual(comparisonDictionary["HbO2"].Wavelengths[2], chromophoreSpectrumDictionary["HbO2"].Wavelengths[2]);
        }

        /// <summary>
        /// Validate loading spectral data from tab-delimited file and update existing data
        /// </summary>
        [Test]
        public void Validate_loading_spectral_database_from_tsv_in_resources()
        {
            var stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var myChromophoreList = new List<ChromophoreSpectrum>();
            //create 2 sets of values for the tab delimited file
            var chromophoreSpectrum = new ChromophoreSpectrum
            {
                Name = "HbO2",
                AbsorptionCoefficientUnit = AbsorptionCoefficientUnit.InverseMillimeters,
                MolarUnit = MolarUnit.None,
                ChromophoreCoefficientType = ChromophoreCoefficientType.MolarAbsorptionCoefficient
            };
            myChromophoreList.Add(chromophoreSpectrum);
            var chromophoreSpectrum2 = new ChromophoreSpectrum
            {
                Name = "Hb",
                AbsorptionCoefficientUnit = AbsorptionCoefficientUnit.InverseMillimeters,
                MolarUnit = MolarUnit.None,
                ChromophoreCoefficientType = ChromophoreCoefficientType.MolarAbsorptionCoefficient
            };
            myChromophoreList.Add(chromophoreSpectrum2);
            var testDictionary = myChromophoreList.ToDictionary();
            SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
            testDictionary.WriteToJson("dictionary2.json");
            Assert.IsTrue(FileIO.FileExists("dictionary2.json"));
        }

        [Test]
        public void Test_get_spectra_from_file_null_stream()
        {
            Assert.IsNull(SpectralDatabase.GetSpectraFromFile(null, true));
        }

        [Test]
        public void Test_get_spectra_from_file_column_error()
        {
            var byteArray = Encoding.UTF8.GetBytes("invalid");
            var stream = new MemoryStream(byteArray);
            Assert.Throws<Exception>(() => SpectralDatabase.GetSpectraFromFile(stream, false));
        }

        [Test]
        public void Test_get_spectra_from_file_no_lambda_error()
        {
            var byteArray = Encoding.UTF8.GetBytes("%LABDA nm\tHbO2 1/(mm*uM)\tHb 1/(mm*uM)\n\n600\t0.737\t3.3802");
            var stream = new MemoryStream(byteArray);
            Assert.Throws<Exception>(() => SpectralDatabase.GetSpectraFromFile(stream, false));
        }

        [Test]
        public void Test_get_spectra_from_file_invalid_header()
        {
            var byteArray = Encoding.UTF8.GetBytes("%LAMBDA nm\tHbO2\tHb 1/(mm*uM)\n\n600\t0.737\t3.3802");
            var stream = new MemoryStream(byteArray);
            Assert.Throws<Exception>(() => SpectralDatabase.GetSpectraFromFile(stream, false));
        }

        [Test]
        public void Test_get_spectra_from_file_header_data_mismatch()
        {
            var byteArray = Encoding.UTF8.GetBytes("%LAMBDA nm\tHbO2 1/(mm*uM)\tHb 1/(mm*uM)\n\n600\t0.737");
            var stream = new MemoryStream(byteArray);
            Assert.Throws<Exception>(() => SpectralDatabase.GetSpectraFromFile(stream, false));
        }

        [Test]
        public void Test_get_spectra_from_file_header_column_mismatch()
        {
            var byteArray = Encoding.UTF8.GetBytes("%LAMBDA nm\tHbO2 1/(mm*uM)\tHb 1/(mm*uM)\n600\t0.737\t3.3802\n602\t0.6135\t3.1372\n604\t0.4901");
            var stream = new MemoryStream(byteArray);
            Assert.Throws<Exception>(() => SpectralDatabase.GetSpectraFromFile(stream, false));
        }

        /// <summary>
        /// Validate loading spectral database and header from tab-delimited file with conversion
        /// </summary>
        [Test]
        public void Validate_loading_spectral_database_and_header_from_tsv_in_resources()
        {
            var stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testSpectra = SpectralDatabase.GetSpectraFromFile(stream, true);
            var testDictionary = testSpectra.ToDictionary();
            Assert.IsInstanceOf<List<ChromophoreSpectrum>>(testSpectra);
            Assert.IsInstanceOf<ChromophoreSpectrumDictionary>(testDictionary);
        }

        /// <summary>
        /// Validate loading spectral database and header from tab-delimited file with conversion
        /// </summary>
        [Test]
        public void Validate_data_from_tsv()
        {
            const int lineNumber = 5;
            string[] row;

            var stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");
            var testSpectra = SpectralDatabase.GetSpectraFromFile(stream, true);
            var testDictionary = testSpectra.ToDictionary();
            stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");
            using (var readFile = new StreamReader(stream))
            {
                // read n lines (there is one line of header so
                for (var i = 0; i <= lineNumber; i++)
                {
                    readFile.ReadLine();
                }
                // get a line from the stream and split the data
                var line = readFile.ReadLine();
                row = line.Split('\t');
            }
            Assert.AreEqual(testDictionary["Hb"].Wavelengths[lineNumber], Convert.ToDouble(row[0]));
            // dc: this would be only for MolarExtinctionCoefficient or FractionalExtinctionCoefficient, not MolarAbsorptionCoefficient or FractionalAbsorptionCoefficient
            // multiply the value by ln(10)
            // double k =  Math.Log(10);
            const double k = 1D;
            var spectra = Convert.ToDouble(row[1]) * k;
            // test that the values in the text stream match the ones in the object
            Assert.AreEqual(testDictionary["HbO2"].Spectrum[lineNumber], spectra);
            spectra = Convert.ToDouble(row[2]) * k;
            Assert.AreEqual(testDictionary["Hb"].Spectrum[lineNumber], spectra);
        }

        /// <summary>
        /// Validate loading spectral database and header from tab-delimited file with no conversion
        /// </summary>
        [Test]
        public void Validate_loading_spectral_database_and_header_from_tsv_no_conversion()
        {
            var stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testSpectra = SpectralDatabase.GetSpectraFromFile(stream, false);
            var testDictionary = testSpectra.ToDictionary();
            Assert.IsInstanceOf<List<ChromophoreSpectrum>>(testSpectra);
            Assert.IsInstanceOf<ChromophoreSpectrumDictionary>(testDictionary);
        }

        /// <summary>
        /// Validate writing the tab-delimited text files
        /// </summary>
        [Test]
        public void Validate_write_text_files_from_file_in_resources()
        {
            var testDictionary = SpectralDatabase.GetDefaultDatabaseFromFileInResources();
            SpectralDatabase.WriteDatabaseToFiles(testDictionary);
            Assert.IsTrue(FileIO.FileExists("absorber-Fat.txt"));
            Assert.IsTrue(FileIO.FileExists("absorber-H2O.txt"));
            Assert.IsTrue(FileIO.FileExists("absorber-Hb.txt"));
            Assert.IsTrue(FileIO.FileExists("absorber-HbO2.txt"));
            Assert.IsTrue(FileIO.FileExists("absorber-Melanin.txt"));
            Assert.IsTrue(FileIO.FileExists("absorber-Nigrosin.txt"));
        }

        private static ChromophoreSpectrumDictionary CreateDictionary()
        {
            var name = "Melanin";
            var muaUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            var molarUnit = MolarUnit.MicroMolar;
            var coeffType = ChromophoreCoefficientType.FractionalAbsorptionCoefficient;

            // populate list of wavelengths
            var wavelengths = new List<double> {0.0, 1.0, 2.0};

            // populate list of values
            var values = new List<double> {0.1, 1.1, 2.1};

            var chromophoreSpectrum = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnit, molarUnit, WavelengthUnit.Nanometers);

            var testDictionary = new ChromophoreSpectrumDictionary
            {
                {
                    chromophoreSpectrum.Name, chromophoreSpectrum
                }
            };

            name = "HbO2";
            muaUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            molarUnit = MolarUnit.MicroMolar;
            coeffType = ChromophoreCoefficientType.MolarAbsorptionCoefficient;

            chromophoreSpectrum = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnit, molarUnit, WavelengthUnit.Nanometers);
            testDictionary.Add(chromophoreSpectrum.Name, chromophoreSpectrum);

            return testDictionary;
        }
    }
}
