using Vts.Common;

namespace Vts.MonteCarlo.Sources
{   
    // todo: re-do this file for new sources

    /// <summary>
    /// Implements ISourceInput.  Defines input data for PointSource implementation
    /// including position, direction and range of theta and phi.
    /// </summary>
    public class PointSourceCollimatedInput : ISourceInput
    {
        // this handles point
        public PointSourceCollimatedInput( 
            Position pointLocation,
            Direction solidAngleAxis,
            DoubleRange thetaRange, 
            DoubleRange phiRange) 
        {
            PointLocation = pointLocation;
            SolidAngleAxis = solidAngleAxis;
            ThetaRange = thetaRange;
            PhiRange = phiRange;
        }
        public PointSourceCollimatedInput()
            : this(
                new Position (0, 0, 0),
                new Direction(0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                new DoubleRange(0.0, 0, 1)) { }

        public Position PointLocation { get; set; }
        public Direction SolidAngleAxis { get; set; }
        public DoubleRange ThetaRange { get; set; }
        public DoubleRange PhiRange { get; set; }
    }
}
