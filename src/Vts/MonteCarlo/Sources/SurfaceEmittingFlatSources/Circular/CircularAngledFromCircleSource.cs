using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Defines input data for CircularAngledFromCircleSource implementation 
    /// including radius, source profile, point position, and initial tissue region index.
    /// The angle of the source is determined by the position on the tissue surface (dictated by the source
    /// profile) and the *uniformly sampled* circle position in air.
    /// All of the "AngledFrom" series of sources translate the source on tissue
    /// and the source in air separately.
    /// </summary>
    public class CircularAngledFromCircleSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CircularAngledFromCircleSourceInput class
        /// </summary>
        /// <param name="radiusOnTissue">The radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">source profile (Flat/Gaussian) of circle on tissue</param>
        /// <param name="translationFromOrigin">center of circle location</param>
        /// <param name="radiusInAir">radius of originating circle</param>
        /// <param name="circleInAirTranslationFromOrigin">Center of circle in air location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CircularAngledFromCircleSourceInput(
            double radiusOnTissue,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double radiusInAir,
            Position circleInAirTranslationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "CircularAngledFromCircle";
            RadiusOnTissue = radiusOnTissue;
            SourceProfile = sourceProfile;
            TranslationFromOrigin = translationFromOrigin;
            RadiusInAir = radiusInAir;
            CircleInAirTranslationFromOrigin = circleInAirTranslationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CircularAngledFromCircleSourceInput class
        /// </summary>
        /// <param name="radiusOnTissue">Radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">source profile (Flat/Gaussian) of circle on tissue</param>
        /// <param name="translationFromOrigin">Circle on tissue translation</param>
        /// <param name="radiusInAir">Radius of circle in air</param>
        /// <param name="circleInAirTranslationFromOrigin">Circle in air location</param>
        public CircularAngledFromCircleSourceInput(
            double radiusOnTissue,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double radiusInAir,
            Position circleInAirTranslationFromOrigin)
            : this(
                radiusOnTissue,
                sourceProfile,
                translationFromOrigin,
                radiusInAir,
                circleInAirTranslationFromOrigin,
                0) { }

        /// <summary>
        /// Initializes the default constructor of CircularAngledFromCircleSourceInput class
        /// </summary>
        public CircularAngledFromCircleSourceInput()
            : this(
                10.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultPosition.Clone(),
                1.0,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Circular source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The radius of the circular source on tissue surface
        /// </summary>
        public double RadiusOnTissue { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }       
        /// <summary>
        /// Circle on tissue translation
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// The radius of the circular source in air
        /// </summary>
        public double RadiusInAir { get; set; }
        /// <summary>
        /// Circle in Air translation
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

            return new CircularAngledFromCircleSource(
                this.RadiusOnTissue,
                this.SourceProfile,
                this.TranslationFromOrigin,
                this.RadiusInAir,
                this.CircleInAirTranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CircularAngledFromCircleSource with radius on tissue surface, source profile,  
    /// point position, and initial tissue region index. 
    /// </summary>
    public class CircularAngledFromCircleSource : CircularSourceBase
    {
        Position _circleInAirTranslationFromOrigin;
        double _radiusOnTissue, _radiusInAir;

        /// <summary>
        /// Returns an instance of  Circular Source Angled From Point with specified radius,
        /// source profile (Flat/Gaussian), point position
        /// </summary>
        /// <param name="radiusOnTissue">The radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="translationFromOrigin">Circle on tissue translation</param>
        /// <param name="radiusInAir">radius of circle in air</param>     
        /// <param name="circleInAirTranslationFromOrigin">circle in air translation</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CircularAngledFromCircleSource(            
            double radiusOnTissue,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double radiusInAir,
            Position circleInAirTranslationFromOrigin,
            int initialTissueRegionIndex = 0)
            : base(                
                radiusOnTissue,
                0.0,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(), // newDirectionOfPrincipalSourceAxis
                translationFromOrigin,
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(), // beamRotationFromInwardNormal
                initialTissueRegionIndex)
        {
            _radiusOnTissue = radiusOnTissue;
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
