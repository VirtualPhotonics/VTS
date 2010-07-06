namespace Vts.SpectralMapping
{
    public class ChromophoreAbsorber : BindableObject, IChromophoreAbsorber
    {
         private string _Name;
         private double _Concentration;
         private ChromophoreCoefficientType _ChromophoreCoefficientType;

         /// <summary>
         /// Creates a chromophore with a particular value. Name is used to reference the spectral database.
         /// </summary>
         /// <param name="name"></param>
         /// <param name="concentration"></param>
         /// 
         public ChromophoreAbsorber(string name, double concentration, ChromophoreCoefficientType chromophoreCoefficientType)
         {
             Name = name;
             Concentration = concentration;
             ChromophoreCoefficientType = chromophoreCoefficientType;
         }

         /// <summary>
         /// Overload for creating chromophore with simple table lookup. Only works for "built-in" chromophore types
         /// </summary>
         /// <param name="chromophoreType"></param>
         /// <param name="concentration"></param>
         public ChromophoreAbsorber(ChromophoreType chromophoreType, double concentration)
             : this(
                 chromophoreType.ToString(),
                 concentration,
                 chromophoreType.GetCoefficientType()) { }

         public ChromophoreCoefficientType ChromophoreCoefficientType
         {
             get { return _ChromophoreCoefficientType; }
             set
             {
                 _ChromophoreCoefficientType = value;
                 OnPropertyChanged("ChromophoreCoefficientType");
                 OnPropertyChanged("ConcentrationUnits");
             }
         }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public double Concentration
        {
            get { return _Concentration; }
            set
            {
                _Concentration = value;
                OnPropertyChanged("Concentration");
            }
        }

        public string ConcentrationUnits
        {
            get 
            {
                return _ChromophoreCoefficientType.GetInternationalizedString();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public double GetMua(double wavelength)
        {
            return Concentration * SpectralDatabase.GetSpectrumValue(Name, wavelength);
        }
    }
}
