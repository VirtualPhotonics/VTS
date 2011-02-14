using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel.Application.Panels
{
    public class MultiLayerTissueViewModel : BindableObject
    {
        private MultiLayerTissueInput _input;

        private ObservableCollection<LayerRegionViewModel> _regions;

        public MultiLayerTissueViewModel(MultiLayerTissueInput input)
        {
            _input = input;

            _regions = new ObservableCollection<LayerRegionViewModel>(
                _input.Regions.Select(r => new LayerRegionViewModel((LayerRegion) r)));
        }

        public MultiLayerTissueViewModel() 
            : this(new MultiLayerTissueInput())
        {
        }

        public ObservableCollection<LayerRegionViewModel> Regions
        {
            get { return _regions; }
            set
            {
                _regions = value;
                OnPropertyChanged("Regions");
            }
        }

        public MultiLayerTissueInput GetMultilayerTissueInput()
        {
            return _input;
        }
    }
}
