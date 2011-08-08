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
        public Tissue(IList<IChromophoreAbsorber> absorbers, IScatterer scatterer, string name)
        {
            Absorbers = absorbers;
            Scatterer = scatterer;
            Name = name;
        }

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
            SetAbsorbers(tissueType);
            SetScatterer(tissueType);
        }

        /// <summary>
        /// Sets the absorbers for the specified tissue type
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        public void SetAbsorbers(TissueType tissueType)
        {
            TissueType = tissueType;
            Absorbers = new List<IChromophoreAbsorber>();
            switch (tissueType)
            {
                case (TissueType.Skin):
                    //ref: Meglinski, Matcher, Computer Methods and Programs in Biomedicine 70, 2003, 179-186.
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 28.4)); //225.8
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 22.4)); //338.7
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.7));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.0));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Melanin, 0.0051));
                    break;

                case (TissueType.BreastPreMenopause):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 6.9));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 19.6));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.345));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.41));
                    break;

                case (TissueType.BreastPostMenopause):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 3.75));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 11.25));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.205));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.585));
                    break;

                case (TissueType.BrainWhiteMatter):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 24));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 56));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.80));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.12));
                    break;

                case (TissueType.BrainGrayMatter):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 24));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 56));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.80));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.12));
                    break;

                case (TissueType.Liver):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 66));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 124));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.87));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.02));
                    // Scatterers.Add(new PowerLawScatterer(
                    break;
                case (TissueType.IntralipidPhantom):
                case (TissueType.PolystyreneSpherePhantom):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Nigrosin, 0.01));
                    break;
                case (TissueType.Custom):
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Hb, 20));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.HbO2, 20));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.H2O, 0.0));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Fat, 0.0));
                    Absorbers.Add(new ChromophoreAbsorber(ChromophoreType.Melanin,0.0));
                    break;
            }
        }

        /// <summary>
        /// Sets the scatterer type for the specified tissue type
        /// </summary>
        /// <param name="tissueType">Tissue type</param>
        public void SetScatterer(TissueType tissueType)
        {
            TissueType = tissueType;
            switch (tissueType)
            {
                case TissueType.Skin:
                case TissueType.Liver:
                case TissueType.BrainWhiteMatter:
                case TissueType.BrainGrayMatter:
                case TissueType.BreastPreMenopause:
                case TissueType.BreastPostMenopause:
                case TissueType.Custom:
                    Scatterer = new PowerLawScatterer(tissueType);
                    break;
                case TissueType.IntralipidPhantom:
                    Scatterer = new IntralipidScatterer();
                    break;
                case TissueType.PolystyreneSpherePhantom:
                    Scatterer = new MieScatterer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tissueType");
            }
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
    }
}
