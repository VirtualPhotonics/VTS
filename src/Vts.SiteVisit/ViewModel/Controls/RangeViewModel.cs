using System.Collections.Generic;
using Vts.Common;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model exposing the DoubleRange model class with change notification
    /// </summary>
    public class RangeViewModel : BindableObject
    {
        // Range model - backing store for public properties
        private DoubleRange Range { get; set; }

        private string _units;
        private string _title;

        /// <summary>
        /// A double representing the start of the range
        /// </summary>
        public double Start
        {
            get { return Range.Start; }
            set
            {
                Range.Start = value;
                OnPropertyChanged("Start");
            }
        }

        /// <summary>
        /// A double representing the end of the range
        /// </summary>
        public double Stop
        {
            get { return Range.Stop; }
            set
            {
                Range.Stop = value;
                OnPropertyChanged("Stop");
            }
        }

        /// <summary>
        /// An integer representing the number of values in the range
        /// </summary>
        public int Number
        {
            get { return Range.Count; }
            set
            {
                Range.Count = value;
                OnPropertyChanged("Number");
            }
        }

        public string Units
        {
            get { return _units; }
            set
            {
                _units = value;
                OnPropertyChanged("Units");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
                OnPropertyChanged("ShowTitle");
            }
        }

        public bool ShowTitle
        {
            get { return Title.Length > 0; }
        }

        public IEnumerable<double> Values
        {
            get { return Range.AsEnumerable(); }
        }

        public RangeViewModel() : this(new DoubleRange(1.0, 6.0, 60), "mm", "Range:") { }

        public RangeViewModel(DoubleRange range, string units, string title)
        {
            Range = range;
            Units = units;
            Title = title;

            Range.PropertyChanged += (s, a) =>
            {
                this.OnPropertyChanged("Start");
                this.OnPropertyChanged("Stop");
                this.OnPropertyChanged("Number");
            };
            // right now, we're doing manual databinding to the selected item. need to enable databinding 
            // confused, though - do we need to use strings? or, how to make generics work with dependency properties?
            //RangeSpacingOptionControlViewModel = new OptionViewModel<ScalingType>("RangeSpacingVM");
            //RangeSpacingOptionControlViewModel.PropertyChanged += new PropertyChangedEventHandler(RangeSpacingOptionControlViewModel_PropertyChanged);
        }
    }
}
