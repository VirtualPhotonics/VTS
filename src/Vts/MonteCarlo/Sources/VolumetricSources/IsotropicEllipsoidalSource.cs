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
    public class IsotropicEllipsoidalSource : EllipsoidalSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of  Isotropic Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        /// translation, and source axis rotation
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public IsotropicEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
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
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Isotropic Ellipsoidal Source with a given source profile (Flat/Gaussian) and translation
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="cParameter"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        public IsotropicEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            Position translationFromOrigin)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                translationFromOrigin,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }
              

        /// <summary>
        /// Returns an instance of  Isotropic Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        /// and source axis rotation
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="cParameter"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public IsotropicEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                new Position(0, 0, 0),                
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of  Isotropic Ellipsoidal Source with a given source profile (Flat/Gaussian).
        /// </summary>
        /// <param name="aParameter"></param>
        /// <param name="bParameter"></param>
        /// <param name="cParameter"></param>
        /// <param name="sourceProfile"></param>
        public IsotropicEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                new Position(0, 0, 0),                
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        #endregion

        //Isotropic Ellipsoidal Source
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);
        }
    }

}
