using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements CustomVolumetricCuboidalSource with length, width, height, source 
    /// profile, polar angle range,azimuthal angle range, direction, position and 
    /// initial tissue region index.
    /// </summary>
    public class CustomVolumetricCuboidalSource : VolumetricCuboidalSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;       

        /// <summary>
        /// Returns an instance of  Custom Cuboidal Source with a given source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, new source axis direction, and translation.
        /// </summary>
        /// <param name="cubeLengthX">The length of the cuboid</param>
        /// <param name="cubeWidthY">The width of the cuboid</param>
        /// <param name="cubeHeightZ">The height of the cuboid</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomVolumetricCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                initialTissueRegionIndex)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();

            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
        }

        /// <summary>
        /// Returns direction
        /// </summary>
        /// <returns>new direction</returns>
        protected override Direction GetFinalDirection()
        {
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
