using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
{   
    // todo: re-do this file for new sources

    /// <summary>
    /// Implements ISourceInput.  Defines input data for Directional PointSource 
    /// implementation including position, and direction.
    /// </summary>
    public class DirectionalPointSourceInput : ISourceInput
    {
        // this handles point
        public DirectionalPointSourceInput(
            Position pointLocation,
            Direction direction,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.DirectionalPoint;
            PointLocation = pointLocation;
            EmittingDirection = direction;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        public DirectionalPointSourceInput()
            : this(
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                0) { }

        public Position PointLocation { get; set; }
        public Direction EmittingDirection { get; set; }
        public SourceType SourceType { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
