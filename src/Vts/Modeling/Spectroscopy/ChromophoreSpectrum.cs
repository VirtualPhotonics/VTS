using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vts.SpectralMapping
{
    [KnownType(typeof(ChromophoreCoefficientType))]
    [KnownType(typeof(AbsorptionCoefficientUnits))]
    [DataContract]
    public class ChromophoreSpectrum : ISpectrum
    {
        [DataMember]
        public IList<double> Spectrum { get; set; }
        [DataMember]
        public IList<double> Wavelengths { get; set; }
        [DataMember]
        public ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        [DataMember]
        public AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
        [DataMember]
        public string Name { get; set; }

        public ChromophoreSpectrum(IList<double> wavelengths, IList<double> spectrum, string name, ChromophoreCoefficientType coeffType, AbsorptionCoefficientUnits absUnits)
        {
            ChromophoreCoefficientType = coeffType;
            AbsorptionCoefficientUnits = absUnits;
            Name = name;
            Spectrum = spectrum;
            Wavelengths = wavelengths;
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
}
