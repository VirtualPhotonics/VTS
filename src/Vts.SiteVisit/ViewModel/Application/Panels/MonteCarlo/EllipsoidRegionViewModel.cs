using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Tissues;

namespace Vts.SiteVisit.ViewModel
{
    public class EllipsoidRegionViewModel : BindableObject
    {
        private EllipsoidRegion _region;
        private OpticalPropertyViewModel _opticalPropertyVM;

        public EllipsoidRegionViewModel(EllipsoidRegion region)
        {
            _region = region;
            _opticalPropertyVM = new OpticalPropertyViewModel(_region.RegionOP, "mm-1", "");
        }

        public EllipsoidRegionViewModel() : this(new EllipsoidRegion())
        {
        }

        public double X
        {
            get { return _region.Center.X; }
            set
            {
                _region.Center.X = value;
                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get { return _region.Center.Y; }
            set
            {
                _region.Center.Y = value;
                OnPropertyChanged("Y");
            }
        }

        public double Z
        {
            get { return _region.Center.Z; }
            set
            {
                _region.Center.Z = value;
                OnPropertyChanged("Z");
            }
        }

        public double Dx
        {
            get { return _region.Dx; }
            set
            {
                _region.Dx = value;
                OnPropertyChanged("Dx");
            }
        }
        
        public double Dy
        {
            get { return _region.Dy; }
            set
            {
                _region.Dy = value;
                OnPropertyChanged("Dy");
            }
        }

        public double Dz
        {
            get { return _region.Dz; }
            set
            {
                _region.Dz = value;
                OnPropertyChanged("Dz");
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

        public EllipsoidRegion GetEllipsoidRegion()
        {
            return _region;
        }
    }
}
