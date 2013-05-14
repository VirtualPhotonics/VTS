using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhaseFunctionInputs;
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
                phaseFunction = new LookupTablePhaseFunction(rng, lutPhaseFunctionInput.RegionPhaseFunctionData);
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
