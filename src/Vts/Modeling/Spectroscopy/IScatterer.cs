namespace Vts.SpectralMapping
{
    /// <summary>
    /// Interface contract for all Scatterer implementations
    /// </summary>
    public interface IScatterer
    {
        /// <summary>
        /// The scatterer type as defined by the enum <see cref="Vts.ScatteringType">ScatteringType</see>
        /// </summary>
        ScatteringType ScattererType { get; }

        /// <summary>
        /// Returns the anisotropy coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The anisotropy coefficient g</returns>
        double GetG(double wavelength);
        
        /// <summary>
        /// Returns the scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The scattering coefficient Mus</returns>
        double GetMus(double wavelength);

        /// <summary>
        /// Returns the reduced scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        double GetMusp(double wavelength);
    }
}
