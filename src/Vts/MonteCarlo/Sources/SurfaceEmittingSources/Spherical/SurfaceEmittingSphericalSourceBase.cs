using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SurfaceEmittingSphericalSourceBase : ISource
    {
        protected DoubleRange _polarAngleRangeToDefineSphericalSurface;  
        protected DoubleRange _azimuthalAngleRangeToDefineSphericalSurface;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _radius;

        protected SurfaceEmittingSphericalSourceBase(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,  
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,            
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            _radius = radius;
            _polarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface.Clone();
            _azimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface.Clone();
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone(); 
            _translationFromOrigin = translationFromOrigin.Clone();        
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(_polarAngleRangeToDefineSphericalSurface, _azimuthalAngleRangeToDefineSphericalSurface, Rng);
                        
            //Source starts from anywhere in the sphere
            Position finalPosition = GetFinalPositionFromProfileType(finalDirection, _radius, Rng);

            //Lambertian distribution (uniform hemispherical distribution)
            PolarAzimuthalAngles polarAzimuthalPair = SourceToolbox.GetRandomPolarAzimuthalPairForGivenPolarAndAzimuthalAngleRangeRandom(
                SourceDefaults.DefaultHalfPolarAngleRange.Clone().Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone().Clone(),
                Rng);

            //Avoid updating the finalDirection during following rotation
            Position dummyPosition = finalPosition;

            //Rotate polar azimutahl angle by polarAzimuthalPair vector
            SourceToolbox.UpdateDireactionAndPositionAfterRotatingByGivenPolarAndAzimuthalAngle(polarAzimuthalPair, ref finalDirection, ref dummyPosition);

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
               

        private static Position GetFinalPositionFromProfileType(Direction finalDirection, double radius, Random rng)
        {
            if (radius == 0.0)
                return new Position(0, 0, 0);
            else
                return new Position(
                    radius * finalDirection.Ux,
                    radius * finalDirection.Uy,
                    radius * finalDirection.Uz);           
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
