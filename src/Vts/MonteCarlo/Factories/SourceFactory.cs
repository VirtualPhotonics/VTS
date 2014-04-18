using System;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class SourceFactory
    {
        /// <summary>
        /// Method to instantiate the correct source given ISourceInput
        /// </summary>
        /// <param name="input">ISourceInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="rng">random number generator instance</param>
        /// <returns>ISource</returns>
        public static ISource GetSource(ISourceInput input, ITissue tissue, Random rng)
        {
            switch (input.SourceType)
            {                
                case SourceType.DirectionalPoint:
                    var dpInput = (DirectionalPointSourceInput)input;
                    return new DirectionalPointSource(
                        dpInput.Direction,
                        dpInput.PointLocation,
                        dpInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.IsotropicPoint:
                    var ipInput = (IsotropicPointSourceInput)input;
                    return new IsotropicPointSource(
                        ipInput.PointLocation,
                        ipInput.InitialTissueRegionIndex) { Rng = rng }; 

                case SourceType.CustomPoint:
                    var cpInput = (CustomPointSourceInput)input;
                    return new CustomPointSource(
                        cpInput.PolarAngleEmissionRange,
                        cpInput.AzimuthalAngleEmissionRange,
                        cpInput.Direction,
                        cpInput.PointLocation,
                        cpInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.DirectionalLine:
                    var dlInput = (DirectionalLineSourceInput)input;
                    return new DirectionalLineSource(
                        dlInput.ThetaConvOrDiv,
                        dlInput.LineLength,
                        dlInput.SourceProfile,
                        dlInput.NewDirectionOfPrincipalSourceAxis,
                        dlInput.TranslationFromOrigin,
                        dlInput.BeamRotationFromInwardNormal,
                        dlInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.IsotropicLine:
                    var ilInput = (IsotropicLineSourceInput)input;
                    return new IsotropicLineSource(
                        ilInput.LineLength,
                        ilInput.SourceProfile,
                        ilInput.NewDirectionOfPrincipalSourceAxis,
                        ilInput.TranslationFromOrigin,
                        ilInput.BeamRotationFromInwardNormal) { Rng = rng };                   

                case SourceType.CustomLine:
                    var clInput = (CustomLineSourceInput)input;
                    return new CustomLineSource(
                        clInput.LineLength,
                        clInput.SourceProfile,
                        clInput.PolarAngleEmissionRange,
                        clInput.AzimuthalAngleEmissionRange,
                        clInput.NewDirectionOfPrincipalSourceAxis,
                        clInput.TranslationFromOrigin,
                        clInput.BeamRotationFromInwardNormal,
                        clInput.InitialTissueRegionIndex) { Rng = rng };  

                case SourceType.DirectionalCircular:
                    var dcInput = (DirectionalCircularSourceInput)input;
                    return new DirectionalCircularSource(
                        dcInput.ThetaConvOrDiv,
                        dcInput.OuterRadius,
                        dcInput.InnerRadius,
                        dcInput.SourceProfile,
                        dcInput.NewDirectionOfPrincipalSourceAxis,
                        dcInput.TranslationFromOrigin,
                        dcInput.BeamRotationFromInwardNormal,
                        dcInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.CustomCircular:
                    var ccInput = (CustomCircularSourceInput)input;
                    return new CustomCircularSource(
                        ccInput.OuterRadius,
                        ccInput.InnerRadius,
                        ccInput.SourceProfile,
                        ccInput.PolarAngleEmissionRange,
                        ccInput.AzimuthalAngleEmissionRange,
                        ccInput.NewDirectionOfPrincipalSourceAxis,
                        ccInput.TranslationFromOrigin,
                        ccInput.BeamRotationFromInwardNormal,
                        ccInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.LambertianSurfaceEmittingCubiodal:
                    var lsecInput = (LambertianSurfaceEmittingCuboidalSourceInput)input;
                    return new LambertianSurfaceEmittingCuboidalSource(
                        lsecInput.CubeLengthX,
                        lsecInput.CubeWidthY,
                        lsecInput.CubeHeightZ,
                        lsecInput.SourceProfile,
                        lsecInput.NewDirectionOfPrincipalSourceAxis,
                        lsecInput.TranslationFromOrigin,
                        lsecInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.LambertianSurfaceEmittingCylindricalFiber:
                    var lsecfInput = (LambertianSurfaceEmittingCylindricalFiberSourceInput)input;
                    return new LambertianSurfaceEmittingCylindricalFiberSource(
                        lsecfInput.FiberRadius,
                        lsecfInput.FiberHeightZ,
                        lsecfInput.CurvedSurfaceEfficiency,
                        lsecfInput.BottomSurfaceEfficiency,
                        lsecfInput.NewDirectionOfPrincipalSourceAxis,
                        lsecfInput.TranslationFromOrigin,
                        lsecfInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.DirectionalElliptical:
                    var deInput = (DirectionalEllipticalSourceInput)input;
                    return new DirectionalEllipticalSource(
                        deInput.ThetaConvOrDiv,
                        deInput.AParameter,
                        deInput.BParameter,
                        deInput.SourceProfile,
                        deInput.NewDirectionOfPrincipalSourceAxis,
                        deInput.TranslationFromOrigin,
                        deInput.BeamRotationFromInwardNormal,
                        deInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.CustomElliptical:
                    var ceInput = (CustomEllipticalSourceInput)input;
                    return new CustomEllipticalSource(
                        ceInput.AParameter,
                        ceInput.BParameter,
                        ceInput.SourceProfile,
                        ceInput.PolarAngleEmissionRange,
                        ceInput.AzimuthalAngleEmissionRange,
                        ceInput.NewDirectionOfPrincipalSourceAxis,
                        ceInput.TranslationFromOrigin,
                        ceInput.BeamRotationFromInwardNormal,
                        ceInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.DirectionalRectangular:
                    var drInput = (DirectionalRectangularSourceInput)input;
                    return new DirectionalRectangularSource(
                        drInput.ThetaConvOrDiv,
                        drInput.RectLengthX,
                        drInput.RectWidthY,
                        drInput.SourceProfile,
                        drInput.NewDirectionOfPrincipalSourceAxis,
                        drInput.TranslationFromOrigin,
                        drInput.BeamRotationFromInwardNormal,
                        drInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.CustomRectangular:
                    var crInput = (CustomRectangularSourceInput)input;
                    return new CustomRectangularSource(
                        crInput.RectLengthX,
                        crInput.RectWidthY,
                        crInput.SourceProfile,
                        crInput.PolarAngleEmissionRange,
                        crInput.AzimuthalAngleEmissionRange,
                        crInput.NewDirectionOfPrincipalSourceAxis,
                        crInput.TranslationFromOrigin,
                        crInput.BeamRotationFromInwardNormal,
                        crInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.LambertianSurfaceEmittingSpherical:
                    var lsesInput = (LambertianSurfaceEmittingSphericalSourceInput)input;
                    return new LambertianSurfaceEmittingSphericalSource(
                        lsesInput.Radius,
                        lsesInput.TranslationFromOrigin,
                        lsesInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.CustomSurfaceEmittingSpherical:
                    var csesInput = (CustomSurfaceEmittingSphericalSourceInput)input;
                    return new CustomSurfaceEmittingSphericalSource(
                        csesInput.Radius,
                        csesInput.PolarAngleRangeToDefineSphericalSurface,
                        csesInput.AzimuthalAngleRangeToDefineSphericalSurface,
                        csesInput.NewDirectionOfPrincipalSourceAxis,
                        csesInput.TranslationFromOrigin,
                        csesInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.LambertianSurfaceEmittingTubular:
                    var lsetInput = (LambertianSurfaceEmittingTubularSourceInput)input;
                    return new LambertianSurfaceEmittingTubularSource(
                        lsetInput.TubeRadius,
                        lsetInput.TubeHeightZ,
                        lsetInput.NewDirectionOfPrincipalSourceAxis,
                        lsetInput.TranslationFromOrigin,
                        lsetInput.InitialTissueRegionIndex) { Rng = rng };  

                case SourceType.IsotropicVolumetricCuboidal:
                    var ivcInput = (IsotropicVolumetricCuboidalSourceInput)input;
                    return new IsotropicVolumetricCuboidalSource(
                        ivcInput.CubeLengthX,
                        ivcInput.CubeWidthY,
                        ivcInput.CubeHeightZ,
                        ivcInput.SourceProfile,
                        ivcInput.NewDirectionOfPrincipalSourceAxis,
                        ivcInput.TranslationFromOrigin,
                        ivcInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.CustomVolumetricCubiodal:
                    var cvcInput = (CustomVolumetricCuboidalSourceInput)input;
                    return new CustomVolumetricCuboidalSource(
                        cvcInput.CubeLengthX,
                        cvcInput.CubeWidthY,
                        cvcInput.CubeHeightZ,
                        cvcInput.SourceProfile,
                        cvcInput.PolarAngleEmissionRange,
                        cvcInput.AzimuthalAngleEmissionRange,
                        cvcInput.NewDirectionOfPrincipalSourceAxis,
                        cvcInput.TranslationFromOrigin,
                        cvcInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.IsotropicVolumetricEllipsoidal:
                    var iveInput = (IsotropicVolumetricEllipsoidalSourceInput)input;
                    return new IsotropicVolumetricEllipsoidalSource(
                        iveInput.AParameter,
                        iveInput.BParameter,
                        iveInput.CParameter,
                        iveInput.SourceProfile,
                        iveInput.NewDirectionOfPrincipalSourceAxis,
                        iveInput.TranslationFromOrigin,
                        iveInput.InitialTissueRegionIndex) { Rng = rng };

                case SourceType.CustomVolumetricEllipsoidal:
                    var cveInput = (CustomVolumetricEllipsoidalSourceInput)input;
                    return new CustomVolumetricEllipsoidalSource(
                        cveInput.AParameter,
                        cveInput.BParameter,
                        cveInput.CParameter,
                        cveInput.SourceProfile,
                        cveInput.PolarAngleEmissionRange,
                        cveInput.AzimuthalAngleEmissionRange,
                        cveInput.NewDirectionOfPrincipalSourceAxis,
                        cveInput.TranslationFromOrigin,
                        cveInput.InitialTissueRegionIndex) { Rng = rng };

                default: 
                    throw new NotImplementedException(
                        "Problem generating ISource instance. Check that SourceInput, si, has a matching ISource definition.");
            }
        }
    }
}
