using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel
{
    public class MultiLayerTissueViewModel : BindableObject
    {
        private MultiLayerTissueInput _input;

        private ObservableCollection<LayerRegionViewModel> _layerRegionsVM;

        public MultiLayerTissueViewModel(MultiLayerTissueInput input)
        {
            _input = input;

            _layerRegionsVM = new ObservableCollection<LayerRegionViewModel>(
                _input.Regions.Select(r => new LayerRegionViewModel((LayerRegion) r)));
        }

        public MultiLayerTissueViewModel() 
            : this(new MultiLayerTissueInput())
        {
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

        public MultiLayerTissueInput GetMultilayerTissueInput()
        {
            return _input;
        }
    }
}
