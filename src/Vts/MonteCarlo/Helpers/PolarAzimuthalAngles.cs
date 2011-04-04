using System.IO;

namespace Vts.MonteCarlo.Helpers
{
    public class PolarAzimuthalAngles
    {
        private double _theta;
        private double _phi;

        /// <summary>
        /// Returns polar azimutahl angle angle in spheral coordinate system
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        public PolarAzimuthalAngles(double theta, double phi)
        {
            _theta = theta;
            _phi = phi;
        }

        public PolarAzimuthalAngles()
            : this(0, 0)
        {
        }

        public double Theta { get { return _theta; } set { _theta = value; } }
        public double Phi { get { return _phi; } set { _phi = value; } }

        public PolarAzimuthalAngles Clone()
        {
            return new PolarAzimuthalAngles(Theta, Phi);
        }
    }
}
