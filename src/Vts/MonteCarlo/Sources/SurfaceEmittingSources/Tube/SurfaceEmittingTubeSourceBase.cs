using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SurfaceEmittingTubeSourceBase : ISource
    {        
        protected Position _translationFromOrigin;
        protected ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _tubeRadius;
        protected double _tubeHeightZ;
        

        protected SurfaceEmittingTubeSourceBase(
            double tubeRadius,
            double tubeHeightZ,                               
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _tubeRadius = tubeRadius;
            _tubeHeightZ = tubeHeightZ;           
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true); //??           
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            

            //sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                new DoubleRange(0, 0.5 * Math.PI), 
                new DoubleRange(0, 2.0 * Math.PI),
                Rng);

            //Translate the photon to _tubeRadius length below the origin. Ring lies on yz plane.
            Position finalPosition = new Position(0.0, 0.0, _tubeRadius);

            //Sample a ring that emits photons outside.
            SourceToolbox.DoSourceRotationAroundXAxis(
                2.0 * Math.PI * Rng.NextDouble(),
                ref finalDirection,
                ref finalPosition);

            //Ring lies on xy plane. z= 0;
            SourceToolbox.DoSourceRotationAroundYAxis(
                0.5 * Math.PI,
                ref finalDirection,
                ref finalPosition);

            //Sample tube height
            finalPosition.Z = _tubeHeightZ * (2.0 * Rng.NextDouble() -1.0);

            
            
            //Translation and source rotation
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
