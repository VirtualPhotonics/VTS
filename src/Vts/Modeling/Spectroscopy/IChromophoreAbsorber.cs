using System.ComponentModel;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// A general interface for a chromophore absorber
    /// </summary>
    public interface IChromophoreAbsorber : IAbsorber, INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the chromophore absorber
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The concentration of the chromophore absorber
        /// </summary>
        double Concentration { get; set; }
        /// <summary>
        /// The chromophore coefficient type
        /// </summary>
        ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        //ChromophoreType ChromophoreType { get; set; }
        //AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
        //ChromDataDistanceUnits DistanceUnits { get; set; }
        //ChromDataConcentrationUnits ConcentrationUnits { get; set; }
    }
}
