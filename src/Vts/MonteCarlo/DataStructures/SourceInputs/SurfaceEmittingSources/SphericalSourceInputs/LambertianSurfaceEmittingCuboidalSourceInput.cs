using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class LambertianSurfaceEmittingSphericalSourceInput : ISourceInput
    {
        // this handles custom circular
        public LambertianSurfaceEmittingSphericalSourceInput(
            double radius,
            Position translationFromOrigin)
        {
            SourceType = SourceType.LambertianSurfaceEmittingSpherical;
            Radius = radius;            
            TranslationFromOrigin = translationFromOrigin;
        }

        public LambertianSurfaceEmittingSphericalSourceInput(
            double radius)
            : this(
                radius,
                SourceDefaults.DefaultPosition) { }

        public SourceType SourceType { get; set; }
        public double Radius { get; set; }        
        public Position TranslationFromOrigin { get; set; }
    }
}