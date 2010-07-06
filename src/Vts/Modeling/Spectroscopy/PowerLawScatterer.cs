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
        private double _A;
        private double _B;
        private double _C;
        private double _D;

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
        /// <param name="a"></param>
        /// <param name="b"></param>
        public PowerLawScatterer(double a, double b)
            : this(a,b,0.0,0.0)
        {
        }

        public ScatteringType ScattererType { get { return ScatteringType.PowerLaw; } }

        public double A
        {
            get { return _A; }
            set
            {
                _A = value;
                OnPropertyChanged("A");
            }
        }

        public double B
        {
            get { return _B; }
            set
            {
                _B = value;
                OnPropertyChanged("B");
            }
        }

        public double C
        {
            get { return _C; }
            set
            {
                _C = value;
                OnPropertyChanged("C");
            }
        }

        public double D
        {
            get { return _D; }
            set
            {
                _D = value;
                OnPropertyChanged("D");
            }
        }

        public static PowerLawScatterer Create(TissueType tissueType)
        {
            double a, b, c, d;
            switch (tissueType)
            {
                case (TissueType.Skin):
                default:
                    a = 1.2;
                    b = 1.42;
                    c = 0.0;
                    d = 0.0;
                    break;
                case TissueType.BreastPreMenopause:
                    a = 0.67;
                    b = 0.95;
                    c = 0.0;
                    d = 0.0;
                    break;
                case TissueType.BreastPostMenopause:
                    a = 0.72;
                    b = 0.58;
                    c = 0.0;
                    d = 0.0;
                    break;
                case (TissueType.BrainWhiteMatter):
                    a = 3.56;
                    b = 0.84;
                    d = 0.0;
                    c = 0.0;
                    break;
                case (TissueType.BrainGrayMatter):
                    a = 0.56;
                    b = 1.36;
                    c = 0.0;
                    d = 0.0;
                    break;
                case (TissueType.Liver):
                    a = 0.84;
                    b = 0.55;
                    c = 0.0;
                    d = 0.0;
                    break;
            }

            return new PowerLawScatterer(a, b, c, d);
        }

        /// <summary>
        /// Returns mus' based on Steve Jacques' Skin Optics Summary:
        /// http://omlc.ogi.edu/news/jan98/skinoptics.html
        /// </summary>
        /// <param name="wavelength"></param>
        /// <returns></returns>
        public double GetMusp(double wavelength)
        {
            return A * Math.Pow(wavelength/1000, -B) + C * Math.Pow(wavelength/1000, -D);
        }

        /// <summary>
        /// Returns a fixed g (scattering anisotropy) of 0.9
        /// </summary>
        /// <param name="wavelength">The wavelength, in nanometers</param>
        /// <returns>The scattering anisotropy. This is the cosine of the average scattering angle.</returns>
        public double GetG(double wavelength) { return 0.9; }

        /// <summary>
        /// Returns mus based on mus' and g 
        /// </summary>
        /// <param name="wavelength">The wavelength, in nanometers</param>
        /// <returns>The scattering coefficient, mus</returns>
        public double GetMus(double wavelength) { return GetMusp(wavelength) / (1.0 - GetG(wavelength)); }

    }
}
