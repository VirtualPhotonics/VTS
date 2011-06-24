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

        public static Dictionary<string, ChromophoreSpectrum> AppendDatabaseFromFile(Dictionary<string, ChromophoreSpectrum> existingDictionary, List<ChromophoreSpectrum> chromophoreSpectrumData, Stream fileStream, int startLine)
        {
            //create a new dictionary
            Dictionary<string, ChromophoreSpectrum> chromDictionary = CreateDatabaseFromFile(chromophoreSpectrumData, fileStream, startLine);
            foreach (var item in chromDictionary)
            {
                existingDictionary.Add(item.Key, item.Value);
            }
            return existingDictionary;
        }

        public static Dictionary<string, ChromophoreSpectrum> CreateDatabaseFromFile(List<ChromophoreSpectrum> chromophoreSpectrumData, Stream fileStream, int startLine)
        {
            //Get the number of items in the List of ChromophoreSpectrum
            int spectra = chromophoreSpectrumData.Count;
            if (spectra < 1)
            {
                return null;
            }

            if (fileStream == null)
            {
                return null;
            }

            //create a new dictionary
            Dictionary<string, ChromophoreSpectrum> chromDictionary = new Dictionary<string, ChromophoreSpectrum>();

            // create a list of wavelengths
            List<double> wavelengths = new List<double>();
            // create a list of list of values
            List<List<double>> valuesList = new List<List<double>>();

            try
            {
                using (StreamReader readFile = new StreamReader(fileStream))
                {
                    string line;
                    string[] row;

                    //read the first lines to the start of the data
                    for (int i = 1; i < startLine; i++)
                    {
                        line = readFile.ReadLine();
                    }

                    line = readFile.ReadLine();
                    row = line.Split('\t'); //file is separated by tabs

                    //get the number of columns in the first line of data
                    int columns = row.Length - 1;

                    //the number of columns of data is equal to the number of columns - 1
                    if (spectra == columns)
                    {
                        //loop through the columns and create the lists
                        for (int i = 0; i < columns; i++)
                        {
                            //create a list of doubles in the value list
                            List<double> values = new List<double>();
                            valuesList.Add(values);
                        }

                        do
                        {
                            if (!line.StartsWith("%"))
                            {
                                row = line.Split('\t');

                                //write the wavelength value once
                                double wlEntry = Convert.ToDouble(row[0]);
                                wavelengths.Add((double)wlEntry);

                                //loop through the spectra and get the data
                                for (int i = 0; i < spectra; i++)
                                {
                                    //need to multiply MolarAbsorptionCoefficients by ln(10)
                                    double k = 1.0;
                                    if (chromophoreSpectrumData[i].ChromophoreCoefficientType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
                                    {
                                        k = Math.Log(10);
                                    }

                                    double valEntry = Convert.ToDouble(row[i]);
                                    valuesList[i].Add((double)valEntry * k);
                                }
                            }
                        } while ((line = readFile.ReadLine()) != null);

                        //loop through the spectra and create the dictionary
                        for (int i = 0; i < spectra; i++)
                        {
                            chromophoreSpectrumData[i].Wavelengths = wavelengths;
                            chromophoreSpectrumData[i].Spectrum = valuesList[i];
                            chromDictionary.Add(chromophoreSpectrumData[i].Name, chromophoreSpectrumData[i]);
                        }
                    }
                    else
                    {
                        //error, the data and values do not match
                        throw new Exception("The chromophore data columns and the file data do not match");
                    }
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
