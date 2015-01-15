using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomSurfaceEmittingSphericalSource 
    /// implementation including radius, source profile, polar angle range, azimuthal angle 
    /// range, direction, position and initial tissue region index.
    /// </summary>
    public class CustomSurfaceEmittingSphericalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomSurfaceEmittingSphericalSourceInput class
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="polarAngleRangeToDefineSphericalSurface">Polar angle range to define spherical surface</param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface">Azimuthal angle range to define spherical surface</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomSurfaceEmittingSphericalSourceInput(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "CustomSurfaceEmittingSpherical";
            Radius = radius;
            PolarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface;
            AzimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CustomSurfaceEmittingSphericalSourceInput class
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="polarAngleRangeToDefineSphericalSurface">polar angle range to define spherical surface</param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface">azimuthal angle range to define spherical surface</param>
        public CustomSurfaceEmittingSphericalSourceInput(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface)
            : this(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of CustomSurfaceEmittingSphericalSourceInput class
        /// </summary>
        public CustomSurfaceEmittingSphericalSourceInput()
            : this(
                1.0,
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Spherical source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The radius of the sphere
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Polar angle range to define spherical surface
        /// </summary>
        public DoubleRange PolarAngleRangeToDefineSphericalSurface { get; set; }
        /// <summary>
        /// Azimuthal angle range to define spherical surface
        /// </summary>
        public DoubleRange AzimuthalAngleRangeToDefineSphericalSurface { get; set; }
        /// <summary>
        /// New source axis direction
        /// </summary>
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new CustomSurfaceEmittingSphericalSource(
                this.Radius,
                this.PolarAngleRangeToDefineSphericalSurface,
                this.AzimuthalAngleRangeToDefineSphericalSurface,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CustomSurfaceEmittingSphericalSource with radius, source profile, 
    /// polar angle range, azimuthal angle range, direction, position and initial 
    /// tissue region index.
    /// </summary>
    public class CustomSurfaceEmittingSphericalSource : SurfaceEmittingSphericalSourceBase
    {
        /// <summary>
        /// Returns an instance of Custom Spherical Surface Emitting Source with a user defined surface area
        /// (based on polar azimuthal angle range), new source axis direction, and translation,
        /// </summary>
        /// <param name="radius">The radius of the sphere</param>
        /// <param name="polarAngleRangeToDefineSphericalSurface">Polar angle range to define the emitting area of the sphere</param>
        /// <param name="azimuthalAngleRangeToDefineSphericalSurface">Azimuthal angle range to define the emitting area of the sphere</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomSurfaceEmittingSphericalSource(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                radius,
                polarAngleRangeToDefineSphericalSurface,
                azimuthalAngleRangeToDefineSphericalSurface,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                initialTissueRegionIndex)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
        }
                
    }
}
