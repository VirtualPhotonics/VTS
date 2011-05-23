using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class CircularSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected Position _translationFromOrigin;
        protected PolarAzimuthalAngles _rotationFromInwardNormal;
        protected ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _innerRadius;
        protected double _outerRadius;

        protected CircularSourceBase(
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _innerRadius = innerRadius;
            _outerRadius = outerRadius;
            _sourceProfile = sourceProfile;
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationFromInwardNormal = rotationFromInwardNormal.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, true, true); //??           
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the line
            Position finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _innerRadius, _outerRadius, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection(finalPosition);

            //Rotation and translation
            SourceToolbox.UpdateDirectionAndPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _translationFromOrigin,
                _rotationFromInwardNormal,
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

        protected abstract Direction GetFinalDirection(Position finalPosition); // position may or may not be needed

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double innerRadius, double outerRadius, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetRandomFlatCirclePosition(
                        new Position(0, 0, 0),
                        innerRadius,
                        outerRadius,
                        rng);
                    break;
                 case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetRandomGaussianCirclePosition(
                        new Position(0, 0, 0),
                        outerRadius,                        
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
