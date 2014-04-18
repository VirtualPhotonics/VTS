using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements CustomLineSource with polar angle range, azimuthal angle range, emitting point
    /// location, direction and initial tissue region index.
    /// </summary>
    public class CustomPointSource : PointSourceBase
    { 
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
        }
    }
}
