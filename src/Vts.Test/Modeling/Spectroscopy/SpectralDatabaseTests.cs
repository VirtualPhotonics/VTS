using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.SpectralMapping;
using Vts.IO;
using System.Runtime.Serialization;
#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif
namespace Vts.Test.Modeling.Spectroscopy
{
    /// <summary>
    /// Tests for spectral database reading and writing
    /// </summary>
    [KnownType(typeof(ChromophoreSpectrum))]
    [TestFixture]
    public class SpectralDatabaseTests
    {
#if SILVERLIGHT
        private IsolatedStorageFile _objStore;
#endif
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
#if SILVERLIGHT
            _objStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (_objStore.FileExists("SpectralDictionary.xml")) 
            {
                _objStore.DeleteFile("SpectralDictionary.xml");
            }
            if (_objStore.FileExists("dictionary.xml"))
            {
                _objStore.DeleteFile("dictionary.xml");
            }
            if (_objStore.FileExists("dictionary2.xml"))
            {
                _objStore.DeleteFile("dictionary2.xml");
            }
            if (_objStore.FileExists("dictionary3.xml"))
            {
                _objStore.DeleteFile("dictionary3.xml");
            }
            if (_objStore.FileExists("dictionary4.xml"))
            {
                _objStore.DeleteFile("dictionary4.xml");
            }
            if (_objStore.FileExists("dictionary5.xml"))
            {
                _objStore.DeleteFile("dictionary5.xml");
            }
            if (_objStore.FileExists("absorber-Fat.txt"))
            {
                _objStore.DeleteFile("absorber-Fat.txt");
            }
            if (_objStore.FileExists("absorber-Hb.txt"))
            {
                _objStore.DeleteFile("absorber-Hb.txt");
            }
            if (_objStore.FileExists("absorber-HbO2.txt"))
            {
                _objStore.DeleteFile("absorber-HbO2.txt");
            }
            if (_objStore.FileExists("absorber-H2O.txt"))
            {
                _objStore.DeleteFile("absorber-H2O.txt");
            }
            if (_objStore.FileExists("absorber-Melanin.txt"))
            {
                _objStore.DeleteFile("absorber-Melanin.txt");
            }
            if (_objStore.FileExists("absorber-Nigrosin.txt"))
            {
                _objStore.DeleteFile("absorber-Nigrosin.txt");
            }
#else
            if (File.Exists("SpectralDictionary.xml"))
            {
                File.Delete("SpectralDictionary.xml");
            }
            if (File.Exists("dictionary.xml"))
            {
                File.Delete("dictionary.xml");
            }
            if (File.Exists("dictionary2.xml"))
            {
                File.Delete("dictionary2.xml");
            }
            if (File.Exists("dictionary3.xml"))
            {
                File.Delete("dictionary3.xml");
            }
            if (File.Exists("dictionary4.xml"))
            {
                File.Delete("dictionary4.xml");
            }
            if (File.Exists("dictionary5.xml"))
            {
                File.Delete("dictionary5.xml");
            }
            if (File.Exists("absorber-Fat.txt"))
            {
                File.Delete("absorber-Fat.txt");
            }
            if (File.Exists("absorber-Hb.txt"))
            {
                File.Delete("absorber-Hb.txt");
            }
            if (File.Exists("absorber-HbO2.txt"))
            {
                File.Delete("absorber-HbO2.txt");
            }
            if (File.Exists("absorber-H2O.txt"))
            {
                File.Delete("absorber-H2O.txt");
            }
            if (File.Exists("absorber-Melanin.txt"))
            {
                File.Delete("absorber-Melanin.txt");
            }
            if (File.Exists("absorber-Nigrosin.txt"))
            {
                File.Delete("absorber-Nigrosin.txt");
            }
#endif
        }

        /// <summary>
        /// validate loading the default spectral database
        /// </summary>
        [Test]
        public void validate_loading_spectral_database()
        {
            var _testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            Assert.IsNotNull(_testDictionary);
        }

        /// <summary>
        /// Validate the serialization of the spectral database
        /// </summary>
        [Test]
        public void validate_serializing_spectral_database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            testDictionary.WriteToXML("SpectralDictionary.xml");
#if SILVERLIGHT
            Assert.IsTrue(_objStore.FileExists("SpectralDictionary.xml"));
#else
            Assert.IsTrue(File.Exists("SpectralDictionary.xml"));
#endif
        }

        /// <summary>
        /// Validate the deserialization of the spectral database
        /// </summary>
        [Test]
        public void validate_deserializing_spectral_database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            testDictionary.WriteToXML("dictionary.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary.xml");
            Assert.IsNotNull(Dvalues);
        }

        /// <summary>
        /// Validate the data after serializing and deserializing the database
        /// </summary>
        [Test]
        public void validate_serialized_data()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            testDictionary.WriteToXML("dictionary2.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary2.xml");
            Assert.AreEqual(Dvalues["Hb"].Wavelengths[5], testDictionary["Hb"].Wavelengths[5]);
        }

        /// <summary>
        /// validate loading spectral data from tab-delimited file
        /// </summary>
        [Test]
        public void validate_loading_spectral_database_from_tsv()
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
            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(myChromophoreList, stream, 2);
            testDictionary.WriteToXML("dictionary3.xml");
#if SILVERLIGHT
            Assert.IsTrue(_objStore.FileExists("dictionary3.xml"));
#else
            Assert.IsTrue(File.Exists("dictionary3.xml"));
#endif
        }

        /// <summary>
        /// validate loading spectral database and header from tab-delimited file with conversion
        /// </summary>
        [Test]
        public void validate_Loading_Spectral_Database_and_header_from_tsv()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream);
            testDictionary.WriteToXML("dictionary4.xml");
#if SILVERLIGHT
            Assert.IsTrue(_objStore.FileExists("dictionary4.xml"));
#else
            Assert.IsTrue(File.Exists("dictionary4.xml"));
#endif
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
            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream);
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
            // multiply the value by ln(10)
            double k =  Math.Log(10);
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

            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream, false);
            testDictionary.WriteToXML("dictionary5.xml");
#if SILVERLIGHT
            Assert.IsTrue(_objStore.FileExists("dictionary5.xml"));
#else
            Assert.IsTrue(File.Exists("dictionary5.xml"));
#endif
        }

        /// <summary>
        /// validate writing the tab-delimited text files
        /// </summary>
        [Test]
        public void validate_write_text_files()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            SpectralDatabase.WriteDatabaseToFiles(testDictionary);
#if SILVERLIGHT
            Assert.IsTrue(_objStore.FileExists("absorber-Fat.txt"));
            Assert.IsTrue(_objStore.FileExists("absorber-H2O.txt"));
            Assert.IsTrue(_objStore.FileExists("absorber-Hb.txt"));
            Assert.IsTrue(_objStore.FileExists("absorber-HbO2.txt"));
            Assert.IsTrue(_objStore.FileExists("absorber-Melanin.txt"));
            Assert.IsTrue(_objStore.FileExists("absorber-Nigrosin.txt"));
#else
            Assert.IsTrue(File.Exists("absorber-Fat.txt"));
            Assert.IsTrue(File.Exists("absorber-H2O.txt"));
            Assert.IsTrue(File.Exists("absorber-Hb.txt"));
            Assert.IsTrue(File.Exists("absorber-HbO2.txt"));
            Assert.IsTrue(File.Exists("absorber-Melanin.txt"));
            Assert.IsTrue(File.Exists("absorber-Nigrosin.txt"));
#endif
        }
    }
}
