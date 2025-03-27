﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomLineSource implementation 
    /// including polar angle range, azimuthal angle range, emitting point location,
    /// direction and initial tissue region index.
    /// "Custom" type sources assume Isotropic angular distribution
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
            SourceType = "CustomPoint";
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
        public string SourceType { get; set; }
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

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng ??= new Random();

            return new CustomPointSource(
                PolarAngleEmissionRange,
                AzimuthalAngleEmissionRange,
                Direction,
                PointLocation,
                InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CustomLineSource with polar angle range, azimuthal angle range, emitting point
    /// location, direction and initial tissue region index.
    /// </summary>
    public class CustomPointSource : PointSourceBase
    {
        private readonly DoubleRange _polarAngleEmissionRange;
        private readonly DoubleRange _azimuthalAngleEmissionRange;
        /// <summary>
        /// Returns an instance of Custom Point Source for a given polar and azimuthal angle range, 
        /// new source axis direction, and  translation.
        /// </summary>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">Point source emitting direction</param>
        /// <param name="pointLocation">New position</param>  
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position pointLocation = null,
            int initialTissueRegionIndex = 0
            )
            : base(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,                
                newDirectionOfPrincipalSourceAxis,
                pointLocation,
                initialTissueRegionIndex)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }
}
