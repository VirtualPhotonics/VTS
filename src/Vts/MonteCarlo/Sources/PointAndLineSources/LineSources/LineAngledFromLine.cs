using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for a line source
    /// of width tissueLineLength ray traced back to line in air (pupil in 2D).
    /// This is different than sampling theta,phi angles performed in LineSources
    /// All of the "AngledFrom" series of sources translate the source on tissue
    /// and the source in air separately.
    /// </summary>
    public class LineAngledFromLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomLineSourceInput class
        /// </summary>
        /// <param name="tissueLineLength">The length of line source on tissue</param>
        /// <param name="sourceProfile">Profile of line on tissue surface</param>
        /// <param name="translationFromOrigin">center position of line source on tissue</param>
        /// <param name="lineInAirLength">The length of the line source in air</param>
        /// <param name="lineInAirTranslationFromOrigin">center position of line source in air (Z assumed to be negative)</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LineAngledFromLineSourceInput(
            double tissueLineLength,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double lineInAirLength,
            Position lineInAirTranslationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "LineAngledFromLine";
            TissueLineLength = tissueLineLength;
            SourceProfile = sourceProfile;
            TranslationFromOrigin = translationFromOrigin;
            LineInAirLength = lineInAirLength;
            LineInAirTranslationFromOrigin = lineInAirTranslationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes the default constructor of LineAngledFromLineSourceInput class
        /// </summary>
        public LineAngledFromLineSourceInput()
            : this(
                10.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultPosition.Clone(),
                1.0,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Line source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the line source on tissue surface
        /// </summary>
        public double TissueLineLength { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }        
        /// <summary>
        /// Line in tissue surface translation
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// The length of the line source in air
        /// </summary>
        public double LineInAirLength { get; set; }
        /// <summary>
        /// Line in air translation
        /// </summary>
        public Position LineInAirTranslationFromOrigin { get; set; }
        /// <summary>
        /// Initial tissue region index, this will always be 0
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            return new LineAngledFromLineSource(
                        TissueLineLength,
                        SourceProfile,
                        TranslationFromOrigin,
                        LineInAirLength,
                        LineInAirTranslationFromOrigin,
                        InitialTissueRegionIndex) {};
        }
    }

    /// <summary>
    /// Implements LineAngledFromLineSource with line length, direction, position, 
    /// and initial tissue region index.
    /// </summary>
    public class LineAngledFromLineSource : LineSourceBase
    {
        private readonly double _lineInAirLength;
        private readonly Position _lineInAirCenterPosition;

        /// <summary>
        /// Initializes a new instance of the LineAngledFromLineSource class
        /// </summary>
        /// <param name="tissueLineLength">The length of line source on tissue</param>
        /// <param name="sourceProfile">Profile of line on tissue</param>
        /// <param name="translationFromOrigin">line on tissue surface translation</param>
        /// <param name="lineInAirLength">The length of the line source in air</param>
        /// <param name="lineInAirTranslationFromOrigin">line in air translation</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LineAngledFromLineSource(
            double tissueLineLength,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            double lineInAirLength,
            Position lineInAirTranslationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                  tissueLineLength,
                  sourceProfile,
                  SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(), // newDirectionOfPrincipalSourceAxis
                  translationFromOrigin,
                  SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(), // beamRotationFromInwardNormal
                  initialTissueRegionIndex)
        {
            _lineInAirLength = lineInAirLength;
            _lineInAirCenterPosition = lineInAirTranslationFromOrigin;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position on *tissue*</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            // randomly sample length of line in air
            var xLocation = Rng.NextDouble() * _lineInAirLength + _lineInAirCenterPosition.X - _lineInAirLength / 2;
            var pointInLine = new Position(xLocation, 0, _lineInAirCenterPosition.Z);
            // this code assumes translationFromOrigin has Z<0
            // determine distance from position=tissuePosition to translationFromOrigin
            var distance = Math.Sqrt(
                (position.X - pointInLine.X) * (position.X - pointInLine.X) +
                (position.Y - pointInLine.Y) * (position.Y - pointInLine.Y) +
                (position.Z - pointInLine.Z) * (position.Z - pointInLine.Z));
            return new Direction((position.X - pointInLine.X) / distance,
                                 (position.Y - pointInLine.Y) / distance,
                                 (position.Z - pointInLine.Z) / distance);
        }
    }

}
