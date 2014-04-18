using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SLExtensions.Input;
using Vts.Common;
using Vts.Factories;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;
using Vts.SpectralMapping;

#if WHITELIST
using Vts.Gui.Silverlight.ViewModel.Application;
#endif

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing Spectral panel functionality
    /// </summary>
    public partial class SpectralMappingViewModel : BindableObject
    {
        private List<Tissue> _tissues;
        private Tissue _selectedTissue;
        private double _mua;
        private double _g;
        private double _musp;
        private double _wavelength;
        private BloodConcentrationViewModel _bloodConcentrationVM;
        private RangeViewModel _wavelengthRangeVM;
        private OptionViewModel<ScatteringType> _scatteringTypeVM;
        private string _scatteringTypeName;
        private IScatterer _scatterer;

        public SpectralMappingViewModel()
        {
#if WHITELIST 
            ScatteringTypeVM = new OptionViewModel<ScatteringType>("Scatterer Type:", true, WhiteList.ScatteringTypes);
#else 
            ScatteringTypeVM = new OptionViewModel<ScatteringType>("Scatterer Type:", true);
#endif
            ScatteringTypeVM.PropertyChanged += (sender, args) =>
            {
                if (SelectedTissue.ScattererType != ScatteringTypeVM.SelectedValue)
                {
                    SelectedTissue.Scatterer = SolverFactory.GetScattererType(ScatteringTypeVM.SelectedValue);
                    //LM - Temporary Fix to reset the tissue type after a new scatterer is created
                    if (SelectedTissue.ScattererType == ScatteringType.PowerLaw)
                    {
                       PowerLawScatterer myScatterer = (PowerLawScatterer)SelectedTissue.Scatterer;
                       myScatterer.SetTissueType(SelectedTissue.TissueType);
                    }
                    ScatteringTypeName = SelectedTissue.Scatterer.GetType().FullName;
                }
                OnPropertyChanged("Scatterer");
            };

            WavelengthRangeVM =
                new RangeViewModel(
                    new DoubleRange(650.0, 1000.0, 176),
                    "nm", "Wavelength Range:");

           Tissues = new List<Tissue>
            {
                new Tissue(TissueType.Skin),
                new Tissue(TissueType.BrainWhiteMatter),
                new Tissue(TissueType.BrainGrayMatter),
                new Tissue(TissueType.BreastPreMenopause),
                new Tissue(TissueType.BreastPostMenopause),
                new Tissue(TissueType.Liver),
                new Tissue(TissueType.IntralipidPhantom),
                //new Tissue(TissueType.PolystyreneSpherePhantom),
                new Tissue(TissueType.Custom)
            };
            
            BloodConcentrationVM = new BloodConcentrationViewModel();
            #region DC notes 1
            // DC NOTES on how to propagate the correct hemoglobin instances into BloodConcentrationVM:
            // Upon setting SelectedTissue (below), we internally update the BloodConcentrationVM hemoglobin references 
            // This is the simplest solution, but maybe violates SOC...(see SelectedTissue property for details)
            // A second alternative way would be to override AfterPropertyChanged (see AfterPropertyChanged method below)
            #endregion 
            SelectedTissue = Tissues.First();
            ScatteringTypeName = SelectedTissue.GetType().FullName;
            Wavelength = 650;

            UpdateOpticalProperties();

            Commands.PlotMuaSpectra.Executed += PlotMuaSpectra_Executed;
            Commands.PlotMusprimeSpectra.Executed += PlotMusprimeSpectra_Executed;
            Commands.UpdateOpticalProperties.Executed += UpdateOpticalProperties_Executed;
        }

        #region DC notes 2
        //protected override void AfterPropertyChanged(string propertyName)
        //{
        //    if (propertyName == "SelectedTissue")
        //    {
        //        // update the BloodConcentrationViewModel to point to the IChromophoreAbsorber instances 
        //        // specified in the updated SelectedTissue
        //        var hb = _SelectedTissue.Absorbers.Where(abs => abs.Name == "Hb").FirstOrDefault();
        //        var hbO2 = _SelectedTissue.Absorbers.Where(abs => abs.Name == "HbO2").FirstOrDefault();
        //
        //        // only assign the values if both queries return valid (non-null) instances of IChromophoreAbsorber
        //        if (hb != null && hbO2 != null)
        //        {
        //            BloodConcentrationVM.Hb = hb;
        //            BloodConcentrationVM.HbO2 = hbO2;
        //        }
        //    }
        //    base.AfterPropertyChanged(propertyName);
        //}
        #endregion 

        /// <summary>
        /// Simple pass-through for SelectedTissue.Scatterer 
        /// to allow simpler data binding in Views
        /// </summary>
        public IScatterer Scatterer
        {
            get { return _selectedTissue.Scatterer; }
        }

        public string ScatteringTypeName
        {
            get { return _scatteringTypeName; }
            set
            {
                _scatteringTypeName = value;
                OnPropertyChanged("ScatteringTypeName");
            }
        }

        public OptionViewModel<ScatteringType> ScatteringTypeVM
        {
            get { return _scatteringTypeVM; }
            set
            {
                _scatteringTypeVM = value;
                OnPropertyChanged("ScatteringTypeVM");
            }
        }

        public Tissue SelectedTissue
        {
            get { return _selectedTissue; }
            set
            {
                // var realScatterer = value.Scatterer;

                _selectedTissue = value;
                OnPropertyChanged("SelectedTissue");
                OnPropertyChanged("Scatterer");

                ScatteringTypeVM.Options[_selectedTissue.Scatterer.ScattererType].IsSelected = true;
                ScatteringTypeName = _selectedTissue.Scatterer.GetType().FullName;

                UpdateOpticalProperties();

                // update the BloodConcentrationViewModel to point to the IChromophoreAbsorber instances 
                // specified in the updated SelectedTissue
                var hb = _selectedTissue.Absorbers.Where(abs => abs.Name == "Hb").FirstOrDefault();
                var hbO2 = _selectedTissue.Absorbers.Where(abs => abs.Name == "HbO2").FirstOrDefault();

                // only assign the values if both queries return valid (non-null) instances of IChromophoreAbsorber
                if (hb != null && hbO2 != null)
                {
                    BloodConcentrationVM.Hb = hb;
                    BloodConcentrationVM.HbO2 = hbO2;
                    BloodConcentrationVM.DisplayBloodVM = true;
                }
                else
                    BloodConcentrationVM.DisplayBloodVM = false;
            }
        }

        public List<Tissue> Tissues
        {
            get { return _tissues; }
            set
            {
                _tissues = value;
                this.OnPropertyChanged("Tissues");
            }
        }

        public double Wavelength
        {
            get { return _wavelength; }
            set
            {
                _wavelength = value;
                UpdateOpticalProperties();
                this.OnPropertyChanged("Wavelength");
            }
        }

        public double Mua
        {
            get { return _mua; }
            set
            {
                _mua = value;
                this.OnPropertyChanged("Mua");
            }
        }

        public double G
        {
            get { return _g; }
            set
            {
                _g = value;
                this.OnPropertyChanged("G");
            }
        }

        public double Musp
        {
            get { return _musp; }
            set
            {
                _musp = value;
                this.OnPropertyChanged("Musp");
                this.OnPropertyChanged("ScatteringTypeVM");
            }
        }

        public RangeViewModel WavelengthRangeVM
        {
            get { return _wavelengthRangeVM; }
            set
            {
                _wavelengthRangeVM = value;
//                this.OnPropertyChanged("WavelengthRangeVM");

            }
        }

        public BloodConcentrationViewModel BloodConcentrationVM
        {
            get { return _bloodConcentrationVM; }
            set
            {
                _bloodConcentrationVM = value;
                this.OnPropertyChanged("BloodConcentrationVM");
                this.OnPropertyChanged("SelectedTissue");
            }
        }

        private void UpdateOpticalProperties()
        {
            G = SelectedTissue.GetG(Wavelength);
            Musp = SelectedTissue.GetMusp(Wavelength);
            Mua = SelectedTissue.GetMua(Wavelength);
        }
                
        void PlotMuaSpectra_Executed(object sender, ExecutedEventArgs e)
        {
            PlotAxesLabels axesLabels = new PlotAxesLabels("Wavelength", "nm", IndependentVariableAxis.Wavelength, "μa", "mm-1");
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            IEnumerable<Point> points = ExecutePlotMuaSpectra();
            Commands.Plot_PlotValues.Execute(new PlotData(points, "μa spectra"));

            double minWavelength = WavelengthRangeVM.Values.Min();
            double maxWavelength = WavelengthRangeVM.Values.Max();
            Commands.TextOutput_PostMessage.Execute("Plotted μa spectrum; wavelength range [nm]: [" + minWavelength + ", " + maxWavelength + "]\r");
        }

        public IEnumerable<Point> ExecutePlotMuaSpectra()
        {
            var independentValues = WavelengthRangeVM.Values;
            var dependentValues =
                from w in independentValues
                select SelectedTissue.GetMua(w);

            return  Enumerable.Zip(independentValues, dependentValues, (x, y) => new Point(x, y));
        }

        void PlotMusprimeSpectra_Executed(object sender, ExecutedEventArgs e)
        {
            PlotAxesLabels axesLabels = new PlotAxesLabels("Wavelength", "nm", IndependentVariableAxis.Wavelength, "μs'", "mm-1");
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            IEnumerable<Point> points = ExecutePlotMusprimeSpectra();
            Commands.Plot_PlotValues.Execute(new PlotData(points, "μs' spectra"));

            double minWavelength = WavelengthRangeVM.Values.Min();
            double maxWavelength = WavelengthRangeVM.Values.Max();
            Commands.TextOutput_PostMessage.Execute("Plotted μs' spectrum; wavelength range [nm]: [" + minWavelength + ", " + maxWavelength + "]\r");
        }

        public IEnumerable<Point> ExecutePlotMusprimeSpectra()
        {
            var independentValues = WavelengthRangeVM.Values;
            var dependentValues = independentValues.Select(w => SelectedTissue.GetMusp(w));

            return  Enumerable.Zip(independentValues, dependentValues, (x, y) => new Point(x, y));
        }

        void UpdateOpticalProperties_Executed(object sender, ExecutedEventArgs e)
        {
            UpdateOpticalProperties();
        }
    }
}
