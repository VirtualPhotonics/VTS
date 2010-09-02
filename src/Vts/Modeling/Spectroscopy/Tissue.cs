using System;
using System.Collections.Generic;
using System.Linq;

namespace Vts.SpectralMapping
{
    public class Tissue : BindableObject, IAbsorber, IScatterer
    {
        private IList<IChromophoreAbsorber> _Absorbers;
        private IScatterer _Scatterer;

        public Tissue(IList<IChromophoreAbsorber> absorbers, IScatterer scatterer, string name)
        {
            Absorbers = absorbers;
            Scatterer = scatterer;
            Name = name;
        }

        public Tissue(TissueType tissueType)
        {
            SetPredefinedTissueDefinitions(tissueType);
            Name = tissueType.ToString();
        }

        public ScatteringType ScattererType { get { return _Scatterer.ScattererType; } }

        public string Name { get; set; }

        public IList<IChromophoreAbsorber> Absorbers
        {
            get { return _Absorbers; }
            set
            {
                _Absorbers = value;
                OnPropertyChanged("Absorbers");
            }
        }

        public IScatterer Scatterer
        {
            get { return _Scatterer; }
            set
            {
                _Scatterer = value;
                OnPropertyChanged("Scatterer");
            }
        }

        private void SetPredefinedTissueDefinitions(TissueType tissueType) // this data should be in XML
        {
            SetAbsorbers(tissueType);
            SetScatterer(tissueType);
        }

        public void SetAbsorbers(TissueType tissueType)
        {
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

        public void SetScatterer(TissueType tissueType)
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
                    Scatterer = new PowerLawScatterer();
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

        public override string ToString()
        {
            return Name;
        }

        public double GetMua(double wavelength)
        {
            double mua = 0.0;
            for (int i = 0; i < Absorbers.Count; i++)
            {
                mua += Absorbers[i].GetMua(wavelength);
            }
            return mua;
        }

        public double GetMusp(double wavelength)
        {
            return Scatterer != null ? Scatterer.GetMusp(wavelength) : 0;
        }

        public double GetG(double wavelength)
        {
            return Scatterer != null ? Scatterer.GetG(wavelength) : 0;
        }

        public double GetMus(double wavelength)
        {
            return Scatterer != null ? Scatterer.GetMus(wavelength) : 0;
        }
    }
}
