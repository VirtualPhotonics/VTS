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
    public class CustomRectangularSource : RectangularSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        #region Constructors
        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, inward normal rotation, and source axis rotation
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                translationFromOrigin,
                rotationFromInwardNormal,
                rotationOfPrincipalSourceAxis)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, and inward normal rotation.
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, true, false);
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, and source axis rotation.
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis
            )
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and translation
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, inward normal rotation, and source axis rotation
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationFromInwardNormal"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range and inward normal rotation.
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and source axis rotation
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range.
        /// </summary>
        /// <param name="rectLengthX"></param>
        /// <param name="rectWidthY"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        public CustomRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        #endregion

        //CustomRectangularSource
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            return SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
