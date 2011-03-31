using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    public class PointSourceMultiDirectional : ISource
    {        
        private Position _translationFromOrigin;
        private PolarAzimuthalRotationAngles _rotationFromInwardNormal;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public PointSourceMultiDirectional(           
            Position translationFromOrigin,
            PolarAzimuthalRotationAngles rotationFromInwardNormal)
        {            
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationFromInwardNormal = rotationFromInwardNormal.Clone();
        }
               
        /// <summary>
        /// Returns an instance of CustomPointSource with a specified translation, pointed normally inward
        /// </summary>        
        /// <param name="translationFromOrigin"></param>
        public PointSourceMultiDirectional(
            Position translationFromOrigin)
            : this(
                translationFromOrigin,
                new PolarAzimuthalRotationAngles(0, 0))
        {
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from the origin
            Position initialPosition = new Position(0, 0, 0);

            // sample angular distribution
            Direction initialDirection = SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);

            Direction finalDirection = initialDirection;

            //If source rotation angles are not equal to zero, returns the direction after rotation
            if ((_rotationFromInwardNormal.ThetaRotation == 0.0) && (_rotationFromInwardNormal.PhiRotation == 0.0))
            { }
            else
            { finalDirection = SourceToolbox.GetDirectionAfterRotationByGivenPolarAndAzimuthalAngle(_rotationFromInwardNormal, initialDirection); }

            Position finalPosition = initialPosition;

            //if translation is required, update the position            
            if ((_translationFromOrigin.X == 0.0) && (_translationFromOrigin.Y == 0.0) && (_translationFromOrigin.Z == 0.0))
            { }
            else
            { finalPosition = SourceToolbox.GetPositionafterTranslation(initialPosition, _translationFromOrigin); }     
          


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
