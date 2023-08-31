using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for EllipticalSourceBase
    /// </summary>
    public abstract class EllipticalSourceBase : ISource
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
        /// "a" parameter of the ellipse source
        /// </summary>
        protected double _aParameter;
        /// <summary>
        /// "b" parameter of the ellipse source
        /// </summary>
        protected double _bParameter;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines EllipticalSourceBase class
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected EllipticalSourceBase(
            double aParameter,
            double bParameter,
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

            _aParameter = aParameter;
            _bParameter = bParameter;
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
            //Source starts from anywhere in the ellipse
            var finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _aParameter, _bParameter, Rng);

            // sample angular distribution
            var finalDirection = GetFinalDirection(finalPosition);

            //Find the relevent polar and azimuthal pair for the direction
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
        /// <param name="position">position</param>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(Position position); // position may or may not be needed

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double aParameter, double bParameter, Random rng)
        {
            var finalPosition = SourceDefaults.DefaultPosition.Clone();
            switch (sourceProfile.SourceProfileType)
            {
                case SourceProfileType.Flat:
                    finalPosition = SourceToolbox.GetPositionInAnEllipseRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        aParameter,
                        bParameter,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    if (sourceProfile is GaussianSourceProfile gaussianProfile)
                        finalPosition = SourceToolbox.GetPositionInAnEllipseRandomGaussian(
                            SourceDefaults.DefaultPosition.Clone(),
                            aParameter,
                            bParameter,
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
