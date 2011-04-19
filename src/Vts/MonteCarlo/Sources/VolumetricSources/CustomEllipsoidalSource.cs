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
    public class CustomEllipsoidalSource : EllipsoidalSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        #region Constructors

        /// <summary>
        /// Returns an instance of  Custom Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, and source axis rotation
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public CustomEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                translationFromOrigin,
                rotationOfPrincipalSourceAxis)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of  Custom Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and translation
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="cParameter"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }
              

        /// <summary>
        /// Returns an instance of  Custom Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and source axis rotation
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="cParameter"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),                
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of  Custom Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range.
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="cParameter"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        public CustomEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),                
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        #endregion

        //CustomEllipsoidalSource
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
