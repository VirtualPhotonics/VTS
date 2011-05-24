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
    public class CustomPointSource : PointSourceBase
    { 
        /// <summary>
        /// Returns an instance of Custom Point Source for a given polar and azimuthal angle range, 
        /// new source axis direction, and  translation.
        /// </summary>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="pointLocation">New position</param>        
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position pointLocation = null
            )
            : base(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,                
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
