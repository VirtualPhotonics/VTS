using Vts.Common;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model exposing the Position model class with change notification
    /// </summary>
    public class PositionViewModel : BindableObject
    {
        // Position model - backing store for public properties
        private Position _position;
        private string _units;
        private string _title;

        public PositionViewModel() : this(new Position(0,0,0), "mm", "Position:") { }

        public PositionViewModel(Position position, string units, string title)
        {
            _position = position;
            Units = units;
            Title = title;
        }
        
        /// <summary>
        /// A double representing the x-component of the position
        /// </summary>
        public double X
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// A double representing the y-component of the position
        /// </summary>
        public double Y
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// A double representing the z-component of the position
        /// </summary>
        public double Z
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                OnPropertyChanged("Z");
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

        public Position GetPosition()
        {
            return _position;
        }
    }
}
