using System;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate Detector given IDetectorInput and ITissue
    /// </summary>
    public static class DetectorFactory
    {
        public static IDetector GetDetector(IDetectorInput di, ITissue tissue, 
            AbsorptionWeightingType awt)
        {
            IDetector d = null;
            if (di is DetectorInput)
            {
                return new Detector((DetectorInput)di, tissue, awt);
            }
            if (di is pMCDetectorInput)
            {
                return new pMCDetector((pMCDetectorInput)di, tissue);
            }

            if (d == null)
                throw new ArgumentException(
                    "Problem generating IDetector instance. Check that DetectorInput, di, has a matching IDetector definition.");

            return d;
        }
    }
}
