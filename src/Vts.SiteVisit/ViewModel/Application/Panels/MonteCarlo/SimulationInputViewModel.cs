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
                             _simulationInput.TissueInput = new MultiLayerTissueInput();
                            break;
                        case MonteCarlo.TissueType.SingleEllipsoid:
                            _simulationInput.TissueInput = new SingleEllipsoidTissueInput();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    UpdateTissueInput(_simulationInput.TissueInput);
                };

            UpdateTissueInput(_simulationInput.TissueInput);
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
                UpdateTissueInput(_simulationInput.TissueInput);
            }
        }

        private void UpdateTissueInput(ITissueInput tissueInput)
        {
            switch (tissueInput.TissueType)
            {
                case MonteCarlo.TissueType.MultiLayer:
                    TissueInputVM = new MultiLayerTissueViewModel((MultiLayerTissueInput)tissueInput);
                    break;
                case MonteCarlo.TissueType.SingleEllipsoid:
                    TissueInputVM = new SingleEllipsoidTissueViewModel((SingleEllipsoidTissueInput)tissueInput);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
