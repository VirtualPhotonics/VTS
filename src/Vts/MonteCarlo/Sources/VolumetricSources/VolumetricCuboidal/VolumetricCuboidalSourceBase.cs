using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for VolumetricCuboidalSourceBase
    /// </summary>
    public abstract class VolumetricCuboidalSourceBase : ISource
    {
        /// <summary>
        /// Source profile type
        /// </summary>
        protected ISourceProfile _sourceProfile;
        /// <summary>
        /// New source axis direction
        /// </summary>
        protected Direction _newDirectionOfPrincipalSourceAxis;
        /// <summary>
        /// New source location
        /// </summary>
        protected Position _translationFromOrigin;
        /// <summary>
        /// Source rotation and translation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// The length of the cuboid
        /// </summary>
        protected double _cubeLengthX;
        /// <summary>
        /// The width of the cuboid
        /// </summary>
        protected double _cubeWidthY;
        /// <summary>
        /// The height of the cuboid
        /// </summary>
        protected double _cubeHeightZ;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines VolumetricCuboidalSourceBase class
        /// </summary>
        /// <param name="cubeLengthX">The length of the cuboid</param>
        /// <param name="cubeWidthY">The width of the cuboid</param>
        /// <param name="cubeHeightZ">The height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected VolumetricCuboidalSourceBase(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                false);

            _cubeLengthX = cubeLengthX;
            _cubeWidthY = cubeWidthY;
            _cubeHeightZ = cubeHeightZ;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the cuboid
            var finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeWidthY, _cubeHeightZ, Rng);

            // sample angular distribution
            var finalDirection = GetFinalDirection();

            //Find the relevent polar and azimuthal pair for the direction
            var _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, 1.0, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }

        /// <summary>
        /// Returns direction
        /// </summary>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(); // position may or may not be needed

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double cubeLengthX, double cubeWidthY, double cubeHeightZ, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.SourceProfileType)
            {
                case SourceProfileType.Flat:
                    finalPosition = SourceToolbox.GetPositionInACuboidRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        cubeLengthX,
                        cubeWidthY,
                        cubeHeightZ,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    if (sourceProfile is GaussianSourceProfile gaussianProfile)
                        finalPosition = SourceToolbox.GetPositionInACuboidRandomGaussian(
                            SourceDefaults.DefaultPosition.Clone(),
                            0.5 * cubeLengthX,
                            0.5 * cubeWidthY,
                            0.5 * cubeHeightZ,
                            gaussianProfile.BeamDiaFWHM,
                            rng);
                    break;
                case SourceProfileType.Image:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(sourceProfile.SourceProfileType.ToString());
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
