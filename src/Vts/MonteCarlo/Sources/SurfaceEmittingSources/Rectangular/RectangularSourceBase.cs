using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class RectangularSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;
        protected PolarAzimuthalAngles _beamRotationFromInwardNormal;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _rectLengthX;
        protected double _rectWidthY;

        protected RectangularSourceBase(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 beamRotationFromInwardNormal != SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone());
            
            _rectLengthX = rectLengthX;
            _rectWidthY = rectWidthY;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _beamRotationFromInwardNormal = beamRotationFromInwardNormal.Clone();         
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the line
            Position finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _rectLengthX, _rectWidthY, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection(finalPosition);

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _beamRotationFromInwardNormal,
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

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double rectLengthX, double rectWidthY, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetPositionInARectangleRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        rectLengthX,
                        rectWidthY,
                        rng);
                    break;                                
                case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetPositionInARectangleRandomGaussian(
                        SourceDefaults.DefaultPosition.Clone(),
                        rectLengthX,
                        rectWidthY,
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
