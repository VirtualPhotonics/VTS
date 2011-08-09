using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel.Application.Panels
{
    public class SimulationInputViewModel : BindableObject
    {
        private SimulationInput _input;

        private object _tissueInputVM;

        public SimulationInputViewModel(SimulationInput input)
        {
            SimulationInput = input; // use the property to invoke the appropriate change notification
        }

        public SimulationInputViewModel()
            : this(new SimulationInput())
        {
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
            get { return _input; }
            set
            {
                _input = value;
                // todo: in the future, delegate to AssignTissueInputViewModel(_input.TisueInput), where
                // the assignment is done based on the input type
                _tissueInputVM = new MultiLayerTissueViewModel((MultiLayerTissueInput)_input.TissueInput);
                OnPropertyChanged("SimulationInput");
                OnPropertyChanged("TissueInputVM");
            }
        }
    }
}
