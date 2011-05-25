using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class IsotropicVolumetricEllipsoidalSourceInput : ISourceInput
    {
        // this handles isotropic ellipsoidal (volumetric)
        public IsotropicVolumetricEllipsoidalSourceInput(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            SourceType = SourceType.IsotropicVolumetricEllipsoidal;
            AParameter = aParameter;
            BParameter = bParameter;
            CParameter = cParameter;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
        }

        public IsotropicVolumetricEllipsoidalSourceInput(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                SourceDefaults.DefaultPosition) { }

        public SourceType SourceType { get; set; }
        public double AParameter { get; set; }
        public double BParameter { get; set; }
        public double CParameter { get; set; }
        public ISourceProfile SourceProfile { get; set; }     
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
    }
}