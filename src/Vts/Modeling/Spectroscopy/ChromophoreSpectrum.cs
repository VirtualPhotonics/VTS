using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// A class representing the chromophore spectrum data
    /// </summary>
    [KnownType(typeof(ChromophoreSpectrum))]
    public class ChromophoreSpectrum : ISpectrum
    {
        /// <summary>
        /// Constructor to create the chromophore spectrum
        /// </summary>
        /// <param name="wavelengths">A list of wavelengths</param>
        /// <param name="spectrum">A list of spectral values</param>
        /// <param name="name">The name of the chromophore absorber</param>
        /// <param name="coeffType">The chromophore coefficient type</param>
        /// <param name="absUnits">The absorption coefficient units</param>
        /// <param name="molarUnit">The molar units</param>
        public ChromophoreSpectrum(List<double> wavelengths, List<double> spectrum, string name, ChromophoreCoefficientType coeffType, AbsorptionCoefficientUnit absUnits, MolarUnit molarUnit)
        {
            ChromophoreCoefficientType = coeffType;
            AbsorptionCoefficientUnit = absUnits;
            MolarUnit = molarUnit;
            Name = name;
            Spectrum = spectrum;
            Wavelengths = wavelengths;
        }

        /// <summary>
        /// Default constructor to create the chromophore spectrum
        /// </summary>
        public ChromophoreSpectrum()
            : this(new List<double>(),
                   new List<double>(),
                   "",
                   ChromophoreCoefficientType.FractionalAbsorptionCoefficient,
                   AbsorptionCoefficientUnit.InverseMillimeters,
                   MolarUnit.Molar)
        {
        }

        /// <summary>
        /// A list of doubles representing the spectral values
        /// </summary>
        public List<double> Spectrum { get; set; }
        /// <summary>
        /// A list of doubles representing the wavelengths
        /// </summary>
        public List<double> Wavelengths { get; set; }
        /// <summary>
        /// The chromophore coefficient type
        /// </summary>
        public ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }      
        //public AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
        /// <summary>
        /// The absorption coefficient units
        /// </summary>
        public AbsorptionCoefficientUnit AbsorptionCoefficientUnit { get; set; }
        /// <summary>
        /// The molar units
        /// </summary>
        public MolarUnit MolarUnit { get; set; }
        /// <summary>
        /// Name of the chromophore absorber
        /// </summary>
        public string Name { get; set; }

        #region ISpectrum Members

        /// <summary>
        /// Linearly interpolates known spectra to attain spectral value
        /// </summary>
        /// <param name="wavelength">the wavelength at which to attain the spectral value</param>
        /// <returns>the spectral value as a double</returns>
        public double GetSpectralValue(double wavelength)
        {
            return Vts.Common.Math.Interpolation.interp1(Wavelengths, Spectrum, wavelength);
        }

        #endregion
    }

    //[DataContract]
    //public class ChromophoreSpectrum : ISpectrum
    //{
    //    [DataMember]
    //    public IList<double> Spectrum { get; set; }
    //    [DataMember]
    //    public IList<double> Wavelengths { get; set; }
    //    [DataMember]
    //    public ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
    //    [DataMember]
    //    public AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
    //    [DataMember(Name="CSName")]
    //    public string Name { get; set; }

    //    public ChromophoreSpectrum(IList<double> wavelengths, IList<double> spectrum, string name, ChromophoreCoefficientType coeffType, AbsorptionCoefficientUnits absUnits)
    //    {
    //        ChromophoreCoefficientType = coeffType;
    //        AbsorptionCoefficientUnits = absUnits;
    //        Name = name;
    //        Spectrum = spectrum;
    //        Wavelengths = wavelengths;
    //    }

    //    #region ISpectrum Members

    //    /// <summary>
    //    /// Linearly interpolates known spectra to attain spectral value
    //    /// </summary>
    //    /// <param name="wavelength"></param>
    //    /// <returns></returns>
    //    public double GetSpectralValue(double wavelength)
    //    {
    //        return Vts.Common.Math.Interpolation.interp1(Wavelengths, Spectrum, wavelength);
    //    }

    //    #endregion
    //}
}
