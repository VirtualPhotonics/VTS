using System;
using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    // todo: re-do this file for new sources

    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomLineSource implementation 
    /// including polar angle range, azimuthal angle range, emitting point location,
    /// direction and initial tissue region index.
    /// </summary>
    public class CustomPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the CustomPointSourceInput class
        /// </summary>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="pointLocation">position</param>
        /// <param name="direction">direction</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
        public CustomPointSourceInput(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position pointLocation,
            Direction direction,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.CustomPoint;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            PointLocation = pointLocation;
            Direction = direction;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of the CustomPointSourceInput class
        /// </summary>
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
        public Direction Direction { get; set; }
        public SourceType SourceType { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
