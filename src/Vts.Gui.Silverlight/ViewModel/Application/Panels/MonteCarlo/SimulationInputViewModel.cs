using System;
using Vts.MonteCarlo;

#if WHITELIST
using Vts.Gui.Silverlight.ViewModel.Application;
#endif

namespace Vts.Gui.Silverlight.ViewModel
{
    public class SimulationInputViewModel : BindableObject
    {
        private SimulationInput _simulationInput;
        private SimulationOptionsViewModel _simulationOptionsVM;
        private OptionViewModel<Vts.MonteCarlo.TissueType> _tissueTypeVM;

        private object _tissueInputVM;

        public SimulationInputViewModel(SimulationInput input)
        {
            _simulationInput = input; // use the property to invoke the appropriate change notification

            _simulationOptionsVM = new SimulationOptionsViewModel(_simulationInput.Options);

#if WHITELIST 
            TissueTypeVM = new OptionViewModel<Vts.MonteCarlo.TissueType>("Tissue Type:", true, _simulationInput.TissueInput.TissueType, WhiteList.TissueTypes);
#else
            TissueTypeVM = new OptionViewModel<Vts.MonteCarlo.TissueType>("Tissue Type:", true, _simulationInput.TissueInput.TissueType);
#endif

            _tissueTypeVM.PropertyChanged += (sender, args) =>
                {
                    switch (_tissueTypeVM.SelectedValue)
                    {
                        case MonteCarlo.TissueType.MultiLayer:
                             _simulationInput.TissueInput = new MultiLayerTissueInput();
                            break;
                        case MonteCarlo.TissueType.SingleEllipsoid:
                            _simulationInput.TissueInput = new SingleEllipsoidTissueInput();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    UpdateTissueTypeVM(_simulationInput.TissueInput.TissueType);
                };

            UpdateTissueInputVM(_simulationInput.TissueInput);
        }

        public SimulationInputViewModel()
            : this(new SimulationInput())
        {
        }

        public SimulationInput SimulationInput
        {
            get { return _simulationInput; }
            set
            {
                _simulationInput = value;
                // OnPropertyChanged("SimulationInput");  // nobody binds to this
                OnPropertyChanged("N");
                _simulationOptionsVM.SimulationOptions = _simulationInput.Options;
                UpdateTissueInputVM(_simulationInput.TissueInput);
            }
        }

        public long N
        {
            get { return _simulationInput.N; }
            set
            {
                _simulationInput.N = value;
                OnPropertyChanged("N");
            }
        }


        public SimulationOptionsViewModel SimulationOptionsVM
        {
            get { return _simulationOptionsVM; }
            set
            {
                _simulationOptionsVM = value;
                OnPropertyChanged("SimulationOptionsVM");
            }
        }

        public OptionViewModel<Vts.MonteCarlo.TissueType> TissueTypeVM
        {
            get { return _tissueTypeVM; }
            set
            {
                _tissueTypeVM = value;
                OnPropertyChanged("TissueTypeVM");
            }
        }
        
        public object TissueInputVM
        {
            get { return _tissueInputVM; }
            set
            {
                _tissueInputVM = value;
                OnPropertyChanged("TissueInputVM");
            }
        }

        private void UpdateTissueTypeVM(MonteCarlo.TissueType tissueType)
        {
            switch (tissueType)
            {
                case MonteCarlo.TissueType.MultiLayer:
                    _simulationInput.TissueInput = new MultiLayerTissueInput();
                    break;
                case MonteCarlo.TissueType.SingleEllipsoid:
                    _simulationInput.TissueInput = new SingleEllipsoidTissueInput();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            TissueTypeVM.Options[tissueType].IsSelected = true;

        }

        private void UpdateTissueInputVM(ITissueInput tissueInput)
        {
            switch (tissueInput.TissueType)
            {
                case MonteCarlo.TissueType.MultiLayer:
                    TissueInputVM = new MultiRegionTissueViewModel((MultiLayerTissueInput)tissueInput);
                    //TissueInputVM = new MultiLayerTissueViewModel((MultiLayerTissueInput)tissueInput);
                    break;
                case MonteCarlo.TissueType.SingleEllipsoid:
                    TissueInputVM = new MultiRegionTissueViewModel((SingleEllipsoidTissueInput)tissueInput);
                    //TissueInputVM = new SingleEllipsoidTissueViewModel((SingleEllipsoidTissueInput)tissueInput);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
