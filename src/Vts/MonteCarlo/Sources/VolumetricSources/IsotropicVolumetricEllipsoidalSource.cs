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
    public class IsotropicVolumetricEllipsoidalSource : VolumetricEllipsoidalSourceBase
    {
       
        /// <summary>
        /// Returns an instance of  Isotropic Ellipsoidal Source with a given source profile (Flat/Gaussian), 
        ///  new source axis direction, and translation.
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        public IsotropicVolumetricEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null)
            : base(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis;
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition;
        }
                

        //Isotropic Ellipsoidal Source
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);
        }
    }

}
