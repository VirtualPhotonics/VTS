using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class IsotropicSphericalSurfaceEmittingSource : SphericalSurfaceEmittingSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of Isotropic Spherical Surface Emitting Source with a specified translation.
        /// </summary>
        /// <param name="radius">The radius of the sphere</param> 
        /// <param name="translationFromOrigin">New source location</param>
        public IsotropicSphericalSurfaceEmittingSource(
            double radius,
            Position translationFromOrigin)
            : base(
                radius,
                new DoubleRange(0, Math.PI),
                new DoubleRange(0, 2.0 *Math.PI),
                translationFromOrigin,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Isotropic Spherical Surface Emitting Source.
        /// </summary>
        /// <param name="radius"></param>
        public IsotropicSphericalSurfaceEmittingSource(
            double radius)
            : this(
                radius,                
                new Position(0, 0, 0))
                
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        #endregion
    }

}
