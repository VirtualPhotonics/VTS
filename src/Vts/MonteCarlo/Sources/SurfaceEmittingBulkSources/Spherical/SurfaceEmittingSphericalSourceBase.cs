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
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 false);
            _radius = radius;
            _polarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface.Clone();
            _azimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface.Clone();
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone(); 
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Returns direction
        /// </summary>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(Direction direction, Position position);

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            // sample angular distribution and compute normal vector direction (normalVectorDirection)
            Direction normalVectorDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleRangeToDefineSphericalSurface,
                _azimuthalAngleRangeToDefineSphericalSurface,
                Rng);

            //Compute emitting location based on normalVectorDirection
            Position finalPosition = GetFinalPositionFromProfileType(normalVectorDirection, _radius);

            //Get final direction
            Direction finalDirection = GetFinalDirection(normalVectorDirection, finalPosition);

            //Find the relevant polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Translation and source rotation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, 1.0, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }
               
        private static Position GetFinalPositionFromProfileType(Direction finalDirection, double radius)
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
