using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Messaging;
using GalaSoft.MvvmLight.Command;
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
            ScatteringTypeVM = new OptionViewModel<ScatteringType>("Scatterer Type", true, WhiteList.ScatteringTypes);
#else
            ScatteringTypeVM = new OptionViewModel<ScatteringType>("Scatterer Type", true);
#endif
            ScatteringTypeVM.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SelectedValue" && SelectedTissue != null)//SelectedTissue.ScattererType != ScatteringTypeVM.SelectedValue)
                {
                    SelectedTissue.Scatterer = SolverFactory.GetScattererType(ScatteringTypeVM.SelectedValue);
                    var bindableScatterer = SelectedTissue.Scatterer as INotifyPropertyChanged;
                    if (bindableScatterer != null)
                    {
                        bindableScatterer.PropertyChanged += (s, a) => UpdateOpticalProperties();
                    }
                    //LM - Temporary Fix to reset the tissue type after a new scatterer is created
                    if (SelectedTissue.ScattererType == ScatteringType.PowerLaw)
                    {
                        PowerLawScatterer myScatterer = (PowerLawScatterer)SelectedTissue.Scatterer;
                        myScatterer.SetTissueType(SelectedTissue.TissueType);
                    }
                    ScatteringTypeName = SelectedTissue.Scatterer.GetType().FullName;
                }
                OnPropertyChanged("Scatterer");
                UpdateOpticalProperties();
            };

            WavelengthRangeVM = new RangeViewModel(new DoubleRange(650.0, 1000.0, 36), "nm", IndependentVariableAxis.Wavelength, "Wavelength Range");

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
            BloodConcentrationVM.PropertyChanged += (sender, args) => UpdateOpticalProperties();

            SelectedTissue = Tissues.First();
            ScatteringTypeVM.SelectedValue = SelectedTissue.ScattererType; // forces update to all bindings established in hanlder for ScatteringTypeVM.PropertyChanged above
            ScatteringTypeName = SelectedTissue.GetType().FullName;
            OpticalProperties = new OpticalProperties(0.01, 1, 0.8, 1.4);
            Wavelength = 650;

            PlotMuaSpectrumCommand = new RelayCommand(PlotMuaSpectrum_Executed);
            PlotMuspSpectrumCommand = new RelayCommand(PlotMuspSpectrum_Executed);

            Commands.SD_SetWavelength.Executed += (snder, args) => // updates when solution domain is involved in spectral feedback
            {
                // Wavelength = (double) args.Parameter; // this will ping-pong back to FS (stack overflow), so repeating setter logic here:
                _wavelength = (double)args.Parameter;
                UpdateOpticalProperties();
                // Commands.Spec_UpdateWavelength.Execute(_wavelength); (don't do this)
                
                this.OnPropertyChanged("Wavelength");
            };
        }

        public RelayCommand PlotMuaSpectrumCommand { get; set; }
        public RelayCommand PlotMuspSpectrumCommand { get; set; }

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
                Commands.Spec_UpdateWavelength.Execute(_wavelength);
                this.OnPropertyChanged("Wavelength");
            }
        }

        public OpticalProperties OpticalProperties { get; private set; }

        public double Mua
        {
            get { return OpticalProperties.Mua; }
            set
            {
                OpticalProperties.Mua = value;
                this.OnPropertyChanged("Mua");
            }
        }

        public double Musp
        {
            get { return OpticalProperties.Musp; }
            set
            {
                OpticalProperties.Musp = value;
                this.OnPropertyChanged("Musp");
                this.OnPropertyChanged("ScatteringTypeVM");
            }
        }

        public double G
        {
            get { return OpticalProperties.G; }
            set
            {
                OpticalProperties.G = value;
                this.OnPropertyChanged("G");
            }
        }

        public double N
        {
            get { return OpticalProperties.N; }
            set
            {
                OpticalProperties.N = value;
                this.OnPropertyChanged("N");
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
            OpticalProperties = SelectedTissue.GetOpticalProperties(Wavelength);
            this.OnPropertyChanged("Mua");
            this.OnPropertyChanged("Musp");
            this.OnPropertyChanged("G");
            this.OnPropertyChanged("N");
            this.OnPropertyChanged("OpticalProperties");
            Commands.Spec_UpdateOpticalProperties.Execute(OpticalProperties);
        }

        private void PlotMuaSpectrum_Executed()
        {
            var axisType = IndependentVariableAxis.Wavelength;
            var axisUnits = IndependentVariableAxisUnits.NM;
            PlotAxesLabels axesLabels = new PlotAxesLabels(
                "μa",
                "mm-1",
                new IndependentAxisViewModel
                {
                    AxisType = axisType,
                    AxisLabel = axisType.GetInternationalizedString(),
                    AxisUnits = axisUnits.GetInternationalizedString(),
                    AxisRangeVM = WavelengthRangeVM,
                });

            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            var tissue = SelectedTissue;
            var wavelengths = WavelengthRangeVM.Values.ToArray();
            var points = new Point[wavelengths.Length];
            for (int wvi = 0; wvi < wavelengths.Length; wvi++)
            {
                var wavelength = wavelengths[wvi];
                points[wvi] = new Point(wavelength, tissue.GetMua(wavelength));
            }
            Commands.Plot_PlotValues.Execute(new []{ new PlotData(points, "μa spectrum" )} );

            double minWavelength = WavelengthRangeVM.Values.Min();
            double maxWavelength = WavelengthRangeVM.Values.Max();
            Commands.TextOutput_PostMessage.Execute("Plotted μa spectrum; wavelength range [nm]: [" + minWavelength + ", " + maxWavelength + "]\r");
        }

        private void PlotMuspSpectrum_Executed()
        {
            var axisType = IndependentVariableAxis.Wavelength;
            var axisUnits = IndependentVariableAxisUnits.NM;
            PlotAxesLabels axesLabels = new PlotAxesLabels(
                "μa",
                "mm-1",
                new IndependentAxisViewModel
                {
                    AxisType = axisType,
                    AxisLabel = axisType.GetInternationalizedString(),
                    AxisUnits = axisUnits.GetInternationalizedString(),
                    AxisRangeVM = WavelengthRangeVM
                });
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            var tissue = SelectedTissue;
            var wavelengths = WavelengthRangeVM.Values.ToArray();
            var points = new Point[wavelengths.Length];
            for (int wvi = 0; wvi < wavelengths.Length; wvi++)
            {
                var wavelength = wavelengths[wvi];
                points[wvi] = new Point(wavelength, tissue.GetMusp(wavelength));
            }

            Commands.Plot_PlotValues.Execute(new[] { new PlotData(points, "μs' spectrum") });

            double minWavelength = WavelengthRangeVM.Values.Min();
            double maxWavelength = WavelengthRangeVM.Values.Max();
            Commands.TextOutput_PostMessage.Execute("Plotted μs' spectrum; wavelength range [nm]: [" + minWavelength + ", " + maxWavelength + "]\r");
        }
    }
}
