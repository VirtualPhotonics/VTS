using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel.Application.Panels
{
    public class LayerRegionViewModel : BindableObject
    {
        private LayerRegion _region;
        private RangeViewModel _zRangeVM;
        private OpticalPropertyViewModel _opticalPropertyVM;

        public LayerRegionViewModel() : this(new LayerRegion())
        {
        }

        public LayerRegionViewModel(LayerRegion region)
        {
            _region = region;
            _zRangeVM = new RangeViewModel(_region.ZRange, "mm", "");
            _opticalPropertyVM = new OpticalPropertyViewModel(_region.RegionOP, "mm-1", "");
        }

        public RangeViewModel ZRangeVM
        {
            get { return _zRangeVM; }
            set
            {
                _zRangeVM = value;
                OnPropertyChanged("ZRangeVM");
            }
        }

        public OpticalPropertyViewModel OpticalPropertyVM
        {
            get { return _opticalPropertyVM; }
            set
            {
                _opticalPropertyVM = value;
                OnPropertyChanged("OpticalPropertyVM");
            }
        }

        public LayerRegion GetLayerRegion()
        {
            return _region;
        }
    }
}
