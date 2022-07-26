namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Implements ThreeAxisRotation class
    /// </summary>
    public class ThreeAxisRotation
    {

        /// <summary>
        /// Initializes a new instance of the ThreeAxisRotation class
        /// </summary>
        /// <param name="xRotation">Rotation angle around x-axis</param>
        /// <param name="yRotation">Rotation angle around y-axis</param>
        /// <param name="zRotation">Rotation angle around z-axis</param>
        public ThreeAxisRotation(double xRotation, double yRotation, double zRotation)
        {
            XRotation = xRotation;
            YRotation = yRotation;
            ZRotation = zRotation;
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
        public double XRotation { get; set; } // alpha
        /// <summary>
        /// rotation angle around y-axis
        /// </summary>
        public double YRotation { get; set; } // beta
        /// <summary>
        /// rotation angle around z-axis
        /// </summary>
        public double ZRotation { get; set; }  // gamma

        /// <summary>
        /// method to clone ThreeAxisRotation
        /// </summary>
        /// <returns>ThreeAxisRotation clone</returns>
        public ThreeAxisRotation Clone()
        {
            return new ThreeAxisRotation(XRotation, YRotation, ZRotation);
        }
        
    }
}
