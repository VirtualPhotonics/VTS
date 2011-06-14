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
    public class DirectionalLineSource : LineSourceBase
    {
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative  collimated:zero

       
       /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Line Source with a specified length, and
        /// source profile (Flat/Gaussian), new source axis direction, translation, and  inward normal ray rotation
       /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle</param>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Ray rotation from inward normal</param>
        public DirectionalLineSource(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal =null)
                : base(
                    lineLength,
                    sourceProfile,
                    newDirectionOfPrincipalSourceAxis,
                    translationFromOrigin,
                    beamRotationFromInwardNormal)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone();            
        }
                


        //Converging / diveriging or collimated line source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            if (_lineLength == 0.0)
                return (SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                            Rng));
            else
            {
                //Calculate polar angle                      
                var polarAngle = 0.0;    //for collimated line source

                // sign is negative for diverging and positive positive for converging 
                if (_thetaConvOrDiv != 0.0)
                {
                    var height = 0.5 * _lineLength / Math.Tan(_thetaConvOrDiv);
                    polarAngle = Math.Atan(finalPosition.X / height);
                }
                return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, finalPosition));
            }
        }
    }
}
