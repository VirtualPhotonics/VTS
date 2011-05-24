using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    // todo: re-do this file for new sources

    /// <summary>
    /// Implements ISourceInput.  Defines input data for Custom PointSource implementation
    /// including position, direction and range of theta (polar angle) and phi (azimuthal angle).
    /// </summary>
    public class CustomPointSourceInput : ISourceInput
    {
        // this handles point
        public CustomPointSourceInput(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position pointLocation,
            Direction emittingDirection)
        {
            SourceType = SourceType.DirectionalPoint;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            PointLocation = pointLocation;
            EmittingDirection = emittingDirection;
        }

        public CustomPointSourceInput()
            : this(
                new DoubleRange(0.0, 0.0),
                new DoubleRange(0.0, 0.0),
                new Position(0, 0, 0),
                new Direction(0, 0, 1)) { }

        public DoubleRange PolarAngleEmissionRange { get; set; }
        public DoubleRange AzimuthalAngleEmissionRange { get; set; }
        public Position PointLocation { get; set; }
        public Direction EmittingDirection { get; set; }
        public SourceType SourceType { get; set; }
    }
}
