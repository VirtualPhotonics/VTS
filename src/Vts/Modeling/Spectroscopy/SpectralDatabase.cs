using System.Collections.Generic;

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
                if (_InternalDictionary == null)
                    _InternalDictionary = SpectralDatabaseLoader.GetDatabaseFromFile("SpectraData1.xml");
                return _InternalDictionary;
            }
        }

        private static Dictionary<string, ChromophoreSpectrum> _InternalDictionary;
        //private static Dictionary<string, ChromophoreCoefficientType> _InternalDictionaryCoefficientType;
        /// <summary>
        /// Method to retrieve a spectral value keyed by it's name in the database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="wavelength"></param>
        /// <returns></returns>
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
        
        
    }
}
