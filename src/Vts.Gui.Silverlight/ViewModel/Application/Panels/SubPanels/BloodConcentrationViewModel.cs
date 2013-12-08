using Vts.SpectralMapping;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing Blood Concentration sub-panel functionality
    /// </summary>
    public class BloodConcentrationViewModel : BindableObject
    {
        // Backing-fields for chromophores. For consistency, all other properties are
        // designed to set these values, and do not have a backing store of their own
        private IChromophoreAbsorber _Hb;
        private IChromophoreAbsorber _HbO2;
        private bool _DisplayBloodVM = true;

        public BloodConcentrationViewModel()
            : this(
                new ChromophoreAbsorber(ChromophoreType.Hb, 10.0),
                new ChromophoreAbsorber(ChromophoreType.HbO2, 30.0)) { }

        public BloodConcentrationViewModel(IChromophoreAbsorber hb, IChromophoreAbsorber hbO2)
        {
            HbO2 = hbO2;
            Hb = hb;
        }

        /// <summary>
        /// ChromophoreAbsorber representing the concentration of oxy-hemoglobin (uM)
        /// </summary>
        public bool DisplayBloodVM
        {
            get { return _DisplayBloodVM; }
            set
            {
                _DisplayBloodVM = value;
                this.OnPropertyChanged("DisplayBloodVM");
            }
        }

        public IChromophoreAbsorber HbO2
        {
            get { return _HbO2; }
            set
            {
                if (_HbO2 != null) // unsubscribe any existing property changed event
                {
                    _HbO2.PropertyChanged -= (s, a) => UpdateStO2AndTotalHb();
                }

                if (value != null)
                {
                    _HbO2 = value;
                    this.OnPropertyChanged("HbO2");
                    // subscribe to the new property changed event
                    _HbO2.PropertyChanged += (s, a) => UpdateStO2AndTotalHb();
                    // make sure that whatever's bound to StO2 and TotalHb will update
                    UpdateStO2AndTotalHb();
                }
            }
        }

        private void UpdateStO2AndTotalHb()
        {
            this.OnPropertyChanged("StO2");
            this.OnPropertyChanged("TotalHb");
            this.OnPropertyChanged("BloodVolumeFraction");
        }

        /// <summary>
        /// ChromophoreAbsorber representing the concentration of deoxy-hemoglobin (uM)
        /// </summary>
        public IChromophoreAbsorber Hb
        {
            get { return _Hb; }
            set
            {
                if (_Hb != null) // unsubscribe any existing property changed event
                    _Hb.PropertyChanged -= (s, a) => UpdateStO2AndTotalHb();

                if (value != null)
                {
                    _Hb = value;
                    //_Hb.Concentration = FormatOutput(_Hb.Concentration);
                    this.OnPropertyChanged("Hb");
                    // subscribe to the new property changed event
                    _Hb.PropertyChanged += (s, a) => UpdateStO2AndTotalHb();
                    // make sure that whatever's bound to StO2 and TotalHb will update
                    UpdateStO2AndTotalHb();
                }
            }
        }

        /// <summary>
        /// Property to specify tissue oxygen saturation (unitless)
        /// </summary>
        /// <remarks>
        /// This is just a pass-through to Hb and HbO2, based on the existing TotalHb value
        /// </remarks>
        public double StO2
        {
            get{ return HbO2.Concentration / (Hb.Concentration + HbO2.Concentration); }
            set
            {
                // calculate the new Hb and HbO2 values based on the existing TotalHb
                // storing them in temporary local fields (to break the circular reference)
                var hb = (1 - value) * TotalHb;
                var hbO2 = value * TotalHb;

                // after calculated, assign them to the concentration properties of 
                // the ChromphoreAbsorber instances
                Hb.Concentration = hb;
                HbO2.Concentration = hbO2;

                this.OnPropertyChanged("StO2");
            }
        }

        /// <summary>
        /// Property to specify total hemoglobin concentration, HbT (uM)
        /// </summary>
        /// <remarks>
        /// This is just a pass-through to Hb and HbO2, based on the existing StO2 value
        /// </remarks>
        public double TotalHb
        {
            get { return Hb.Concentration + HbO2.Concentration; }
            set
            {
                // calculate the new Hb and HbO2 values based on the existing StO2
                // storing them in temporary local fields (to break the circular reference)
                var hbO2 = value * StO2;
                var hb = value * (1 - StO2);

                // after calculated, assign them to the concentration properties of 
                // the ChromphoreAbsorber instances
                Hb.Concentration = hb;
                HbO2.Concentration = hbO2;

                this.OnPropertyChanged("TotalHb");

                // call this last, because BloodVolumeFraction depends on TotalHb being updated
                this.OnPropertyChanged("BloodVolumeFraction");
            }
        }

        /// <summary>
        /// Property to specify blood volume fraction (vb) as an alternative to TotalHb
        /// </summary>
        /// <remarks>
        /// This is just a pass-through to TotalHb, assuming 150gHb/L for whole blood
        /// todo: verify that the 150g/L value is for *whole blood* not RBCs
        /// (otherwise, need to account for Hct)
        /// </remarks>
        public double BloodVolumeFraction
        {
            get { return TotalHb / 1E6 * 64500 / 150; }
            set
            {
                //BloodVolumeFraction = value * 1E6 / 64500 * 150;
                TotalHb = value * 1E6 / 64500 * 150; // TotalHb will internally fire OnPropertyChanged() here

                this.OnPropertyChanged("BloodVolumeFraction");
                this.OnPropertyChanged("TotalHb");

            }
        }
    }
}
