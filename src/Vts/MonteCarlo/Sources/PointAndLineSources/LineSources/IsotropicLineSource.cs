using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements IsotropicLineSource with line length, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class IsotropicLineSource : LineSourceBase
    {     
        /// <summary>
        /// Returns an instance of isotropicLineSource with line length, source profile, direction, position, 
        /// inward normal beam rotation and initial tissue region index.
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Ray rotation from inward normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                lineLength,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {            
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {                   
            var azimuthalAngleEmissionRange = SourceDefaults.DefaultAzimuthalAngleRange.Clone();
            var polarAngleEmissionRange = SourceDefaults.DefaultFullPolarAngleRange.Clone();

            //Sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(polarAngleEmissionRange, azimuthalAngleEmissionRange, Rng);

            return finalDirection;
        }
    }
}