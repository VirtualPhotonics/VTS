using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
{   
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomLineSource implementation 
    /// including converging/diverging angle, emitting point location, direction and 
    /// initial tissue region index.
    /// </summary>
    public class DirectionalPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of DirectionalPointSourceInput class
        /// </summary>
        /// <param name="pointLocation">New position</param>
        /// <param name="direction">Point source emitting direction</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalPointSourceInput(
            Position pointLocation,
            Direction direction,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.DirectionalPoint;
            PointLocation = pointLocation;
            Direction = direction;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of DirectionalPointSourceInput class
        /// </summary>
        public DirectionalPointSourceInput()
            : this(
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
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
        /// Point source emitting direction
        /// </summary>
        public Direction Direction { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
    }
}
