using System;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Factory to determine phase function
    /// </summary>
    public static class PhaseFunctionFactory
    {
        /// <summary>
        /// Method to determine phase function
        /// </summary>
        /// <param name="tissueRegion">tissue region in question</param>
        /// <param name="ti">tissue input</param>
        /// <param name="rng">random number generator</param>
        /// <returns></returns>
        public static IPhaseFunction GetPhaseFunction(ITissueRegion tissueRegion, ITissueInput ti, Random rng)
        {
            IPhaseFunction phaseFunction = null;

            if (ti.RegionPhaseFunctionInputs.Count == 0)
            {
                throw new Exception(String.Format("Dictionary {0} is empty", ti.RegionPhaseFunctionInputs.ToString()));
            }

            if (!ti.RegionPhaseFunctionInputs.ContainsKey(tissueRegion.PhaseFunctionKey))
            {
                throw new Exception(String.Format("Key {0} was not found", tissueRegion.PhaseFunctionKey));
            }

            var input = ti.RegionPhaseFunctionInputs[tissueRegion.PhaseFunctionKey];

            if (input is HenyeyGreensteinPhaseFunctionInput)
            {
                var hgPhaseFunctionInput = (HenyeyGreensteinPhaseFunctionInput)input;
                phaseFunction = new HenyeyGreensteinPhaseFunction(tissueRegion.RegionOP.G, rng);
            }

            if (input is LookupTablePhaseFunctionInput)
            {
                var lutPhaseFunctionInput = (LookupTablePhaseFunctionInput)input;
                if (lutPhaseFunctionInput.RegionPhaseFunctionData is PolarLookupTablePhaseFunctionData)
                {
                    phaseFunction = new LookupTablePhaseFunction(
                        (PolarLookupTablePhaseFunctionData)lutPhaseFunctionInput.RegionPhaseFunctionData, rng);   
                }
                if (lutPhaseFunctionInput.RegionPhaseFunctionData is PolarAndAzimuthalLookupTablePhaseFunctionData)
                {
                    phaseFunction = new LookupTablePhaseFunction(
                        (PolarAndAzimuthalLookupTablePhaseFunctionData)lutPhaseFunctionInput.RegionPhaseFunctionData, rng);
                }
            }

            if (input is BidirectionalPhaseFunctionInput)
            {
                var bdPhaseFunctionInput = (BidirectionalPhaseFunctionInput)input;
                phaseFunction = new BidirectionalPhaseFunction(tissueRegion.RegionOP.G, rng);
            }

            if (input is ReynoldsMcCormickPhaseFunctionInput)
            {
                var rmPhaseFunctionInput = (ReynoldsMcCormickPhaseFunctionInput)input;
                phaseFunction = new ReynoldsMcCormickPhaseFunction(tissueRegion.RegionOP.G, rmPhaseFunctionInput.Alpha, rng);
            }
            return phaseFunction;
        }
    }
}
