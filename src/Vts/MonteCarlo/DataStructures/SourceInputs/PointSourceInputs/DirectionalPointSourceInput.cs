using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{   
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomLineSource implementation 
    /// including converging/diverging angle, emitting point location, direction and 
    /// initial tissue region index.
    /// </summary>
    public class DirectionalPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the DirectionalPointSourceInput class
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="direction">direction</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
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
        /// Initializes a new instance of the DirectionalPointSourceInput class
        /// </summary>
        public DirectionalPointSourceInput()
            : this(
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                0) { }

        public Position PointLocation { get; set; }
        public Direction Direction { get; set; }
        public SourceType SourceType { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
