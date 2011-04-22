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
    public class CustomSurfaceEmittingCuboidalSource : SurfaceEmittingCuboidalSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of Custom Surface Emitting Cuboidal Source with a given source profile, polar angle range,
        /// source axis rotation and translation
        /// </summary>
        /// <param name="cubeLengthX">The length of cube (along x axis)</param>
        /// <param name="cubeWidthY">The  width of cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="translationFromOrigin">Source axis rotation</param>
        /// <param name="rotationOfPrincipalSourceAxis">New source location</param>
        public CustomSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,  
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
            cubeLengthX,
            cubeWidthY, 
            cubeHeightZ,
            sourceProfile, 
            polarAngleEmissionRange,
            translationFromOrigin,
            rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Surface Emitting Cuboidal Source with a given source profile, polar angle range,
        /// and source axis rotation.
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            polarAngleEmissionRange,
            new Position (0,0,0),
            rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Surface Emitting Cuboidal Source with a given source profile, polar angle range,
        /// and translation
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            Position translationFromOrigin)
            : this(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            polarAngleEmissionRange,
            translationFromOrigin,
            new ThreeAxisRotation (0,0,0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Custom Surface Emitting Cuboidal Source with a given source profile, and polar angle range,
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        public CustomSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange)
            : this(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            polarAngleEmissionRange,
            new Position (0,0,0),
            new ThreeAxisRotation (0,0,0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }
        

        #endregion
    }

}
