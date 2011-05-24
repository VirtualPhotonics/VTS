using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class LambertianSurfaceEmittingTubularSourceInput : ISourceInput
    {
        // this handles custom line
        public LambertianSurfaceEmittingTubularSourceInput(
            double tubeRadius,
            double tubeHeightZ,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            SourceType = SourceType.LambertianSurfaceEmittingTubular;
            TubeRadius = tubeRadius;
            TubeHeightZ = tubeHeightZ;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
        }

        public LambertianSurfaceEmittingTubularSourceInput(
            double tubeRadius,
            double tubeHeightZ)
            : this(
                tubeRadius,
                tubeHeightZ,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                SourceDefaults.DefaultPosition) { }

        public SourceType SourceType { get; set; }
        public double TubeRadius { get; set; }
        public double TubeHeightZ { get; set; }   
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
    }
}