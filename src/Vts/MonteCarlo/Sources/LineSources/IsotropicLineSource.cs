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
    public class IsotropicLineSource : LineSourceBase
    {     
        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian),
        /// polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Ray rotation from inward normal</param>
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

        
        //Isotropic line source
        protected override Direction GetFinalDirection(Position finalPosition)
        {                   
            var azimuthalAngleEmissionRange = SourceDefaults.DefaultAzimuthalAngleRange.Clone();
            var polarAngleEmissionRange = SourceDefaults.DefaultFullPolarAngleRange.Clone();

            //Sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(polarAngleEmissionRange, azimuthalAngleEmissionRange, Rng);

            return finalDirection;
        }
    }
}