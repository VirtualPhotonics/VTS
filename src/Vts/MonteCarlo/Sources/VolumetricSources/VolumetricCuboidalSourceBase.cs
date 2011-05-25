using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class VolumetricCuboidalSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;        
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _cubeLengthX;
        protected double _cubeWidthY;
        protected double _cubeHeightZ;

        protected VolumetricCuboidalSourceBase(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                translationFromOrigin != SourceDefaults.DefaultPosition,
                false);

            _cubeLengthX = cubeLengthX;
            _cubeWidthY = cubeWidthY;
            _cubeHeightZ = cubeHeightZ;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();          
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the cuboid
            Position finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeWidthY, _cubeHeightZ, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection();

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Rotation and translation
            SourceToolbox.UpdateDirectionAndPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
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

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double cubeLengthX, double cubeWidthY, double cubeHeightZ, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetRandomFlatCuboidPosition(
                        SourceDefaults.DefaultPosition,
                        cubeLengthX,
                        cubeWidthY,
                        cubeHeightZ,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetRandomGaussianCuboidPosition(
                        SourceDefaults.DefaultPosition,
                        cubeLengthX,
                        cubeWidthY,
                        cubeHeightZ,
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
