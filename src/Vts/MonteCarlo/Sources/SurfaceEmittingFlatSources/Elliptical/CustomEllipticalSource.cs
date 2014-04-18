using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements CustomEllipticalSource with a and b parameter, source profile, polar
    /// angle range, azimuthal angle range, direction, position and inward normal beam 
    /// rotation, and initial tissue region index.
    /// </summary>
    public class CustomEllipticalSource : EllipticalSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        /// <summary>
        /// Returns an instance of  Custom Elliptical Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomEllipticalSource(
            double aParameter,
            double bParameter,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                aParameter,
                bParameter,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone();
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
