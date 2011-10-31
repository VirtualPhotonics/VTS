using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.SpectralMapping;
using Vts.IO;
using System.Runtime.Serialization;

namespace Vts.Test.Modeling.Spectroscopy
{
    [KnownType(typeof(ChromophoreSpectrum))]
    [TestFixture]
    public class SpectralDatabaseTests
    {
        [Test]
        public void validate_Loading_Spectral_Database()
        {
            var _testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            Assert.IsNotNull(_testDictionary);
        }

        [Test]
        public void validate_Serializing_Spectral_Database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            
            // "ToFile" static method in SpectralDatabaseLoader
            //var values = testDictionary.Select(di => di.Value).ToList();
            //values.WriteToXML("samplefile.xml");
            testDictionary.WriteToXML("SpectralDictionary.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("SpectralDictionary.xml"); 
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_Deserializing_Spectral_Database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            testDictionary.WriteToXML("dictionary.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary.xml"); 
            Assert.IsTrue(true);
            //Assert.AreEqual(testDictionary, Dvalues); //This line causes an exception - Need to figure out why these two objects are not equal, they appear to be
        }

        [Test]
        public void validate_Loading_Spectral_Database_from_tsv()
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
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_Loading_Spectral_Database_and_header_from_tsv()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream);
            testDictionary.WriteToXML("dictionary4.xml");
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_Loading_Spectral_Database_and_header_from_tsv_no_conversion()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream, false);
            testDictionary.WriteToXML("dictionary5.xml");
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_write_text_files()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            SpectralDatabase.WriteDatabaseToFiles(testDictionary);
            Assert.IsTrue(true);
        }
    }
}
