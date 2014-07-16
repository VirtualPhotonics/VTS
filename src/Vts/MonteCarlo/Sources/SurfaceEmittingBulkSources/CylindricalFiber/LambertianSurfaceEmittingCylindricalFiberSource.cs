using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements LambertianSurfaceEmittingCylindricalFiberSource with fiber radius, fiber height,
    /// curved surface efficiency, bottom surface efficiency, direction, position, and initial 
    /// tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCylindricalFiberSource : SurfaceEmittingCylindricalFiberSourceBase
    {

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting cylindrical fiber source with source axis rotation and translation
        /// </summary>
        /// <param name="fiberRadius">Fiber radius</param>
        /// <param name="fiberHeightZ">Fiber height</param>
        /// <param name="curvedSurfaceEfficiency">Efficciency of the curved surface (0-1)</param>
        /// <param name="bottomSurfaceEfficiency">Efficciency of the bottom surface (0-1)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCylindricalFiberSource(
            double fiberRadius,
            double fiberHeightZ,
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
            fiberRadius,
            fiberHeightZ,
            curvedSurfaceEfficiency,
            bottomSurfaceEfficiency,
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