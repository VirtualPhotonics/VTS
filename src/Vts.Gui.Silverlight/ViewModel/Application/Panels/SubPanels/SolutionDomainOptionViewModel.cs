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

        private IndependentVariableAxis _IndependentAxisType;
        private string _IndependentAxisLabel;
        private string _IndependentAxisUnits;

        private bool _constantLabelVisible;
        private double _ConstantAxisValueImageHeight;

        //Need to know what Solver panel is selected so we can trigger the correct command
        //todo: Needs to be rewritten so that this UserControl is not aware of the container
        private SolverType _SolverType;

        public SolutionDomainOptionViewModel(string groupName, SolutionDomainType defaultType)
            : base(groupName)
        {
            //InitializeControls();
            ROfRhoOption = Options[SolutionDomainType.ROfRho];
            ROfFxOption = Options[SolutionDomainType.ROfFx];
            ROfRhoAndTimeOption = Options[SolutionDomainType.ROfRhoAndTime];
            ROfFxAndTimeOption = Options[SolutionDomainType.ROfFxAndTime];
            ROfRhoAndFtOption = Options[SolutionDomainType.ROfRhoAndFt];
            ROfFxAndFtOption = Options[SolutionDomainType.ROfFxAndFt];

            this.PropertyChanged += (sender, args) =>
            {
                if (sender is SolutionDomainOptionViewModel &&
                    args.PropertyName == "SelectedValue")
                    UpdateOptions(SelectedValue);
            };
            UpdateOptions(defaultType);
        }

        public SolutionDomainOptionViewModel()
            : this("", SolutionDomainType.ROfRho) { }

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
        public double ConstantAxisValueImageHeight
        {
            get { return _ConstantAxisValueImageHeight; }
            set
            {
                _ConstantAxisValueImageHeight = value;
                OnPropertyChanged("ConstantAxisValueImageHeight");
            }
        }
        public SolverType SolverType
        {
            get { return _SolverType; }
            set
            {
                _SolverType = value;
                OnPropertyChanged("SolverType");
            }
        }

        private void UpdateOptions(SolutionDomainType selectedType)
        {
            switch (selectedType)
            {
                case SolutionDomainType.ROfRho:
                default:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Rho });
                    ConstantLabelVisible = false;
                    break;
                case SolutionDomainType.ROfFx:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx,
                            new[] { IndependentVariableAxis.Fx });
                    ConstantLabelVisible = false;
                    break;
                case SolutionDomainType.ROfRhoAndTime:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time });
                    ConstantLabelVisible = true;
                    break;
                case SolutionDomainType.ROfFxAndTime:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx,
                            new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time });
                    ConstantLabelVisible = true;
                    break;
                case SolutionDomainType.ROfRhoAndFt:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft });
                    ConstantLabelVisible = true;
                    break;
                case SolutionDomainType.ROfFxAndFt:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false, IndependentVariableAxis.Fx,
                            new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft });
                    ConstantLabelVisible = true;
                    break;
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

            //todo: expose AfterUpdateAxes() event and wire these up from parent VM
            switch (SolverType)
            {
                case SolverType.Fluence:
                    Commands.FluenceSolver_SetIndependentVariableRange.Execute(GetDefaultIndependentAxisRange(IndependentAxisType));
                    break;
                case SolverType.Inverse:
                    Commands.IS_SetIndependentVariableRange.Execute(GetDefaultIndependentAxisRange(IndependentAxisType));
                    break;
                case SolverType.Forward:
                default:
                    Commands.FS_SetIndependentVariableRange.Execute(GetDefaultIndependentAxisRange(IndependentAxisType));
                    break;
            }

            if (IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                // this filters to find the *other* choice (the one not selected).
                // assumes that there are only two choices 
                // TODO: make compatible with wavelengths? (should be fine if only one other axis)
                var constantAxisOption = IndependentVariableAxisOptionVM.Options.Where(o => o.Key != IndependentAxisType).First().Value;
                ConstantAxisType = constantAxisOption.Value;
                ConstantAxisLabel = constantAxisOption.DisplayName;
                ConstantAxisUnits = ConstantAxisType.GetUnits();

                ConstantAxisValue = ConstantAxisType.GetDefaultConstantAxisValue();
            }
        }

        private RangeViewModel GetDefaultIndependentAxisRange(IndependentVariableAxis independentAxisType)
        {
            return new RangeViewModel(independentAxisType.GetDefaultRange(), independentAxisType.GetUnits(), independentAxisType.GetTitle());
        }


    }
}
