using System;
using Vts.Gui.Silverlight.Model;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing Reflectance domain sub-panel functionality
    /// </summary>
    public class SolutionDomainOptionViewModel : AbstractSolutionDomainOptionViewModel<SolutionDomainType>
    {
        private bool _enableMultiAxis;
        public SolutionDomainOptionViewModel(string groupName, SolutionDomainType defaultType)
            : base(groupName, defaultType)
        {
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
            : this("", SolutionDomainType.ROfRho)
        {
        }

        public OptionModel<SolutionDomainType> ROfRhoOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfFxOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfRhoAndTimeOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfFxAndTimeOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfRhoAndFtOption { get; private set; }
        public OptionModel<SolutionDomainType> ROfFxAndFtOption { get; private set; }

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
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Wavelength }, AllowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho }, AllowMultiAxis);
                    break;
                case SolutionDomainType.ROfFx:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Wavelength }, AllowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx }, AllowMultiAxis);
                    break;
                case SolutionDomainType.ROfRhoAndTime:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time, IndependentVariableAxis.Wavelength }, AllowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time }, AllowMultiAxis);
                    break;
                case SolutionDomainType.ROfFxAndTime:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time, IndependentVariableAxis.Wavelength }, AllowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time }, AllowMultiAxis);
                    break;
                case SolutionDomainType.ROfRhoAndFt:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft, IndependentVariableAxis.Wavelength }, AllowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Rho, new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft }, AllowMultiAxis);
                    break;
                case SolutionDomainType.ROfFxAndFt:
                    IndependentVariableAxisOptionVM = UseSpectralInputs
                        ? new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft, IndependentVariableAxis.Wavelength }, AllowMultiAxis)
                        : new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx, new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft }, AllowMultiAxis);
                    break;
                default:
                    throw new NotImplementedException("selectedType");
            }

            // create a new callback based on the new viewmodel
            IndependentVariableAxisOptionVM.PropertyChanged += (s, a) => UpdateAxes();

            UpdateAxes();
        }
    }

}
