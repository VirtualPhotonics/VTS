using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingCylindricalFiberSource
    /// implementation including tube radius, tube height, curved surface efficiency, bottom surface 
    /// efficiency, direction, position, and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCylindricalFiberSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCylindricalFiberSourceInput class
        /// </summary>
        /// <param name="fiberRadius">Fiber radius</param>
        /// <param name="fiberHeightZ">Fiber height</param>
        /// <param name="curvedSurfaceEfficiency">Efficiency of curved surface (0 - 1)</param>
        /// <param name="bottomSurfaceEfficiency">Efficiency of bottom surface (0 - 1)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCylindricalFiberSourceInput(
            double fiberRadius,
            double fiberHeightZ,
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.LambertianSurfaceEmittingTubular;
            FiberRadius = fiberRadius;
            FiberHeightZ = fiberHeightZ;
            CurvedSurfaceEfficiency = curvedSurfaceEfficiency;
            BottomSurfaceEfficiency = bottomSurfaceEfficiency;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCylindricalFiberSourceInput class
        /// </summary>
        /// <param name="fiberRadius">Fiber radius</param>
        /// <param name="fiberHeightZ">Fiber height</param>
        public LambertianSurfaceEmittingCylindricalFiberSourceInput(
            double fiberRadius,
            double fiberHeightZ)
            : this(
                fiberRadius,
                fiberHeightZ,
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianSurfaceEmittingCylindricalFiberSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingCylindricalFiberSourceInput()
            : this(
                1.0,
                1.0,
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Surface emitting cylindrical fiber source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// Fiber radius
        /// </summary>
        public double FiberRadius { get; set; }
        /// <summary>
        /// Fiber height
        /// </summary>
        public double FiberHeightZ { get; set; }   
        /// <summary>
        /// Efficiency of curved surface (0 - 1)
        /// </summary>
        public double CurvedSurfaceEfficiency { get; set; }
        /// <summary>
        /// Efficiency of bottom surface (0 - 1)
        /// </summary>
        public double BottomSurfaceEfficiency { get; set; }
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