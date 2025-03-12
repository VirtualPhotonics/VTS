﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for IsotropicPointSource implementation 
    /// including emitting position, direction and initial tissue region index.
    /// </summary>
    public class IsotropicPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of IsotropicPointSourceInput class
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicPointSourceInput(
            Position pointLocation,
            int initialTissueRegionIndex)
        {
            SourceType = "IsotropicPoint";
            PointLocation = pointLocation;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of IsotropicPointSourceInput class
        /// </summary>
        public IsotropicPointSourceInput()
            : this(
                new Position(0, 0, 0),
                0)
        {
        }

        /// <summary>
        /// Point source type
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// New position
        /// </summary>
        public Position PointLocation { get; set; }

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

            return new IsotropicPointSource(
                this.PointLocation,
                this.InitialTissueRegionIndex) { Rng = rng }; 
        }
    }

    /// <summary>
    /// Implements IsotropicPointSource with emitting position, direction and initial 
    /// tissue region index.
    /// </summary>
    public class IsotropicPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Isotropic Point Source at a given location
        /// </summary>        
        /// <param name="pointLocation">Location of the point source</param> 
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicPointSource(
            Position pointLocation = null,
            int initialTissueRegionIndex = 0)
            : base(
                SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),                
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                pointLocation,
                initialTissueRegionIndex)
        {
        }

        /// <summary>
        /// Returns direction for Isotropic distribution
        /// </summary>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection()
        {
            //Sample angular distribution with full range of theta and phi
            var finalDirection = SourceToolbox.GetDirectionForIsotropicDistributionRandom(Rng);

            return finalDirection;
        }
    }
}
