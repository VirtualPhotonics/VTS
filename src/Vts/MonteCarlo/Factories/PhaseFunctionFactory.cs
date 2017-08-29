using System;
using Vts.MonteCarlo.LookupTablePhaseFunctionData;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.Factories
{
    public static class PhaseFunctionFactory
    {
        public static IPhaseFunction GetPhaseFunction(ITissueRegion tissueRegion, ITissueInput ti, Random rng)
        {
            IPhaseFunction phaseFunction = null;

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

            return phaseFunction;
        }
    }
}
