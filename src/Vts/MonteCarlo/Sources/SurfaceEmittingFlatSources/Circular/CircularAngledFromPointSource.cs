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
    /// profile) and the point position.
    /// </summary>
    public class CircularAngledFromPointSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CircularAngledFromPointSourceInput class
        /// </summary>
        /// <param name="radius">The radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="pointPosition">Location of originating point</param>
        /// <param name="translationFromOrigin">Center of circle location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CircularAngledFromPointSourceInput(
            double radius,
            ISourceProfile sourceProfile,
            Position pointPosition,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "CircularAngledFromPoint";
            Radius = radius;
            SourceProfile = sourceProfile;
            PointPosition = pointPosition;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CircularAngledFromPointSourceInput class
        /// </summary>
        /// <param name="radius">Radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="pointPosition">Origin point position</param>
        public CircularAngledFromPointSourceInput(
            double radius,
            ISourceProfile sourceProfile,
            Position pointPosition)
            : this(
                radius,
                sourceProfile,
                pointPosition,
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
        /// Origin point position
        /// </summary>
        public Position PointPosition { get; set; }
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
                this.PointPosition,
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
        Position _pointPosition;

        /// <summary>
        /// Returns an instance of  Circular Source Angled From Point with specified radius,
        /// source profile (Flat/Gaussian), point position
        /// </summary>
        /// <param name="radius">The radius of the circular source on tissue surface</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param> 
        /// <param name="pointPosition">Origin point position</param>     
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CircularAngledFromPointSource(            
            double radius,
            ISourceProfile sourceProfile,
            Position pointPosition,
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
            _pointPosition = pointPosition;
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
        }
        
        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            // determine angle from position to PointPosition
            var dist = Math.Sqrt(
                (_pointPosition.X - position.X) * (_pointPosition.X - position.X) +
                (_pointPosition.Y - position.Y) * (_pointPosition.Y - position.Y) +
                (_pointPosition.Z - position.Z) * (_pointPosition.Z - position.Z));
            return new Direction(
                (position.X - _pointPosition.X) / dist,
                (position.Y - _pointPosition.Y) / dist,
                (position.Z - _pointPosition.Z) / dist);
        }
    }

}
