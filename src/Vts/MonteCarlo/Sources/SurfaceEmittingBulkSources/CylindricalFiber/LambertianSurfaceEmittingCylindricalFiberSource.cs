using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingCylindricalFiberSource
    /// implementation including tube radius, tube height, curved surface efficiency, bottom surface 
    /// efficiency, Lambertian order, direction, position, and initial tissue region index.
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
        /// <param name="lambertOrder">Lambertian order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCylindricalFiberSourceInput(
            double fiberRadius,
            double fiberHeightZ,
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianSurfaceEmittingCylindricalFiber";
            FiberRadius = fiberRadius;
            FiberHeightZ = fiberHeightZ;
            CurvedSurfaceEfficiency = curvedSurfaceEfficiency;
            BottomSurfaceEfficiency = bottomSurfaceEfficiency;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCylindricalFiberSourceInput
        /// class assuming LambertOrder=1
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
            SourceType = "LambertianSurfaceEmittingCylindricalFiber";
            FiberRadius = fiberRadius;
            FiberHeightZ = fiberHeightZ;
            CurvedSurfaceEfficiency = curvedSurfaceEfficiency;
            BottomSurfaceEfficiency = bottomSurfaceEfficiency;
            LambertOrder = 1;
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
                1,
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
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Surface emitting cylindrical fiber source type
        /// </summary>
        public string SourceType { get; set; }
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
        /// Lambertian order for angular distribution
        /// </summary>
        public int LambertOrder { get; set; }
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
        /// <param name="rng">random number generator</param>
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng ??= new Random();
            
            return new LambertianSurfaceEmittingCylindricalFiberSource(
                FiberRadius,
                FiberHeightZ,
                CurvedSurfaceEfficiency,
                BottomSurfaceEfficiency,
                LambertOrder,
                NewDirectionOfPrincipalSourceAxis,
                TranslationFromOrigin,
                InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements LambertianSurfaceEmittingCylindricalFiberSource with fiber radius, fiber height,
    /// curved surface efficiency, bottom surface efficiency, direction, position, and initial 
    /// tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCylindricalFiberSource : SurfaceEmittingCylindricalFiberSourceBase
    {
        private readonly int _lambertOrder;

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting cylindrical fiber source with source axis rotation and translation
        /// </summary>
        /// <param name="fiberRadius">Fiber radius</param>
        /// <param name="fiberHeightZ">Fiber height</param>
        /// <param name="curvedSurfaceEfficiency">Efficiency of the curved surface (0-1)</param>
        /// <param name="bottomSurfaceEfficiency">Efficiency of the bottom surface (0-1)</param>
        /// <param name="lambertOrder">Lambertian order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCylindricalFiberSource(
            double fiberRadius,
            double fiberHeightZ,
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
            fiberRadius,
            fiberHeightZ,
            curvedSurfaceEfficiency,
            bottomSurfaceEfficiency,
            newDirectionOfPrincipalSourceAxis,
            translationFromOrigin,
            initialTissueRegionIndex)
        {
            _lambertOrder = lambertOrder;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            //Sample angular distribution with full range of theta and phi
            var finalDirection = SourceToolbox.GetDirectionForLambertianRandom(_lambertOrder, Rng);

            return finalDirection;
        }
    }
}