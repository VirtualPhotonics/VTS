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

        private IndependentVariableAxis _IndependentAxisTwoType;
        private string _IndependentAxisTwoLabel;
        private string _IndependentAxisTwoUnits;

        private IndependentVariableAxis _IndependentAxisThreeType;
        private string _IndependentAxisThreeLabel;
        private string _IndependentAxisThreeUnits;

        private bool _constantLabelVisible;
        private bool _constantLabelTwoVisible;
        private double _ConstantAxisValueImageHeight;
        private double _ConstantAxisValueTwoImageHeight;

        private bool _independentAxisTwoVisible;
        private bool _independentAxisThreeVisible;

        private bool _useSpectralInputs;
        private bool _allowMultiAxis;
        private bool _enableMultiAxis;

        public SolutionDomainOptionViewModel(string groupName, SolutionDomainType defaultType)
            : base(groupName)
        {
            _useSpectralInputs = false;
            _allowMultiAxis = false;
            _enableMultiAxis = true;

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

        public IndependentVariableAxis IndependentAxisTwoType
        {
            get { return _IndependentAxisTwoType; }
            set
            {
                _IndependentAxisTwoType = value;
                OnPropertyChanged("IndependentAxisTwoType");
            }
        }

        public string IndependentAxisTwoLabel
        {
            get { return _IndependentAxisTwoLabel; }
            set
            {
                _IndependentAxisTwoLabel = value;
                OnPropertyChanged("IndependentAxisTwoLabel");
            }
        }

        public string IndependentAxisTwoUnits
        {
            get { return _IndependentAxisTwoUnits; }
            set
            {
                _IndependentAxisTwoUnits = value;
                OnPropertyChanged("IndependentAxisTwoUnits");
            }
        }

        public IndependentVariableAxis IndependentAxisThreeType
        {
            get { return _IndependentAxisThreeType; }
            set
            {
                _IndependentAxisThreeType = value;
                OnPropertyChanged("IndependentAxisThreeType");
            }
        }

        public string IndependentAxisThreeLabel
        {
            get { return _IndependentAxisThreeLabel; }
            set
            {
                _IndependentAxisThreeLabel = value;
                OnPropertyChanged("IndependentAxisThreeLabel");
            }
        }

        public string IndependentAxisThreeUnits
        {
            get { return _IndependentAxisThreeUnits; }
            set
            {
                _IndependentAxisThreeUnits = value;
                OnPropertyChanged("IndependentAxisThreeUnits");
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
        
        public bool IndependentAxisTwoVisible
        {
            get { return _independentAxisTwoVisible; }
            set
            {
                _independentAxisTwoVisible = value;
                OnPropertyChanged("IndependentAxisTwoVisible");
            }
        }

        public bool IndependentAxisThreeVisible
        {
            get { return _independentAxisThreeVisible; }
            set
            {
                _independentAxisThreeVisible = value;
                OnPropertyChanged("IndependentAxisThreeVisible");
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
            }
        }

        public bool EnableMultiAxis
        {
            get { return _enableMultiAxis; }
            set
            {
                _enableMultiAxis = value;
                OnPropertyChanged("EnableMultiAxis");
            }
        }

        private void UpdateOptions(SolutionDomainType selectedType)
        {
            switch (selectedType)
            {
                case SolutionDomainType.ROfRho:
                    IndependentVariableAxisOptionVM = UseSpectralInputs 
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho }, _allowMultiAxis);
                    break;
                case SolutionDomainType.ROfFx:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx }, _allowMultiAxis);
                    break;
                case SolutionDomainType.ROfRhoAndTime:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time }, _allowMultiAxis);
                    break;
                case SolutionDomainType.ROfFxAndTime:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time }, _allowMultiAxis);
                    break;
                case SolutionDomainType.ROfRhoAndFt:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft }, _allowMultiAxis);
                    break;
                case SolutionDomainType.ROfFxAndFt:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft, IndependentVariableAxis.Wavelength }, _allowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft }, _allowMultiAxis);
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
            var numAxes = IndependentVariableAxisOptionVM.SelectedValues.Length;
            var numConstants = IndependentVariableAxisOptionVM.UnSelectedValues.Length;
                // + (_useSpectralInputs ? 1 : 0); // add an "@ lambda" if we're allowing spectral inputs

            ConstantLabelVisible = numConstants > 0 || _allowMultiAxis; // need the multi-select options visible if all axes are chosen as IVs
            ConstantLabelTwoVisible = numConstants > 1;

            // these two aren't directly constructed in SolutionDomainOptionView (yet)...propagate them via an event up to FSVM/ISVM
            IndependentAxisTwoVisible = numAxes > 1;
            IndependentAxisThreeVisible = numAxes > 2;

            if (numAxes > 0)
            {
                IndependentAxisType = IndependentVariableAxisOptionVM.SelectedValues[0];
                IndependentAxisLabel = IndependentVariableAxisOptionVM.SelectedDisplayNames[0];
                IndependentAxisUnits = IndependentAxisType.GetUnits();
            }

            if (numAxes > 1 && _allowMultiAxis)
            {
                IndependentAxisTwoType = IndependentVariableAxisOptionVM.SelectedValues[1];
                IndependentAxisTwoLabel = IndependentVariableAxisOptionVM.SelectedDisplayNames[1];
                IndependentAxisTwoUnits = IndependentAxisTwoType.GetUnits();
            }

            if (numAxes > 2 && _allowMultiAxis)
            {
                IndependentAxisThreeType = IndependentVariableAxisOptionVM.SelectedValues[2];
                IndependentAxisThreeLabel = IndependentVariableAxisOptionVM.SelectedDisplayNames[2];
                IndependentAxisThreeUnits = IndependentAxisThreeType.GetUnits();
            }

            if (numConstants > 0)
            {
                ConstantAxisType = IndependentVariableAxisOptionVM.UnSelectedValues[0];
                ConstantAxisLabel = IndependentVariableAxisOptionVM.UnSelectedDisplayNames[0];
                ConstantAxisUnits = ConstantAxisType.GetUnits();
                ConstantAxisValue = ConstantAxisType.GetDefaultConstantAxisValue();
            }

            if (numConstants > 1)
            {
                ConstantAxisTwoType = IndependentVariableAxisOptionVM.UnSelectedValues[1];
                ConstantAxisTwoLabel = IndependentVariableAxisOptionVM.UnSelectedDisplayNames[1];
                ConstantAxisTwoUnits = ConstantAxisTwoType.GetUnits();
                ConstantAxisTwoValue = ConstantAxisTwoType.GetDefaultConstantAxisValue();
            }
        }
    }
}
