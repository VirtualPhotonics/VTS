using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
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
        /// Initializes a new instance of CustomPointSourceInput class
        /// </summary>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="pointLocation">position</param>
        /// <param name="direction">direction</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
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
        /// Initializes the default constructor of CustomPointSourceInput class
        /// </summary>
        public CustomPointSourceInput()
            : this(
                new DoubleRange(0.0, 0.0),
                new DoubleRange(0.0, 0.0),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                0) { }

        /// <summary>
        /// Point source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// Polar angle range
        /// </summary>
        public DoubleRange PolarAngleEmissionRange { get; set; }
        /// <summary>
        /// Azimuthal angle range
        /// </summary>
        public DoubleRange AzimuthalAngleEmissionRange { get; set; }
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
