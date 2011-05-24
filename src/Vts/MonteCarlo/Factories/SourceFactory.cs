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
                case SourceType.DirectionalPoint:
                    var dpInput = (DirectionalPointSourceInput)input;
                    return new DirectionalPointSource(
                        dpInput.EmittingDirection,
                        dpInput.PointLocation);

                case SourceType.IsotropicPoint:
                    var ipInput = (IsotropicPointSourceInput)input;
                    return new IsotropicPointSource(
                        ipInput.PointLocation); 

                case SourceType.CustomPoint:
                    var cpInput = (CustomPointSourceInput)input;
                    return new CustomPointSource(
                        cpInput.PolarAngleEmissionRange,
                        cpInput.AzimuthalAngleEmissionRange,
                        cpInput.EmittingDirection,
                        cpInput.PointLocation);

                case SourceType.DirectionalLine:
                    var dlInput = (DirectionalLineSourceInput)input;
                    return new DirectionalLineSource(
                        dlInput.ThetaConvOrDiv,
                        dlInput.LineLength,
                        dlInput.SourceProfile,
                        dlInput.NewDirectionOfPrincipalSourceAxis,
                        dlInput.TranslationFromOrigin,
                        dlInput.BeamRotationFromInwardNormal);

                case SourceType.IsotropicLine:
                    var ilInput = (IsotropicLineSourceInput)input;
                    return new IsotropicLineSource(
                        ilInput.LineLength,
                        ilInput.SourceProfile,
                        ilInput.NewDirectionOfPrincipalSourceAxis,
                        ilInput.TranslationFromOrigin,
                        ilInput.BeamRotationFromInwardNormal);                   

                case SourceType.CustomLine:
                     var clInput = (CustomLineSourceInput)input;
                    return new CustomLineSource(
                        clInput.LineLength,
                        clInput.SourceProfile,
                        clInput.PolarAngleEmissionRange,
                        clInput.AzimuthalAngleEmissionRange,
                        clInput.NewDirectionOfPrincipalSourceAxis,
                        clInput.TranslationFromOrigin,
                        clInput.BeamRotationFromInwardNormal);  

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
