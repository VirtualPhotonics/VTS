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
    public class DirectionalCircularSource : CircularSourceBase
    {
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative  collimated:zero

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Circular Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        public DirectionalCircularSource(
            double thetaConvOrDiv,            
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile, 
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null)
            : base(
                outerRadius,
                innerRadius,
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
        

        //Converging, diveriging or collimated Circular Source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            if (_outerRadius == 0.0)
                return (SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                            Rng));
            else
            {
                //Calculate polar angle                      
                var polarAngle = 0.0;    //for collimated Circular source

                // sign is negative for diverging and positive positive for converging 
                if (_thetaConvOrDiv != 0.0)
                {
                    var height = _outerRadius / Math.Tan(_thetaConvOrDiv);
                    polarAngle = Math.Atan(Math.Sqrt(finalPosition.X * finalPosition.X + finalPosition.Y * finalPosition.Y) / height);
                }
                return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, finalPosition));
            }
        }
    }
}
