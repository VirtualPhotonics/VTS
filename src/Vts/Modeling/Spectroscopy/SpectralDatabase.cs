using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vts.IO;
using System.Runtime.Serialization;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// This static class provides simple, application-wide access to the loaded spectra. 
    /// Actual loading is delegated to the LoadChromData class.
    /// </summary>
    public static class SpectralDatabase
    {
        /// <summary>
        /// Private property to store the loaded database
        /// </summary>
        private static Dictionary<string, ChromophoreSpectrum> InternalDictionary
        {
            get
            {
                if (_internalDictionary == null)
                    _internalDictionary = GetDatabaseFromFile("SpectraData1.xml");
                return _internalDictionary;
            }
        }

        private static Dictionary<string, ChromophoreSpectrum> _internalDictionary;

        //private static Dictionary<string, ChromophoreCoefficientType> _InternalDictionaryCoefficientType;
        /// <summary>
        /// Method to retrieve a spectral value keyed by it's name in the database
        /// </summary>
        /// <param name="name">Name of the spectra</param>
        /// <param name="wavelength">The wavelength at which to get the value</param>
        /// <returns>Value at the given wavelength</returns>
        public static double GetSpectrumValue(string name, double wavelength)
        {
            ChromophoreSpectrum spectrum = null;
            InternalDictionary.TryGetValue(name, out spectrum);
            if (spectrum != null)
            {
               return spectrum.GetSpectralValue(wavelength);
            }
            else
            {
                return 0;
            }
        }

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
                AbsorptionCoefficientUnits muaUnits = (AbsorptionCoefficientUnits)Enum.Parse(typeof(AbsorptionCoefficientUnits), chromEntry.Attribute("DataUnits").Value, true);
                var coeffType = (ChromophoreCoefficientType)Enum.Parse(typeof(ChromophoreCoefficientType), coeffString, true);
                //need to multiply MolarAbsorptionCoefficients by ln(10)
                double k = 1.0;
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


                    values.Add((double)valEntry * k);
                }

                ChromophoreSpectrum c = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnits);

                chromDictionary.Add(name, c);
            }

            return chromDictionary;
        }

        public static Dictionary<string, ChromophoreSpectrum> GetDatabaseFromFile(Stream fileStream)
        {
            //create a new dictionary
            Dictionary<string, ChromophoreSpectrum> chromDictionary = new Dictionary<string, ChromophoreSpectrum>();
            string name = "";
            string coeffString = "";
            string dataUnits = "";

            // create a list of wavelengths
            List<double> wavelengths = new List<double>();
            // create a list of values
            List<double> values = new List<double>();

            AbsorptionCoefficientUnits muaUnits;
            ChromophoreCoefficientType coeffType;

            if (fileStream == null)
            {
                return null;
            }

            try
            {
                using (StreamReader readFile = new StreamReader(fileStream))
                {
                    string line;
                    string[] row;

                    //read the first line to get the name, coeffitient and data units
                    line = readFile.ReadLine();
                    row = line.Split(',');
                    name = row[0];
                    coeffString = row[1];
                    dataUnits = row[2];

                    muaUnits = (AbsorptionCoefficientUnits)Enum.Parse(typeof(AbsorptionCoefficientUnits), dataUnits, true);
                    coeffType = (ChromophoreCoefficientType)Enum.Parse(typeof(ChromophoreCoefficientType), coeffString, true);

                    //need to multiply MolarAbsorptionCoefficients by ln(10)
                    double k = 1.0;
                    if (coeffType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
                    {
                        k = Math.Log(10);

                    }

                    while ((line = readFile.ReadLine()) != null)
                    {
                        if (!line.StartsWith("//"))
                        {
                            row = line.Split(',');
                            double wlEntry = Convert.ToDouble(row[0]);
                            double valEntry = Convert.ToDouble(row[1]);
                            wavelengths.Add((double)wlEntry);
                            values.Add((double)valEntry * k);
                        }
                    }

                    ChromophoreSpectrum c = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnits);
                    chromDictionary.Add(name, c);
                }
            }
            catch (Exception e)
            {
                //catch the error
            }

            return chromDictionary;
        }
    }
}
