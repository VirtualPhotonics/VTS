using System.Collections.Generic;

namespace Vts.SpectralMapping
{
    public class ChromophoreSpectrum : ISpectrum
    {
        public IList<double> Spectrum { get; set; }
        public IList<double> Wavelengths { get; set; }
        public ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        public AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
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
