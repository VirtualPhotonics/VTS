using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements LambertianSurfaceEmittingCuboidalSource with length, width, height, source profile, 
    /// direction, position, and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCuboidalSource : SurfaceEmittingCuboidalSourceBase
    {
        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting Cuboidal Source with a given source profile
        /// new source axis direction, and translation,
        /// </summary>
        /// <param name="cubeLengthX">The length of the cube (along x axis)</param>
        /// <param name="cubeWidthY">The width of the cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of the cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>       
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile, 
            Direction newDirectionOfPrincipalSourceAxis = null, 
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
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
