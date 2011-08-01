using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingTubularSource 
    /// implementation including tube radius, height, direction, position and initial tissue 
    /// region index.
    /// </summary>
    public class LambertianSurfaceEmittingTubularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
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
        /// Initializes a new instance of the LambertianSurfaceEmittingTubularSourceInput class
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
        /// Initializes a new instance of the LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingTubularSourceInput()
            : this(
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        public SourceType SourceType { get; set; }
        public double TubeRadius { get; set; }
        public double TubeHeightZ { get; set; }   
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}