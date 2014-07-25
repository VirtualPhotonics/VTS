using System;
using System.Linq;
using Vts.Common;
using Vts.Gui.Silverlight.Extensions;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;

namespace Vts.Gui.Silverlight.ViewModel
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
            FluenceOfRhoAndZOption = Options[FluenceSolutionDomainType.FluenceOfRhoAndZ];
            FluenceOfFxAndZOption = Options[FluenceSolutionDomainType.FluenceOfFxAndZ];
            FluenceOfRhoAndZAndTimeOption = Options[FluenceSolutionDomainType.FluenceOfRhoAndZAndTime];
            FluenceOfFxAndZAndTimeOption = Options[FluenceSolutionDomainType.FluenceOfFxAndZAndTime];
            FluenceOfRhoAndZAndFtOption = Options[FluenceSolutionDomainType.FluenceOfRhoAndZAndFt];
            FluenceOfFxAndZAndFtOption = Options[FluenceSolutionDomainType.FluenceOfFxAndZAndFt];

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
            : this("", FluenceSolutionDomainType.FluenceOfRhoAndZ) { }

        public OptionModel<FluenceSolutionDomainType> FluenceOfRhoAndZOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceOfFxAndZOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceOfRhoAndZAndTimeOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceOfFxAndZAndTimeOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceOfRhoAndZAndFtOption { get; private set; }
        public OptionModel<FluenceSolutionDomainType> FluenceOfFxAndZAndFtOption { get; private set; }

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
                case FluenceSolutionDomainType.FluenceOfRhoAndZ:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Rho});
                    ConstantLabelVisible = false;
                    break;
                case FluenceSolutionDomainType.FluenceOfFxAndZ:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Fx});
                    ConstantLabelVisible = false;
                    break;
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndTime:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Time });
                    ConstantLabelVisible = true;
                    break;
                case FluenceSolutionDomainType.FluenceOfFxAndZAndTime:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Time });
                    ConstantLabelVisible = true;
                    break;
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndFt:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Ft });
                    ConstantLabelVisible = true;
                    break;
                case FluenceSolutionDomainType.FluenceOfFxAndZAndFt:
                    IndependentVariableAxisOptionVM =
                        new OptionViewModel<IndependentVariableAxis>("IndependentAxis", false,
                            new[] { IndependentVariableAxis.Fx, IndependentVariableAxis.Ft });
                    ConstantLabelVisible = true;
                    break;
                default:
                    throw new NotImplementedException("SelectedValue");
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
                case IndependentVariableAxis.Time:
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
            return new RangeViewModel(range, independentAxisType.GetUnits(), independentAxisType, "");
        }

        private double GetDefaultConstantAxisValue(IndependentVariableAxis constantType)
        {
            switch (constantType)
            {
                case IndependentVariableAxis.Rho:
                default:
                    return 1D;
                case IndependentVariableAxis.Time:
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
