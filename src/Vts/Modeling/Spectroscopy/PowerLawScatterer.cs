using System;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// Returns scattering values based on Steve Jacques' Skin Optics Summary:
    /// https://omlc.org/news/jan98/skinoptics.html
    /// This returned reduced scattering follows the approximate formula:
    /// mus' = A1*(lamda/lambda0)(-b1) + A2*(lambda/lambda0)(-b2)
    /// with default value of lambda0=1000nm
    /// </summary>
    public class PowerLawScatterer : BindableObject, IScatterer
    {
        private double _a;
        private double _b;
        private double _c;
        private double _d;
        private double _lambda0;

        /// <summary>
        /// Constructs a power law scatterer; i.e. mus' = a*(lamda/lambda0)^-b + c*(lambda/lambda0)^-d
        /// </summary>
        /// <param name="a">The first prefactor</param>
        /// <param name="b">The first exponent</param>
        /// <param name="c">The second prefactor</param>
        /// <param name="d">The second exponent</param>
        /// <param name="lambda0">Wavelength normalization factor</param>
        public PowerLawScatterer(double a, double b, double c, double d, double lambda0)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            Lambda0 = lambda0;
        }

        /// <summary>
        /// Constructs a power law scatterer; i.e. mus' = a*lamda^-b + c*lambda^-d
        /// </summary>
        /// <param name="a">The first prefactor</param>
        /// <param name="b">The first exponent</param>
        /// <param name="c">The second prefactor</param>
        /// <param name="d">The second exponent</param>
        public PowerLawScatterer(double a, double b, double c, double d)
            : this(a, b, c, d, 1000.0)
        {
        }

        /// <summary>
        /// Creates a power law scatterer; i.e. mus' = a*(lambda/lambda0)^-b
        /// </summary>
        /// <param name="a">The first prefactor</param>
        /// <param name="b">The first exponent</param>
        /// <param name="lambda0">Wavelength normalization factor</param>
        public PowerLawScatterer(double a, double b, double lambda0)
            : this(a, b, 0.0, 0.0, lambda0)
        {
        }

        /// <summary>
        /// Creates a power law scatterer; i.e. mus' = a*lambda^-b
        /// </summary>
        /// <param name="a">The first prefactor</param>
        /// <param name="b">The first exponent</param>
        public PowerLawScatterer(double a, double b)
            : this(a,b,0.0,0.0, 1000.0)
        {
        }

        /// <summary>
        /// Creates a power law scatterer using the specified tissue type
        /// </summary>
        /// <param name="tissueType">The tissue type as defined by the enum <see cref="Vts.TissueType">TissueType</see></param>
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
            Lambda0 = 1000;
            switch (tissueType)
            {
                case TissueType.Skin:
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
                case TissueType.BrainWhiteMatter:
                    A = 3.56;
                    B = 0.84;
                    C = 0.0;
                    D = 0.0;
                    break;
                case TissueType.BrainGrayMatter:
                    A = 0.56;
                    B = 1.36;
                    C = 0.0;
                    D = 0.0;
                    break;
                case TissueType.Liver:
                    A = 0.84;
                    B = 0.55;
                    C = 0.0;
                    D = 0.0;
                    break;
                case TissueType.Custom:
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
        public ScatteringType ScattererType => ScatteringType.PowerLaw;

        /// <summary>
        /// The first prefactor
        /// </summary>
        public double A
        {
            get => _a;
            set 
            {
                _a = value;
                OnPropertyChanged("A");
            }
        }

        /// <summary>
        /// The first exponent
        /// </summary>
        public double B
        {
            get => _b;
            set
            {
                _b = value;
                OnPropertyChanged("B");
            }
        }

        /// <summary>
        /// The second prefactor
        /// </summary>
        public double C
        {
            get => _c;
            set
            {
                _c = value;
                OnPropertyChanged("C");
            }
        }

        /// <summary>
        /// The second exponent
        /// </summary>
        public double D
        {
            get => _d;
            set
            {
                _d = value;
                OnPropertyChanged("D");
            }
        }

        /// <summary>
        /// The wavelength normalization factor
        /// </summary>
        public double Lambda0
        {
            get => _lambda0;
            set
            {
                _lambda0 = value;
                OnPropertyChanged(nameof(Lambda0));
            }
        }

        /// <summary>
        /// Returns mus' based on Steve Jacques' Skin Optics Summary:
        /// https://omlc.ogi.edu/news/jan98/skinoptics.html
        /// and normalizes wavelength by _lambda0 (default=1000[nm])
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        public double GetMusp(double wavelength)
        {
            return A * Math.Pow(wavelength / _lambda0, -B) + C * Math.Pow(wavelength / _lambda0, -D);
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
