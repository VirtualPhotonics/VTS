using System.IO;

namespace Vts.Common
{
    public class PolarAzimuthalRotationAngles
    {
        private double _thetaRotation;
        private double _phiRotation;

        public PolarAzimuthalRotationAngles(double thetaRotation, double phiRotation)
        {
            _thetaRotation = thetaRotation;
            _phiRotation = phiRotation;
        }

        public PolarAzimuthalRotationAngles()
            : this(0, 0)
        {
        }

        public double ThetaRotation { get { return _thetaRotation; } set { _thetaRotation = value; } }
        public double PhiRotation { get { return _phiRotation; } set { _phiRotation = value; } }

        public PolarAzimuthalRotationAngles Clone()
        {
            return new PolarAzimuthalRotationAngles(ThetaRotation, PhiRotation);
        }
    }
}
