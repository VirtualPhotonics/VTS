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
        /// Returns an instance of Isotropic Point Source at the translated location
        /// </summary>        
        /// <param name="translationFromOrigin">New source location</param> 
        public IsotropicPointSource(
            Position translationFromOrigin)
            : base(
                new DoubleRange(0, Math.PI),
                new DoubleRange(0, 2.0 * Math.PI),
                translationFromOrigin,
                new PolarAzimuthalAngles (0,0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

 
        /// <summary>
        ///  Returns an instance of Isotropic Point Source emitting at the origin.
        /// </summary>
        public IsotropicPointSource()
            : this(
                new Position(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

    }
}
