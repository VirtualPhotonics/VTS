using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    public class SourceDefaults
    {
        public static Direction DefaultDirectionOfPrincipalSourceAxis = new Direction(0.0, 0.0, 1.0);
        public static Position DefaultPosition = new Position(0.0, 0.0, 0.0);
        public static PolarAzimuthalAngles DefaultBeamRoationFromInwardNormal = new PolarAzimuthalAngles(0.0, 0.0);
        public static DoubleRange DefaultFullPolarAngleRange = new DoubleRange(0, Math.PI);
        public static DoubleRange DefaultHalfPolarAngleRange = new DoubleRange(0, 0.5 * Math.PI);
        public static DoubleRange DefaultAzimuthalAngleRange = new DoubleRange(0, 2.0 * Math.PI);
    }
}
