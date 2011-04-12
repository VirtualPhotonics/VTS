using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class CuboidalSurfaceEmmitingSourceFlat : ISource
    {        
        private Position _translationFromOrigin;
        private ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        private SourceFlags _rotationAndTranslationFlags;
        private double _xLength = 1.0;
        private double _yLength = 1.0;
        private double _zLength = 1.0;

        /// <summary>
        /// Returns an instance of Cuboidal Surface Emission Source with a specified translation and source axis rotation
        /// </summary>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="zLength"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CuboidalSurfaceEmmitingSourceFlat(
            double xLength,
            double yLength,
            double zLength,            
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {           
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Cuboidal Surface Emission Source with a specified translation
        /// </summary>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="zLength"></param>
        /// <param name="translationFromOrigin"></param>
        public CuboidalSurfaceEmmitingSourceFlat(
            double xLength,
            double yLength,
            double zLength,
                        Position translationFromOrigin)
            : this(
                xLength,
                yLength,
                zLength,
                translationFromOrigin,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Cuboidal Surface Emission Source with a source axis rotation
        /// </summary>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="zLength"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CuboidalSurfaceEmmitingSourceFlat(
            double xLength,
            double yLength,
            double zLength,            
            ThreeAxisRotation rotationOfPrincipalSourceAxis
            )
            : this(
                xLength,
                yLength,
                zLength,
                new Position(0, 0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Cuboidal Surface Emission Source with no translation or source axis rotation
        /// </summary>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="zLength"></param>
        public CuboidalSurfaceEmmitingSourceFlat(
            double xLength,
            double yLength,
            double zLength)
            : this(
                xLength,
                yLength,
                zLength,               
                new Position(0, 0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Sample polar and azimuthal angle
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                new DoubleRange (0, 0.5* Math.PI),
                new DoubleRange(0, 2.0 * Math.PI),
                Rng);

            Position finalPosition = new Position();

            //Obtain the position and the direction
            SourceToolbox.DoDirectionAndPositionForCuboidSurface(
             ref finalDirection,
             ref finalPosition,
             _xLength,
             _yLength,
             _zLength,
             Rng);

            //Rotation and translation
            SourceToolbox.DoRotationandTranslationForGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _translationFromOrigin,
                _rotationOfPrincipalSourceAxis,
                _rotationAndTranslationFlags);  

            //Rotation and translation
            SourceToolbox.DoRotationandTranslationForGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _translationFromOrigin,
                _rotationOfPrincipalSourceAxis,
                _rotationAndTranslationFlags);

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
}
