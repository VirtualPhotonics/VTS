using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements CustomLineSource  with converging/diverging angle, emitting point location, 
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
    }
}
