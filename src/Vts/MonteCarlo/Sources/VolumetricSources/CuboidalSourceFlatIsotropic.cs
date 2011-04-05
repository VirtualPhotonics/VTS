using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class CuboidalSourceFlatIsotropic : ISource
    {
        private Position _translationFromOrigin;
        private ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        private double _lengthX = 1.0;
        private double _lengthY = 1.0;
        private double _lengthZ = 1.0;
        private SourceFlags _rotationAndTranslationFlags;
        private double _radius;

        /// <summary>
        /// Returns an instance of Isotropic Flat Cuboidal Source with a specified translation and source axis rotation
        /// </summary>
        /// <param name="lengthX"></param>
        /// <param name="lengthY"></param>
        /// <param name="lengthZ"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CuboidalSourceFlatIsotropic(
            double lengthX,
            double lengthY,    
            double lengthZ,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true); 
        }

        /// <summary>
        /// Returns an instance of Isotropic Flat Cuboidal Source with a specified translation but no source axis rotation
        /// </summary>
        /// <param name="lengthX"></param>
        /// <param name="lengthY"></param>
        /// <param name="lengthZ"></param>
        /// <param name="translationFromOrigin"></param>
        public CuboidalSourceFlatIsotropic(
            double lengthX,
            double lengthY,
            double lengthZ, 
            Position translationFromOrigin)
            : this(
                lengthX,
                lengthY,
                lengthZ,
                translationFromOrigin,
                new ThreeAxisRotation (0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false); 
        }

        /// <summary>
        /// Returns an instance of Isotropic Flat Cuboidal Source with source axis rotation
        /// </summary>
        /// <param name="lengthX"></param>
        /// <param name="lengthY"></param>
        /// <param name="lengthZ"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CuboidalSourceFlatIsotropic(
            double lengthX,
            double lengthY,
            double lengthZ,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lengthX,
                lengthY,
                lengthZ, 
                new Position(0, 0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Isotropic Flat Cuboidal Source with no translation and no source axis rotation
        /// </summary>
        /// <param name="lengthX"></param>
        /// <param name="lengthY"></param>
        /// <param name="lengthZ"></param>
        public CuboidalSourceFlatIsotropic(
            double lengthX,
            double lengthY,
            double lengthZ)
            : this(
                lengthX,
                lengthY,
                lengthZ, 
                new Position(0, 0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Sample source position
            Position finalPosition = SourceToolbox.GetRandomFlatCuboidPosition(new Position(0, 0, 0),
                                    _lengthX,
                                    _lengthY,
                                    _lengthZ,
                                    Rng);
          

            //Source oriented along z-axis
            Direction finalDirection = SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);

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
