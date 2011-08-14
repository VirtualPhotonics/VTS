using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel
{
    public class SingleEllipsoidTissueViewModel : BindableObject
    {
        private SingleEllipsoidTissueInput _input;

        private ObservableCollection<LayerRegionViewModel> _layerRegionsVM;

        private EllipsoidRegionViewModel _ellipsoidRegionVM;

        public SingleEllipsoidTissueViewModel(SingleEllipsoidTissueInput input)
        {
            _input = input;

            _layerRegionsVM = new ObservableCollection<LayerRegionViewModel>(
                _input.Regions.Select(r => new LayerRegionViewModel((LayerRegion)r)));
        }

        public SingleEllipsoidTissueViewModel()
            : this(new SingleEllipsoidTissueInput())
        {
        }

        public EllipsoidRegionViewModel EllipsoidRegionVM
        {
            get { return _ellipsoidRegionVM; }
            set
            {
                _ellipsoidRegionVM = value;
                OnPropertyChanged("EllipsoidRegionVM");
            }
        }

        public ObservableCollection<LayerRegionViewModel> LayerRegionsVM
        {
            get { return _layerRegionsVM; }
            set
            {
                _layerRegionsVM = value;
                OnPropertyChanged("LayerRegionsVM");
            }
        }

        public SingleEllipsoidTissueInput GetSingleEllipsoidTissueInput()
        {
            return _input;
        }
    }
}
