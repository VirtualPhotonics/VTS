using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods to aid in setting up Monte Carlo simulation input values. 
    /// </summary>
    public static class SimulationInputExtensions
    {
        /// <summary>
        /// Method to overwrite data in SimulationInput.  Used in MC CommandLineApplication 
        /// paramsweep and paramsweepdelta.
        /// </summary>
        /// <param name="data">base SimulationInput</param>
        /// <param name="inputParameter">parameter in SimulationInput to be overwritten</param>
        /// <param name="value">value of parameter</param>
        /// <returns>updated SimulationInput with new data</returns>
        public static SimulationInput WithValue(this SimulationInput data, string inputParameter, double value)
        {
            var result = data.Clone();

            // append sweep value to the output name
            result.OutputName += ("_" + inputParameter + "_" + String.Format("{0:f}", value));

            var parameterString = inputParameter.ToLower().TrimEnd("0123456789".ToCharArray());
            var regionString = inputParameter.Substring(parameterString.Length);

            int regionIndex;
            if (!int.TryParse(regionString, out regionIndex))
            {
                regionIndex = -1;
            }

            switch (parameterString)
            {
                case "mua":
                    if (regionIndex >= 0 && result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.Mua = value;
                    break;
                case "mus":
                    if (regionIndex >= 0 && result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.Mus = value;
                    break;
                case "g":
                    if (regionIndex >= 0 && result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.G = value;
                    break;
                case "n":
                    if (regionIndex >= 0 && result.TissueInput.Regions.Count() > regionIndex)
                        result.TissueInput.Regions[regionIndex].RegionOP.N = value;
                    break;
                case "d":
                    var multiLayerTissueInput = result.TissueInput as MultiLayerTissueInput;
                    if (multiLayerTissueInput != null && regionIndex >= 0 && multiLayerTissueInput.Regions.Count() > regionIndex)
                    {
                        var layerRegion = (LayerRegion)multiLayerTissueInput.Regions.Skip(regionIndex).First();

                        // keep a separate copy of the range before we modify it
                        var previousRange = layerRegion.ZRange.Clone();

                        // modify the target layer thickness by specifying Stop (which internally updtes Delta as well)
                        layerRegion.ZRange.Stop = layerRegion.ZRange.Start + value;

                        // then, update the rest of the following layers with an adjusted thickness
                        var changeInThickness = layerRegion.ZRange.Delta - previousRange.Delta;
                        foreach (var region in multiLayerTissueInput.Regions.Skip(regionIndex + 1).Select(r => (LayerRegion)r))
                        {

                            region.ZRange = new DoubleRange(
                                region.ZRange.Start + changeInThickness,
                                region.ZRange.Stop + changeInThickness,
                                region.ZRange.Count);
                        }
                    }
                    break;
                case "source_x":
                case "source_y":
                case "source_z":
                    Action<Position> sourcePositionModifier = null;
                    switch (parameterString)
                    {
                        case "source_x":
                            sourcePositionModifier = p => p.X = value;
                            break;
                        case "source_y":
                            sourcePositionModifier = p => p.Y = value;
                            break;
                        case "source_z":
                            sourcePositionModifier = p => p.Z = value;
                            break;
                    }
                    Position sourcePosition = null;
                    dynamic sourceInput = result.SourceInput;
                    switch (result.SourceInput.SourceType)
                    {
                        case SourceType.IsotropicPoint:
                        case SourceType.DirectionalPoint:
                        case SourceType.CustomPoint:
                            sourcePosition = (Position)sourceInput.PointLocation;
                            break;
                        case SourceType.IsotropicLine:
                        case SourceType.DirectionalLine:
                        case SourceType.CustomLine:
                        case SourceType.DirectionalCircular:
                        case SourceType.CustomCircular:
                        case SourceType.DirectionalElliptical:
                        case SourceType.CustomElliptical:
                        case SourceType.DirectionalRectangular:
                        case SourceType.CustomRectangular:
                        case SourceType.LambertianSurfaceEmittingSpherical:
                        case SourceType.CustomSurfaceEmittingSpherical:
                        case SourceType.LambertianSurfaceEmittingCubiodal:
                        case SourceType.LambertianSurfaceEmittingTubular:
                        case SourceType.LambertianSurfaceEmittingCylindricalFiber:
                        case SourceType.IsotropicVolumetricCuboidal:
                        case SourceType.CustomVolumetricCubiodal:
                        case SourceType.IsotropicVolumetricEllipsoidal:
                        case SourceType.CustomVolumetricEllipsoidal:
                        default:
                            sourcePosition = (Position)sourceInput.TranslationFromOrigin;
                            break;
                    }
                    sourcePositionModifier(sourcePosition);
                    break;

                case "inclusion_position_x":
                case "inclusion_position_y":
                case "inclusion_position_z":
                    Action<Position> inclusionPositionModifier = null;
                    switch (parameterString)
                    {
                        case "inclusion_position_x":
                            inclusionPositionModifier = p => p.X = value;
                            break;
                        case "inclusion_position_y":
                            inclusionPositionModifier = p => p.Y = value;
                            break;
                        case "inclusion_position_z":
                            inclusionPositionModifier = p => p.Z = value;
                            break;
                    }
                    dynamic inclusionRegion = null;
                    switch (result.TissueInput.TissueType)
                    {
                        case TissueType.SingleEllipsoid:
                            inclusionRegion = ((SingleEllipsoidTissueInput)result.TissueInput).EllipsoidRegion;
                            //var singleEllipsoidTissueInput = (SingleEllipsoidTissueInput) result.TissueInput;
                            //inclusionPosition = ((EllipsoidRegion)singleEllipsoidTissueInput.EllipsoidRegion).Center;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    inclusionPositionModifier(inclusionRegion.Center); // dynamic binding...works?
                    break;
                case "inclusion_radius_x":
                case "inclusion_radius_y":
                case "inclusion_radius_z":
                    dynamic tissueInputWithRadius = result.TissueInput;
                    switch (result.TissueInput.TissueType)
                    {
                        case TissueType.SingleEllipsoid:
                            switch (parameterString)
                            {
                                case "inclusion_radius_x":
                                    ((EllipsoidRegion)tissueInputWithRadius.EllipsoidRegion).Dx = value;
                                    break;
                                case "inclusion_radius_y":
                                    ((EllipsoidRegion)tissueInputWithRadius.EllipsoidRegion).Dy = value;
                                    break;
                                case "inclusion_radius_z":
                                    ((EllipsoidRegion)tissueInputWithRadius.EllipsoidRegion).Dz = value;
                                    break;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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

        /// <summary>
        /// Method to perform parameter sweeps of data and create new SimulationInput with each data
        /// value in sweep.
        /// </summary>
        /// <param name="input">base SimulationInput</param>
        /// <param name="values">values to be used</param>
        /// <param name="inputParameterType">parameter identifier</param>
        /// <returns>updated IEnumerable of SimulationInput</returns>
        public static IEnumerable<SimulationInput> WithParameterSweep(this IEnumerable<SimulationInput> input, IEnumerable<double> values, string inputParameterType)
        {
            return input.SelectMany(i => values, (i, j) => i.WithValue(inputParameterType, j));
        }
    }
}
