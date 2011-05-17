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
        /// <param name="pointLocation">New source location</param> 
        public IsotropicPointSource(
            Position pointLocation)
            : base(
                new DoubleRange(0.0, Math.PI),
                new DoubleRange(0.0, 2.0 * Math.PI),
                pointLocation,
                new PolarAzimuthalAngles(0.0, 0.0))
        {
        }

 
        /// <summary>
        ///  Returns an instance of Isotropic Point Source emitting at the origin (0,0,0).
        /// </summary>
        public IsotropicPointSource()
            : this(
                new Position(0.0, 0.0, 0.0))
        {
        }

    }
}
