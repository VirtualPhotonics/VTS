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
    public class LambertianSurfaceEmittingTubeSource : SurfaceEmittingTubeSourceBase
    {
        #region Constructors

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source with source axis rotation and translation
        /// </summary>
        /// <param name="tubeRadius"></param>
        /// <param name="tubeHeightZ"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LambertianSurfaceEmittingTubeSource(
            double tubeRadius,
            double tubeHeightZ,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : base(
            tubeRadius,
            tubeHeightZ,
            translationFromOrigin,
            rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source with source axis rotation
        /// </summary>
        /// <param name="tubeRadius"></param>
        /// <param name="tubeHeightZ"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LambertianSurfaceEmittingTubeSource(
            double tubeRadius,
            double tubeHeightZ,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
            tubeRadius,
            tubeHeightZ,            
            new Position(0, 0, 0),
            rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source with translation
        /// </summary>
        /// <param name="tubeRadius"></param>
        /// <param name="tubeHeightZ"></param>
        /// <param name="translationFromOrigin"></param>
        public LambertianSurfaceEmittingTubeSource(
            double tubeRadius,
            double tubeHeightZ,
            Position translationFromOrigin)
            : this(
            tubeRadius,
            tubeHeightZ,
            translationFromOrigin,            
            new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting tube Source
        /// </summary>
        /// <param name="tubeRadius"></param>
        /// <param name="tubeHeightZ"></param>
        public LambertianSurfaceEmittingTubeSource(
            double tubeRadius,
            double tubeHeightZ)
            : this(
            tubeRadius,
            tubeHeightZ,
            new Position(0, 0, 0),
            new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }


        #endregion
    }

}
