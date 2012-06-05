using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vts.IO;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// This static class provides simple, application-wide access to the loaded spectra. 
    /// </summary>
    public static class SpectralDatabase
    {
        /// <summary>
        /// Private property to store the loaded database
        /// </summary>
        private static ChromophoreSpectrumDictionary InternalDictionary
        {
            get
            {
                if (_internalDictionary == null)
                    _internalDictionary = GetDatabaseFromFile();
                return _internalDictionary;
            }
        }

        private static ChromophoreSpectrumDictionary _internalDictionary;

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

        /// <summary>
        /// Returns a dictionary of Chromophore spectrum from the file SpectralDictionary.xml in resources
        /// </summary>
        /// <returns>Dictionary of Chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary GetDatabaseFromFile()
        {
            return FileIO.ReadFromXMLInResources<ChromophoreSpectrumDictionary>("Modeling/Spectroscopy/Resources/SpectralDictionary.xml", "Vts");
        }

        /// <summary>
        /// Returns a dictionary of Chromophore spectrum from the specified file
        /// </summary>
        /// <returns>Dictionary of Chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary GetDatabaseFromFile(string fileName)
        {
            return FileIO.ReadFromXML<ChromophoreSpectrumDictionary>(fileName);
        }

        /// <summary>
        /// Appends a new chromophore spectra dictionary created from a tab-delimited stream onto an existing dictionary of chromophore spectra
        /// </summary>
        /// <param name="existingDictionary">The existing dictionary to which to append</param>
        /// <param name="chromophoreSpectrumData">Header data for the chromophore spectrum</param>
        /// <param name="fileStream">The file stream</param>
        /// <param name="startLine">Line number to start reading data</param>
        /// <returns>The new dictionary of chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary AppendDatabaseFromFile(ChromophoreSpectrumDictionary existingDictionary, List<ChromophoreSpectrum> chromophoreSpectrumData, Stream fileStream, int startLine)
        {
            //create a new dictionary
            ChromophoreSpectrumDictionary chromophoreDictionary = CreateDatabaseFromFile(chromophoreSpectrumData, fileStream, startLine);
            foreach (var item in chromophoreDictionary)
            {
                existingDictionary.Add(item.Key, item.Value);
            }
            return existingDictionary;
        }

        /// <summary>
        /// Appends a new chromophore spectra dictionary created from a tab-delimited stream onto an existing dictionary of chromophore spectra
        /// </summary>
        /// <param name="existingDictionary">The existing dictionary to which to append</param>
        /// <param name="fileStream">The file stream</param>
        /// <returns>The new dictionary of chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary AppendDatabaseFromFile(ChromophoreSpectrumDictionary existingDictionary, Stream fileStream)
        {
            //create a new dictionary
            ChromophoreSpectrumDictionary chromophoreDictionary = CreateDatabaseFromFile(fileStream);
            foreach (var item in chromophoreDictionary)
            {
                existingDictionary.Add(item.Key, item.Value);
            }
            return existingDictionary;
        }

        /// <summary>
        /// Appends a new chromophore spectra dictionary created from a tab-delimited stream onto an existing dictionary of chromophore spectra
        /// </summary>
        /// <param name="existingDictionary">The existing dictionary to which to append</param>
        /// <param name="fileStream">The file stream</param>
        /// <param name="convert">boolean value to determine if the data should be converted</param>
        /// <returns>The new dictionary of chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary AppendDatabaseFromFile(ChromophoreSpectrumDictionary existingDictionary, Stream fileStream, bool convert)
        {
            //create a new dictionary
            ChromophoreSpectrumDictionary chromophoreDictionary = CreateDatabaseFromFile(fileStream, convert);
            foreach (var item in chromophoreDictionary)
            {
                existingDictionary.Add(item.Key, item.Value);
            }
            return existingDictionary;
        }

        /// <summary>
        /// Creates a dictionary of chromophore spectra from a file stream of tab-delimited data starting at a given line number
        /// Tab delimited data is in the format: Wavelength  1st Column  2nd Column  3rd Column  ...
        /// </summary>
        /// <param name="chromophoreSpectrumData">Header data for the chromophore spectrum</param>
        /// <param name="fileStream">The file stream</param>
        /// <param name="startLine">Line number to start reading data</param>
        /// <returns>The dictionary of chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary CreateDatabaseFromFile(List<ChromophoreSpectrum> chromophoreSpectrumData, Stream fileStream, int startLine)
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
            ChromophoreSpectrumDictionary chromDictionary = new ChromophoreSpectrumDictionary();

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

                                    double valEntry = Convert.ToDouble(row[i+1]);
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
                throw new Exception(e.Message);
            }
            return chromDictionary;
        }

        /// <summary>
        /// Creates a dictionary of chromophore spectra from a file stream of tab-delimited data, converts the data to work in the VTS
        /// The header data is written as a comment line starting with %
        /// Tab delimited data is in the format: Wavelength  1st Column  2nd Column  3rd Column  ...
        /// </summary>
        /// <param name="fileStream">The file stream of spectral data</param>
        /// <returns>The dictionary of chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary CreateDatabaseFromFile(Stream fileStream)
        {
            return CreateDatabaseFromFile(fileStream, true);
        }

        /// <summary>
        /// Creates a dictionary of chromophore spectra from a file stream of tab-delimited data, converts the data to work in the VTS
        /// The header data is written as a comment line starting with %
        /// Tab delimited data is in the format: Wavelength  1st Column  2nd Column  3rd Column  ...
        /// </summary>
        /// <param name="fileStream">The file stream of spectral data</param>
        /// <param name="convert">Boolean which is true if the data should be converted</param>
        /// <returns>The dictionary of chromophore spectrum</returns>
        public static ChromophoreSpectrumDictionary CreateDatabaseFromFile(Stream fileStream, bool convert)
        {
            //if the file stream is null return a null dictionary
            if (fileStream == null)
            {
                return null;
            }

            //create a new dictionary
            ChromophoreSpectrumDictionary chromophoreDictionary = new ChromophoreSpectrumDictionary();

            //create a list of ChromophoreSpectrum
            List<ChromophoreSpectrum> ChromophoreList = new List<ChromophoreSpectrum>();

            // create a list of wavelengths
            List<double> wavelengths = new List<double>();
            // create a list of list of values
            List<List<double>> valuesList = new List<List<double>>();

            try
            {
                using (StreamReader readFile = new StreamReader(fileStream))
                {
                    string line;
                    string[] headerrow;
                    string[] row;
                    string[] header;
                    int hcolumns;
                    int columns;

                    line = readFile.ReadLine();
                    //check for the comment line where the header data is located
                    if (line.StartsWith("%"))
                    {
                        line = line.Substring(1);
                    }
                    headerrow = line.Split('\t'); //file is separated by tabs

                    //if the number of columns is not greater that 2
                    if (headerrow.Length < 2)
                    {
                        //error, there must be at least 2 columns (4 values in the header)
                        throw new Exception("There are not enough columns in the header, header is wavelength wavelength_units chromophore_absorber_name units");
                    }
                    //get the number of columns in the first line of data
                    hcolumns = headerrow.Length; //each column has a name and unit value

                    //read the second line of data, check that the number of columns match
                    line = readFile.ReadLine();
                    row = line.Split('\t'); //file is separated by tabs
                    columns = row.Length;

                    //the number of columns of data is equal to the number of header columns
                    if (hcolumns == columns)
                    {
                        //the first column must have a header value of LAMBDA
                        if (!headerrow[0].StartsWith("LAMBDA", StringComparison.CurrentCultureIgnoreCase))
                        {
                            //error, the first column is lambda
                            throw new Exception("First column must be lambda");
                        }
                        //split the units from the name
                        header = headerrow[0].Split(' ');
                        WavelengthUnit wavelengthUnit = SpectralConverter.getWavelengthUnit(header[1]);
                        AbsorptionCoefficientUnit absorptionCoefficientUnit;
                        MolarUnit molarUnit;

                        //loop through the remaining columns and write the header data
                        for (int i = 1; i < hcolumns; i++)
                        {
                            //split the units from the name
                            header = headerrow[i].Split(' ');
                            if (header.Length != 2)
                            {
                                //error, there must be a name and unit value in the  header
                                throw new Exception("The header columns must be name<space>units");
                            }
                            string name = header[0].ToString(); //get the name of the chromophore absorber
                            ChromophoreType chromophoreType = (ChromophoreType)Enum.Parse(typeof(ChromophoreType), name, true);
                            //get the chromophore coefficient type
                            ChromophoreCoefficientType chromophoreCoefficientType = chromophoreType.GetCoefficientType();
                            //get the absorption coefficient units and the molar units
                            //parse the value of header[1] - the units
                            absorptionCoefficientUnit = SpectralConverter.getAbsorptionCoefficientUnit(header[1]);
                            molarUnit = SpectralConverter.getMolarUnit(header[1]);
                            //write the values to the dictionary
                            ChromophoreSpectrum CS = new ChromophoreSpectrum(new List<double>(), new List<double>(), name, chromophoreCoefficientType, absorptionCoefficientUnit, molarUnit, wavelengthUnit);
                            ChromophoreList.Add(CS);
                        }

                        //loop through the columns and create the lists (ignore the wavelength column)
                        for (int i = 1; i < columns; i++)
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

                                if (row.Length != columns)
                                {
                                    throw new Exception("Invalid data at line: " + (wavelengths.Count + 2).ToString());
                                }
                                //write the wavelength value once
                                double wlEntry = convert ? Convert.ToDouble(row[0]).ConvertWavelength(wavelengthUnit) : Convert.ToDouble(row[0]);
                                wavelengths.Add((double)wlEntry);

                                //loop through the spectra and get the data
                                for (int i = 0; i < columns - 1; i++)
                                {
                                    //need to multiply MolarAbsorptionCoefficients by ln(10)
                                    double k = 1.0;
                                    if (ChromophoreList[i].ChromophoreCoefficientType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
                                    {
                                        k = Math.Log(10);
                                    }
                                    double valEntry = convert ? Convert.ToDouble(row[i+1]).ConvertCoefficient(ChromophoreList[i].AbsorptionCoefficientUnit, ChromophoreList[i].MolarUnit) : Convert.ToDouble(row[i+1]);
                                    valuesList[i].Add((double)valEntry * k);

                                }
                            }
                        } while ((line = readFile.ReadLine()) != null);

                        //loop through the spectra and create the dictionary
                        for (int i = 0; i < columns - 1; i++)
                        {
                            ChromophoreList[i].Wavelengths = wavelengths;
                            ChromophoreList[i].Spectrum = valuesList[i];
                            //if the data was converted, rewrite the absorption coefficient units and the molar units
                            if (convert)
                            {
                                ChromophoreList[i].AbsorptionCoefficientUnit = AbsorptionCoefficientUnit.InverseMillimeters;
                                ChromophoreList[i].MolarUnit = MolarUnit.MicroMolar;
                                ChromophoreList[i].WavelengthUnit = WavelengthUnit.Nanometers;
                            }
                            chromophoreDictionary.Add(ChromophoreList[i].Name, ChromophoreList[i]);
                        }
                    }
                    else
                    {
                        //error, the data and values do not match
                        throw new Exception("The chromophore data header columns and data columns do not match");
                    }
                }
            }
            catch (Exception e)
            {
                //catch the error
                throw new Exception(e.Message);
            }
            return chromophoreDictionary;
        }

        /// <summary>
        /// Writes the Chromophore dictionary to separate text files
        /// </summary>
        /// <param name="ChromophoreDictionary">The dictionary to write</param>
        public static void WriteDatabaseToFiles(ChromophoreSpectrumDictionary ChromophoreDictionary)
        {
            //loop through each of the ChromophoreSpectrum objects
            foreach (var item in ChromophoreDictionary)
            {
                ChromophoreSpectrum CS = item.Value;
                StringBuilder cd = new StringBuilder();
                //Get the spectral units
                string units = SpectralConverter.getSpectralUnit(CS.MolarUnit, CS.AbsorptionCoefficientUnit);
                string wavelengthUnits = SpectralConverter.getWavelengthUnit(CS.WavelengthUnit);

                //write the first line with the header - LAMBDA<space>units<tab>Name<space>units
                cd.AppendLine("%LAMBDA " + wavelengthUnits + "\t" + CS.Name + " " + units);
                int counter = 0;
                foreach (var wavelength in CS.Wavelengths)
                {
                    //need to divide MolarAbsorptionCoefficients by ln(10)
                    double k = 1.0;
                    if (CS.ChromophoreCoefficientType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
                    {
                        k = Math.Log(10);
                    }
                    var spectrum = CS.Spectrum[counter] / k;
                    cd.AppendLine(wavelength + "\t" + spectrum);
                    counter++;
                }
                //write to text file
                FileIO.WriteToTextFile(cd.ToString(), "absorber-" + CS.Name + ".txt");
            }
        }

        ///// <summary>
        ///// Returns a dictionary of chromophore spectrum from the specified file
        ///// </summary>
        ///// <param name="filename">Name of the XML file</param>
        ///// <returns>Dictionary of Chromophore spectrum</returns>
        //public static Dictionary<string, ChromophoreSpectrum> GetDatabaseFromFile(string filename)
        //{
        //    // Keyed by name, so that it's extensible by other users (other users can't create new enums...)
        //    Dictionary<string, ChromophoreSpectrum> chromDictionary = new Dictionary<string, ChromophoreSpectrum>();
        //    Stream stream = StreamFinder.GetFileStreamFromResources("Modeling/Spectroscopy/Resources/" + filename, "Vts");
        //    if (stream == null)
        //        throw new NullReferenceException("Can not open database file");
        //    XElement element = XElement.Load(XmlReader.Create(stream));

        //    foreach (XElement chromEntry in element.Elements())
        //    {
        //        string name = chromEntry.Attribute("Name").Value;
        //        var coeffString = chromEntry.Attribute("DataContent").Value;
        //        AbsorptionCoefficientUnit muaUnit = (AbsorptionCoefficientUnit)Enum.Parse(typeof(AbsorptionCoefficientUnit), chromEntry.Attribute("AbsorptionCoefficientUnits").Value, true);
        //        MolarUnit molarUnit = (MolarUnit)Enum.Parse(typeof(MolarUnit), chromEntry.Attribute("MolarUnits").Value, true);
        //        //AbsorptionCoefficientUnits muaUnits = (AbsorptionCoefficientUnits)Enum.Parse(typeof(AbsorptionCoefficientUnits), chromEntry.Attribute("DataUnits").Value, true);
        //        var coeffType = (ChromophoreCoefficientType)Enum.Parse(typeof(ChromophoreCoefficientType), coeffString, true);
        //        //need to multiply MolarAbsorptionCoefficients by ln(10)
        //        double k = 1.0;
        //        if (coeffType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
        //        {
        //            k = Math.Log(10);

        //        }
        //        //else if (coeffType == ChromophoreCoefficientType.PercentAbsorptionCoefficient)
        //        //{

        //        //}

        //        // populate list of wavelengths
        //        List<double> wavelengths = new List<double>();
        //        XElement wavelengthCollection = chromEntry.Element("Wavelengths");
        //        foreach (XElement wlEntry in wavelengthCollection.Descendants())
        //        {
        //            wavelengths.Add((double)wlEntry);
        //        }

        //        // populate list of values
        //        List<double> values = new List<double>();
        //        XElement valueCollection = chromEntry.Element("Values");
        //        foreach (XElement valEntry in valueCollection.Descendants())
        //        {


        //            values.Add((double)valEntry * k);
        //        }

        //        ChromophoreSpectrum c = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnit, molarUnit);

        //        chromDictionary.Add(name, c);
        //    }

        //    return chromDictionary;
        //}
    }
}
