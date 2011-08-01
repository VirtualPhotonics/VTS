using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// Implements ISourceInput. Defines input data for IsotropicPointSource implementation 
    /// including emitting position, direction and initial tissue region index.
    /// </summary>
    public class IsotropicPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the IsotropicPointSourceInput class
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
        public IsotropicPointSourceInput(
            Position pointLocation,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.IsotropicPoint;
            PointLocation = pointLocation;
            InitialTissueRegionIndex = initialTissueRegionIndex;            
        }

        /// <summary>
        /// Initializes a new instance of the IsotropicPointSourceInput class
        /// </summary>
        public IsotropicPointSourceInput()
            : this(
                new Position (0, 0, 0),
                0) { }

        public Position PointLocation { get; set; }
        public SourceType SourceType { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
