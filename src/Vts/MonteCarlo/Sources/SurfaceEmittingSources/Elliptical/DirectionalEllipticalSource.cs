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
    public class DirectionalEllipticalSource : EllipticalSourceBase
    {
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative

       

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Elliptical Source with specified length and width, 
        /// source profile (Flat/Gaussian), polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        public DirectionalEllipticalSource(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null)
            : base(
                aParameter,
                bParameter,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis;
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition;
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal;
        }
                

        //Converging, diveriging or collimated Elliptical Source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            if ((_aParameter == 0.0) && (_bParameter == 0.0))
                return (SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange,
                            Rng));

            else
            {
                //Calculate polar angle                      
                var polarAngle = 0.0;    //for collimated elliptical source

                // sign is negative for diverging and positive positive for converging 
                if (_thetaConvOrDiv != 0.0)
                {
                    var height = _aParameter / Math.Tan(_thetaConvOrDiv);
                    polarAngle = Math.Atan(Math.Sqrt(finalPosition.X * finalPosition.X + finalPosition.Y * finalPosition.Y) / height);
                }
                return (SourceToolbox.GetDirectionForGiven2DPositionAndPolarAngle(polarAngle, finalPosition));
            }
        }
    }
}
