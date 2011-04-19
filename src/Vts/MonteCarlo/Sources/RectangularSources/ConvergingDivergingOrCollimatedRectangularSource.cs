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
    public class ConvergingDivergingOrCollimatedRectangularSource : RectangularSourceBase
    {
        private double _thetaConvOrDiv;  //convergence:positive, divergence:negative

        #region Constructors

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, translation, inward normal rotation, 
        /// and source axis rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle</param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
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
            _thetaConvOrDiv = thetaConvOrDiv;// SourceToolbox.NAToPolarAngle(numericalAperture);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, translation, and inward normal rotation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                translationFromOrigin,
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, true, false);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, translation,  and source axis rotation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }


        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, and translation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Position translationFromOrigin)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range,  inward normal rotation, and source axis rotation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, true);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range and inward normal rotation.
        /// </summary>        
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }


        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range and source axis rotation.
        /// </summary>        
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public ConvergingDivergingOrCollimatedRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }


        #endregion

        //Converging / diveriging or collimated rectangular source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            //Calculate polar angle           
            var azimuthalAngleEmissionRange = new DoubleRange(0.0, 2 * Math.PI);
            var polarAngle = 0.0;    //for collimated rectangular source

            // sign is negative for diverging and positive positive for converging 
            if (_thetaConvOrDiv != 0.0)
            {
                var sign = _thetaConvOrDiv / Math.Abs(_thetaConvOrDiv);
                var height = 0.5 * Math.Sqrt(_rectLengthX*_rectLengthX+_rectWidthY*_rectWidthY) / Math.Tan(_thetaConvOrDiv);
                polarAngle = Math.Atan(sign * Math.Sqrt(finalPosition.X * finalPosition.X + finalPosition.Y * finalPosition.Y) / height);
            }

            //Sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomAzimuthalAngle(polarAngle, azimuthalAngleEmissionRange, Rng);

            return finalDirection;
        }
    }
}
