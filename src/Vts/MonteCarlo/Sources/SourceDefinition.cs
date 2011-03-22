using Vts.Common;
using System;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

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

    

    public static class DemoRngInjection
    {
        public static void Demo()
        {
            var seed = 2;
            var rng = new MathNet.Numerics.Random.MersenneTwister(seed);
            var tissue = new MultiLayerTissue();

            var isotropicSource = new IsotropicPointSource2(new Position(0, 0, 1))
            {
                Rng = rng
            };

            var customSource = new CustomPointSourceNew(
                new DoubleRange(0, Math.PI/2),
                new DoubleRange(0, Math.PI),
                new Position(0, 0, 1), 
                new SourceOrientation(0,0)) //todo: is SourceOrientation still desirable?
            {
                Rng = rng
            };

            isotropicSource.GetNextPhoton(tissue);
        }
    }


    
    public class SourceHelper
    {
        private Position _translationFromOrigin;
        private SourceOrientation _rotationFromInwardNormal;
        private Direction _rotatedPrincipalSourceAxis;

        public SourceHelper(
            Position _translationFromOrigin,
            SourceOrientation _rotationFromInwardNormal)
            
        {
            _translationFromOrigin = _translationFromOrigin.Clone();
            _rotationFromInwardNormal = _rotationFromInwardNormal.Clone();
            _rotatedPrincipalSourceAxis = GetUltimatePrincipalAxisFromPolarOrientation(_rotationFromInwardNormal);
        }

        private Direction GetUltimatePrincipalAxisFromPolarOrientation(
            SourceOrientation polarOrientation)
        {
            // todo: Janaka help
            return new Direction();
        }

        public Position TranslationFromOrigin
        {
            get { return _translationFromOrigin; }
            set { _translationFromOrigin = value; }
        }

        public SourceOrientation RotationFromInwardNormal
        {
            get { return _rotationFromInwardNormal; }
            set { _rotationFromInwardNormal = value; }
        }

        public Direction RotatedPrincipalSourceAxis
        {
            get { return _rotatedPrincipalSourceAxis; }
            set { _rotatedPrincipalSourceAxis = value; }
        }

        public Direction RotateDirectionToPrincipalAxis(Direction input)
        {
            // combine input and RotatedPrincipalSourceAxis (or RotationFromInwardNormal)
            // to create a final direction
            // todo: implement

            var finalDirection = input; //temp
            return finalDirection;
        }

    }
}
