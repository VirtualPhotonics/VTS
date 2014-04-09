using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingTubularSource 
    /// implementation including tube radius, height, direction, position and initial tissue 
    /// region index.
    /// </summary>
    public class LambertianSurfaceEmittingTubularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingTubularSourceInput(
            double tubeRadius,
            double tubeHeightZ,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.LambertianSurfaceEmittingTubular;
            TubeRadius = tubeRadius;
            TubeHeightZ = tubeHeightZ;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        public LambertianSurfaceEmittingTubularSourceInput(
            double tubeRadius,
            double tubeHeightZ)
            : this(
                tubeRadius,
                tubeHeightZ,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingTubularSourceInput()
            : this(
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Tubular source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// Tube radius
        /// </summary>
        public double TubeRadius { get; set; }
        /// <summary>
        /// Tube height
        /// </summary>
        public double TubeHeightZ { get; set; }
        /// <summary>
        /// New source axis direction
        /// </summary>
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
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