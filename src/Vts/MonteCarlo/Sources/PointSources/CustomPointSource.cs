using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomPointSource : PointSourceBase
    { 
        /// <summary>
        /// Returns an instance of Custom Point Source for a given polar and azimuthal angle range, 
        /// translation, and inward normal rotation.
        /// </summary>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="pointLocation">New position</param>
        /// <param name="newDirection">New Direction</param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position pointLocation,
            Direction newDirection)
            : base(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                pointLocation, 
                SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(newDirection))
        {
        }

        /// <summary>
        ///  Returns an instance of Custom Point Source for a given polar and azimuthal angle range and translation.
        /// </summary>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin
            )
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new Direction(0.0, 0.0, 1.0))
        {
        }

        /// <summary>
        /// Returns an instance of Custom Point Source for a given polar and azimuthal angle range, 
        /// and inward normal rotation.
        /// </summary>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="newDirection">New Direction</param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirection)
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0.0, 0.0, 0.0),
                newDirection)
        {
        }

        /// <summary>
        ///  Returns an instance of Custom Point Source for a given polar and azimuthal angle range.
        /// </summary>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0.0, 0.0, 0.0),
                new Direction(0.0, 0.0, 1.0))
        {
        }
    }
}
