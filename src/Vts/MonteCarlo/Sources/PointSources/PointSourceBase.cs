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
        protected PolarAzimuthalAngles _rotationFromInwardNormal;        
        protected SourceFlags _rotationAndTranslationFlags;       

        protected PointSourceBase( 
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                translationFromOrigin != SourceDefaults.DefaultTranslationFromOrigin,
                rotationFromInwardNormal != SourceDefaults.DefaultRoationFromInwardNormal,
                false);

            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();    
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationFromInwardNormal = rotationFromInwardNormal.Clone();                   
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts at the origin
            Position finalPosition = new Position(0, 0, 0);

            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
            
            //Rotation and translation
            SourceToolbox.DoRotationandTranslationForGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _translationFromOrigin,
                _rotationFromInwardNormal,                
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
