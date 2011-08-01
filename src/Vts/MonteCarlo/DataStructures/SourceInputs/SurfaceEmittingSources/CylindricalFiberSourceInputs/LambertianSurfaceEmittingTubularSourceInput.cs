using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingCylindricalFiberSource
    /// implementation including tube radius, tube height, curved surface efficiency, bottom surface 
    /// efficiency, direction, position, and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCylindricalFiberSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the LambertianSurfaceEmittingCylindricalFiberSourceInput class
        /// </summary>
        /// <param name="fiberRadius">Fiber radius</param>
        /// <param name="fiberHeightZ">Fiber height</param>
        /// <param name="curvedSurfaceEfficiency">Efficiency of curved surface (0 - 1)</param>
        /// <param name="bottomSurfaceEfficiency">Efficiency of bottom surface (0 - 1)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
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
        /// Initializes a new instance of the LambertianSurfaceEmittingCylindricalFiberSourceInput class
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
        /// Initializes a new instance of the LambertianSurfaceEmittingCylindricalFiberSourceInput class
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

        public SourceType SourceType { get; set; }
        public double FiberRadius { get; set; }
        public double FiberHeightZ { get; set; }   
        public double CurvedSurfaceEfficiency { get; set; }
        public double BottomSurfaceEfficiency { get; set; }  
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}