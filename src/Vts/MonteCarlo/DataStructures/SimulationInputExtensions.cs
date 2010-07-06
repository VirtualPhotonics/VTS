using System;
using System.Collections.Generic;
using System.Linq;
using Vts.IO;

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

        
        //private static IEnumerable<SiRegionOP.MulationInput> WithParameterSweep(this IEnumerable<SiRegionOP.MulationInput> input, IEnumerable<double> values, Action<SiRegionOP.MulationInput, double> assignmentAction)
        //{
        //    return input.SelectMany(i => values, (i, j) => i.WithValue(assignmentAction, j));
        //}

    }

}
