//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Vts.FemModeling.MGRTE._2D;

//namespace Vts.SiteVisit.ViewModel
//{
//    public class FemLayerRegionViewModel : BindableObject
//    {
//        private LayerTissueRegion _region;
//        private string _name;
//        private RangeViewModel _zRangeVM;
//        private OpticalPropertyViewModel _opticalPropertyVM;

//        public FemLayerRegionViewModel(LayerTissueRegion region, string name)
//        {
//            _region = region;
            
//            if (string.IsNullOrEmpty(name))
//            {
//                _name = _region.IsAir() ? "Air" : "Tissue";
//            }
//            else
//            {
//                _name = name;
//            }
//            _zRangeVM = new RangeViewModel(_region.ZRange, "mm", "", false);
//            _opticalPropertyVM = new OpticalPropertyViewModel(_region.RegionOP, "mm-1", "", true, true, true, true);
//        }

//        //public LayerRegionViewModel(LayerRegion region) : this(region, "")
//        //{
//        //}

//        public FemLayerRegionViewModel()
//            : this(new LayerTissueRegion(), "")
//        {
//        }

//        public RangeViewModel ZRangeVM
//        {
//            get { return _zRangeVM; }
//            set
//            {
//                _zRangeVM = value;
//                OnPropertyChanged("ZRangeVM");
//            }
//        }

//        public string Name
//        {
//            get { return _name; }
//            set
//            {
//                _name = value;
//                OnPropertyChanged("Name");
//            }
//        }

//        public OpticalPropertyViewModel OpticalPropertyVM
//        {
//            get { return _opticalPropertyVM; }
//            set
//            {
//                _opticalPropertyVM = value;
//                OnPropertyChanged("OpticalPropertyVM");
//            }
//        }

//        public bool IsLayer { get { return true; } }
//        public bool IsEllipsoid { get { return false; } }

//        //public LayerRegion GetLayerRegion()
//        //{
//        //    return _region;
//        //}
//    }
//}
