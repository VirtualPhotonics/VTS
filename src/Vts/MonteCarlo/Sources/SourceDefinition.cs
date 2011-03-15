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

    public class IsotropicPointSource2 : ISource
    {
        private SourceHelper _helper;

        /// <summary>
        /// Creates an instance of IsotropicPointsource given a specified translation from the origin
        /// </summary>
        /// <param name="translationFromOrigin">Translation vector (x,y,z) from origin</param>
        public IsotropicPointSource2(Position translationFromOrigin)
        {
            _helper = new SourceHelper(translationFromOrigin, new SourceOrientation(0, 0));
        }

        /// <summary>
        /// Creates an instance of IsotropicPointSource located at the (0,0,0) Origin
        /// </summary>
        public IsotropicPointSource2()
            : this(new Position(0,0,0))
        {
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // create a random radial direction along axis orthogonal to line direction
            var direction = SourceToolbox.SampleIsotropicRadialDirection(Rng);            
            var position = _helper.TranslationFromOrigin;

            // for more complex sources with orientation:
            // var sourcePosition = CalculateWhereToPutThePhoton();  //i.e. on the surface of a fiber
            // var finalPosition = _helper.TranslateFromOrigin(sourcePosition);
            // var finalDirection = _helper.RotateDirectionToPrincipalAxis(direction);         
            
            // the handling of specular needs work
            var weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);
            
            var dataPoint = new PhotonDataPoint(
                position,
                direction,
                weight,
                0.0,
                PhotonStateType.NotSet);

            var photon = new Photon { DP = dataPoint };

            return photon;
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion
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

            var customSource = new CustomPointSource2(
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

    public class CustomPointSource2 : ISource
    {
        private SourceHelper _helper;
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        public CustomPointSource2(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin, 
            SourceOrientation rotationFromInwardNormal)
        {
            _helper = new SourceHelper(translationFromOrigin, rotationFromInwardNormal);
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
        }

        /// <summary>
        /// Returns an instance of CustomPointSource with a specified translation, pointed normally inward
        /// </summary>
        /// <param name="translationFromOrigin"></param>
        public CustomPointSource2(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange, 
            Position translationFromOrigin)
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new SourceOrientation(0, 0))
        {
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // var initialPosition = SampleFiberSurface()
            // var finalPosition = initialPosition + _helper.TranslationFromOrigin;

            var finalPosition = _helper.TranslationFromOrigin;

            // sample angular distribution
            var initialDirection = SourceToolbox.SampleAngularDistributionDirection(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange, 
                Rng);

            var finalDirection = _helper.RotateDirectionToPrincipalAxis(initialDirection);  

            // the handling of specular needs work
            var weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

            var dataPoint = new PhotonDataPoint(
                finalPosition,
                finalDirection,
                weight,
                0.0,
                PhotonStateType.NotSet);

            var photon = new Photon { DP = dataPoint };

            return photon;
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion

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
