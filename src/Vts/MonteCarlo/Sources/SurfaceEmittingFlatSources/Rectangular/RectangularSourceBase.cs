using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for RectangularSourceBase
    /// </summary>
    public abstract class RectangularSourceBase : ISource
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
        /// Beam rotation from inward normal
        /// </summary>
        protected PolarAzimuthalAngles _beamRotationFromInwardNormal;
        /// <summary>
        /// Source rotation and translation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// The length of the Rectangular Source
        /// </summary>
        protected double _rectLengthX;
        /// <summary>
        /// The width of the Rectangular Source
        /// </summary>
        protected double _rectWidthY;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines RectangularSourceBase class
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected RectangularSourceBase(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 beamRotationFromInwardNormal != SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone());
            
            _rectLengthX = rectLengthX;
            _rectWidthY = rectWidthY;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _beamRotationFromInwardNormal = beamRotationFromInwardNormal.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the line
            var finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _rectLengthX, _rectWidthY, Rng);

            // sample angular distribution
            var finalDirection = GetFinalDirection(finalPosition);

            //Find the relevant polar and azimuthal pair for the direction
            var rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _beamRotationFromInwardNormal,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, 1.0, tissue, _initialTissueRegionIndex, Rng); 

            return photon;
        }

        /// <summary>
        /// Returns final direction for a given position
        /// </summary>
        /// <param name="position">Current position</param>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(Position position); // position may or may not be needed
        /// <summary>
        /// returns final position
        /// </summary>
        /// <returns>Position</returns>
        protected virtual Position GetFinalPosition()
        {
            return GetFinalPositionFromProfileType(_sourceProfile, _rectLengthX, _rectWidthY, Rng);
        }
        /// <summary>
        /// returns final position from profile type
        /// </summary>
        /// <param name="sourceProfile">ISourceProfile</param>
        /// <param name="rectLengthX">rectangular length in x direction (length)</param>
        /// <param name="rectWidthY">rectangular length in y direction (width)</param>
        /// <param name="rng">random number generator</param>
        /// <returns>Position</returns>
        protected static Position GetFinalPositionFromProfileType(
            ISourceProfile sourceProfile, double rectLengthX, double rectWidthY, Random rng)
        {
            var finalPosition = SourceDefaults.DefaultPosition.Clone();
            switch (sourceProfile.SourceProfileType)
            {
                case SourceProfileType.Flat:
                    finalPosition = SourceToolbox.GetPositionInARectangleRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        rectLengthX,
                        rectWidthY,
                        rng);
                    break;                                
                case SourceProfileType.Gaussian:
                    if (sourceProfile is GaussianSourceProfile gaussianProfile) 
                        finalPosition = SourceToolbox.GetPositionInARectangleRandomGaussian(
                            SourceDefaults.DefaultPosition.Clone(),
                            0.5 * rectLengthX,
                            0.5 * rectWidthY,
                            gaussianProfile.BeamDiaFWHM,
                            rng);
                    break;
                case SourceProfileType.Image:
                    finalPosition = ((ImageSourceProfile)sourceProfile).GetPositionInARectangleBasedOnImageIntensity(
                        rng);
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
