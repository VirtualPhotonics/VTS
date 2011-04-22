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
    public class CustomSurfaceEmittingSphericalSource : SurfaceEmittingSphericalSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of Custom Spherical Surface Emitting Source with user defined surface
        /// (based on polar azimuthal angle range), translation and source axis rotation
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="polarAngleRangeToDefineSphericalSurface">polar angle range to define the emitting area of the sphere</param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface">azimuthal angle range to define the emitting area of the sphere</param>
        /// <param name="translationFromOrigin">New source location</param>
        public CustomSurfaceEmittingSphericalSource(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                translationFromOrigin,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Spherical Surface Emitting Source with user defined surface
        /// (based on polar azimuthal angle range) and translation.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleRangeToDefineSphericalSurface"></param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomSurfaceEmittingSphericalSource(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            Position translationFromOrigin)
            : this(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                translationFromOrigin,
                new ThreeAxisRotation(0,0,0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Custom Spherical Surface Emitting Source with user defined surface
        /// (based on polar azimuthal angle range), and source axis rotation
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleRangeToDefineSphericalSurface"></param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomSurfaceEmittingSphericalSource(
           double radius,
           DoubleRange polarAngleRangeToDefineSphericalSurface,
           DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                new Position(0, 0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Spherical Surface Emitting Source with user defined surface
        /// (based on polar azimuthal angle range).
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleRangeToDefineSphericalSurface"></param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface"></param>
        public CustomSurfaceEmittingSphericalSource(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface)
            : this(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                new Position(0,0,0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }
        
        #endregion
    }

}
