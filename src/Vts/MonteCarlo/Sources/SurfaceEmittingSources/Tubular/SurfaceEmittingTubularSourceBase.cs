using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SurfaceEmittingTubularSourceBase : ISource
    {        
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;        
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _tubeRadius;
        protected double _tubeHeightZ;
        

        protected SurfaceEmittingTubularSourceBase(
            double tubeRadius,
            double tubeHeightZ,  
            Direction newDirectionOfPrincipalSourceAxis,                  
            Position translationFromOrigin)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 false);
            
            _tubeRadius = tubeRadius;
            _tubeHeightZ = tubeHeightZ;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();      
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(), 
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                Rng);

            //Translate the photon to _tubeRadius length below the origin. Ring lies on yz plane.
            Position finalPosition = new Position(0.0, 0.0, _tubeRadius);

            //Sample a ring that emits photons outside.
            SourceToolbox.UpdateDireactionAndPositionAfterRotatingAroundXAxis(
                2.0 * Math.PI * Rng.NextDouble(),
                ref finalDirection,
                ref finalPosition);

            //Ring lies on xy plane. z= 0;
            SourceToolbox.UpdateDireactionAndPositionAfterRotatingAroundYAxis(
                0.5 * Math.PI,
                ref finalDirection,
                ref finalPosition);

            //Sample tube height
            finalPosition.Z = _tubeHeightZ * (2.0 * Rng.NextDouble() -1.0);

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Translation and source rotation
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
