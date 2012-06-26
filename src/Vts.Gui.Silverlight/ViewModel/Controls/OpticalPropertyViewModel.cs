namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model exposing the OpticalProperties model class with change notification
    /// </summary>
    public class OpticalPropertyViewModel : BindableObject
    {
        private string _Units;
        private string _Title;
        private bool _enableMua;
        private bool _enableMusp;
        private bool _enableG;
        private bool _enableN;

        #region Model data

        public double Mua
        {
            get { return OpticalProperties.Mua; }
            set
            {
                OpticalProperties.Mua = value;
                OnPropertyChanged("Mua");
            }
        }
        public double Musp
        {
            get { return OpticalProperties.Musp; }
            set
            {
                OpticalProperties.Musp = value;
                OnPropertyChanged("Musp");
            }
        }
        public double G
        {
            get { return OpticalProperties.G; }
            set
            {
                OpticalProperties.G = value;
                OnPropertyChanged("G");
            }
        }
        public double N
        {
            get { return OpticalProperties.N; }
            set
            {
                OpticalProperties.N = value;
                OnPropertyChanged("N");
            }
        }

        #endregion

        public string Units
        {
            get { return _Units; }
            set
            {
                _Units = value;
                OnPropertyChanged("Units");
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool ShowTitle { get { return !(Title == ""); } }

        public bool EnableMua
        {
            get { return _enableMua; }
            set
            {
                _enableMua = value;
                OnPropertyChanged("EnableMua");
            }
        }

        public bool EnableMusp
        {
            get { return _enableMusp; }
            set
            {
                _enableMusp = value;
                OnPropertyChanged("EnableMusp");
            }
        }

        public bool EnableG
        {
            get { return _enableG; }
            set
            {
                _enableG = value;
                OnPropertyChanged("EnableG");
            }
        }

        public bool EnableN
        {
            get { return _enableN; }
            set
            {
                _enableN = value;
                OnPropertyChanged("EnableN");
            }
        }

        private OpticalProperties OpticalProperties { get; set; }

        public OpticalPropertyViewModel() 
            : this( 
                new OpticalProperties(0.01, 1.0, 0.8, 1.4), 
                IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(), 
                "") { }

        public OpticalPropertyViewModel(OpticalProperties opticalProperties, string units, string title)
            : this(opticalProperties, units,title, true, true, false, true)
        {

        }
        
        public OpticalPropertyViewModel(OpticalProperties opticalProperties, string units, string title, 
            bool enableMua, bool enableMusp, bool enableG, bool enableN)
        {
            OpticalProperties = opticalProperties;
            Units = units;
            Title = title;

            _enableMua = enableMua;
            _enableMusp = enableMusp;
            _enableG = enableG;
            _enableN = enableN;
        }

        /// <summary>
        /// Helper method. Can't be bound to.
        /// </summary>
        /// <returns></returns>
        public OpticalProperties GetOpticalProperties() { return OpticalProperties; }

        public override string ToString()
        {
            return OpticalProperties.ToString() + "; Units = " + Units;
        }
    }
}
