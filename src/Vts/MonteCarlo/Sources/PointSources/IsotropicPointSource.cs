using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class IsotropicPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Isotropic Point Source at a given location
        /// </summary>        
        /// <param name="location">Location of the point source</param> 
        public IsotropicPointSource(
            Position location = null)
            : base(
                SourceDefaults.DefaultFullPolarAngleRange.Clone().Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone().Clone(),                
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone().Clone(),
                location)
        {
            if (location == null)
                location = SourceDefaults.DefaultPosition.Clone();
        }    
    }
}
