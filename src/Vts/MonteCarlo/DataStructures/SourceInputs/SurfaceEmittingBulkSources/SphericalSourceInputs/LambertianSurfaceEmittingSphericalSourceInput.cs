using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingSphericalSource 
    /// implementation including radius, position and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingSphericalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingSphericalSourceInput class
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingSphericalSourceInput(
            double radius,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.LambertianSurfaceEmittingSpherical;
            Radius = radius;            
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingSphericalSourceInput class
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        public LambertianSurfaceEmittingSphericalSourceInput(
            double radius)
            : this(
                radius,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianSurfaceEmittingSphericalSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingSphericalSourceInput()
            : this(
                1.0,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Spherical source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// The radius of the sphere
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
    }
}