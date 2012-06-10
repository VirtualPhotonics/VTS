using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class DetectorFactory
    {
        private static Dictionary<string, IProvider<IDetector>> _providers;

        // todo: look up ways of using MEF to register 3rd party plug-ins at runtime
        public static void RegisterProvider(IProvider<IDetector> provider)
        {
            var typeString = provider.TargetType.ToString();
            if(_providers.ContainsKey(typeString))
            {
                _providers.Remove(typeString);
            }
            _providers.Add(typeString, provider);
        }
        
        public static void RegisterProviders(IEnumerable<IProvider<IDetector>> providers)
        {
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }
        }

        /// <summary>
        /// Method to instantiate all detectors given list of IDetectorInputs.  This method calls
        /// the method below that instantiates a single detector.
        /// </summary>
        /// <param name="detectorInputs">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <returns>List of IDetector</returns>
        public static IList<IDetector> GetDetectors(IEnumerable<IDetectorInput> detectorInputs, ITissue tissue, bool tallySecondMoment)
        {
            if (detectorInputs == null)
            {
                return null;
            }
            return detectorInputs.Select(detectorInput => GetDetector(detectorInput, tissue, tallySecondMoment)).ToList();
        }
        /// <summary>
        /// Method that instantiates the correct detector class given a IDetectorInput
        /// </summary>
        /// <param name="detectorInput">IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <returns>IDetector</returns>
        public static IDetector GetDetector(
            IDetectorInput detectorInput,
            ITissue tissue,
            bool tallySecondMoment)
        {
            switch (detectorInput.TallyType)
            {
                // IDetector(s):
                case TallyType.RDiffuse:
                    var rdinput = (RDiffuseDetectorInput)detectorInput;
                    //return new RDiffuseDetector(tallySecondMoment, rdinput.Name);
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
                case TallyType.ROfFx:
                    var rfxinput = (ROfFxDetectorInput)detectorInput;
                    return new ROfFxDetector(rfxinput.Fx, tallySecondMoment, rfxinput.Name);
                case TallyType.ROfFxAndTime:
                    var rfxtinput = (ROfFxAndTimeDetectorInput)detectorInput;
                    return new ROfFxAndTimeDetector(rfxtinput.Fx, rfxtinput.Time, tallySecondMoment, rfxtinput.Name);
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
                case TallyType.RadianceOfRho:
                    var drinput = (RadianceOfRhoDetectorInput)detectorInput;
                    return new RadianceOfRhoDetector(drinput.ZDepth, drinput.Rho, tissue, tallySecondMoment, drinput.Name);

                // IHistoryDetector(s):
                case TallyType.FluenceOfRhoAndZ:
                    var frzinput = (FluenceOfRhoAndZDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZDetector(frzinput.Rho, frzinput.Z, tissue, tallySecondMoment, frzinput.Name);
                case TallyType.FluenceOfRhoAndZAndTime:
                    var frztinput = (FluenceOfRhoAndZAndTimeDetectorInput)detectorInput;
                    return new FluenceOfRhoAndZAndTimeDetector(frztinput.Rho, frztinput.Z, frztinput.Time, tissue, tallySecondMoment, frztinput.Name);
                case TallyType.FluenceOfXAndYAndZ:
                    var fxyzinput = (FluenceOfXAndYAndZDetectorInput)detectorInput;
                    return new FluenceOfXAndYAndZDetector(fxyzinput.X, fxyzinput.Y, fxyzinput.Z, tissue, tallySecondMoment, fxyzinput.Name);
                case TallyType.AOfRhoAndZ:
                    var arzinput = (AOfRhoAndZDetectorInput)detectorInput;
                    return new AOfRhoAndZDetector(arzinput.Rho, arzinput.Z, tissue, tallySecondMoment, arzinput.Name);
                case TallyType.ATotal:
                    var ainput = (ATotalDetectorInput)detectorInput;
                    return new ATotalDetector(tissue, tallySecondMoment, ainput.Name);
                case TallyType.RadianceOfRhoAndZAndAngle:
                    var rrzainput = (RadianceOfRhoAndZAndAngleDetectorInput)detectorInput;
                    return new RadianceOfRhoAndZAndAngleDetector(rrzainput.Rho, rrzainput.Z, rrzainput.Angle, tissue, tallySecondMoment, rrzainput.Name);
                case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
                    var rxyztpinput = (RadianceOfXAndYAndZAndThetaAndPhiDetectorInput)detectorInput;
                    return new RadianceOfXAndYAndZAndThetaAndPhiDetector(rxyztpinput.X, rxyztpinput.Y, rxyztpinput.Z, rxyztpinput.Theta, rxyztpinput.Phi, tissue, tallySecondMoment, rxyztpinput.Name);
                case TallyType.ReflectedMTOfRhoAndSubregionHist:
                    var rmtrsinput = (ReflectedMTOfRhoAndSubregionHistDetectorInput)detectorInput;
                    return new ReflectedMTOfRhoAndSubregionHistDetector(rmtrsinput.Rho, rmtrsinput.MTBins, tissue, tallySecondMoment, rmtrsinput.Name);
                case TallyType.ReflectedTimeOfRhoAndSubregionHist:
                    var rtrsinput = (ReflectedTimeOfRhoAndSubregionHistDetectorInput)detectorInput;
                    return new ReflectedTimeOfRhoAndSubregionHistDetector(rtrsinput.Rho, rtrsinput.Time, tissue, tallySecondMoment, rtrsinput.Name);

                // pMC Detector(s):
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
                case TallyType.pMCROfFx:
                    var prfxinput = (pMCROfFxDetectorInput)detectorInput;
                    return new pMCROfFxDetector(
                        prfxinput.Fx,
                        tissue,
                        prfxinput.PerturbedOps.ToArray(), // todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
                        prfxinput.PerturbedRegionsIndices.ToArray(),// todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
                        tallySecondMoment,
                        prfxinput.Name
                        );
                case TallyType.pMCROfFxAndTime:
                    var prfxtinput = (pMCROfFxAndTimeDetectorInput)detectorInput;
                    return new pMCROfFxAndTimeDetector(
                        prfxtinput.Fx,
                        prfxtinput.Time,
                        tissue,
                        prfxtinput.PerturbedOps.ToArray(),// todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
                        prfxtinput.PerturbedRegionsIndices.ToArray(),// todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
                        tallySecondMoment,
                        prfxtinput.Name
                        );
                default:
                    return null;
            }
        }
    }
}
