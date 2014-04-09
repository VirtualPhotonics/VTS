using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements LambertianSurfaceEmittingSphericalSource with radius, position and
    /// initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingSphericalSource : SurfaceEmittingSphericalSourceBase
    {
        /// <summary>
        /// Returns an instance of Lambertian Spherical Surface Emitting Source with a specified translation.
        /// </summary>
        /// <param name="radius">The radius of the sphere</param> 
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingSphericalSource(
            double radius,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                radius,
                SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                translationFromOrigin,
                initialTissueRegionIndex)
        {
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
        }        
    }
}
