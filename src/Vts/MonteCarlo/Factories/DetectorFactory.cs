using System.Collections.Generic;
using System;
using System.Linq;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class DetectorFactory
    {
        public static IList<IDetector> GetDetectors(IEnumerable<IDetectorInput> detectorInputs, ITissue tissue, bool tallySecondMoment)
        {
            if (detectorInputs == null)
            {
                return null;
            }
            return detectorInputs.Select(detectorInput => GetDetector(detectorInput, tissue, tallySecondMoment)).ToList();
        }
       
        public static IDetector GetDetector(
            IDetectorInput detectorInput,
            ITissue tissue,
            bool tallySecondMoment)
        {
            switch (detectorInput.TallyType)
            {
                // ISurfaceDetector(s):
                case TallyType.RDiffuse:
                    var rdinput = (RDiffuseDetectorInput)detectorInput;
                    return new RDiffuseDetector(tallySecondMoment, rdinput.Name);
                case TallyType.RSpecular:
                    var rsinput = (RSpecularDetectorInput)detectorInput;
                    return new RSpecularDetector(tallySecondMoment, rsinput.Name);
                case TallyType.ROfRho:
                    var rrinput = (ROfRhoDetectorInput)detectorInput;
                    return new ROfRhoDetector(rrinput.Rho, tallySecondMoment, rrinput.Name);
                case TallyType.ROfAngle:
                    var rainput = (ROfAngleDetectorInput)detectorInput;
                    return new ROfAngleDetector(rainput.Angle, tallySecondMoment, rainput.Name);
                case TallyType.ROfRhoAndTime:
                    var rrtinput = (ROfRhoAndTimeDetectorInput)detectorInput;
                    return new ROfRhoAndTimeDetector(rrtinput.Rho, rrtinput.Time, tallySecondMoment, rrtinput.Name);
                case TallyType.ROfRhoAndAngle:
                    var rrainput = (ROfRhoAndAngleDetectorInput)detectorInput;
                    return new ROfRhoAndAngleDetector(rrainput.Rho, rrainput.Angle, tallySecondMoment, rrainput.Name);
                case TallyType.ROfXAndY:
                    var rxyinput = (ROfXAndYDetectorInput)detectorInput;
                    return new ROfXAndYDetector(rxyinput.X, rxyinput.Y, tallySecondMoment, rxyinput.Name);
                case TallyType.ROfRhoAndOmega:
                    var rroinput = (ROfRhoAndOmegaDetectorInput)detectorInput;
                    return new ROfRhoAndOmegaDetector(rroinput.Rho, rroinput.Omega, tallySecondMoment, rroinput.Name);
                case TallyType.TDiffuse:
                    var tdinput = (TDiffuseDetectorInput)detectorInput;
                    return new TDiffuseDetector(tallySecondMoment, tdinput.Name);
                case TallyType.TOfAngle:
                    var tainput = (TOfAngleDetectorInput)detectorInput;
                    return new TOfAngleDetector(tainput.Angle, tallySecondMoment, tainput.Name);
                case TallyType.TOfRho:
                    var trinput = (TOfRhoDetectorInput)detectorInput;
                    return new TOfRhoDetector(trinput.Rho, tallySecondMoment, trinput.Name);
                case TallyType.TOfRhoAndAngle:
                    var trainput = (TOfRhoAndAngleDetectorInput)detectorInput;
                    return new TOfRhoAndAngleDetector(trainput.Rho, trainput.Angle, tallySecondMoment, trainput.Name);
                case TallyType.DosimetryOfRho:
                    var drinput = (DosimetryOfRhoDetectorInput)detectorInput;
                    return new DosimetryOfRhoDetector(drinput.ZDepth, drinput.Rho, tissue, tallySecondMoment, drinput.Name);

                // IVolumeDetector(s):
                case TallyType.FluenceOfRhoAndZ:
                    var frzinput = (FluenceOfRhoAndZDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZDetector(frzinput.Rho, frzinput.Z, tissue, tallySecondMoment, frzinput.Name);
                case TallyType.FluenceOfRhoAndZAndTime:
                    var frztinput = (FluenceOfRhoAndZAndTimeDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZAndTimeDetector(frztinput.Rho, frztinput.Z, frztinput.Time, tissue, tallySecondMoment, frztinput.Name);
                case TallyType.AOfRhoAndZ:
                    var arzinput = (AOfRhoAndZDetectorInput)detectorInput;
                    return new AOfRhoAndZDetector(arzinput.Rho, arzinput.Z, tissue, tallySecondMoment, arzinput.Name);
                case TallyType.ATotal:
                    var ainput = (ATotalDetectorInput)detectorInput;
                    return new ATotalDetector(tissue, tallySecondMoment, ainput.Name);

                default:
                    return null;
            }
        }
        // pMC methods
        public static IList<IDetector> GetDetectors(IEnumerable<IpMCDetectorInput> detectorInputs, ITissue tissue, bool tallySecondMoment)
        {
            return detectorInputs.Select(detectorInput => GetpMCDetector(detectorInput, tissue, tallySecondMoment)).ToList();
        }
        public static IDetector GetpMCDetector(
            IpMCDetectorInput detectorInput,
            ITissue tissue,
            bool tallySecondMoment)
        {
            switch (detectorInput.TallyType)
            {
                case TallyType.pMCROfRhoAndTime:
                    var prrtinput = (pMCROfRhoAndTimeDetectorInput)detectorInput;
                    return new pMCROfRhoAndTimeDetector(
                        prrtinput.Rho, 
                        prrtinput.Time, 
                        tissue, 
                        prrtinput.PerturbedOps, 
                        prrtinput.PerturbedRegionsIndices,
                        tallySecondMoment,
                        prrtinput.Name);
                case TallyType.pMCROfRho:
                    var prrinput = (pMCROfRhoDetectorInput)detectorInput;
                    return new pMCROfRhoDetector(
                        prrinput.Rho, 
                        tissue, 
                        prrinput.PerturbedOps, 
                        prrinput.PerturbedRegionsIndices,
                        tallySecondMoment,
                        prrinput.Name
                        );

                default:
                    return null;
            }
        }
 
        public static IVolumeDetector GetHistoryDetector(
            IDetectorInput detectorInput,
            ITissue tissue,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices)
        {
            throw new NotSupportedException("not implemented yet");
        }
    }
}
