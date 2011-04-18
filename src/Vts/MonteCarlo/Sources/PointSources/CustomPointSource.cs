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
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : base(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin, 
                rotationFromInwardNormal)
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
                new PolarAzimuthalAngles(0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Custom Point Source for a given polar and azimuthal angle range, 
        /// and inward normal rotation.
        /// </summary>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public CustomPointSource(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                rotationFromInwardNormal)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
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
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

    }
}
