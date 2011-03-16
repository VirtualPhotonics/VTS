using System.Collections.Generic;
using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
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
                case TallyType.RDiffuse:
                    return new RDiffuseDetector();
                case TallyType.ROfRho:
                    var rrinput = (ROfRhoDetectorInput)detectorInput;
                    return new ROfRhoDetector(rrinput.Rho);
                case TallyType.ROfAngle:
                    var rainput = (ROfAngleDetectorInput)detectorInput;
                    return new ROfAngleDetector(rainput.Angle);
                case TallyType.ROfRhoAndTime:
                    var rrtinput = (ROfRhoAndTimeDetectorInput)detectorInput;
                    return new ROfRhoAndTimeDetector(rrtinput.Rho, rrtinput.Time, tissue);
                case TallyType.ROfRhoAndAngle:
                    var rrainput = (ROfRhoAndAngleDetectorInput)detectorInput;
                    return new ROfRhoAndAngleDetector(rrainput.Rho, rrainput.Angle);
                case TallyType.ROfXAndY:
                    var rxyinput = (ROfXAndYDetectorInput)detectorInput;
                    return new RofXAndYDetector(rxyinput.X, rxyinput.Y);
                case TallyType.ROfRhoAndOmega:
                    var rroinput = (ROfRhoAndOmegaDetectorInput)detectorInput;
                    return new ROfRhoAndOmegaDetector(rroinput.Rho, rroinput.Omega, tissue);
                case TallyType.TDiffuse:
                    return new TDiffuseDetector();
                case TallyType.TOfAngle:
                    var tainput = (TOfAngleDetectorInput)detectorInput;
                    return new TOfAngleDetector(tainput.Angle);
                case TallyType.TOfRho:
                    var trinput = (TOfRhoDetectorInput)detectorInput;
                    return new TOfRhoDetector(trinput.Rho);
                case TallyType.TOfRhoAndAngle:
                    var trainput = (TOfRhoAndAngleDetectorInput)detectorInput;
                    return new TOfRhoAndAngleDetector(trainput.Rho, trainput.Angle);
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
                    var frzinput = (FluenceOfRhoAndZDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZDetector(frzinput.Rho, frzinput.Z, tissue);
                case TallyType.FluenceOfRhoAndZAndTime:
                    var frztinput = (FluenceOfRhoAndZAndTimeDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZAndTimeDetector(frztinput.Rho, frztinput.Z, frztinput.Time, tissue);
                case TallyType.AOfRhoAndZ:
                    var arzinput = (AOfRhoAndZDetectorInput)detectorInput;
                    return new AOfRhoAndZDetector(arzinput.Rho, arzinput.Z, tissue);
                case TallyType.ATotal:
                    return new ATotalDetector(tissue);
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
                case TallyType.pMuaMusInROfRhoAndTime:
                    var prrtinput = (pMCROfRhoAndTimeDetectorInput)detectorInput;
                    return new pMCMuaMusROfRhoAndTimeDetector(prrtinput.Rho, prrtinput.Time, tissue, prrtinput.PerturbedOps, prrtinput.PerturbedRegionsIndices);
                case TallyType.pMuaMusInROfRho:
                    var input = (pMCROfRhoAndTimeDetectorInput)detectorInput;
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
