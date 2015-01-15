using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingTubularSource 
    /// implementation including tube radius, height, direction, position and initial tissue 
    /// region index.
    /// </summary>
    public class LambertianSurfaceEmittingTubularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingTubularSourceInput(
            double tubeRadius,
            double tubeHeightZ,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianSurfaceEmittingTubular";
            TubeRadius = tubeRadius;
            TubeHeightZ = tubeHeightZ;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        public LambertianSurfaceEmittingTubularSourceInput(
            double tubeRadius,
            double tubeHeightZ)
            : this(
                tubeRadius,
                tubeHeightZ,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianSurfaceEmittingTubularSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingTubularSourceInput()
            : this(
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Tubular source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// Tube radius
        /// </summary>
        public double TubeRadius { get; set; }
        /// <summary>
        /// Tube height
        /// </summary>
        public double TubeHeightZ { get; set; }
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

            return new LambertianSurfaceEmittingTubularSource(
                this.TubeRadius,
                this.TubeHeightZ,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };  
        }
    }

    /// <summary>
    /// Implements LambertianSurfaceEmittingTubularSource with tube radius, height, 
    /// direction, position and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingTubularSource : SurfaceEmittingTubularSourceBase
    {  
        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source with source axis rotation and translation
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingTubularSource(
            double tubeRadius,
            double tubeHeightZ,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
            tubeRadius,
            tubeHeightZ,
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
