using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class EllipsoidalSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected Position _translationFromOrigin;
        protected ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _aParameter;
        protected double _bParameter;
        protected double _cParameter;

        protected EllipsoidalSourceBase(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _aParameter = aParameter;
            _bParameter = bParameter;
            _cParameter = cParameter;
            _sourceProfile = sourceProfile;
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true); //??           
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the ellipsoid
            Position finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _aParameter, _bParameter, _cParameter, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection();

            //Rotation and translation
            SourceToolbox.UpdateDirectionAndPositionAfterGivenFlags(
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

        protected abstract Direction GetFinalDirection(); // position may or may not be needed

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double aParameter, double bParameter, double cParameter, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetRandomFlatEllipsoidPosition(
                        new Position(0, 0, 0),
                        2 * aParameter,
                        2 * bParameter,
                        2 * cParameter,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetRandomGaussianEllipsoidPosition(
                        new Position(0, 0, 0),
                        2 * aParameter,
                        2 * bParameter,
                        2 * cParameter,
                        gaussianProfile.BeamDiaFWHM,
                        rng);
                    break;
            }
            return finalPosition;
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
