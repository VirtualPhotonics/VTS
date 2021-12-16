using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for CircularAngledFromPointSource implementation 
    /// including radius, source profile, point position, and initial tissue region index.
    /// The angle of the source is determined by the position on the tissue surface (dictated by the source
    /// profile) and the point position in the air.
    /// </summary>
    public class CircularAngledFromPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CircularAngledFromPointSourceInput class
        /// </summary>
        /// <param name="radius">The radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="pointLocation">Location of originating point WITHOUT TRANSLATION</param>
        /// <param name="translationFromOrigin">Center of circle location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CircularAngledFromPointSourceInput(
            double radius,
            ISourceProfile sourceProfile,
            Position pointLocation,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "CircularAngledFromPoint";
            Radius = radius;
            SourceProfile = sourceProfile;
            PointLocation = pointLocation;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CircularAngledFromPointSourceInput class
        /// </summary>
        /// <param name="radius">Radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="pointLocation">Origin point location</param>
        public CircularAngledFromPointSourceInput(
            double radius,
            ISourceProfile sourceProfile,
            Position pointLocation)
            : this(
                radius,
                sourceProfile,
                pointLocation,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of CircularAngledFromPointSourceInput class
        /// </summary>
        public CircularAngledFromPointSourceInput()
            : this(
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Circular source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The radius of the circular source on tissue surface
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
        /// <summary>
        /// Origin point location
        /// </summary>
        public Position PointLocation { get; set; }
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

            return new CircularAngledFromPointSource(
                this.Radius,
                this.SourceProfile,
                this.PointLocation,
                this.TranslationFromOrigin,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CircularAngledFromPointSource with radius on tissue surface, source profile,  
    /// point position, and initial tissue region index. 
    /// </summary>
    public class CircularAngledFromPointSource : CircularSourceBase
    {
        Position _pointLocation;

        /// <summary>
        /// Returns an instance of  Circular Source Angled From Point with specified radius,
        /// source profile (Flat/Gaussian), point position
        /// </summary>
        /// <param name="radius">The radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param> 
        /// <param name="pointLocation">Origin point location</param>     
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CircularAngledFromPointSource(            
            double radius,
            ISourceProfile sourceProfile,
            Position pointLocation,
            Position translationFromOrigin,
            int initialTissueRegionIndex = 0)
            : base(                
                radius,
                0.0,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(), // newDirectionOfPrincipalSourceAxis
                translationFromOrigin, 
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(), // beamRotationFromInwardNormal
                initialTissueRegionIndex)
        {
            _pointLocation = pointLocation;
        }
        
        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            // determine angle from position to PointLocation
            var dist = Math.Sqrt(
                (_pointLocation.X - position.X) * (_pointLocation.X - position.X) +
                (_pointLocation.Y - position.Y) * (_pointLocation.Y - position.Y) +
                (_pointLocation.Z - position.Z) * (_pointLocation.Z - position.Z));
            return new Direction(
                (position.X - _pointLocation.X) / dist,
                (position.Y - _pointLocation.Y) / dist,
                (position.Z - _pointLocation.Z) / dist);
        }
    }

}
