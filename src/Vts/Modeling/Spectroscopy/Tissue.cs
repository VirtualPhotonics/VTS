using System;
using System.Collections.Generic;
using System.Linq;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// Class to represent a tissue
    /// </summary>
    public class Tissue : BindableObject, IAbsorber, IScatterer
    {
        /// <summary>
        /// Creates a tissue with the specified absorbers, scatterer and name
        /// </summary>
        /// <param name="absorbers">List of chromophore absorbers</param>
        /// <param name="scatterer">scatterer</param>
        /// <param name="name">Name of the tissue</param>
        public Tissue(IList<IChromophoreAbsorber> absorbers, IScatterer scatterer, string name, double? n =  1.4)
        {
            Absorbers = absorbers;
            Scatterer = scatterer;
            Name = name;
            N = n ?? 1.4;
            //N = n != null ? n.Value : 1.4;
        }

        /// <summary>
        /// Index of refraction (default == 1.4)
        /// </summary>
        public double N { get; set; }

        /// <summary>
        /// Creates a tissue with the specified tissue type
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        public Tissue(TissueType tissueType)
        {
            TissueType = tissueType;
            SetPredefinedTissueDefinitions(tissueType);
            Name = tissueType.ToString();
        }

        /// <summary>
        /// Scatterer type
        /// </summary>
        public ScatteringType ScattererType { get { return Scatterer.ScattererType; } }
        
        /// <summary>
        /// Type of tissue
        /// </summary>
        public TissueType TissueType { get; set; }

        /// <summary>
        /// Name of the tissue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of chromophore absorbers
        /// </summary>
        public IList<IChromophoreAbsorber> Absorbers { get; set; }

        /// <summary>
        /// Scatterer
        /// </summary>
        public IScatterer Scatterer { get; set; } 

        /// <summary>
        /// Set the absorbers and the scatterer for the specified tissue type
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        private void SetPredefinedTissueDefinitions(TissueType tissueType) // this data should be in XML
        {
            TissueType = tissueType;
            Absorbers = TissueProvider.CreateAbsorbers(tissueType);
            Scatterer = TissueProvider.CreateScatterer(tissueType);
            N = 1.4;
        }
        
        /// <summary>
        /// Returns the name of the tissue
        /// </summary>
        /// <returns>The name of the tissue</returns>
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
            double mua = 0.0;
            for (int i = 0; i < Absorbers.Count; i++)
            {
                mua += Absorbers[i].GetMua(wavelength);
            }
            return mua;
        }

        /// <summary>
        /// Returns the reduced scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        public double GetMusp(double wavelength)
        {
            return Scatterer != null ? Scatterer.GetMusp(wavelength) : 0;
        }

        /// <summary>
        /// Returns the anisotropy coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The anisotropy coeffient g</returns>
        public double GetG(double wavelength)
        {
            return Scatterer != null ? Scatterer.GetG(wavelength) : 0;
        }

        /// <summary>
        /// Returns the scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The scattering coefficient Mus</returns>
        public double GetMus(double wavelength)
        {
            return Scatterer != null ? Scatterer.GetMus(wavelength) : 0;
        }

        /// <summary>
        /// Returns the optical properties for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The optical properties</returns>
        public OpticalProperties GetOpticalProperties(double wavelength)
        {
            var mua = GetMua(wavelength);
            var musp = GetMusp(wavelength);
            var g = GetG(wavelength);
            var n = N;
            return new OpticalProperties(mua, musp, g, n);
        }

        /// <summary>
        /// Returns the optical properties for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The optical properties</returns>
        public OpticalProperties[] GetOpticalProperties(double[] wavelengths)
        {
            var opArray = new OpticalProperties[wavelengths.Length];
            for (int i = 0; i < wavelengths.Length; i++)
            {
                opArray[i] = GetOpticalProperties(wavelengths[i]);
            }
            return opArray;
        }
    }

    public static class TissueProvider
    {
        /// <summary>
        /// Creates standard templates lists of absorbers for the specified tissue type
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        public static IChromophoreAbsorber[] CreateAbsorbers(TissueType tissueType)
        {
            // todo: this should come from a file...
            var defaultAbsorberDictionary = new Dictionary<TissueType, Dictionary<ChromophoreType, double>>
                {
                    //ref: Meglinski, Matcher, Computer Methods and Programs in Biomedicine 70, 2003, 179-186.
                    { 
                        TissueType.Skin,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 28.4},
                            {ChromophoreType.HbO2, 22.4},
                            {ChromophoreType.H2O, 0.7},
                            {ChromophoreType.Fat, 0.0},
                            {ChromophoreType.Melanin, 0.0051},
                        }
                    },
                    { 
                        TissueType.BreastPreMenopause,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 6.9},
                            {ChromophoreType.HbO2, 19.6},
                            {ChromophoreType.H2O, 0.345},
                            {ChromophoreType.Fat, 0.41},
                        }
                    },
                    { 
                        TissueType.BreastPostMenopause,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 3.75},
                            {ChromophoreType.HbO2, 11.25},
                            {ChromophoreType.H2O, 0.205},
                            {ChromophoreType.Fat,  0.585},
                        }
                    },
                    { 
                        TissueType.BrainWhiteMatter,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 24},
                            {ChromophoreType.HbO2, 56},
                            {ChromophoreType.H2O, 0.80},
                            {ChromophoreType.Fat,  0.12},
                        }
                    },
                    { 
                        TissueType.BrainGrayMatter,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 24},
                            {ChromophoreType.HbO2, 56},
                            {ChromophoreType.H2O, 0.80},
                            {ChromophoreType.Fat,  0.12},
                        }
                    },
                    { 
                        TissueType.Liver,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 66},
                            {ChromophoreType.HbO2, 124},
                            {ChromophoreType.H2O, 0.87},
                            {ChromophoreType.Fat,  0.02},
                        }
                    },
                    { 
                        TissueType.IntralipidPhantom,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Nigrosin, 0.01}
                        }
                    },
                    { 
                        TissueType.PolystyreneSpherePhantom,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Nigrosin, 0.01}
                        }
                    },
                    { 
                        TissueType.Custom,
                        new Dictionary<ChromophoreType, double>
                        {
                            {ChromophoreType.Hb, 20},
                            {ChromophoreType.HbO2, 20},
                            {ChromophoreType.H2O, 0.0},
                            {ChromophoreType.Fat,  0.0},
                            {ChromophoreType.Melanin, 0.0},
                        }
                    }
                };

            var absorbers = defaultAbsorberDictionary[tissueType]
                .Select(kvp => new ChromophoreAbsorber(kvp.Key, kvp.Value))
                .ToArray();

            return absorbers;
        }

        /// <summary>
        /// Sets the scatterer type for the specified tissue type
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        public static IScatterer CreateScatterer(TissueType tissueType)
        {
            switch (tissueType)
            {
                case TissueType.Skin:
                case TissueType.Liver:
                case TissueType.BrainWhiteMatter:
                case TissueType.BrainGrayMatter:
                case TissueType.BreastPreMenopause:
                case TissueType.BreastPostMenopause:
                case TissueType.Custom:
                    return new PowerLawScatterer(tissueType);
                case TissueType.IntralipidPhantom:
                    return new IntralipidScatterer();
                case TissueType.PolystyreneSpherePhantom:
                    return new MieScatterer();
                default:
                    throw new ArgumentOutOfRangeException("tissueType");
            }
        }

    }
}
