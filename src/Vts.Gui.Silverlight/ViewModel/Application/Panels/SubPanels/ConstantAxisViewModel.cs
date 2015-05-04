using Vts.Gui.Silverlight.Input;

namespace Vts.Gui.Silverlight.ViewModel
{
    public class ConstantAxisViewModel : BindableObject
    {
        private double _axisValue;
        private string _axisUnits;
        private string _axisLabel;
        private IndependentVariableAxis _axisType;
        private int _imageHeight = 50;

        public IndependentVariableAxis AxisType
        {
            get { return _axisType; }
            set
            {
                _axisType = value;
                OnPropertyChanged("AxisType");
            }
        }

        public string AxisLabel
        {
            get { return _axisLabel; }
            set
            {
                _axisLabel = value;
                OnPropertyChanged("AxisLabel");
            }
        }

        public string AxisUnits
        {
            get { return _axisUnits; }
            set
            {
                _axisUnits = value;
                OnPropertyChanged("AxisUnits");
            }
        }

        public double AxisValue
        {
            get { return _axisValue; }
            set
            {
                _axisValue = value;
                if (AxisType == IndependentVariableAxis.Wavelength)
                {
                    // update the world that this has changed, and react to it if desired (e.g. in Spectral Panel)
                    Commands.SD_SetWavelength.Execute(AxisValue);
                }
                OnPropertyChanged("AxisValue");
            }
        }

        public int ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                OnPropertyChanged("ImageHeight");
            }
        }
    }
}
