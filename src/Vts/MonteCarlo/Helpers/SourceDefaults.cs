using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Define SourceDefault values
    /// </summary>
    public static class SourceDefaults
    {
        /// <summary>
        /// Default direction (positive z-axis)
        /// </summary>
        public static Direction DefaultDirectionOfPrincipalSourceAxis => new Direction(0.0, 0.0, 1.0);
        /// <summary>
        /// Default position (0, 0, 0)
        /// </summary>
        public static Position DefaultPosition => new Position(0.0, 0.0, 0.0);
        /// <summary>
        /// Default beam rotation angle from inward normal (0.0, 0.0)
        /// </summary>
        public static PolarAzimuthalAngles DefaultBeamRotationFromInwardNormal => new PolarAzimuthalAngles(0.0, 0.0);
        /// <summary>
        /// Default beam rotation angle from inward normal (0.0, 0.0)
        /// </summary>
        [Obsolete("DefaultBeamRoationFromInwardNormal is deprecated, please use DefaultBeamRotationFromInwardNormal instead.")]
        public static PolarAzimuthalAngles DefaultBeamRoationFromInwardNormal => new PolarAzimuthalAngles(0.0, 0.0);
        /// <summary>
        /// Default full polar angle range (0.0, PI)
        /// </summary>
        public static DoubleRange DefaultFullPolarAngleRange => new DoubleRange(0, Math.PI);
        /// <summary>
        /// Default half polar angle range (0.0, PI/2)
        /// </summary>
        public static DoubleRange DefaultHalfPolarAngleRange => new DoubleRange(0, 0.5 * Math.PI);
        /// <summary>
        /// Default azimuthal angle range (0.0, 2PI)
        /// </summary>
        public static DoubleRange DefaultAzimuthalAngleRange => new DoubleRange(0, 2.0 * Math.PI);
    }
}
