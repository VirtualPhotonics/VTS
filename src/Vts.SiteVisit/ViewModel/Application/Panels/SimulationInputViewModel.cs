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
            _input = input;

            // todo: in the future, delegate to AssignTissueInputViewModel(_input.TisueInput), where
            // the assignment is done based on the input type
            _tissueInputVM = new MultiLayerTissueViewModel((MultiLayerTissueInput)_input.TissueInput);
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

        public SimulationInput GetSimulationInput()
        {
            return _input;
        }
    }
}
