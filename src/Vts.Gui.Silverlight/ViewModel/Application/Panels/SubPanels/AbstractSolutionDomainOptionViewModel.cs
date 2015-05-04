using System;
using System.Linq;
using Vts.Extensions;
using Vts.Gui.Silverlight.Extensions;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing domain sub-panel functionality (abstract - implemented for reflectance and fluence)
    /// </summary>
    public class AbstractSolutionDomainOptionViewModel<TDomainType> : OptionViewModel<TDomainType>
    {
        private OptionViewModel<IndependentVariableAxis> _IndependentVariableAxisOptionVM;

        private IndependentAxisViewModel[] _independentAxesVMs;
        private ConstantAxisViewModel[] _constantAxesVMs;

        private double _ConstantAxisValueImageHeight;
        private double _ConstantAxisValueTwoImageHeight;

        private bool _useSpectralInputs;
        private bool _allowMultiAxis;

        private bool _showIndependentAxisChoice;

        public AbstractSolutionDomainOptionViewModel(string groupName, TDomainType defaultType)
            : base(groupName)
        {
            _useSpectralInputs = false;
            _allowMultiAxis = false;
            _showIndependentAxisChoice = false;
        }

        public AbstractSolutionDomainOptionViewModel()
            : this("", default(TDomainType)) { }

        public event EventHandler SettingsLoaded = delegate { };

        public OptionViewModel<IndependentVariableAxis> IndependentVariableAxisOptionVM
        {
            get { return _IndependentVariableAxisOptionVM; }
            set
            {
                _IndependentVariableAxisOptionVM = value;
                OnPropertyChanged("IndependentVariableAxisOptionVM");
            }
        }

        public IndependentAxisViewModel[] IndependentAxesVMs
        {
            get { return _independentAxesVMs; }
            set
            {
                _independentAxesVMs = value;
                OnPropertyChanged("IndependentAxesVMs");
            }
        }

        public ConstantAxisViewModel[] ConstantAxesVMs
        {
            get { return _constantAxesVMs; }
            set
            {
                _constantAxesVMs = value;
                OnPropertyChanged("ConstantAxesVMs");
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

        public bool ShowIndependentAxisChoice
        {
            get { return _showIndependentAxisChoice; }
            set
            {
                _showIndependentAxisChoice = value;
                OnPropertyChanged("ShowIndependentAxisChoice");
            }
        }
        
        public virtual int NativeAxesCount { get { return 1; } }

        protected virtual void UpdateAxes()
        {
            var numAxes = IndependentVariableAxisOptionVM.SelectedValues.Length;
            var numConstants = IndependentVariableAxisOptionVM.UnSelectedValues.Length;

            // create new local VMs for independent and constant axes
            var independentAxesVMs = Enumerable.Range(0, numAxes).Select(i =>
                new IndependentAxisViewModel
                {
                    AxisType = IndependentVariableAxisOptionVM.SelectedValues[i],
                    AxisLabel = IndependentVariableAxisOptionVM.SelectedDisplayNames[i],
                    AxisUnits = IndependentVariableAxisOptionVM.SelectedValues[i].GetUnits(),
                    AxisRangeVM = new RangeViewModel(
                        IndependentVariableAxisOptionVM.SelectedValues[i].GetDefaultRange(),
                        IndependentVariableAxisOptionVM.SelectedValues[i].GetUnits(),
                        IndependentVariableAxisOptionVM.SelectedValues[i],
                        IndependentVariableAxisOptionVM.SelectedValues[i].GetTitle())
                }).ToArray();
            var constantAxesVMs = Enumerable.Range(0, numConstants).Select(i =>
                new ConstantAxisViewModel
                {
                    AxisType = IndependentVariableAxisOptionVM.UnSelectedValues[i],
                    AxisLabel = IndependentVariableAxisOptionVM.UnSelectedDisplayNames[i],
                    AxisUnits = IndependentVariableAxisOptionVM.UnSelectedValues[i].GetUnits(),
                    AxisValue = IndependentVariableAxisOptionVM.UnSelectedValues[i].GetDefaultConstantAxisValue(),
                }).ToArray();

            // assign callbacks
            independentAxesVMs.ForEach(vm => vm.PropertyChanged += (s, a) => OnPropertyChanged("IndependentAxesVMs"));
            constantAxesVMs.ForEach(vm => vm.PropertyChanged += (s, a) => OnPropertyChanged("ConstantAxesVMs"));

            // and then set them, fully formed, to the member variable, firing change notification
            IndependentAxesVMs = independentAxesVMs;
            ConstantAxesVMs = constantAxesVMs;

            ShowIndependentAxisChoice = numAxes + numConstants > NativeAxesCount;
        }
    }
}
