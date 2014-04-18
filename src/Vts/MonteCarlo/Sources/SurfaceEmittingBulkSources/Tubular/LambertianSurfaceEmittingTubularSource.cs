using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements LambertianSurfaceEmittingTubularSource with tube radius, height, 
    /// direction, position and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingTubularSource : SurfaceEmittingTubularSourceBase
    {  
        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source with source axis rotation and translation
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingTubularSource(
            double tubeRadius,
            double tubeHeightZ,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
            tubeRadius,
            tubeHeightZ,
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
