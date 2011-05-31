using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vts.IO;
using System.Runtime.Serialization;

namespace Vts.SpectralMapping
{
    //[KnownType(typeof(ChromophoreSpectrum))]
    public static class SpectralDatabaseLoader
    {
        public static Dictionary<string, ChromophoreSpectrum> GetDatabaseFromFile()
        {
            return GetDatabaseFromFile("SpectraData1.xml");
        }
        public static Dictionary<string, ChromophoreSpectrum> GetDatabaseFromFile(string filename)
         {
             // Keyed by name, so that it's extensible by other users (other users can't create new enums...)
             Dictionary<string, ChromophoreSpectrum> chromDictionary = new Dictionary<string, ChromophoreSpectrum>();
            Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/" + filename, "Vts");
            if (stream == null)
                throw new NullReferenceException("Can not open database file");
            XElement element = XElement.Load(XmlReader.Create(stream));

            foreach (XElement chromEntry in element.Elements())
            {
                string name = chromEntry.Attribute("Name").Value;
                var coeffString = chromEntry.Attribute("DataContent").Value;
                AbsorptionCoefficientUnits muaUnits = (AbsorptionCoefficientUnits)Enum.Parse(typeof(AbsorptionCoefficientUnits),chromEntry.Attribute("DataUnits").Value,true);
                var coeffType = (ChromophoreCoefficientType)Enum.Parse(typeof(ChromophoreCoefficientType), coeffString, true);
                //need to multiply MolarAbsorptionCoefficients by ln(10)
                double k=1.0;
                if (coeffType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
                {
                    k = Math.Log(10);

                }
                //else if (coeffType == ChromophoreCoefficientType.PercentAbsorptionCoefficient)
                //{
                    
                //}

                // populate list of wavelengths
                List<double> wavelengths = new List<double>();
                XElement wavelengthCollection = chromEntry.Element("Wavelengths");
                foreach (XElement wlEntry in wavelengthCollection.Descendants())
                {
                    wavelengths.Add((double)wlEntry);
                }

                // populate list of values
                List<double> values = new List<double>();
                XElement valueCollection = chromEntry.Element("Values");
                foreach (XElement valEntry in valueCollection.Descendants())
                {
                    
                    
                    values.Add((double)valEntry*k);
                }

                ChromophoreSpectrum c = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnits);

                chromDictionary.Add(name, c);
            }
            
            return chromDictionary;
        }
        
        //public static Dictionary<ChromType, Chromophore> ChromDataDC = new Dictionary<ChromType, Chromophore>();


        //public static Dictionary<ChromophoreType, List<double>> ChromData = new Dictionary<ChromophoreType, List<double>>();
        //public static Dictionary<ChromophoreType, List<double>> WavelengthsCollection = new Dictionary<ChromophoreType, List<double>>();
        //public static Dictionary<ChromophoreType, List<Enum>> DataSpecs = new Dictionary<ChromophoreType, List<Enum>>();

        //public LoadChromData()
        //    : this("Data.xml")
        //{
        //    //
        //}

        //public LoadChromData(string filename)
        //{
        //    LoadData(filename);
        //}

        //public void LoadData(string filename)
        //{
        //    XElement element = XElement.Load(filename);

        //    foreach (XElement chromEntry in element.Elements())
        //    {
        //        //read the name of a chromophore from chromEntry
        //        string ChromName = chromEntry.Attribute("Name").Value;
        //        //cast the name to the Enum.ChromType
        //        //will throw a Null Exception Error or an Argument Exception error if chromophore name does not exist in the Enum.ChromType
        //        ChromophoreType chromophoreName = (ChromophoreType)Enum.Parse(typeof(ChromophoreType), ChromName, true);

        //        string ChromDataContent = chromEntry.Attribute("DataContent").Value;
        //        string ChromDataUnit = chromEntry.Attribute("DataUnits").Value;

        //        ChromDataContent chromophoreDataContent = (ChromDataContent)Enum.Parse(typeof(ChromDataContent), ChromDataContent, true);

        //        ChromDataUnit chromophoreDataUnit = (ChromDataUnit)Enum.Parse(typeof(ChromDataUnit), ChromDataUnit, true);

        //        List<Enum> Specs = new List<Enum>();
        //        Specs.Add(chromophoreDataContent);
        //        Specs.Add(chromophoreDataUnit);
        //        DataSpecs.Add(chromophoreName, Specs);
        //        //in the chromophore entry select elements that contain wavelengths
        //        //and write them to the list wavelengthsChrom
        //        //as well as to the dictionary wavelengthsCollection that contains wavelength range of each chromophore
        //        List<double> wavelengthsChrom = new List<double>();
        //        XElement wavelengths = chromEntry.Element("Wavelengths");
        //        foreach (XElement wlEntry in wavelengths.Descendants())
        //        {
        //            wavelengthsChrom.Add((double)wlEntry);
        //        }

        //        WavelengthsCollection.Add(chromophoreName, wavelengthsChrom);

        //        //in the chromophore entry select elements that contain spectra values
        //        //and write them to the list currentChromSpectra
        //        //write to the dictionary chromMua that contains chromophore spectra keyed by chromophore name (of the type Enum.ChromType)
        //        XElement values = chromEntry.Element("Values");
        //        List<double> currentChromSpectra = new List<double>();
        //        foreach (XElement valEntry in values.Descendants())
        //        {
        //            currentChromSpectra.Add((double)valEntry);
        //        }

        //        ChromData.Add(chromophoreName, currentChromSpectra);


        //    }


        //}


        //private static XElement LoadDocument(string filename)
        //{
        //    XmlReader reader = XmlReader.Create(filename);
        //    XElement element = XElement.Load(reader);

        //    return element;
        //}
    }
}
