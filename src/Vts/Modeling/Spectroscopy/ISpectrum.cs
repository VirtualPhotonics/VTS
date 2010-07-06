namespace Vts.SpectralMapping
{
    /// <summary>
    /// General interface for any spectrum (ie. ChromophoreSpectrum or ScatteringSpectrum)
    /// </summary>
    public interface ISpectrum
    {
        double GetSpectralValue(double wavelength);
        ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
    }
}
