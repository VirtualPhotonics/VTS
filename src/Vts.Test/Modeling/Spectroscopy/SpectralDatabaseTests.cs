using System;
using System.Collections.Generic;
using System.IO;
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
        /// Runs before every unit test after the TestFixtureSetup
        /// </summary>
        [SetUp]
        public void setup()
        {
        }

        /// <summary>
        /// clear all previously generated folders and files
        /// </summary>
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
            if (FileIO.FileExists("SpectralDictionary.json"))
            {
                FileIO.FileDelete("SpectralDictionary.json");
            }
            if (FileIO.FileExists("dictionary.json"))
            {
                FileIO.FileDelete("dictionary.json");
            }
            if (FileIO.FileExists("dictionary2.json"))
            {
                FileIO.FileDelete("dictionary2.json");
            }
            if (FileIO.FileExists("dictionary3.json"))
            {
                FileIO.FileDelete("dictionary3.json");
            }
            if (FileIO.FileExists("dictionary4.json"))
            {
                FileIO.FileDelete("dictionary4.json");
            }
            if (FileIO.FileExists("dictionary5.json"))
            {
                FileIO.FileDelete("dictionary5.json");
            }
            if (FileIO.FileExists("absorber-Fat.txt"))
            {
                FileIO.FileDelete("absorber-Fat.txt");
            }
            if (FileIO.FileExists("absorber-Hb.txt"))
            {
                FileIO.FileDelete("absorber-Hb.txt");
            }
            if (FileIO.FileExists("absorber-HbO2.txt"))
            {
                FileIO.FileDelete("absorber-HbO2.txt");
            }
            if (FileIO.FileExists("absorber-H2O.txt"))
            {
                FileIO.FileDelete("absorber-H2O.txt");
            }
            if (FileIO.FileExists("absorber-Melanin.txt"))
            {
                FileIO.FileDelete("absorber-Melanin.txt");
            }
            if (FileIO.FileExists("absorber-Nigrosin.txt"))
            {
                FileIO.FileDelete("absorber-Nigrosin.txt");
            }
        }

        /// <summary>
        /// validate loading the default spectral database
        /// </summary>
        [Test]
        public void validate_loading_spectral_database_from_file_in_resources()
        {
            var _testDictionary = SpectralDatabase.GetDefaultDatabaseFromFileInResources();
            Assert.IsNotNull(_testDictionary);
        }

        /// <summary>
        /// Validate the serialization of the spectral database
        /// </summary>
        [Test]
        public void validate_serializing_spectral_database()
        {
            var testDictionary = CreateDictionary();
            testDictionary.WriteToJson("SpectralDictionary.json");
            Assert.IsTrue(FileIO.FileExists("SpectralDictionary.json"));
        }

        /// <summary>
        /// Validate the deserialization of the spectral database
        /// </summary>
        [Test]
        public void validate_deserializing_spectral_database()
        {
            var testDictionary = CreateDictionary();
            testDictionary.WriteToJson("dictionary.json");
            var Dvalues = FileIO.ReadFromJson<ChromophoreSpectrumDictionary>("dictionary.json");
            Assert.IsNotNull(Dvalues);
        }

        /// <summary>
        /// Validate the data after serializing and deserializing the database
        /// </summary>
        [Test]
        public void validate_serialized_data()
        {
            var testDictionary = CreateDictionary();
            testDictionary.WriteToJson("dictionary2.json");
            var Dvalues = FileIO.ReadFromJson<ChromophoreSpectrumDictionary>("dictionary2.json");
            Assert.AreEqual(Dvalues["HbO2"].Wavelengths[2], testDictionary["HbO2"].Wavelengths[2]);
        }

        /// <summary>
        /// validate loading spectral data from tab-delimited file and update existing data
        /// </summary>
        [Test]
        public void validate_loading_spectral_database_from_tsv_in_resources()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            List<ChromophoreSpectrum> myChromophoreList = new List<ChromophoreSpectrum>();
            //create 2 sets of values for the tab delimeted file
            ChromophoreSpectrum c = new ChromophoreSpectrum();
            c.Name = "HbO2";
            c.AbsorptionCoefficientUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            c.MolarUnit = MolarUnit.None;
            c.ChromophoreCoefficientType = ChromophoreCoefficientType.MolarAbsorptionCoefficient;
            myChromophoreList.Add(c);
            ChromophoreSpectrum c2 = new ChromophoreSpectrum();
            c2.Name = "Hb";
            c2.AbsorptionCoefficientUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            c2.MolarUnit = MolarUnit.None;
            c2.ChromophoreCoefficientType = ChromophoreCoefficientType.MolarAbsorptionCoefficient;
            myChromophoreList.Add(c2);
            var testDictionary = myChromophoreList.ToDictionary();
            SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
            testDictionary.WriteToJson("dictionary3.json");
            Assert.IsTrue(FileIO.FileExists("dictionary3.json"));
        }

        /// <summary>
        /// validate loading spectral database and header from tab-delimited file with conversion
        /// </summary>
        [Test]
        public void validate_Loading_Spectral_Database_and_header_from_tsv_in_resources()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testSpectra = SpectralDatabase.GetSpectraFromFile(stream, true);
            var testDictionary = testSpectra.ToDictionary();
            testDictionary.WriteToJson("dictionary4.json");
            Assert.IsTrue(FileIO.FileExists("dictionary4.json"));
        }

        /// <summary>
        /// validate loading spectral database and header from tab-delimited file with conversion
        /// </summary>
        [Test]
        public void validate_data_from_tsv()
        {
            int linenumber = 5;
            string line;
            string[] row;

            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");
            var testSpectra = SpectralDatabase.GetSpectraFromFile(stream, true);
            var testDictionary = testSpectra.ToDictionary();
            stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");
            using (StreamReader readFile = new StreamReader(stream))
            {
                // read n lines (there is one line of header so
                for (int i = 0; i <= linenumber; i++)
                {
                    readFile.ReadLine();
                }
                // get a line from the stream and split the data
                line = readFile.ReadLine();
                row = line.Split('\t');
            }
            Assert.AreEqual(testDictionary["Hb"].Wavelengths[linenumber], Convert.ToDouble(row[0]));
            // dc: this would be only for MolarExtinctionCoefficient or FractionalExtinctionCoefficient, not MolarAbsorptionCoefficient or FractionalAbsorptionCoefficient
            // multiply the value by ln(10)
            // double k =  Math.Log(10);
            double k = 1D;
            double spectra = Convert.ToDouble(row[1]) * k;
            // test that the values in the text stream match the ones in the object
            Assert.AreEqual(testDictionary["HbO2"].Spectrum[linenumber], spectra);
            spectra = Convert.ToDouble(row[2]) * k;
            Assert.AreEqual(testDictionary["Hb"].Spectrum[linenumber], spectra);
        }

        /// <summary>
        /// validate loading spectral database and header from tab-delimited file with no conversion
        /// </summary>
        [Test]
        public void validate_loading_spectral_database_and_header_from_tsv_no_conversion()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testSpectra = SpectralDatabase.GetSpectraFromFile(stream, false);
            var testDictionary = testSpectra.ToDictionary();
            testDictionary.WriteToJson("dictionary5.json");
            Assert.IsTrue(FileIO.FileExists("dictionary5.json"));
        }

        /// <summary>
        /// validate writing the tab-delimited text files
        /// </summary>
        [Test]
        public void validate_write_text_files_from_file_in_resources()
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

        private ChromophoreSpectrumDictionary CreateDictionary()
        {
            string name = "Melanin";
            AbsorptionCoefficientUnit muaUnit = AbsorptionCoefficientUnit.InverseMillimeters;
            MolarUnit molarUnit = MolarUnit.MicroMolar;
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

            ChromophoreSpectrum chromophoreSpectrum = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnit, molarUnit, WavelengthUnit.Nanometers);
            
            var testDictionary = new ChromophoreSpectrumDictionary();
            testDictionary.Add(chromophoreSpectrum.Name, chromophoreSpectrum);

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
