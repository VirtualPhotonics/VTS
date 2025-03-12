﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
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
            SourceType = "DirectionalPoint";
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
        public string SourceType { get; set; }
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
            rng = rng ?? new Random();

            return new DirectionalPointSource(
                this.Direction,
                this.PointLocation,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements DirectionalPointSource  with emitting point location, 
    /// direction and initial tissue region index.
    /// </summary>
    public class DirectionalPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Directional Point Source with a given emission direction at a given location
        /// </summary>        
        /// <param name="direction">Point source emitting direction</param>
        /// <param name="pointLocation">New position</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalPointSource(
            Direction direction,
            Position pointLocation = null,
            int initialTissueRegionIndex = 0)
            : base(
                new DoubleRange(0.0, 0.0),
                new DoubleRange(0.0, 0.0),
                direction,
                pointLocation,
                initialTissueRegionIndex)
        {
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <returns>new direction</returns>       
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                new DoubleRange(0.0, 0.0),
                    new DoubleRange(0.0,0.0),
                    Rng);
        }
    }
}
