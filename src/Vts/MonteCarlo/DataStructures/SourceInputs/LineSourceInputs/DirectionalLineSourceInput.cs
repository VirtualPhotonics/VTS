using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class DirectionalLineSourceInput : ISourceInput
    {
        // this handles directional line
        public DirectionalLineSourceInput(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis) 
        {
            SourceType = SourceType.DirectionalLine;
            ThetaConvOrDiv = thetaConvOrDiv;
            LineLength = lineLength;
            SourceProfile = sourceProfile;
            TranslationFromOrigin = translationFromOrigin;
            RotationFromInwardNormal = rotationFromInwardNormal;
            RotationOfPrincipalSourceAxis = RotationOfPrincipalSourceAxis;
        }

        public DirectionalLineSourceInput(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                lineLength,
                sourceProfile,
                new Position (0.0, 0.0, 0.0),
                new PolarAzimuthalAngles (0.0, 0.0),
                new ThreeAxisRotation(0.0, 0.0, 0.0)) { }

        public SourceType SourceType { get; set; }
        public double ThetaConvOrDiv { get; set; }
        public double LineLength { get; set; }
        public ISourceProfile SourceProfile { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public PolarAzimuthalAngles RotationFromInwardNormal { get; set; }
        public ThreeAxisRotation RotationOfPrincipalSourceAxis { get; set; }

  

    }
}
