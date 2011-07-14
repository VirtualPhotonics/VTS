using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vts.SpectralMapping
{
    [KnownType(typeof(ChromophoreSpectrum))]
    public class ChromophoreSpectrum : ISpectrum
    {
        public IList<double> Spectrum { get; set; }
        public IList<double> Wavelengths { get; set; }
        public ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }      
        //public AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
        public AbsorptionCoefficientUnit AbsorptionCoefficientUnit { get; set; }
        public MolarUnit MolarUnit { get; set; }
        public string Name { get; set; }

        public ChromophoreSpectrum(IList<double> wavelengths, IList<double> spectrum, string name, ChromophoreCoefficientType coeffType, AbsorptionCoefficientUnit absUnits, MolarUnit molarUnit)
        {
            ChromophoreCoefficientType = coeffType;
            AbsorptionCoefficientUnit = absUnits;
            MolarUnit = molarUnit;
            Name = name;
            Spectrum = spectrum;
            Wavelengths = wavelengths;
        }

        public ChromophoreSpectrum ()
            : this(new List<double>(), 
                   new List<double>(), 
                   "", 
                   ChromophoreCoefficientType.FractionalAbsorptionCoefficient,
                   AbsorptionCoefficientUnit.InverseMillimeters,
                   MolarUnit.Molar)
	    {
	    }

        #region ISpectrum Members

        /// <summary>
        /// Linearly interpolates known spectra to attain spectral value
        /// </summary>
        /// <param name="wavelength"></param>
        /// <returns></returns>
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
