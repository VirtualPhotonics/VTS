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
    public class DirectionalCircularSource : CircularSourceBase
    {
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative

        #region Constructors

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, translation, inward normal rotation, 
        /// and source axis rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
                innerRadius,
                outerRadius,
                sourceProfile,
                translationFromOrigin,
                rotationFromInwardNormal,
                rotationOfPrincipalSourceAxis)
        {
            _thetaConvOrDiv = thetaConvOrDiv;// SourceToolbox.NAToPolarAngle(numericalAperture);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, translation, and inward normal rotation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                translationFromOrigin,
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, true, false);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, translation,  and source axis rotation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }


        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, and translation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            Position translationFromOrigin)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range,  inward normal rotation, and source axis rotation.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, true);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range and inward normal rotation.
        /// </summary>        
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }


        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range and source axis rotation.
        /// </summary>        
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width,
        /// source profile (Flat/Gaussian), polar and azimuthal angle range.
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,
            double innerRadius,
            double outerRadius,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                innerRadius,
                outerRadius,
                sourceProfile,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }


        #endregion

        //Converging, diveriging or collimated Circular Source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            //Calculate polar angle           
            var azimuthalAngleEmissionRange = new DoubleRange(0.0, 2 * Math.PI);
            var polarAngle = 0.0;    //for collimated Circular source

            // sign is negative for diverging and positive positive for converging 
            if (_thetaConvOrDiv != 0.0)
            {
                var height = _outerRadius / Math.Tan(_thetaConvOrDiv);
                polarAngle = Math.Atan(Math.Sqrt(finalPosition.X*finalPosition.X + finalPosition.Y*finalPosition.Y) / height);
            }

            //Get final direction
            Direction finalDirection = SourceToolbox.GetDirectionForGiven2DPositionAndPolarAngle(polarAngle, finalPosition);

            return finalDirection;
        }
    }
}
