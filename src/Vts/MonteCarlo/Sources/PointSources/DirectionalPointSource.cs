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
    public class DirectionalPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Directional Point Source with a given emission direction at a given location
        /// </summary>        
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        public DirectionalPointSource(
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : base(
                new DoubleRange (0, 0),
                new DoubleRange(0, 0),
                translationFromOrigin,
                rotationFromInwardNormal)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, true, false);
        }

        /// <summary>
        ///  Returns an instance of Directional Point Source emitting along the z-axis at the translated location.
        /// </summary>
        /// <param name="translationFromOrigin"></param>
        public DirectionalPointSource(
            Position translationFromOrigin
            )
            : this(
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Directional Point Source with a inward normal rotation at the origin.
        /// </summary>
        /// <param name="rotationFromInwardNormal"></param>
        public DirectionalPointSource(
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                new Position(0, 0, 0),
                rotationFromInwardNormal)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }

        /// <summary>
        ///  Returns an instance of Directional Point Source emitting along the z-axis at the origin.
        /// </summary>
        public DirectionalPointSource()
            : this(
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

    }
}
