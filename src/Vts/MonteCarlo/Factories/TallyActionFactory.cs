using System.Collections.Generic;
using System;
using Vts.Common;
using Vts.MonteCarlo.TallyActions;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class TallyActionFactory
    {
        public static bool IsHistoryTally(TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.FluenceOfRhoAndZ:
                    return true;
                case TallyType.FluenceOfRhoAndZAndTime:
                    return true;
                case TallyType.AOfRhoAndZ:
                    return true;
                case TallyType.ATotal:
                    return true;
                default:
                    return false;
            }
        }

        public static ITerminationTally GetTerminationTallyAction(
            TallyType tallyType,
            ITissue tissue,
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y)
        {
            switch (tallyType)
            {
                default:
                case TallyType.RDiffuse:
                    return new RDiffuseTally();
                case TallyType.ROfRho:
                    return new ROfRhoTally(rho);
                case TallyType.ROfAngle:
                    return new ROfAngleTally(angle);
                case TallyType.ROfRhoAndTime:
                    return new ROfRhoAndTimeTally(rho, time);
                case TallyType.ROfRhoAndAngle:
                    return new ROfRhoAndAngleTally(rho, angle);
                case TallyType.ROfXAndY:
                    return new ROfXAndYTally(x, y);
                case TallyType.ROfRhoAndOmega:
                    return new ROfRhoAndOmegaTally(rho, omega);
                case TallyType.TDiffuse:
                    return new TDiffuseTally();
                case TallyType.TOfAngle:
                    return new TOfAngleTally(angle);
                case TallyType.TOfRho:
                    return new TOfRhoTally(rho);
                case TallyType.TOfRhoAndAngle:
                    return new TOfRhoAndAngleTally(rho, angle);
            }
        }

        public static IHistoryTally GetHistoryTallyAction(
            TallyType tallyType,
            AbsorptionWeightingType awt,
            ITissue tissue,
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y)
        {
            switch (tallyType)
            {
                default:
                case TallyType.FluenceOfRhoAndZ:
                    return new FluenceOfRhoAndZTally(rho, z, tissue);
                case TallyType.FluenceOfRhoAndZAndTime:
                    return new FluenceOfRhoAndZAndTimeTally(rho, z, time, tissue);
                case TallyType.AOfRhoAndZ:
                    return new AOfRhoAndZTally(rho, z, tissue);
                case TallyType.ATotal:
                    return new ATotalTally(tissue);
            }
        }

        // pMC overload
        public static ITerminationTally GetTerminationTallyAction(
            TallyType tallyType,
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y,
            AbsorptionWeightingType awt,
            IList<OpticalProperties> referenceOps,
            IList<int> perturbedRegionsIndices)
        {
            switch (tallyType)
            {
                default:
                case TallyType.pMuaMusInROfRhoAndTime:
                    return new pMuaMusInROfRhoAndTimeTally(rho, time, awt, referenceOps, perturbedRegionsIndices);
                case TallyType.pMuaMusInROfRho:
                    return new pMuaMusInROfRhoTally(rho, awt, referenceOps, perturbedRegionsIndices);
            }
        }
 
        public static IHistoryTally GetHistoryTallyAction(
            TallyType tallyType,
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y,
            AbsorptionWeightingType awt,
            IList<OpticalProperties> referenceOps,
            IList<int> perturbedRegionsIndices)
        {
            throw new NotSupportedException("not implemented yet");
        }
    }
}
