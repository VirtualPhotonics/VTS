namespace Vts.SpectralMapping
{
    /// <summary>
    /// Interface contract for all absorber implementations
    /// </summary>
    public interface IAbsorber
    {
        /// <summary>
        /// Returns Mua (absorption coefficient) for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The absorption coefficient Mua</returns>
        double GetMua(double wavelength);
    }
}
