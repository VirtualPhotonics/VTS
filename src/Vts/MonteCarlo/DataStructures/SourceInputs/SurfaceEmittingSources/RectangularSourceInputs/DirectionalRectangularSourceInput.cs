using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class DirectionalRectangularSourceInput : ISourceInput
    {
        // this handles directional rectangular
        public DirectionalRectangularSourceInput(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal) 
        {
            SourceType = SourceType.DirectionalRectangular;
            ThetaConvOrDiv = thetaConvOrDiv;
            RectLengthX = rectLengthX;
            RectWidthY = rectWidthY;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
        }

        public DirectionalRectangularSourceInput(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                rectLengthX,
                rectWidthY,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                SourceDefaults.DefaultPosition,
                SourceDefaults.DefaultBeamRoationFromInwardNormal) { }

        public SourceType SourceType { get; set; }
        public double ThetaConvOrDiv { get; set; }
        public double RectLengthX { get; set; }
        public double RectWidthY { get; set; }
        public ISourceProfile SourceProfile { get; set; }
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
    }
}
