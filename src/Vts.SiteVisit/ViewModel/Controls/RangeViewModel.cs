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
        private DoubleRange _range;

        private string _units;
        private string _title;

        public RangeViewModel() : this(new DoubleRange(1.0, 6.0, 60), "mm", "Range:") { }

        public RangeViewModel(DoubleRange range, string units, string title)
        {
            _range = range;
            Units = units;
            Title = title;

            // todo: does this do anything? (start, stop, number already directly modified)
            _range.PropertyChanged += (s, a) =>
            {
                this.OnPropertyChanged("Start");
                this.OnPropertyChanged("Stop");
                this.OnPropertyChanged("Number");
            };
        }

        /// <summary>
        /// A double representing the start of the range
        /// </summary>
        public double Start
        {
            get { return _range.Start; }
            set
            {
                _range.Start = value;
                OnPropertyChanged("Start");
            }
        }

        /// <summary>
        /// A double representing the end of the range
        /// </summary>
        public double Stop
        {
            get { return _range.Stop; }
            set
            {
                _range.Stop = value;
                OnPropertyChanged("Stop");
            }
        }

        /// <summary>
        /// An integer representing the number of values in the range
        /// </summary>
        public int Number
        {
            get { return _range.Count; }
            set
            {
                _range.Count = value;
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
            get { return _range.AsEnumerable(); }
        }
    }
}
