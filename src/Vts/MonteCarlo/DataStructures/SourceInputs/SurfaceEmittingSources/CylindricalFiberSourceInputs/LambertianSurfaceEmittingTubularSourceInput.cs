using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class LambertianSurfaceEmittingCylindricalFiberSourceInput : ISourceInput
    {
        // this handles custom line
        public LambertianSurfaceEmittingCylindricalFiberSourceInput(
            double tubeRadius,
            double tubeHeightZ,
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            SourceType = SourceType.LambertianSurfaceEmittingTubular;
            TubeRadius = tubeRadius;
            TubeHeightZ = tubeHeightZ;
            CurvedSurfaceEfficiency = curvedSurfaceEfficiency;
            BottomSurfaceEfficiency = bottomSurfaceEfficiency;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
        }

        public LambertianSurfaceEmittingCylindricalFiberSourceInput(
            double tubeRadius,
            double tubeHeightZ)
            : this(
                tubeRadius,
                tubeHeightZ,
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone()) { }

        public SourceType SourceType { get; set; }
        public double TubeRadius { get; set; }
        public double TubeHeightZ { get; set; }   
        public double CurvedSurfaceEfficiency { get; set; }
        public double BottomSurfaceEfficiency { get; set; }  
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
    }
}