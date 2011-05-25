using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class IsotropicVolumetricCuboidalSourceInput : ISourceInput
    {
        // this handles isotropic cuboidal (volumetric)
        public IsotropicVolumetricCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin)
        {
            SourceType = SourceType.CustomVolumetricCubiodal;
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
        }

        public IsotropicVolumetricCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                SourceDefaults.DefaultPosition) { }

        public SourceType SourceType { get; set; }
        public double CubeLengthX { get; set; }
        public double CubeWidthY { get; set; }
        public double CubeHeightZ { get; set; }
        public ISourceProfile SourceProfile { get; set; }      
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
    }
}