using System;
using System.Linq;
using Vts.Gui.Silverlight.Extensions;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing Reflectance domain sub-panel functionality
    /// </summary>
    public class SolutionDomainOptionViewModel : OptionViewModel<SolutionDomainType>
    {
        private OptionViewModel<IndependentVariableAxis> _IndependentVariableAxisOptionVM;

        // only enabled if using 5-variable forward kernel
        private IndependentVariableAxis _ConstantAxisType;
        private double _ConstantAxisValue;
        private string _ConstantAxisLabel;
        private string _ConstantAxisUnits;

        // dc additions for reflectance vs wavelength at fixed space/time
        private IndependentVariableAxis _ConstantAxisTwoType;
        private double _ConstantAxisTwoValue;
        private string _ConstantAxisTwoLabel;
        private string _ConstantAxisTwoUnits;

        private IndependentVariableAxis _IndependentAxisType;
        private string _IndependentAxisLabel;
        private string _IndependentAxisUnits;

        private bool _constantLabelVisible;
        private bool _constantLabelTwoVisible;
        private double _ConstantAxisValueImageHeight;
        private double _ConstantAxisValueTwoImageHeight;

        private bool _useSpectralInputs;
        private bool _allowMultiAxis;

        public SolutionDomainOptionViewModel(string groupName, SolutionDomainType defaultType)
            : base(groupName)
        {
            _useSpectralInputs = false;
            _allowMultiAxis = false;

            ROfRhoOption = Options[SolutionDomainType.ROfRho];
            ROfFxOption = Options[SolutionDomainType.ROfFx];
            ROfRhoAndTimeOption = Options[SolutionDomainType.ROfRhoAndTime];
            ROfFxAndTimeOption = Options[SolutionDomainType.ROfFxAndTime];
            ROfRhoAndFtOption = Options[SolutionDomainType.ROfRhoAndFt];
            ROfFxAndFtOption = Options[SolutionDomainType.ROfFxAndFt];

            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SelectedValue" ||
                    args.PropertyName == "UseSpectralInputs" ||
                    args.PropertyName == "AllowMultiAxis")
                    UpdateOptions(SelectedValue);
            };
            UpdateOptions(defaultType);
        }

        public SolutionDomainOptionViewModel()
            : this("", SolutionDomainType.ROfRho) { }

        public event EventHandler SettingsLoaded = delegate { };

        public OptionModel<SolutionDomainType> ROfRhoOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfFxOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfRhoAndTimeOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfFxAndTimeOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfRhoAndFtOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfFxAndFtOption { get; private set; }

        public OptionViewModel<IndependentVariableAxis> IndependentVariableAxisOptionVM
        {
            get { return _IndependentVariableAxisOptionVM; }
            set
            {
                _IndependentVariableAxisOptionVM = value;
                OnPropertyChanged("IndependentVariableAxisOptionVM");
            }
        }

        public IndependentVariableAxis IndependentAxisType
        {
            get { return _IndependentAxisType; }
            set
            {
                _IndependentAxisType = value;
                OnPropertyChanged("IndependentAxisType");
            }
        }

        public string IndependentAxisLabel
        {
            get { return _IndependentAxisLabel; }
            set
            {
                _IndependentAxisLabel = value;
                OnPropertyChanged("IndependentAxisLabel");
            }
        }

        public string IndependentAxisUnits
        {
            get { return _IndependentAxisUnits; }
            set
            {
                _IndependentAxisUnits = value;
                OnPropertyChanged("IndependentAxisUnits");
            }
        }

        public IndependentVariableAxis ConstantAxisType
        {
            get { return _ConstantAxisType; }
            set
            {
                _ConstantAxisType = value;

                OnPropertyChanged("ConstantAxisType");
            }
        }

        public double ConstantAxisValue
        {
            get { return _ConstantAxisValue; }
            set
            {
                _ConstantAxisValue = value;
                if (ConstantAxisType == IndependentVariableAxis.Wavelength)
                {
                    Commands.SD_SetWavelength.Execute(_ConstantAxisValue);
                }
                OnPropertyChanged("ConstantAxisValue");
            }
        }

        public string ConstantAxisLabel
        {
            get { return _ConstantAxisLabel; }
            set
            {
                _ConstantAxisLabel = value;
                OnPropertyChanged("ConstantAxisLabel");
            }
        }

        public string ConstantAxisUnits
        {
            get { return _ConstantAxisUnits; }
            set
            {
                _ConstantAxisUnits = value;
                OnPropertyChanged("ConstantAxisUnits");
            }
        }
        
        //dc additions for data vs lambda at fixed time & space
        public IndependentVariableAxis ConstantAxisTwoType
        {
            get { return _ConstantAxisTwoType; }
            set
            {
                _ConstantAxisTwoType = value;

                OnPropertyChanged("ConstantAxisTwoType");
            }
        }

        public double ConstantAxisTwoValue
        {
            get { return _ConstantAxisTwoValue; }
            set
            {
                _ConstantAxisTwoValue = value;
                // update the world that this has changed, and react to it if desired (e.g. in Spectral Panel)
                if (ConstantAxisTwoType == IndependentVariableAxis.Wavelength)
                {
                    Commands.SD_SetWavelength.Execute(_ConstantAxisTwoValue);
                }
                OnPropertyChanged("ConstantAxisTwoValue");
            }
        }

        public string ConstantAxisTwoLabel
        {
            get { return _ConstantAxisTwoLabel; }
            set
            {
                _ConstantAxisTwoLabel = value;
                OnPropertyChanged("ConstantAxisTwoLabel");
            }
        }

        public string ConstantAxisTwoUnits
        {
            get { return _ConstantAxisTwoUnits; }
            set
            {
                _ConstantAxisTwoUnits = value;
                OnPropertyChanged("ConstantAxisTwoUnits");
            }
        }

        public bool ConstantLabelVisible
        {
            get { return _constantLabelVisible; }
            set
            {
                _constantLabelVisible = value;
                ConstantAxisValueImageHeight = ConstantLabelVisible ? 50 : 0;
                OnPropertyChanged("ConstantLabelVisible");
            }
        }

        public bool ConstantLabelTwoVisible
        {
            get { return _constantLabelTwoVisible; }
            set
            {
                _constantLabelTwoVisible = value;
                ConstantAxisValueTwoImageHeight = ConstantLabelTwoVisible ? 50 : 0;
                OnPropertyChanged("ConstantLabelTwoVisible");
            }
        }

        public double ConstantAxisValueImageHeight
        {
            get { return _ConstantAxisValueImageHeight; }
            set
            {
                _ConstantAxisValueImageHeight = value;
                OnPropertyChanged("ConstantAxisValueImageHeight");
            }
        }

        public double ConstantAxisValueTwoImageHeight
        {
            get { return _ConstantAxisValueTwoImageHeight; }
            set
            {
                _ConstantAxisValueTwoImageHeight = value;
                OnPropertyChanged("ConstantAxisValueTwoImageHeight");
            }
        }

        public bool UseSpectralInputs
        {
            get { return _useSpectralInputs; }
            set
            {
                _useSpectralInputs = value;
                OnPropertyChanged("UseSpectralInputs");
            }
        }

        public bool AllowMultiAxis
        {
            get { return _allowMultiAxis; }
            set
            {
                _allowMultiAxis = value;
                OnPropertyChanged("AllowMultiAxis");
                //OnPropertyChanged("AllowSingleAxis");
            }
        }

        //public bool AllowSingleAxis
        //{
        //    get { return !_allowMultiAxis; }
        //}

        private void UpdateOptions(SolutionDomainType selectedType)
        {
            switch (selectedType)
            {
                case SolutionDomainType.ROfRho:
                    IndependentVariableAxisOptionVM = UseSpectralInputs 
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho }, _allowMultiAxis);
                    ConstantLabelVisible = UseSpectralInputs;
                    ConstantLabelTwoVisible = false;
                    break;
                case SolutionDomainType.ROfFx:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx }, _allowMultiAxis);
                    ConstantLabelVisible = UseSpectralInputs;
                    ConstantLabelTwoVisible = false;
                    break;
                case SolutionDomainType.ROfRhoAndTime:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time }, _allowMultiAxis);
                    ConstantLabelVisible = true;
                    ConstantLabelTwoVisible = UseSpectralInputs;
                    break;
                case SolutionDomainType.ROfFxAndTime:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time }, _allowMultiAxis);
                    ConstantLabelVisible = true;
                    ConstantLabelTwoVisible = UseSpectralInputs;
                    break;
                case SolutionDomainType.ROfRhoAndFt:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft }, _allowMultiAxis);
                    ConstantLabelVisible = true;
                    ConstantLabelTwoVisible = UseSpectralInputs;
                    break;
                case SolutionDomainType.ROfFxAndFt:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft }, _allowMultiAxis);
                    ConstantLabelVisible = true;
                    ConstantLabelTwoVisible = UseSpectralInputs;
                    break;
                default:
                    throw new NotImplementedException("selectedType");
            }
            // create a new callback based on the new viewmodel
            IndependentVariableAxisOptionVM.PropertyChanged += (s, a) => UpdateAxes();

            UpdateAxes();
        }

        private void UpdateAxes()
        {
            IndependentAxisType = IndependentVariableAxisOptionVM.SelectedValue;
            IndependentAxisLabel = IndependentVariableAxisOptionVM.SelectedDisplayName;
            IndependentAxisUnits = IndependentAxisType.GetUnits();

            if (IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                // this filters to find the *other* choice (the one not selected).
                // assumes that there are only two choices 
                var constantAxisOption = IndependentVariableAxisOptionVM.Options.Where(o => o.Key != IndependentAxisType).First().Value;
                ConstantAxisType = constantAxisOption.Value;
                ConstantAxisLabel = constantAxisOption.DisplayName;
                ConstantAxisUnits = ConstantAxisType.GetUnits();
                ConstantAxisValue = ConstantAxisType.GetDefaultConstantAxisValue();

                var constantAxisTwoOptionQuery = IndependentVariableAxisOptionVM.Options.Where(o => o.Key != IndependentAxisType).Skip(1).Take(1);
                if (constantAxisTwoOptionQuery.Any())
                {
                    var constantAxisTwoOption = constantAxisTwoOptionQuery.First().Value;
                    ConstantAxisTwoType = constantAxisTwoOption.Value;
                    ConstantAxisTwoLabel = constantAxisTwoOption.DisplayName;
                    ConstantAxisTwoUnits = ConstantAxisTwoType.GetUnits();
                    ConstantAxisTwoValue = ConstantAxisTwoType.GetDefaultConstantAxisValue();
                }
            }
        }
    }
}
