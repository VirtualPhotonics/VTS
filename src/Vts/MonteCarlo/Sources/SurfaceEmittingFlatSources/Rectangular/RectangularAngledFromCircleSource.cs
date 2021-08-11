using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for RectangularAngledFromCircleSource
    /// implementation including length, width, source profile,
    /// and initial tissue region index.
    /// The angle of the source is determined by the position on the tissue surface (dictated
    /// by the source profile) and the *uniformly sampled* circle position in air.
    /// All of the "AngledFrom" series of sources translate the source on tissue
    /// and the source in air separately.
    /// </summary>
    public class RectangularAngledFromCircleSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of RectangularAngledFromCircleSourceInput class
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian} of rectangle</param>
        /// <param name="translationFromOrigin">rectangle on tissue surface translation</param>
        /// <param name="radiusInAir">radius of originating circle</param>
        /// <param name="circleInAirTranslationFromOrigin">Center of circle in air location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public RectangularAngledFromCircleSourceInput(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double radiusInAir,
            Position circleInAirTranslationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "RectangularAngledFromCircle";
            RectLengthX = rectLengthX;
            RectWidthY = rectWidthY;
            SourceProfile = sourceProfile;
            TranslationFromOrigin = translationFromOrigin;
            RadiusInAir = radiusInAir; 
            CircleInAirTranslationFromOrigin = circleInAirTranslationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of RectangularAngledFromCircleSourceInput class
        /// </summary>
        public RectangularAngledFromCircleSourceInput()
            : this(
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultPosition.Clone(),
                0.1,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Rectangular source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the Rectangular Source
        /// </summary>
        public double RectLengthX { get; set; }
        /// <summary>
        /// The width of the Rectangular Source
        /// </summary>
        public double RectWidthY { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
        /// <summary>
        /// Rectangle on tissue surface translation
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// radius of circle in air
        /// </summary>
        public double RadiusInAir { get; set; }
        /// <summary>
        /// Circle in air translation
        /// </summary>
        public Position CircleInAirTranslationFromOrigin { get; set; }
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

            return new RectangularAngledFromCircleSource(
                this.RectLengthX,
                this.RectWidthY,
                this.SourceProfile,
                this.TranslationFromOrigin,
                this.RadiusInAir,
                this.CircleInAirTranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements RectangularAngledFromCircleSource with length, width, source profile,  
    /// circle in air translation from origin, and initial tissue
    /// region index.
    /// </summary>
    public class RectangularAngledFromCircleSource : RectangularSourceBase
    {
        Position _circleInAirTranslationFromOrigin;
        double _radiusInAir;
        /// <summary>
        /// Returns an instance of Custom Rectangular Source with specified length and width,
        /// source profile (Flat/Gaussian),translation of circle in air
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>  
        /// <param name="translationFromOrigin">New rectangular source location</param>    
        /// <param name="radiusInAir">radius of circle in air</param>
        /// <param name="circleInAirTranslationFromOrigin">New circle in air location</param>    
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public RectangularAngledFromCircleSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double radiusInAir,
            Position circleInAirTranslationFromOrigin,
            int initialTissueRegionIndex = 0)
            : base(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(), // newDirectionOfPrincipalSourceAxis
                translationFromOrigin,
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(), // beamRotationFromInwardNormal
                initialTissueRegionIndex)
        {
            _radiusInAir = radiusInAir;
            _circleInAirTranslationFromOrigin = circleInAirTranslationFromOrigin;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            // randomly sample length of flat circle in air
            var positionInAir = SourceToolbox.GetPositionInACircleRandomFlat(
                _circleInAirTranslationFromOrigin, 0.0, _radiusInAir, Rng);
            // determine angle from positionInAir to PointLocation on tissue
            var dist = Math.Sqrt(
                (positionInAir.X - position.X) * (positionInAir.X - position.X) +
                (positionInAir.Y - position.Y) * (positionInAir.Y - position.Y) +
                (positionInAir.Z - position.Z) * (positionInAir.Z - position.Z));
            return new Direction(
                (position.X - positionInAir.X) / dist,
                (position.Y - positionInAir.Y) / dist,
                (position.Z - positionInAir.Z) / dist);
        }
    }

}
