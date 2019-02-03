using System;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// An intralipid scatterer, based on Mie theory fit to experimental data by van Staveren et al. 
    /// For more info, visit http://omlc.ogi.edu/spectra/intralipid/index.html
    /// </summary>
    public class IntralipidScatterer : BindableObject, IScatterer
    {
        private double _volumeFraction;

        /// <summary>
        /// Creates an intralipid scatterer with the given volume fraction
        /// </summary>
        /// <param name="volumeFraction">Volume fraction</param>
        public IntralipidScatterer(double volumeFraction)
        {
            _volumeFraction = volumeFraction;
        }

        /// <summary>
        /// Creates an intralipid scatterer with a volume fraction of 0.01
        /// </summary>
        public IntralipidScatterer()
            : this (0.01)
        {
        }

        /// <summary>
        /// Scattering type, set to intralipid
        /// </summary>
        public ScatteringType ScattererType { get { return ScatteringType.Intralipid; } }

        /// <summary>
        /// The volume fraction
        /// Note: force value to be between [0, 1]
        /// </summary>
        public double VolumeFraction
        {
            get { return _volumeFraction; }
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                if (value < 0)
                {
                    value = 0;
                }
                _volumeFraction = value;
                OnPropertyChanged("VolumeFraction");
            }
        }

        /// <summary>
        /// Returns the anisotropy coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The anisotropy coeffient g</returns>
        public double GetG(double wavelength)
        {
            return 1.1 - 5.8E-4 * wavelength;
        }

        /// <summary>
        /// Returns the scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The scattering coefficient Mus</returns>
        public double GetMus(double wavelength)
        {
            return _volumeFraction * 2.54E9 * Math.Pow(wavelength, -2.4);
        }

        /// <summary>
        /// Returns the reduced scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        public double GetMusp(double wavelength)
        {
            return GetMus(wavelength) * (1 - GetG(wavelength));
        }
    }
}
