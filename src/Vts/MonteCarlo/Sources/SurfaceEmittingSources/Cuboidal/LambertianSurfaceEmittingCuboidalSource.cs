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
    public class LambertianSurfaceEmittingCuboidalSource : SurfaceEmittingCuboidalSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting Cuboidal Source with a given source profile
        /// source axis rotation and translation
        /// </summary>
        /// <param name="cubeLengthX">The length of cube (along x axis)</param>
        /// <param name="cubeWidthY">The  width of cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="translationFromOrigin">Source axis rotation</param>
        /// <param name="rotationOfPrincipalSourceAxis">New source location</param>
        public LambertianSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,            
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            new DoubleRange(0, 0.5*Math.PI),
            translationFromOrigin,
            rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting Cuboidal Source with a given source profile,
        /// and source axis rotation.
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LambertianSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            new Position(0, 0, 0),
            rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting Cuboidal Source with a given source profile,
        /// and translation
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        public LambertianSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Position translationFromOrigin)
            : this(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            translationFromOrigin,
            new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting Cuboidal Source with a given source profile.
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        public LambertianSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile)
            : this(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            new Position(0, 0, 0),
            new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }


        #endregion
    }

}
