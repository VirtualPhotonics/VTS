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
    public class DirectionalRectangularSource : RectangularSourceBase
    {
        private double _thetaConvOrDiv;  //convergence:positive, divergence:negative

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Rectangular Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle</param>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        public DirectionalRectangularSource(
            double thetaConvOrDiv,
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null)
            : base(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone().Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone().Clone();
        }

        
        //Converging / diveriging or collimated rectangular source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            

            if ((_rectLengthX == 0.0) && (_rectWidthY == 0.0))
                return (SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange.Clone().Clone(),
                            Rng));
            else
            {
                //Calculate polar angle                       
                var polarAngle = 0.0;    //for collimated rectangular source
                
                // sign is negative for diverging and positive positive for converging 
                if (_thetaConvOrDiv != 0.0)
                {
                    var height = 0.5 * Math.Sqrt(_rectLengthX * _rectLengthX + _rectWidthY * _rectWidthY) / Math.Tan(_thetaConvOrDiv);
                    polarAngle = Math.Atan(Math.Sqrt(finalPosition.X * finalPosition.X + finalPosition.Y * finalPosition.Y) / height);
                }
                return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, finalPosition));
            }
        }
    }
}
