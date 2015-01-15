using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomVolumetricCuboidalSource 
    /// implementation including length, width, height, source profile, polar angle range,
    /// azimuthal angle range, direction, position and initial tissue region index.
    /// </summary>
    public class CustomVolumetricCuboidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomVolumetricCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">Length of the cuboid</param>
        /// <param name="cubeWidthY">Width of the cuboid</param>
        /// <param name="cubeHeightZ">Height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomVolumetricCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "CustomVolumetricCubiodal";
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            SourceProfile = sourceProfile;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CustomVolumetricCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">Length of the cuboid</param>
        /// <param name="cubeWidthY">Width of the cuboid</param>
        /// <param name="cubeHeightZ">Height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        public CustomVolumetricCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of CustomVolumetricCuboidalSourceInput class
        /// </summary>
        public CustomVolumetricCuboidalSourceInput()
            : this(
                1.0,
                1.0,
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Volumetric Cuboidal source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the cube (along x axis)
        /// </summary>
        public double CubeLengthX { get; set; }
        /// <summary>
        /// The width of the cube (along y axis)
        /// </summary>
        public double CubeWidthY { get; set; }
        /// <summary>
        /// The height of the cube (along z axis)
        /// </summary>
        public double CubeHeightZ { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
        /// <summary>
        /// Polar angle range
        /// </summary>
        public DoubleRange PolarAngleEmissionRange { get; set; }
        /// <summary>
        /// Azimuthal angle range
        /// </summary>
        public DoubleRange AzimuthalAngleEmissionRange { get; set; }
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

            return new CustomVolumetricCuboidalSource(
                this.CubeLengthX,
                this.CubeWidthY,
                this.CubeHeightZ,
                this.SourceProfile,
                this.PolarAngleEmissionRange,
                this.AzimuthalAngleEmissionRange,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CustomVolumetricCuboidalSource with length, width, height, source 
    /// profile, polar angle range,azimuthal angle range, direction, position and 
    /// initial tissue region index.
    /// </summary>
    public class CustomVolumetricCuboidalSource : VolumetricCuboidalSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;       

        /// <summary>
        /// Returns an instance of  Custom Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, new source axis direction, and translation.
        /// </summary>
        /// <param name="cubeLengthX">The length of the cuboid</param>
        /// <param name="cubeWidthY">The width of the cuboid</param>
        /// <param name="cubeHeightZ">The height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomVolumetricCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                initialTissueRegionIndex)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();

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
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
