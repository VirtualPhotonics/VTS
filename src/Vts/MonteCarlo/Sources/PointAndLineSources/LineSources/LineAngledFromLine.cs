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
    /// </summary>
    public class LineAngledFromLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomLineSourceInput class
        /// </summary>
        /// <param name="tissueLineLength">The length of line source on tissue</param>
        /// <param name="sourceProfile">Profile of line on tissue surface</param>
        /// <param name="lineInAirLength">The length of the line source in air</param>
        /// <param name="lineInAirTranslationFromOrigin">center position of line source in air (Z assumed to be negative)</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LineAngledFromLineSourceInput(
            double tissueLineLength,
            ISourceProfile sourceProfile,
            double lineInAirLength,
            Position lineInAirTranslationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "LineAngledFromLine";
            TissueLineLength = tissueLineLength;
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
        /// The length of the line source in air
        /// </summary>
        public double LineInAirLength { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position LineInAirTranslationFromOrigin { get; set; }
        /// <summary>
        /// Initial tissue region index, this will always be 0
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            return new LineAngledFromLineSource(
                        TissueLineLength,
                        SourceProfile,
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
        private double _tissueLineLength;
        private double _lineInAirLength;
        private Position _lineInAirCenterPosition;

        /// <summary>
        /// Initializes a new instance of the LineAngledFromLineSource class
        /// </summary>
        /// <param name="tissueLineLength">The length of line source on tissue</param>
        /// <param name="sourceProfile">Profile of line on tissue</param>
        /// <param name="lineInAirLength">The length of the line source in air</param>
        /// <param name="lineInAirTranslationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LineAngledFromLineSource(
            double tissueLineLength,
            ISourceProfile sourceProfile,
            double lineInAirLength,
            Position lineInAirTranslationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                  tissueLineLength,
                  sourceProfile,
                  SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(), // newDirectionOfPrincipalSourceAxis
                  new Position(0,0,0),
                  SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(), // beamRotationFromInwardNormal
                  initialTissueRegionIndex)
        {
            _tissueLineLength = tissueLineLength;
            _lineInAirLength = lineInAirLength;
            _lineInAirCenterPosition = lineInAirTranslationFromOrigin;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position tissuePosition)
        {
            // randomly sample length of line in air
            var xLocation = Rng.NextDouble() * _lineInAirLength + _lineInAirCenterPosition.X - _lineInAirLength / 2;
            var pointInLine = new Position(xLocation, 0, _lineInAirCenterPosition.Z);
            // this code assumes translationFromOrigin has Z<0
            // determine distance from tissuePosition to translationFromOrigin
            var distance = Math.Sqrt(
                (tissuePosition.X - pointInLine.X) * (tissuePosition.X - pointInLine.X) +
                (tissuePosition.Y - pointInLine.Y) * (tissuePosition.Y - pointInLine.Y) +
                (tissuePosition.Z - pointInLine.Z) * (tissuePosition.Z - pointInLine.Z));
            return new Direction((tissuePosition.X - pointInLine.X) / distance,
                                 (tissuePosition.Y - pointInLine.Y) / distance,
                                 (tissuePosition.Z - pointInLine.Z) / distance);
        }
    }

}
