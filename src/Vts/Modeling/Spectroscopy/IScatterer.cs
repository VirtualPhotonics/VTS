namespace Vts.SpectralMapping
{
    /// <summary>
    /// Interface contract for all Scatterer implementations
    /// </summary>
    public interface IScatterer
    {
        ScatteringType ScattererType { get; }
        double GetG(double wavelength);
        double GetMus(double wavelength);
        double GetMusp(double wavelength);
    }
}
