using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    public class SourceDefaults
    {
        public static Position DefaultTranslationFromOrigin = new Position(0.0, 0.0, 0.0);
        public static PolarAzimuthalAngles DefaultRoationFromInwardNormal = new PolarAzimuthalAngles(0.0, 0.0);
        public static Direction InwardNormal = new Direction(0.0, 0.0, 1.0);
    }
}
