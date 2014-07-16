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

        /// <summary>
        /// rotation angle around x-axis
        /// </summary>
        public double XRotation { get { return _xRotation; } set { _xRotation = value; } } // alpha
        /// <summary>
        /// rotation angle around y-axis
        /// </summary>
        public double YRotation { get { return _yRotation; } set { _yRotation = value; } } // beta
        /// <summary>
        /// rotation angle around z-axis
        /// </summary>
        public double ZRotation { get { return _zRotation; } set { _zRotation = value; } }  // gamma

        /// <summary>
        /// method to clone ThreeAxisRotation
        /// </summary>
        /// <returns></returns>
        public ThreeAxisRotation Clone()
        {
            return new ThreeAxisRotation(XRotation, YRotation, ZRotation);
        }
        
    }
}
