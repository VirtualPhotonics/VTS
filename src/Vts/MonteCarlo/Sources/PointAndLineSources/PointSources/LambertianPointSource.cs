using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianPointSource implementation 
    /// including emitting position, direction and initial tissue region index.
    /// </summary>
    public class LambertianPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianPointSourceInput class
        /// </summary>
        /// <param name="pointLocation">position</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianPointSourceInput(
            Position pointLocation,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianPoint";
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

            return new LambertianPointSource(
                this.PointLocation,
                this.InitialTissueRegionIndex) { Rng = rng }; 
        }
    }

    /// <summary>
    /// Implements LambertianPointSource with emitting position, direction and initial 
    /// tissue region index.
    /// </summary>
    public class LambertianPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Lambertian Point Source at a given location
        /// </summary>        
        /// <param name="pointLocation">Location of the point source</param> 
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianPointSource(
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
    }
}
