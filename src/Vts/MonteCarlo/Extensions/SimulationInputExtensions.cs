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
            result.OutputName += ("_" + inputParameter + "_" + String.Format("{0:G15}", value));

            var parameterString = inputParameter.ToLower().TrimEnd("0123456789".ToCharArray());
            var regionString = inputParameter.Substring(parameterString.Length);

            int regionIndex;
            if (!int.TryParse(regionString, out regionIndex))
            {
                regionIndex = -1;
            }

            switch (parameterString)
            {
                case "nphot":
                    result.N = (long)value;
                    break;
                case "seed":
                    result.Options.Seed = (int)value;
                    break;
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
                case "d":  // the following code accommodates MultiLayerTissue and InclusionTissue
                    List<ITissueRegion> layerTissueInput = result.TissueInput.Regions.ToList();
                    if (result.TissueInput is MultiLayerTissueInput)
                    {
                        layerTissueInput = result.TissueInput.Regions.ToList();
                    }

                    var singleEllipsoidTissue = result.TissueInput as SingleEllipsoidTissueInput;
                    if (singleEllipsoidTissue != null)
                    {
                        layerTissueInput = singleEllipsoidTissue.LayerRegions.ToList();
                    }

                    var layerRegion = (LayerTissueRegion)layerTissueInput.Skip(regionIndex).First();

                    // keep a separate copy of the range before we modify it
                    var previousRange = layerRegion.ZRange.Clone();

                    // modify the target layer thickness by specifying Stop (which internally updtes Delta as well)
                    layerRegion.ZRange.Stop = layerRegion.ZRange.Start + value;

                    // then, update the rest of the following layers with an adjusted thickness
                    var changeInThickness = layerRegion.ZRange.Delta - previousRange.Delta;
                    foreach (var region in layerTissueInput.Skip(regionIndex + 1).Select(r => (LayerTissueRegion)r))
                    {
                        region.ZRange = new DoubleRange(
                            region.ZRange.Start + changeInThickness,
                            region.ZRange.Stop + changeInThickness,
                            region.ZRange.Count);
                    }
                    break;
                case "xsourceposition":
                case "ysourceposition":
                case "zsourceposition":
                    Action<Position> sourcePositionModifier = null;
                    switch (parameterString)
                    {
                        case "xsourceposition":
                            sourcePositionModifier = p => p.X = value;
                            break;
                        case "ysourceposition":
                            sourcePositionModifier = p => p.Y = value;
                            break;
                        case "zsourceposition":
                            sourcePositionModifier = p => p.Z = value;
                            break;
                    }
                    Position sourcePosition = null;
                    dynamic sourceInput = result.SourceInput;
                    switch (result.SourceInput.SourceType)
                    {
                        case "IsotropicPoint":
                        case "DirectionalPoint":
                        case "CustomPoint":
                            sourcePosition = (Position)sourceInput.PointLocation;
                            break;
                        case "IsotropicLine":
                        case "DirectionalLine":
                        case "CustomLine":
                        case "DirectionalCircular":
                        case "CustomCircular":
                        case "CircularAngledFromPointSource":
                        case "DirectionalElliptical":
                        case "CustomElliptical":
                        case "DirectionalRectangular":
                        case "CustomRectangular":
                        case "LambertianSurfaceEmittingSpherical":
                        case "CustomSurfaceEmittingSpherical":
                        case "LambertianSurfaceEmittingCuboidal":
                        case "LambertianSurfaceEmittingTubular":
                        case "LambertianSurfaceEmittingCylindricalFiber":
                        case "IsotropicVolumetricCuboidal":
                        case "CustomVolumetricCuboidal":
                        case "IsotropicVolumetricEllipsoidal":
                        case "CustomVolumetricEllipsoidal":
                        default:
                            sourcePosition = (Position)sourceInput.TranslationFromOrigin;
                            break;
                    }
                    sourcePositionModifier(sourcePosition);
                    break;

                case "xinclusionposition":
                case "yinclusionposition":
                case "zinclusionposition":
                    Action<Position> inclusionPositionModifier = null;
                    switch (parameterString)
                    {
                        case "xinclusionposition":
                            inclusionPositionModifier = p => p.X = value;
                            break;
                        case "yinclusionposition":
                            inclusionPositionModifier = p => p.Y = value;
                            break;
                        case "zinclusionposition":
                            inclusionPositionModifier = p => p.Z = value;
                            break;
                    }
                    dynamic inclusionRegion = null;
                    switch (result.TissueInput.TissueType)
                    {
                        case "SingleEllipsoid":
                            inclusionRegion = ((SingleEllipsoidTissueInput)result.TissueInput).EllipsoidRegion;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(parameterString);
                    }
                    inclusionPositionModifier(inclusionRegion.Center); // dynamic binding...works?
                    break;
                case "xinclusionradius":
                case "yinclusionradius":
                case "zinclusionradius":
                    dynamic tissueInputWithRadius = result.TissueInput;
                    switch (result.TissueInput.TissueType)
                    {
                        case "SingleEllipsoid":
                            switch (parameterString)
                            {
                                case "xinclusionradius":
                                    ((EllipsoidTissueRegion)tissueInputWithRadius.EllipsoidRegion).Dx = value;
                                    break;
                                case "yinclusionradius":
                                    ((EllipsoidTissueRegion)tissueInputWithRadius.EllipsoidRegion).Dy = value;
                                    break;
                                case "zinclusionradius":
                                    ((EllipsoidTissueRegion)tissueInputWithRadius.EllipsoidRegion).Dz = value;
                                    break;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(parameterString);
                    }
                    break;

                // consider add Y source position to infiles/simulation capabilities.
                // be careful about rectangle distribution of source

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
