using System.ComponentModel;

namespace Vts.SpectralMapping
{
    public interface IChromophoreAbsorber : IAbsorber, INotifyPropertyChanged
    {
        //ChromophoreType ChromophoreType { get; set; }
        string Name { get; set; }
        double Concentration { get; set; }
        ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        //AbsorptionCoefficientUnits AbsorptionCoefficientUnits { get; set; }
       // ChromDataDistanceUnits DistanceUnits { get; set; }
      //  ChromDataConcentrationUnits ConcentrationUnits { get; set; }
    }
}
