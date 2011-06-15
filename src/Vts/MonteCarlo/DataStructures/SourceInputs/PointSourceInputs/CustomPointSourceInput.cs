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
            Direction emittingDirection,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.CustomPoint;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            PointLocation = pointLocation;
            EmittingDirection = emittingDirection;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        public CustomPointSourceInput()
            : this(
                new DoubleRange(0.0, 0.0),
                new DoubleRange(0.0, 0.0),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                0) { }

        public DoubleRange PolarAngleEmissionRange { get; set; }
        public DoubleRange AzimuthalAngleEmissionRange { get; set; }
        public Position PointLocation { get; set; }
        public Direction EmittingDirection { get; set; }
        public SourceType SourceType { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
