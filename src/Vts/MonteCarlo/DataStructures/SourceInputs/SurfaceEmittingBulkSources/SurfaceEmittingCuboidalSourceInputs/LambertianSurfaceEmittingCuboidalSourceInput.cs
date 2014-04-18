using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingCuboidalSource 
    /// implementation including length, width, height, source profile, direction, position, and
    /// initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCuboidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">The length of the cube (along x axis)</param>
        /// <param name="cubeWidthY">The width of the cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of the cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.LambertianSurfaceEmittingCubiodal;
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            SourceProfile = sourceProfile;          
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">length</param>
        /// <param name="cubeWidthY">width</param>
        /// <param name="cubeHeightZ">height</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public LambertianSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingCuboidalSourceInput()            
            : this(
                1.0,
                1.0,
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Surface Emitting Cuboidal source type
        /// </summary>
        public SourceType SourceType { get; set; }
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