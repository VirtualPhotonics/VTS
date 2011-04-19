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
    public class CustomCuboidalSource : CuboidalSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        #region Constructors

        /// <summary>
        /// Returns an instance of  Custom Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, and source axis rotation
        /// </summary>
        /// <param name="cubeLengthX">Length of the cuboid</param>
        /// <param name="cubeWidthY">Width of the cuboid</param>
        /// <param name="cubeHeightZ">Height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public CustomCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                translationFromOrigin,
                rotationOfPrincipalSourceAxis)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of  Custom Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and translation
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }


        /// <summary>
        /// Returns an instance of  Custom Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and source axis rotation
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of  Custom Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range.
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        public CustomCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        #endregion

        //CustomCuboidalSource
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
