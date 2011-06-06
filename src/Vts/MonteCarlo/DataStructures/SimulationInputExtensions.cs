using System;
using System.Collections.Generic;
using System.Linq;
using Vts.IO;
using Vts.Extensions;
using Vts.MonteCarlo.Tissues;
using Vts.Common;

namespace Vts.MonteCarlo
{
    public static class SimulationInputExtensions
    {
        public static SimulationInput WithValue(this SimulationInput data, InputParameterType type, double value)
        {
            var result = data.Clone();

            switch (type)
            {
                case InputParameterType.Mua1:
                    if (result.TissueInput.Regions.Count() >= 2)
                        result.TissueInput.Regions[1].RegionOP.Mua = value;
                    break;
                case InputParameterType.Mus1:
                    if (result. TissueInput.Regions.Count() >= 2)
                        result. TissueInput.Regions[1].RegionOP.Mus = value;
                    break;
                case InputParameterType.G1:
                    if (result. TissueInput.Regions.Count() >= 2)
                        result. TissueInput.Regions[1].RegionOP.G = value;
                    break;
                case InputParameterType.N1:
                    if (result. TissueInput.Regions.Count() >= 2)
                        result. TissueInput.Regions[1].RegionOP.N = value;
                    break;
                //case InputParameterType.D1:
                //    if (result. TissueInput.Regions.Count() >= 2)
                //        result. TissueInput.Regions[1].Center.Z = value;
                //    break;

                case InputParameterType.Mua2:
                    if (result. TissueInput.Regions.Count() >= 3)
                        result. TissueInput.Regions[2].RegionOP.Mua = value;
                    break;
                case InputParameterType.Mus2:
                    if (result. TissueInput.Regions.Count() >= 3)
                        result. TissueInput.Regions[2].RegionOP.Mus = value;
                    break;
                case InputParameterType.G2:
                    if (result. TissueInput.Regions.Count() >= 3)
                        result. TissueInput.Regions[2].RegionOP.G = value;
                    break;
                case InputParameterType.N2:
                    if (result. TissueInput.Regions.Count() >= 3)
                        result. TissueInput.Regions[2].RegionOP.N = value;
                    break;
                //case InputParameterType.D2:
                //    if (result. TissueInput.Regions.Count() >= 2)
                //        result. TissueInput.Regions[1].d = value;
                //    break;

                //case InputParameterType.XSourcePosition:
                //    result.source.beam_center_x = value;
                //    break;
                // todo: add Y source position to infiles/siRegionOP.Mulation capabilities.
                // be careful about rectangle distribution of source
                //case InputParameterType.YSourcePosition:
                //    result.source.beam_center_y = value;
                //    break;
                //case InputParameterType.XEllipsePosition:
                //    result.tissptr.ellip_x = value;
                //    break;
                //case InputParameterType.YEllipsePosition:
                //    result.tissptr.ellip_y = value;
                //    break;
                //case InputParameterType.ZEllipsePosition:
                //    result.tissptr.ellip_z = value;
                //    break;
                //case InputParameterType.XEllipseRadius:
                //    result.tissptr.ellip_rad_x = value;
                //    break;
                //case InputParameterType.YEllipseRadius:
                //    result.tissptr.ellip_rad_y = value;
                //    break;
                //case InputParameterType.ZEllipseRadius:
                //    result.tissptr.ellip_rad_z = value;
                //    break;

            }
            return result;
        }

        public static SimulationInput WithValue(this SimulationInput data, string inputParameter, double value)
        {
            var result = data.Clone();

            // append sweep value to the output name
            result.OutputName += ("_" + inputParameter + "_" + String.Format("{0:f}", value));

            var parameterString = inputParameter.ToLower().TrimEnd();
            var regionString = inputParameter.Skip(parameterString.Length).ToString();
            
            int regionIndex;
            if (!int.TryParse(regionString, out regionIndex))
            {
                return null;
            }

            switch (parameterString)
            {
                case "mua":
                    if (result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.Mua = value;
                    break;
                case "mus":
                    if (result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.Mus = value;
                    break;
                case "g":
                    if (result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.G = value;
                    break;
                case "n":
                    if (result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.N = value;
                    break;
                case "d":
                    var multiLayerTissueInput = result.TissueInput as MultiLayerTissueInput;
                    if (multiLayerTissueInput != null && multiLayerTissueInput.Regions.Count() > regionIndex)
                    {
                        var regions = multiLayerTissueInput.Regions as IList<LayerRegion>;

                        // keep a separate copy of the range before we modify it
                        var previousRange = regions[regionIndex].ZRange.Clone();

                        // modify the target layer thickness by specifying Stop (which internally updtes Delta as well)
                        regions[regionIndex].ZRange.Stop = regions[regionIndex].ZRange.Start + value;

                        // then, update the rest of the following layers with an adjusted thickness
                        var changeInThickness = regions[regionIndex].ZRange.Delta - previousRange.Delta;
                        regions.Skip(regionIndex+1).ForEach((lr,i)=>lr.ZRange.Stop= lr.ZRange.Start + changeInThickness);
                    }
                    break;

                //case InputParameterType.XSourcePosition:
                //    result.source.beam_center_x = value;
                //    break;
                // todo: add Y source position to infiles/siRegionOP.Mulation capabilities.
                // be careful about rectangle distribution of source
                //case InputParameterType.YSourcePosition:
                //    result.source.beam_center_y = value;
                //    break;
                //case InputParameterType.XEllipsePosition:
                //    result.tissptr.ellip_x = value;
                //    break;
                //case InputParameterType.YEllipsePosition:
                //    result.tissptr.ellip_y = value;
                //    break;
                //case InputParameterType.ZEllipsePosition:
                //    result.tissptr.ellip_z = value;
                //    break;
                //case InputParameterType.XEllipseRadius:
                //    result.tissptr.ellip_rad_x = value;
                //    break;
                //case InputParameterType.YEllipseRadius:
                //    result.tissptr.ellip_rad_y = value;
                //    break;
                //case InputParameterType.ZEllipseRadius:
                //    result.tissptr.ellip_rad_z = value;
                //    break;

            }
            return result;
        }

        public static SimulationInput WithValue(this SimulationInput data, Action<SimulationInput, double> assignmentAction, double value)
        {
            var result = data.Clone();

            assignmentAction(result, value);

            return result;
        }

        public static IEnumerable<SimulationInput> WithParameterSweep(this IEnumerable<SimulationInput> input, IEnumerable<double> values, InputParameterType t)
        {
            return input.SelectMany(i => values, (i, j) => i.WithValue(t, j));
        }

        // new version - attempting to make assignment more dynamic/flexible
        public static IEnumerable<SimulationInput> WithParameterSweep(this IEnumerable<SimulationInput> input, IEnumerable<double> values, string inputParameterType)
        {
            return input.SelectMany(i => values, (i, j) => i.WithValue(inputParameterType, j));
        }
        
        //private static IEnumerable<SiRegionOP.MulationInput> WithParameterSweep(this IEnumerable<SiRegionOP.MulationInput> input, IEnumerable<double> values, Action<SiRegionOP.MulationInput, double> assignmentAction)
        //{
        //    return input.SelectMany(i => values, (i, j) => i.WithValue(assignmentAction, j));
        //}

    }
}
