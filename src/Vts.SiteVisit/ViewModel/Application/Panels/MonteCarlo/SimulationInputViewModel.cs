using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel
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
            _tissueTypeVM = new OptionViewModel<Vts.MonteCarlo.TissueType>("Tissue Type:", true, WhiteList.TissueTypes);
#else
            _tissueTypeVM = new OptionViewModel<Vts.MonteCarlo.TissueType>("Tissue Type:", true);
#endif

            _tissueTypeVM.PropertyChanged += (sender, args) =>
            {
                switch (_tissueTypeVM.SelectedValue)
                {
                    case MonteCarlo.TissueType.MultiLayer:
                        _tissueInputVM = new MultiLayerTissueViewModel(new MultiLayerTissueInput());
                        break;
                    case MonteCarlo.TissueType.SingleEllipsoid:
                        _tissueInputVM = new SingleEllipsoidTissueViewModel(new SingleEllipsoidTissueInput());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            // do this last to make sure (at first) we end up with the originally-specified input
            _tissueInputVM = new MultiLayerTissueViewModel((MultiLayerTissueInput)_simulationInput.TissueInput);
        }

        public SimulationInputViewModel()
            : this(new SimulationInput())
        {
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

        public SimulationInput SimulationInput
        {
            get { return _simulationInput; }
            set
            {
                _simulationInput = value;
                // todo: in the future, delegate to AssignTissueInputViewModel(_input.TisueInput), where
                // the assignment is done based on the input type
                //_tissueInputVM = new MultiLayerTissueViewModel((MultiLayerTissueInput)_input.TissueInput);
                OnPropertyChanged("SimulationInput");
                TissueTypeVM.SelectedValue = _simulationInput.TissueInput.TissueType;
            }
        }
    }
}
