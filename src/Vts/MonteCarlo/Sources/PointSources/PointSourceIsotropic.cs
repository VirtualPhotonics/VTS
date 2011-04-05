using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class PointSourceIsotropic : ISource
    {
        private Position _translationFromOrigin;
        private SourceFlags _rotationAndTranslationFlags;

        /// <summary>
        /// Returns an instance of MultiDirectional PointSource
        /// </summary>
        /// <param name="translationFromOrigin"></param>
        public PointSourceIsotropic(Position translationFromOrigin)
        {
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, false); 
        }
                       
        /// <summary>
        /// Returns an instance of Multidirectional/Isotropic PointSource with no translation
        /// </summary>
        public PointSourceIsotropic()
            : this(new Position(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts at the origin
            Position finalPosition = new Position(0, 0, 0);

            //Source oriented along z-axis
            Direction finalDirection = SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);

            //Rotation and translation
            SourceToolbox.DoRotationandTranslationForGivenFlags(
                ref finalPosition,
                ref finalDirection,               
                _translationFromOrigin,
                new PolarAzimuthalAngles(), // todo: fix
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
