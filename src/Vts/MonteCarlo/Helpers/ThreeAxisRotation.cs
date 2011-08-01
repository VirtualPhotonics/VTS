using System.IO;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Implements ThreeAxisRotation class
    /// </summary>
    public class ThreeAxisRotation
    {
        private double _xRotation;
        private double _yRotation;
        private double _zRotation;

        /// <summary>
        /// Initializes a new instance of the ThreeAxisRotation class
        /// </summary>
        /// <param name="xRotation">Rotation angle around x-axis</param>
        /// <param name="yRotation">Rotation angle around y-axis</param>
        /// <param name="zRotation">Rotation angle around z-axis</param>
        public ThreeAxisRotation(double xRotation, double yRotation, double zRotation)
        {
            _xRotation = xRotation;
            _yRotation = yRotation;
            _zRotation = zRotation;
        }

        /// <summary>
        /// Initializes a new instance of the ThreeAxisRotation class
        /// </summary>
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
