using System.IO;

namespace Vts.Common
{
    public class ThreeAxisRotation
    {
        private double _xRotation;
        private double _yRotation;
        private double _zRotation;

        public ThreeAxisRotation(double xRotation, double yRotation, double zRotation)
        {
            _xRotation = xRotation;
            _yRotation = yRotation;
            _zRotation = zRotation;
        }

        public ThreeAxisRotation()
            : this(0, 0, 0)
        {
        }

        public double XRotation { get { return _xRotation; } set { _xRotation = value; } } // alpha
        public double YRotation { get { return _yRotation; } set { _yRotation = value; } } // beta
        public double ZRotation { get { return _zRotation; } set { _zRotation = value; } }  // gamma

        public ThreeAxisRotation Clone()
        {
            return new ThreeAxisRotation(XRotation, YRotation, ZRotation);
        }
        
    }
}
