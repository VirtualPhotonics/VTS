using System.Linq;
using System.Windows;
using Vts.Common;
using Vts.SiteVisit.Extensions;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.Model;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model implementing Fluence domain sub-panel functionality
    /// </summary>
    public class FluenceSolutionDomainOptionViewModel : OptionViewModel<FluenceSolutionDomainType>
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

        public FluenceSolutionDomainOptionViewModel(string groupName, FluenceSolutionDomainType defaultType)
            : base(groupName)
        {
            //InitializeControls();
            FluenceofRhoOption = Options[FluenceSolutionDomainType.FluenceofRho];
            FluenceofFxOption = Options[FluenceSolutionDomainType.FluenceofFx];
            FluenceofRhoAndTOption = Options[FluenceSolutionDomainType.FluenceofRhoAndT];
            FluenceofFxAndTOption = Options[FluenceSolutionDomainType.FluenceofFxAndT];
            FluenceofRhoAndFtOption = Options[FluenceSolutionDomainType.FluenceofRhoAndFt];
            FluenceofFxAndFtOption = Options[FluenceSolutionDomainType.FluenceofFxAndFt];

            this.PropertyChanged += (sender, args) =>
            {
                if (sender is SolutionDomainOptionViewModel && args.PropertyName == "SelectedValue")
                {
                    UpdateOptions();
                }
            };
            UpdateOptions();
        }

        public FluenceSolutionDomainOptionViewModel()
            : this("", FluenceSolutionDomainType.FluenceofRho) { }

        public OptionModel<FluenceSolutionDomainType> FluenceofRhoOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceofFxOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceofRhoAndTOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceofFxAndTOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceofRhoAndFtOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceofFxAndFtOption { get; private set; }

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

        private void UpdateOptions()
        {
            switch (SelectedValue)
            {
                case FluenceSolutionDomainType.FluenceofRho:
                default:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("",
                            IndependentVariableAxis.Rho);
                    ConstantLabelVisible = false;
                    break;
                case FluenceSolutionDomainType.FluenceofFx:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("",
                            IndependentVariableAxis.Fx);
                    ConstantLabelVisible = false;
                    break;
                case FluenceSolutionDomainType.FluenceofRhoAndT:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("",
                            IndependentVariableAxis.Rho, IndependentVariableAxis.T);
                    ConstantLabelVisible = true;
                    break;
                case FluenceSolutionDomainType.FluenceofFxAndT:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("",
                            IndependentVariableAxis.Fx, IndependentVariableAxis.T);
                    ConstantLabelVisible = true;
                    break;
                case FluenceSolutionDomainType.FluenceofRhoAndFt:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("",
                            IndependentVariableAxis.Rho, IndependentVariableAxis.Ft);
                    ConstantLabelVisible = true;
                    break;
                case FluenceSolutionDomainType.FluenceofFxAndFt:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("",
                            IndependentVariableAxis.Fx, IndependentVariableAxis.Ft);
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

            Commands.FluenceSolver_SetIndependentVariableRange.Execute(GetDefaultIndependentAxisRange(IndependentAxisType));

            if (IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                // this filters to find the *other* choice (the one not selected).
                // assumes that there are only two choices 
                // TODO: make compatible with wavelengths? (should be fine if only one other axis)
                var constantAxisOption = IndependentVariableAxisOptionVM.Options.Where(o => o.Key != IndependentAxisType).First().Value;
                ConstantAxisType = constantAxisOption.Value;
                ConstantAxisLabel = constantAxisOption.DisplayName;
                ConstantAxisUnits = ConstantAxisType.GetUnits();

                ConstantAxisValue = GetDefaultConstantAxisValue(ConstantAxisType);
            }
        }

        private RangeViewModel GetDefaultIndependentAxisRange(IndependentVariableAxis independentAxisType)
        {
            DoubleRange range = null;
            switch (independentAxisType)
            {
                case IndependentVariableAxis.Rho:
                default:
                    range = new DoubleRange(0.1, 19.9, 100); // units=mm
                    break;
                case IndependentVariableAxis.Z:
                    range = new DoubleRange(0.1, 19.9, 100); // units=mm
                    break;
                case IndependentVariableAxis.T:
                    range = new DoubleRange(0D, 0.5D, 10);  // units=ns
                    break;
                case IndependentVariableAxis.Fx:
                    range = new DoubleRange(0D, 0.5D, 60);  // units=[unitless]
                    break;
                case IndependentVariableAxis.Ft:
                    range = new DoubleRange(0D, 0.5D, 60); // units=GHz
                    break;
                case IndependentVariableAxis.Wavelength:
                    range = new DoubleRange(650D, 1000D, 60); // units=nm
                    break;
            }
            return new RangeViewModel(range, independentAxisType.GetUnits(), "");
        }

        private double GetDefaultConstantAxisValue(IndependentVariableAxis constantType)
        {
            switch (constantType)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return 1D;
                case IndependentVariableAxis.T:
                    return 50D;
                case IndependentVariableAxis.Fx:
                    return 0D;
                case IndependentVariableAxis.Ft:
                    return 0D;
                case IndependentVariableAxis.Wavelength:
                    return 650D;
            }
        }
    }
}
