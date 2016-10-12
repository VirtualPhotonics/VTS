using System;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// Class to represent a chromophore absorber
    /// </summary>
    public class ChromophoreAbsorber : BindableObject, IChromophoreAbsorber
    {
        private string _Name;
        private double _Concentration;
        private ChromophoreCoefficientType _ChromophoreCoefficientType;

        /// <summary>
        /// Creates a chromophore with a particular value. Name is used to reference the spectral database.
        /// </summary>
        /// <param name="name">Name of the chromophore</param>
        /// <param name="concentration">Concentration</param>
        /// <param name="chromophoreCoefficientType">Chromophore coefficient type defined by the Enum <see cref="Vts.ChromophoreCoefficientType">ChromophoreCoefficientType</see></param>
        public ChromophoreAbsorber(string name, double concentration, ChromophoreCoefficientType chromophoreCoefficientType)
        {
            Name = name;
            Concentration = concentration;
            ChromophoreCoefficientType = chromophoreCoefficientType;
        }

        /// <summary>
        /// Overload for creating chromophore with simple table lookup. Only works for "built-in" chromophore types
        /// </summary>
        /// <param name="chromophoreType">Chromophore type as defined by the Enum <see cref="Vts.ChromophoreType">ChromophoreType</see></param>
        /// <param name="concentration">Concentration</param>
        public ChromophoreAbsorber(ChromophoreType chromophoreType, double concentration)
            : this(
                chromophoreType.ToString(),
                concentration,
                chromophoreType.GetCoefficientType()) { }

        /// <summary>
        /// Chromophore coefficient type defined by the Enum <see cref="Vts.ChromophoreCoefficientType">ChromophoreCoefficientType</see>
        /// </summary>
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

        /// <summary>
        /// Name of the chromophore
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Concentration
        /// </summary>
        public double Concentration
        {
            get { return _Concentration; }
            set
            {
                _Concentration = value;
                OnPropertyChanged("Concentration");
            }
        }

        /// <summary>
        /// Concentration units based on the chromophore coefficient type
        /// </summary>
        public string ConcentrationUnits
        {
            get
            {
                return _ChromophoreCoefficientType.GetInternationalizedString();
            }
        }

        /// <summary>
        /// Returns the name of the chromophore
        /// </summary>
        /// <returns>Name of the chromophore</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns Mua (absorption coefficient) for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The absorption coefficient Mua</returns>
        public double GetMua(double wavelength)
        {
            // convert extinction coefficients to absorption coefficients
            if (ChromophoreCoefficientType == ChromophoreCoefficientType.MolarAbsorptionCoefficient)
            {
                return Math.Log(10) * Concentration * SpectralDatabase.GetSpectrumValue(Name, wavelength);
            }
            return Concentration * SpectralDatabase.GetSpectrumValue(Name, wavelength);
        }
    }
}
