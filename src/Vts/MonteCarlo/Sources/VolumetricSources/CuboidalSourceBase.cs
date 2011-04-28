﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class CuboidalSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected Position _translationFromOrigin;
        protected ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _cubeLengthX;
        protected double _cubeWidthY;
        protected double _cubeHeightZ;

        protected CuboidalSourceBase(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _cubeLengthX = cubeLengthX;
            _cubeWidthY = cubeWidthY;
            _cubeHeightZ = cubeHeightZ;
            _sourceProfile = sourceProfile;
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true); //??           
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the cuboid
            Position finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeWidthY, _cubeHeightZ, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection();

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

        protected abstract Direction GetFinalDirection(); // position may or may not be needed

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double cubeLengthX, double cubeWidthY, double cubeHeightZ, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetRandomFlatCuboidPosition(
                        new Position(0, 0, 0),
                        cubeLengthX,
                        cubeWidthY,
                        cubeHeightZ,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetRandomGaussianCuboidPosition(
                        new Position(0, 0, 0),
                        cubeLengthX,
                        cubeWidthY,
                        cubeHeightZ,
                        gaussianProfile.StdDev,
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
