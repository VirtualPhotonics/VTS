using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class DirectionalEllipticalSourceInput : ISourceInput
    {
        // this handles directional elliptical source
        public DirectionalEllipticalSourceInput(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal) 
        {
            SourceType = SourceType.DirectionalElliptical;
            ThetaConvOrDiv = thetaConvOrDiv;
            AParameter = aParameter;
            BParameter = bParameter;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
        }

        public DirectionalEllipticalSourceInput(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                aParameter,
                bParameter,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                SourceDefaults.DefaultPosition,
                SourceDefaults.DefaultBeamRoationFromInwardNormal) { }

        public SourceType SourceType { get; set; }
        public double ThetaConvOrDiv { get; set; }
        public double AParameter { get; set; }
        public double BParameter { get; set; }
        public ISourceProfile SourceProfile { get; set; }
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
    }
}
