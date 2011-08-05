namespace Vts.SpectralMapping
{
    /// <summary>
    /// An interface for an absorber
    /// </summary>
    public interface IAbsorber
    {
        /// <summary>
        /// Returns Mua (absorption coefficient) for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>Mua</returns>
        double GetMua(double wavelength);
    }
}
