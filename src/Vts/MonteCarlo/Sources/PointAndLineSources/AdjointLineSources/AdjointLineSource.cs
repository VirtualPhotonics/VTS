using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for an adjoint line source
    /// ray traced back to tissue surface of width tissueLineLength
    /// </summary>
    public class AdjointLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomLineSourceInput class
        /// </summary>
        /// <param name="lineLength">The length of the line source in air</param>
        /// <param name="tissueLineLength">The length of line source on tissue</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public AdjointLineSourceInput(
            double lineLength,
            double tissueLineLength,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "AdjointLine";
            LineLength = lineLength;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        ///// <summary>
        ///// Initializes a new instance of AdjointLineSourceInput class
        ///// </summary>
        ///// <param name="lineLength">The length of the line source</param>
        //public AdjointLineSourceInput(
        //    double lineLength)
        //    : this(
        //        lineLength,
        //        SourceDefaults.DefaultPosition.Clone(),
        //        0) { }

        /// <summary>
        /// Initializes the default constructor of AdjointLineSourceInput class
        /// </summary>
        public AdjointLineSourceInput()
            : this(
                1.0,
                10.0,
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Line source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the line source in air
        /// </summary>
        public double LineLength { get; set; }
        /// <summary>
        /// The length of the line source on tissue surface
        /// </summary>
        public double TissueLineLength { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
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
            return new AdjointLineSource(
                        LineLength,
                        TissueLineLength,
                        TranslationFromOrigin,
                        InitialTissueRegionIndex) {};
        }
    }

    /// <summary>
    /// Implements AdjointLineSource with line length, direction, position, 
    /// and initial tissue region index.
    /// </summary>
    public class AdjointLineSource : AdjointLineSourceBase
    {
        private double _tissueLineLength;

        /// <summary>
        /// Initializes a new instance of the AdjointLineSource class
        /// </summary>
        /// <param name="lineLength">The length of the line source in air</param>
        /// <param name="tissueLineLength">The length of line source on tissue</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public AdjointLineSource(
            double lineLength,
            double tissueLineLength,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                  lineLength,
                  translationFromOrigin,
                  initialTissueRegionIndex)
        {
            _tissueLineLength = tissueLineLength;
        }

        /// <summary>
        /// Returns initial position on tissue surface
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Position GetFinalPosition(Position airPosition)
        {
            // randomly sample length on tissue
            Position tissuePosition = SourceToolbox.GetPositionInALineRandomFlat(
                new Position(0, 0, 0), _tissueLineLength, Rng);
            return tissuePosition;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position tissuePosition)
        {
            // this code assumes translationFromOrigin has Z<0
            // determine distance from tissuePosition to translationFromOrigin
            var distance = Math.Sqrt(
                (tissuePosition.X - _translationFromOrigin.X) * (tissuePosition.X - _translationFromOrigin.X) +
                (tissuePosition.Y - _translationFromOrigin.Y) * (tissuePosition.Y - _translationFromOrigin.Y) +
                (tissuePosition.Z - _translationFromOrigin.Z) * (tissuePosition.Z - _translationFromOrigin.Z));
            return new Direction((tissuePosition.X - _translationFromOrigin.X) / distance,
                                 (tissuePosition.Y - _translationFromOrigin.Y) / distance,
                                 (tissuePosition.Z - _translationFromOrigin.Z) / distance);
        }
    }

}
