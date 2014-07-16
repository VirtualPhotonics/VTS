using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.Gui.Silverlight.ViewModel
{
    public class EllipsoidRegionViewModel : BindableObject
    {
        private EllipsoidRegion _region;
        private string _name;
        private string _units;
        private OpticalPropertyViewModel _opticalPropertyVM;

        public EllipsoidRegionViewModel(EllipsoidRegion region, string name)
        {
            _region = region;
            _name = name ?? "";
            _opticalPropertyVM = new OpticalPropertyViewModel(_region.RegionOP, "mm-1", "", true, true,true, false);
        }

        public EllipsoidRegionViewModel() : this(new EllipsoidRegion(), "")
        {
        }

        public string Name
        {
            get { return _name + (_region.IsAir() ? " (Air)" : " (Tissue)"); }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
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

        public string Units { get { return "mm"; } }
        public bool IsLayer { get { return false; } }
        public bool IsEllipsoid { get { return true; } }

        //public EllipsoidRegion GetEllipsoidRegion()
        //{
        //    return _region;
        //}
    }
}
