using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomSurfaceEmittingCuboidalSource 
    /// implementation including length, width, height, source profile, polar angle range, 
    /// azimuthal angle range, direction, position, and initial tissue region index.
    /// </summary>
    public class CustomSurfaceEmittingCuboidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomSurfaceEmittingCuboidalSourceInput class        
        /// </summary>
        /// <param name="cubeLengthX">The length of cube (along x axis)</param>
        /// <param name="cubeWidthY">The  width of cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.CustomSurfaceEmittingCuboidal;
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            SourceProfile = sourceProfile;
            PolarAngleEmissionRange = polarAngleEmissionRange;           
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CustomSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">The length of cube (along x axis)</param>
        /// <param name="cubeWidthY">The  width of cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        public CustomSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                polarAngleEmissionRange,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of CustomSurfaceEmittingCuboidalSourceInput class        
        /// </summary>
        public CustomSurfaceEmittingCuboidalSourceInput()            
            : this(
                1.0,
                1.0,
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Surface Emitting Cuboidal source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// The length of cube (along x axis)
        /// </summary>
        public double CubeLengthX { get; set; }
        /// <summary>
        /// The  width of cube (along y axis)
        /// </summary>
        public double CubeWidthY { get; set; }
        /// <summary>
        /// The height of cube (along z axis)
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
    }
}