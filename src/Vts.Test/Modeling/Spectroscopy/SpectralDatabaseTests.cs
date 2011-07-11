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
            testDictionary.WriteToXML("dictionary.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary.xml"); 

            Assert.IsTrue(true);
        }

        [Test]
        public void validate_Deserializing_Spectral_Database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
            testDictionary.WriteToXML("dictionary2.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary2.xml"); 
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_Loading_Spectral_Database_from_tsv()
        {
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/Spectra.txt", "Vts");

            List<ChromophoreSpectrum> myChromophoreList = new List<ChromophoreSpectrum>();
            //create 2 sets of values for the tab delimeted file
            ChromophoreSpectrum c = new ChromophoreSpectrum();
            c.Name = "HbO2";
            c.AbsorptionCoefficientUnits = AbsorptionCoefficientUnits.PerMillimeter;
            c.ChromophoreCoefficientType = ChromophoreCoefficientType.MolarAbsorptionCoefficient;
            myChromophoreList.Add(c);
            ChromophoreSpectrum c2 = new ChromophoreSpectrum();
            c2.Name = "Hb";
            c2.AbsorptionCoefficientUnits = AbsorptionCoefficientUnits.PerMillimeter;
            c2.ChromophoreCoefficientType = ChromophoreCoefficientType.MolarAbsorptionCoefficient;
            myChromophoreList.Add(c2);
            var testDictionary = SpectralDatabase.CreateDatabaseFromFile(myChromophoreList, stream, 2);
            testDictionary.WriteToXML("dictionary3.xml");
            Assert.IsTrue(true);
        }
    }
}
