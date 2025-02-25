using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianPointSource implementation 
    /// including emitting position, direction and initial tissue region index.
    /// </summary>
    public class LambertianPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianPointSourceInput class with 
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="lambertOrder">Order of Lambertian angular distribution</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianPointSourceInput(
            Position pointLocation,
            int lambertOrder,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianPoint";
            LambertOrder = lambertOrder;
            PointLocation = pointLocation;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianPointSourceInput class assuming
        /// LambertOrder = 1
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianPointSourceInput(
            Position pointLocation,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianPoint";
            LambertOrder = 1;
            PointLocation = pointLocation;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of LambertianPointSourceInput class
        /// </summary>
        public LambertianPointSourceInput()
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
        /// Lambertian order
        /// </summary>
        public int LambertOrder { get; set; }

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

            return new LambertianPointSource(
                PointLocation,
                LambertOrder,
                InitialTissueRegionIndex) { Rng = rng }; 
        }
    }

    /// <summary>
    /// Implements LambertianPointSource with emitting position, direction and initial 
    /// tissue region index.
    /// </summary>
    public class LambertianPointSource : PointSourceBase
    {
        private readonly int _lambertOrder;

        /// <summary>
        /// Returns an instance of Lambertian Point Source at a given location
        /// </summary>        
        /// <param name="pointLocation">Location of the point source</param>
        /// <param name="lambertOrder">Lambertian order of angular distribution</param> 
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianPointSource(
            Position pointLocation,
            int lambertOrder,
            int initialTissueRegionIndex = 0)
            : base(
                SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),                
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                pointLocation,
                initialTissueRegionIndex)
        {
            _lambertOrder=lambertOrder;
        }

        /// <summary>
        /// Returns direction for Lambertian distribution
        /// </summary>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection()
        {
            //Sample angular distribution with full range of theta and phi
            var finalDirection = SourceToolbox.GetDirectionForLambertianRandom(_lambertOrder, Rng);

            return finalDirection;
        }
    }
}
