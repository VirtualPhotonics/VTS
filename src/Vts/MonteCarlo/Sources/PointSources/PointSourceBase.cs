using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PointSourceBase : ISource
    {
        /// <summary>
        /// 
        /// </summary>
        protected DoubleRange _polarAngleEmissionRange;
        protected DoubleRange _azimuthalAngleEmissionRange;
        protected Position _translationFromOrigin;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis;
        protected SourceFlags _rotationAndTranslationFlags;       

        protected PointSourceBase( 
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone().Clone(),
                translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                false);

            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();    
            _translationFromOrigin = translationFromOrigin.Clone();
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();                   
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts at the origin            
            //Position finalPosition =  new Position(0.0, 0.0, 0.0);
            Position finalPosition = SourceDefaults.DefaultPosition.Clone();

            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);

            //Find the relevent polar and azimuthal pair for the direction
            _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Rotation and translation
            SourceToolbox.UpdateDirectionAndPositionAfterGivenFlags(
                finalPosition,
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
