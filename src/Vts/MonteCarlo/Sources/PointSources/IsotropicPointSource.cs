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
        /// <param name="pointLocation">Location of the point source</param> 
        public IsotropicPointSource(
            Position pointLocation = null)
            : base(
                SourceDefaults.DefaultFullPolarAngleRange,
                SourceDefaults.DefaultAzimuthalAngleRange,                
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                pointLocation)
        {
            if (pointLocation == null)
                pointLocation = SourceDefaults.DefaultPosition;
        }    
    }
}
