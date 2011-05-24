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
    public class DirectionalPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Directional Point Source with a given emission direction at a given location
        /// </summary>        
        /// <param name="newDirectionOfPrincipalSourceAxis">Photon emitting direction</param>
        /// <param name="pointLocation">New position</param>
        public DirectionalPointSource(
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position pointLocation = null)
            : base(
                new DoubleRange(0.0, 0.0),
                new DoubleRange(0.0, 0.0),                
                newDirectionOfPrincipalSourceAxis,
                pointLocation)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis; 
            if (pointLocation == null)
                pointLocation = SourceDefaults.DefaultPosition;            
        }        
    }
}
