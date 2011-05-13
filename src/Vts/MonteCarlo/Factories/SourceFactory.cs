using System;
using Vts.Common;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class SourceFactory
    {
        public static ISource GetSource(ISourceInput input, ITissue tissue, Random rng)
        {
            switch (input.SourceType)
            {
                case SourceType.IsotropicPoint:
                    var ipInput = (DirectionalPointSourceInput)input;
                    return new DirectionalPointSource(
                        ipInput.PointLocation, 
                        ipInput.Direction);
                case SourceType.DirectionalPoint:
                case SourceType.CustomPoint:
                case SourceType.IsotropicLine:
                case SourceType.DirectionalLine:
                case SourceType.CustomLine:
                case SourceType.DirectionalCircular:
                case SourceType.CustomCircular:
                case SourceType.LambertianSurfaceEmittingCubiodal:
                case SourceType.CustomSurfaceEmittingCuboidal:
                case SourceType.DirectionalElliptical:
                case SourceType.CustomElliptical:
                case SourceType.DirectionalRectangular:
                case SourceType.CustomRectangular:
                case SourceType.LambertianSurfaceEmittingSpherical:
                case SourceType.CustomSurfaceEmittingShperical:
                case SourceType.LambertianSurfaceEmittingTube:
                case SourceType.IsotropicCuboidal:
                case SourceType.CustomCubiodal:
                case SourceType.IsotropicEllipsoidal:
                case SourceType.CustomEllipsoidal:
                default: 
                    throw new NotImplementedException(
                        "Problem generating ISource instance. Check that SourceInput, si, has a matching ISource definition.");
            }
        }
    }
}
