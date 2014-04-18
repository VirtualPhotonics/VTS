using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements CustomSurfaceEmittingSphericalSource with radius, source profile, 
    /// polar angle range, azimuthal angle range, direction, position and initial 
    /// tissue region index.
    /// </summary>
    public class CustomSurfaceEmittingSphericalSource : SurfaceEmittingSphericalSourceBase
    {
        /// <summary>
        /// Returns an instance of Custom Spherical Surface Emitting Source with a user defined surface area
        /// (based on polar azimuthal angle range), new source axis direction, and translation,
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="polarAngleRangeToDefineSphericalSurface">Polar angle range to define the emitting area of the sphere</param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface">Azimuthal angle range to define the emitting area of the sphere</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomSurfaceEmittingSphericalSource(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                initialTissueRegionIndex)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
        }
                
    }
}
