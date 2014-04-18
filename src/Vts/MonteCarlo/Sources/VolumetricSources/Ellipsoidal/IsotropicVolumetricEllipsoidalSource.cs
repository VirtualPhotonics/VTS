using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements IsotropicVolumetricEllipsoidalSource with a,b and c parameters, 
    /// source profile, direction, position, and initial tissue region index.
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
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicVolumetricEllipsoidalSource(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                initialTissueRegionIndex)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
        }                

       /// <summary>
       /// Returns direction
       /// </summary>
        /// <returns>new direction</returns>
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetDirectionForIsotropicDistributionRandom(Rng);
        }
    }

}
