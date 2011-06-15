using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class CustomSurfaceEmittingSphericalSourceInput : ISourceInput
    {
        // this handles custom circular
        public CustomSurfaceEmittingSphericalSourceInput(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            SourceType = SourceType.CustomSurfaceEmittingSpherical;
            Radius = radius;
            PolarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface;
            AzimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
        }

        public CustomSurfaceEmittingSphericalSourceInput(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface)
            : this(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone()) { }

        public CustomSurfaceEmittingSphericalSourceInput()
            : this(
                1.0,
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone()) { }

        public SourceType SourceType { get; set; }
        public double Radius { get; set; }
        public DoubleRange PolarAngleRangeToDefineSphericalSurface { get; set; }
        public DoubleRange AzimuthalAngleRangeToDefineSphericalSurface { get; set; }    
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
    }
}