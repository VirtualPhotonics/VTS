using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
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
            if (pointLocation == null)
                pointLocation = SourceDefaults.DefaultPosition.Clone();
        }    
    }
}
