using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for SurfaceEmittingSphericalSourceBase
    /// </summary>
    public abstract class SurfaceEmittingSphericalSourceBase : ISource
    {
        /// <summary>
        /// Polar angle range to define the emitting area of the sphere
        /// </summary>
        protected DoubleRange _polarAngleRangeToDefineSphericalSurface;  
        /// <summary>
        /// Azimuthal angle range to define the emitting area of the sphere
        /// </summary>
        protected DoubleRange _azimuthalAngleRangeToDefineSphericalSurface;
        /// <summary>
        /// New source axis direction
        /// </summary>
        protected Direction _newDirectionOfPrincipalSourceAxis;
        /// <summary>
        /// New source location
        /// </summary>
        protected Position _translationFromOrigin;
        /// <summary>
        /// Source rotation and translation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// The radius of the sphere
        /// </summary>
        protected double _radius;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines SurfaceEmittingSphericalSourceBase class
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="polarAngleRangeToDefineSphericalSurface">polar angle range to define the emitting area of the sphere</param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface">azimuthal angle range to define the emitting area of the sphere</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected SurfaceEmittingSphericalSourceBase(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,  
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,            
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _radius = radius;
            _polarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface.Clone();
            _azimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface.Clone();
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone(); 
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(_polarAngleRangeToDefineSphericalSurface, _azimuthalAngleRangeToDefineSphericalSurface, Rng);
                        
            //Source starts from anywhere in the sphere
            Position finalPosition = GetFinalPositionFromProfileType(finalDirection, _radius, Rng);

            //Lambertian distribution (uniform hemispherical distribution)
            PolarAzimuthalAngles polarAzimuthalPair = SourceToolbox.GetPolarAzimuthalPairForGivenAngleRangeRandom(
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                Rng);

            //Avoid updating the finalDirection during following rotation
            Position dummyPosition = finalPosition;

            //Rotate polar azimutahl angle by polarAzimuthalPair vector
            SourceToolbox.UpdateDirectionPositionAfterRotatingByGivenAnglePair(polarAzimuthalPair, ref finalDirection, ref dummyPosition);

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Translation and source rotation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, tissue, _initialTissueRegionIndex, Rng);

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
