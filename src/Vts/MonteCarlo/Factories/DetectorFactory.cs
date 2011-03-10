using System.Collections.Generic;
using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.TallyActions;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class DetectorFactory
    {
        // todo: collapse all into one static factory method?

        public static ITerminationDetector GetTerminationDetector(
            IDetectorInput detectorInput,
            ITissue tissue)
        {
            switch (detectorInput.TallyType)
            {
                default:
                //case TallyType.RDiffuse:
                //    return new RDiffuseDetector();
                case TallyType.ROfRho:
                    var input = (ROfRhoDetectorInput)detectorInput;
                    return new ROfRhoDetector(input.Rho);
                //case TallyType.ROfAngle:
                //    return new ROfAngleDetector(angle);
                //case TallyType.ROfRhoAndTime:
                //    return new ROfRhoAndTimeDetector(rho, time, tissue);
                //case TallyType.ROfRhoAndAngle:
                //    return new ROfRhoAndAngleDetector(rho, angle);
                //case TallyType.ROfXAndY:
                //    return new RofXAndYDetector(x, y);
                //case TallyType.ROfRhoAndOmega:
                //    return new ROfRhoAndOmegaDetector(rho, omega, tissue);
                //case TallyType.TDiffuse:
                //    return new DiffuseDetector();
                //case TallyType.TOfAngle:
                //    return new TOfAngleDetector(angle);
                //case TallyType.TOfRho:
                //    return new TOfRhoDetector(rho);
                //case TallyType.TOfRhoAndAngle:
                //    return new TOfRhoAndAngleDetector(rho, angle);
            }
        }

        public static IHistoryDetector GetHistoryDetector(
            IDetectorInput detectorInput,
            ITissue tissue)
        {
            switch (detectorInput.TallyType)
            {
                default:
                case TallyType.FluenceOfRhoAndZ:
                    var input = (FluenceOfRhoAndZDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZDetector(input.Rho, input.Z, tissue);
                //case TallyType.FluenceOfRhoAndZAndTime:
                //    return new FluenceOfRhoAndZAndTimeDetector(rho, z, time, tissue);
                //case TallyType.AOfRhoAndZ:
                //    return new AOfRhoAndZDetector(rho, z, tissue);
                //case TallyType.ATotal:
                //    return new ATotalDetector(tissue);
            }
        }

        // pMC overload
        public static IpMCTerminationDetector GetpMCDetector(
            IpMCDetectorInput detectorInput,
            ITissue tissue)
        {
            switch (detectorInput.TallyType)
            {
                default:
                //case TallyType.pMuaMusInROfRhoAndTime:
                //    return new PMuaMusInROfRhoAndTimeDetector(rho, time, tissue, perturbedOps, perturbedRegionsIndices);
                case TallyType.pMuaMusInROfRho:
                    var input = (pMCROfRhoDetectorInput)detectorInput;
                    return new pMCMuaMusROfRhoDetector(input.Rho, tissue, input.PerturbedOps, input.PerturbedRegionsIndices);
            }
        }
 
        public static IHistoryDetector GetHistoryDetector(
            IDetectorInput detectorInput,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices)
        {
            throw new NotSupportedException("not implemented yet");
        }
    }
}
