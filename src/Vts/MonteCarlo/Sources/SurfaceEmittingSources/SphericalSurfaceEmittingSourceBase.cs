using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SphericalSurfaceEmittingSourceBase : ISource
    {
        protected DoubleRange _polarAngleRangeToDefineSphericalSurface;  
        protected DoubleRange _azimuthalAngleRangeToDefineSphericalSurface;        
        protected Position _translationFromOrigin;
        protected ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _radius;        

        protected SphericalSurfaceEmittingSourceBase(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,  
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,            
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _radius = radius;
            _polarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface.Clone();
            _azimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface.Clone();            
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true); //??           
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(_polarAngleRangeToDefineSphericalSurface, _azimuthalAngleRangeToDefineSphericalSurface, Rng);
                        
            //Source starts from anywhere in the ellipsoid
            Position finalPosition = GetFinalPositionFromProfileType(finalDirection, _radius, Rng);

            //Lambertian distribution
            PolarAzimuthalAngles polarAzimuthalPair = SourceToolbox.GetRandomPolarAzimuthalForUniformPolarAndAzimuthalAngleRange(
                new DoubleRange (0.0, 0.5 * Math.PI),
                new DoubleRange(0.0, 2.0 * Math.PI),
                Rng);
            //Avoid updating the finalDirection during following rotation
            Position dummyPosition = finalPosition;

            //Rotate polar azimutahl angle by polarAzimuthalPair vector
            SourceToolbox.DoSourceRotationByGivenPolarAndAzimuthalAngle(polarAzimuthalPair, ref finalDirection, ref dummyPosition);
            
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
               

        private static Position GetFinalPositionFromProfileType(Direction finalDirection, double radius, Random rng)
        {
            Position finalPosition = null;            

            finalPosition.X = radius * finalDirection.Ux;
            finalPosition.Y = radius * finalDirection.Uy;
            finalPosition.Z = radius * finalDirection.Uz;

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
