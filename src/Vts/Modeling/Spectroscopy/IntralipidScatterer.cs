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

        public IntralipidScatterer(double volumeFraction)
        {
            _volumeFraction = volumeFraction;
        }

        public IntralipidScatterer()
            : this (0.01)
        {
        }

        public ScatteringType ScattererType { get { return ScatteringType.Intralipid; } }

        public double VolumeFraction
        {
            get { return _volumeFraction; }
            set
            {
                _volumeFraction = value;
                OnPropertyChanged("VolumeFraction");
            }
        }

        public double GetG(double wavelength)
        {
            return 1.1 - 5.8E-4 * wavelength;
        }

        public double GetMus(double wavelength)
        {
            return _volumeFraction * 2.54E9 * Math.Pow(wavelength, -2.4);
        }

        public double GetMusp(double wavelength)
        {
            return GetMus(wavelength) * (1 - GetG(wavelength));
        }
    }
}
