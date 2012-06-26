//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using Vts.Extensions;
//using Vts.MonteCarlo;
//using Vts.MonteCarlo.Extensions;
//using Vts.MonteCarlo.Tissues;

//namespace Vts.Gui.Silverlight.ViewModel
//{
//    public class MultiLayerTissueViewModel : BindableObject
//    {
//        private MultiLayerTissueInput _input;

//        private ObservableCollection<LayerRegionViewModel> _layerRegionsVM;

//        private int _currentRegionIndex;

//        public MultiLayerTissueViewModel(MultiLayerTissueInput input)
//        {
//            _input = input;

//            _layerRegionsVM = new ObservableCollection<LayerRegionViewModel>(
//                _input.Regions.Select((r, i) => new LayerRegionViewModel(
//                    (LayerRegion)r,
//                    "Layer " + i + (r.IsAir() ? " (Air)" : " (Tissue)"))));

//            _currentRegionIndex = 0;
//        }

//        public MultiLayerTissueViewModel()
//            : this(new MultiLayerTissueInput())
//        {
//        }

//        public ObservableCollection<LayerRegionViewModel> LayerRegionsVM
//        {
//            get { return _layerRegionsVM; }
//            set
//            {
//                _layerRegionsVM = value;
//                OnPropertyChanged("LayerRegionsVM");
//            }
//        }

//        public int CurrentRegionIndex
//        {
//            get { return _currentRegionIndex; }
//            set
//            {
//                //if(value<_layerRegionsVM.Count && value >=0)
//                //{
//                _currentRegionIndex = value;
//                OnPropertyChanged("CurrentRegionIndex");
//                OnPropertyChanged("MinimumRegionIndex");
//                OnPropertyChanged("MaximumRegionIndex");
//                //}
//            }
//        }

//        public int MinimumRegionIndex { get { return 0; } }
//        public int MaximumRegionIndex { get { return _layerRegionsVM != null ? _layerRegionsVM.Count - 1 : 0; } }

//        public MultiLayerTissueInput GetMultilayerTissueInput()
//        {
//            return _input;
//        }
//    }
//}
