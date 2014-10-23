using System.Collections.Generic;
using Vts.Common;

namespace Vts.Gui.Silverlight.ViewModel
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
        private bool _enableNumber;
        private IndependentVariableAxis _axisType;
        
        public RangeViewModel(DoubleRange range, string units, IndependentVariableAxis axisType, string title, bool enableNumber)
        {
            _range = range;
            Units = units;
            Title = title;
            _enableNumber = enableNumber;
            _axisType = axisType;

            // todo: does this do anything? (start, stop, number already directly modified)
            _range.PropertyChanged += (s, a) =>
            {
                this.OnPropertyChanged("Start");
                this.OnPropertyChanged("Stop");
                this.OnPropertyChanged("Number");
            };
        }

        public RangeViewModel(DoubleRange range, string units, IndependentVariableAxis axisType, string title)
            : this(range,units,axisType,title,true)
        {
        }

        public RangeViewModel() : this(new DoubleRange(1.0, 6.0, 60), "mm", IndependentVariableAxis.Rho, "Range:", true) { }

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

        public bool EnableNumber
        {
            get { return _enableNumber; }
            set
            {
                _enableNumber = value;
                OnPropertyChanged("EnableNumber");
            }
        }

        public bool ShowTitle
        {
            get { return Title.Length > 0; }
        }

        public IndependentVariableAxis AxisType
        {
            get { return _axisType; }
            set
            {
                _axisType = value;
                OnPropertyChanged("AxisType");
            }
        }

        public IEnumerable<double> Values
        {
            get { return _range.AsEnumerable(); }
        }
    }
}
