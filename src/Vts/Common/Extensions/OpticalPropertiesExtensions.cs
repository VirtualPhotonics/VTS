using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.Extensions
{
    public static class OpticalPropertiesExtensions
    {
        public static double GetScatterLength(this OpticalProperties op, AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                default:
                case AbsorptionWeightingType.Discrete:
                case AbsorptionWeightingType.Analog:
                    return (op.Mua + op.Mus);
                    break;
                case AbsorptionWeightingType.Continuous:
                    return op.Mus;
                    break;
            }
        }
    }
}
