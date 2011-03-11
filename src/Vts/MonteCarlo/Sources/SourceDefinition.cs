using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    public class SourceOrientation
    {
        private double _thetaRotation;
        private double _phiRotation;

        public SourceOrientation(double thetaRotation, double phiRotation)
        {
            _thetaRotation = thetaRotation;
            _phiRotation = phiRotation;
        }

        public SourceOrientation()
            : this(0, 0)
        {
        }

        public double ThetaRotation { get { return _thetaRotation; } set { _thetaRotation = value; } }
        public double PhiRotation { get { return _phiRotation; } set { _phiRotation = value; } }

        public SourceOrientation Clone()
        {
            return new SourceOrientation(ThetaRotation, PhiRotation);
        }
    }

    public class CartesianRotationVector
    {
        private double _xRotation;
        private double _yRotation;
        private double _zRotation;

        public CartesianRotationVector(double xRotation, double yRotation, double zRotation)
        {
            _xRotation = xRotation;
            _yRotation = yRotation;
            _zRotation = zRotation;
        }

        public CartesianRotationVector()
            : this(0, 0, 0)
        {
        }

        public double XRotation { get { return _xRotation; } set { _xRotation = value; } } // alpha
        public double YRotation { get { return _yRotation; } set { _yRotation = value; } } // beta
        public double ZRotation { get { return _zRotation; } set { _zRotation = value; } }  // gamma

        public CartesianRotationVector Clone()
        {
            return new CartesianRotationVector(XRotation, YRotation, ZRotation);
        }
    }

    public class SourceDefinition
    {
        private Position _position;
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;
        private SourceOrientation _polarOrientation;
        private Direction _rotatedPrincipalSourceAxis;

        public SourceDefinition(
            Position position,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            SourceOrientation polarOrientation)
        {
            _position = position.Clone();
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            _polarOrientation = polarOrientation.Clone();

            _rotatedPrincipalSourceAxis = GetUltimatePrincipalAxisFromPolarOrientation(_polarOrientation);
        }

        private Direction GetUltimatePrincipalAxisFromPolarOrientation(
            SourceOrientation polarOrientation)
        {
            // todo: Janaka help
            return new Direction();
        }

        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Direction RotatedPrincipalSourceAxis
        {
            get { return _rotatedPrincipalSourceAxis; }
            set { _rotatedPrincipalSourceAxis = value; }
        }

        public DoubleRange PolarAngleEmissionRange
        {
            get { return _polarAngleEmissionRange; }
            set { _polarAngleEmissionRange = value; }
        }

        public DoubleRange AzimuthalAngleEmissionRange
        {
            get { return _azimuthalAngleEmissionRange; }
            set { _azimuthalAngleEmissionRange = value; }
        }
    }
}
