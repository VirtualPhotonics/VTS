using System;
using System.Collections.Generic;
using System.Linq;
using Vts.IO;

namespace Vts.MonteCarlo.Detectors
{
    public static class DetectorInputExtensions
    {
        public static DetectorInput WithValue(this DetectorInput data, InputParameterType type, double value)
        {
            var result = data.Clone();

            //switch (type)
            //{
            //    case InputParameterType.Mua1:
            //        if (result.TissueInput.Regions.Count() >= 2)
            //            result.TissueInput.Regions[1].RegionOP.Mua = value;
            //        break;
            //    case InputParameterType.Mus1:
            //        if (result. TissueInput.Regions.Count() >= 2)
            //            result. TissueInput.Regions[1].RegionOP.Mus = value;
            //        break;
            //    case InputParameterType.G1:
            //        if (result. TissueInput.Regions.Count() >= 2)
            //            result. TissueInput.Regions[1].RegionOP.G = value;
            //        break;
            //}
            return result;
        }

        public static DetectorInput WithValue(this DetectorInput data, Action<DetectorInput, double> assignmentAction, double value)
        {
            var result = data.Clone();

            assignmentAction(result, value);

            return result;
        }

        public static IEnumerable<DetectorInput> WithParameterSweep(this IEnumerable<DetectorInput> input, IEnumerable<double> values, InputParameterType t)
        {
            return input.SelectMany(i => values, (i, j) => i.WithValue(t, j));
        }

        
        //private static IEnumerable<SiRegionOP.MulationInput> WithParameterSweep(this IEnumerable<SiRegionOP.MulationInput> input, IEnumerable<double> values, Action<SiRegionOP.MulationInput, double> assignmentAction)
        //{
        //    return input.SelectMany(i => values, (i, j) => i.WithValue(assignmentAction, j));
        //}

    }

}
