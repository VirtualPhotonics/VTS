using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for IsotropicPointSource implementation 
    /// including emitting position, direction and initial tissue region index.
    /// </summary>
    public class IsotropicPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of IsotropicPointSourceInput class
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicPointSourceInput(
            Position pointLocation,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.IsotropicPoint;
            PointLocation = pointLocation;
            InitialTissueRegionIndex = initialTissueRegionIndex;            
        }

        /// <summary>
        /// Initializes the default constructor of IsotropicPointSourceInput class
        /// </summary>
        public IsotropicPointSourceInput()
            : this(
                new Position (0, 0, 0),
                0) { }

        /// <summary>
        /// Point source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// New position
        /// </summary>
        public Position PointLocation { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
    }
}
