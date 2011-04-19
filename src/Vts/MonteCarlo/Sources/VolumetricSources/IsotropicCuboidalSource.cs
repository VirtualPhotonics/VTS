using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class IsotropicCuboidalSource : CuboidalSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of  Isotropic Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// translation, and source axis rotation
        /// </summary>
        /// <param name="cubeLengthX">Length of the cuboid</param>
        /// <param name="cubeWidthY">Width of the cuboid</param>
        /// <param name="cubeHeightZ">Height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian(1D/2D/3D)}</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="rotationOfPrincipalSourceAxis">Source rotation</param>
        public IsotropicCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                translationFromOrigin,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Isotropic Cuboidal Source with a given source profile (Flat/Gaussian) and translation
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        public IsotropicCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Position translationFromOrigin)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                translationFromOrigin,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }


        /// <summary>
        /// Returns an instance of  Isotropic Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// and source axis rotation
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public IsotropicCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                new Position(0, 0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of  Isotropic Cuboidal Source with a given source profile (Flat/Gaussian).
        /// </summary>
        /// <param name="cubeLengthX"></param>
        /// <param name="cubeWidthY"></param>
        /// <param name="cubeHeightZ"></param>
        /// <param name="sourceProfile"></param>
        public IsotropicCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                new Position(0, 0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

        #endregion

        //Isotropic Cuboidal Source
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);
        }
    }

}
