using System.Collections.Generic;
using System;
using Vts.Common;
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
                    var rdinput = (RDiffuseDetectorInput)detectorInput;
                    return new RDiffuseDetector(rdinput.Name);
                case TallyType.ROfRho:
                    var rrinput = (ROfRhoDetectorInput)detectorInput;
                    return new ROfRhoDetector(rrinput.Rho, rrinput.Name);
                case TallyType.ROfAngle:
                    var rainput = (ROfAngleDetectorInput)detectorInput;
                    return new ROfAngleDetector(rainput.Angle, rainput.Name);
                case TallyType.ROfRhoAndTime:
                    var rrtinput = (ROfRhoAndTimeDetectorInput)detectorInput;
                    return new ROfRhoAndTimeDetector(rrtinput.Rho, rrtinput.Time, rrtinput.Name);
                case TallyType.ROfRhoAndAngle:
                    var rrainput = (ROfRhoAndAngleDetectorInput)detectorInput;
                    return new ROfRhoAndAngleDetector(rrainput.Rho, rrainput.Angle, rrainput.Name);
                case TallyType.ROfXAndY:
                    var rxyinput = (ROfXAndYDetectorInput)detectorInput;
                    return new ROfXAndYDetector(rxyinput.X, rxyinput.Y, rxyinput.Name);
                case TallyType.ROfRhoAndOmega:
                    var rroinput = (ROfRhoAndOmegaDetectorInput)detectorInput;
                    return new ROfRhoAndOmegaDetector(rroinput.Rho, rroinput.Omega, rroinput.Name);
                case TallyType.TDiffuse:
                    var tdinput = (TDiffuseDetectorInput)detectorInput;
                    return new TDiffuseDetector(tdinput.Name);
                case TallyType.TOfAngle:
                    var tainput = (TOfAngleDetectorInput)detectorInput;
                    return new TOfAngleDetector(tainput.Angle, tainput.Name);
                case TallyType.TOfRho:
                    var trinput = (TOfRhoDetectorInput)detectorInput;
                    return new TOfRhoDetector(trinput.Rho, trinput.Name);
                case TallyType.TOfRhoAndAngle:
                    var trainput = (TOfRhoAndAngleDetectorInput)detectorInput;
                    return new TOfRhoAndAngleDetector(trainput.Rho, trainput.Angle, trainput.Name);
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
                    return new FluenceOfRhoAndZDetector(frzinput.Rho, frzinput.Z, tissue, frzinput.Name);
                case TallyType.FluenceOfRhoAndZAndTime:
                    var frztinput = (FluenceOfRhoAndZAndTimeDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZAndTimeDetector(frztinput.Rho, frztinput.Z, frztinput.Time, tissue, frztinput.Name);
                case TallyType.AOfRhoAndZ:
                    var arzinput = (AOfRhoAndZDetectorInput)detectorInput;
                    return new AOfRhoAndZDetector(arzinput.Rho, arzinput.Z, tissue, arzinput.Name);
                case TallyType.ATotal:
                    var ainput = (ATotalDetectorInput)detectorInput;
                    return new ATotalDetector(tissue, ainput.Name);
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
                case TallyType.pMCROfRhoAndTime:
                    var prrtinput = (pMCROfRhoAndTimeDetectorInput)detectorInput;
                    return new pMCROfRhoAndTimeDetector(
                        prrtinput.Rho, 
                        prrtinput.Time, 
                        tissue, 
                        prrtinput.PerturbedOps, 
                        prrtinput.PerturbedRegionsIndices,
                        prrtinput.Name);
                case TallyType.pMCROfRho:
                    var prrinput = (pMCROfRhoDetectorInput)detectorInput;
                    return new pMCROfRhoDetector(
                        prrinput.Rho, 
                        tissue, 
                        prrinput.PerturbedOps, 
                        prrinput.PerturbedRegionsIndices,
                        prrinput.Name);
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
