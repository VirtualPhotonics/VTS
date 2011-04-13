using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class DirectionalLineSource : LineSourceBase
    {
        private double _thetaConvOrDiv;

        public DirectionalLineSource(
            double thetaConvOrDiv, // can be negative (diverging) or positive (converging) ?? 
            double lineLength,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
                : base(
                    lineLength,
                    sourceProfile,
                    translationFromOrigin,
                    rotationFromInwardNormal,
                    rotationOfPrincipalSourceAxis)
        {
            _thetaConvOrDiv = thetaConvOrDiv;// SourceToolbox.NAToPolarAngle(numericalAperture);
        }

        protected override Direction GetFinalDirection(Position finalPosition)
        {
            //Calculate polar angle
            var azimuthalAngleEmissionRange = new DoubleRange(0.0, 2 * Math.PI);
            var height = 0.5 * _lineLength * Math.Tan(_thetaConvOrDiv);
            var polarAngle = Math.Atan(finalPosition.X / height);

            //Sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomAzimuthalAngle(polarAngle, azimuthalAngleEmissionRange, Rng);

            return finalDirection;
        }
    }
}
