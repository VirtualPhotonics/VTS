using System;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// Returns scattering values based on Steve Jacques' Skin Optics Summary:
    /// http://omlc.ogi.edu/news/jan98/skinoptics.html
    /// This returned reduced scattering follows the approximate formula:
    /// mus' = A1*lamda(-b1) + A2*lambda(-b2)
    /// </summary>
    public class PowerLawScatterer : BindableObject, IScatterer
    {
        private double _a;
        private double _b;
        private double _c;
        private double _d;

        /// <summary>
        /// Constructs a power law scatterer; i.e. mus' = a*lamda^-b + c*lambda^-d
        /// </summary>
        /// <param name="a">The first prefactor</param>
        /// <param name="b">The first exponent</param>
        /// <param name="c">The second prefactor</param>
        /// <param name="d">The second exponent</param>
        public PowerLawScatterer(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        /// <summary>
        /// Creates a power law scatterer; i.e. mus' = a*lambda^-b
        /// </summary>
        /// <param name="a">The first prefactor</param>
        /// <param name="b">The first exponent</param>
        public PowerLawScatterer(double a, double b)
            : this(a,b,0.0,0.0)
        {
        }

        /// <summary>
        /// Creates a power law scatterer using the specified tissue type
        /// </summary>
        /// <param name="tissueType">The tissue type as defined by the Enum <see cref="Vts.TissueType">TissueType</see></param>
        public PowerLawScatterer(TissueType tissueType)
        {
            SetTissueType(tissueType);
        }

        /// <summary>
        /// Creates a power law scatterer with a tissue type of Custom
        /// </summary>
        public PowerLawScatterer() 
            : this(TissueType.Custom)
        {
        }

        /// <summary>
        /// Sets the values for the first prefactor, the first exponent, the second prefactor and the second exponent for the given tissue type
        /// Skin: 1.2, 1.42, 0.0, 0.0
        /// Breast (pre-menopause): 0.67, 0.95, 0.0, 0.0
        /// Breast (post-menopause): 0.72, 0.58, 0.0, 0.0
        /// Brain (white matter): 3.56, 0.84, 0.0, 0.0
        /// Brain (grey matter): 0.56, 1.36, 0.0, 0.0
        /// Liver: 0.84, 0.55, 0.0, 0.0
        /// Custom: 1.0, 0.1, 0.0, 0.0
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        public void SetTissueType(TissueType tissueType)
        {
            switch (tissueType)
            {
                case (TissueType.Skin):
                    A = 1.2;
                    B = 1.42;
                    C = 0.0;
                    D = 0.0;
                    break;
                case TissueType.BreastPreMenopause:
                    A = 0.67;
                    B = 0.95;
                    C = 0.0;
                    D = 0.0;
                    break;
                case TissueType.BreastPostMenopause:
                    A = 0.72;
                    B = 0.58;
                    C = 0.0;
                    D = 0.0;
                    break;
                case (TissueType.BrainWhiteMatter):
                    A = 3.56;
                    B = 0.84;
                    C = 0.0;
                    D = 0.0;
                    break;
                case (TissueType.BrainGrayMatter):
                    A = 0.56;
                    B = 1.36;
                    C = 0.0;
                    D = 0.0;
                    break;
                case (TissueType.Liver):
                    A = 0.84;
                    B = 0.55;
                    C = 0.0;
                    D = 0.0;
                    break;
                case (TissueType.Custom):
                    A = 1;
                    B = 0.1;
                    C = 0.0;
                    D = 0.0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tissueType");
            }
        }

        /// <summary>
        /// Scattering type, set to power law
        /// </summary>
        public ScatteringType ScattererType { get { return ScatteringType.PowerLaw; } }

        /// <summary>
        /// The first prefactor
        /// </summary>
        public double A
        {
            get { return _a; }
            set 
            {
                _a = value;
                this.OnPropertyChanged("A");
            }
        }

        /// <summary>
        /// The first exponent
        /// </summary>
        public double B
        {
            get { return _b; }
            set
            {
                _b = value;
                this.OnPropertyChanged("B");
            }
        }

        /// <summary>
        /// The second prefactor
        /// </summary>
        public double C
        {
            get { return _c; }
            set
            {
                _c = value;
                this.OnPropertyChanged("C");
            }
        }

        /// <summary>
        /// The second exponent
        /// </summary>
        public double D
        {
            get { return _d; }
            set
            {
                _d = value;
                this.OnPropertyChanged("D");
            }
        }

        /// <summary>
        /// Returns mus' based on Steve Jacques' Skin Optics Summary:
        /// http://omlc.ogi.edu/news/jan98/skinoptics.html
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        public double GetMusp(double wavelength)
        {
            return A * Math.Pow(wavelength/1000, - B) + C * Math.Pow(wavelength/1000, - D);
        }

        /// <summary>
        /// Returns a fixed g (scattering anisotropy) of 0.8
        /// </summary>
        /// <param name="wavelength">The wavelength, in nanometers</param>
        /// <returns>The scattering anisotropy. This is the cosine of the average scattering angle.</returns>
        public double GetG(double wavelength) { return 0.8; }

        /// <summary>
        /// Returns mus based on mus' and g 
        /// </summary>
        /// <param name="wavelength">The wavelength, in nanometers</param>
        /// <returns>The scattering coefficient, mus</returns>
        public double GetMus(double wavelength) { return GetMusp(wavelength) / (1.0 - GetG(wavelength)); }

    }
}
