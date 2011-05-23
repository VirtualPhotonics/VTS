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
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative

        #region Constructors

       /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Line Source with a specified length, and
        /// source profile (Flat/Gaussian)
       /// </summary>
       /// <param name="thetaConvOrDiv"></param>
       /// <param name="lineLength"></param>
       /// <param name="sourceProfile"></param>
       /// <param name="translationFromOrigin"></param>
       /// <param name="rotationFromInwardNormal"></param>
       /// <param name="rotationOfPrincipalSourceAxis"></param>
        public DirectionalLineSource(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles rotationFromInwardNormal =null,
            ThreeAxisRotation rotationOfPrincipalSourceAxis = null)
                : base(
                    lineLength,
                    sourceProfile,
                    translationFromOrigin,
                    rotationFromInwardNormal,
                    rotationOfPrincipalSourceAxis)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultTranslationFromOrigin;
            if (rotationFromInwardNormal == null)
                rotationFromInwardNormal = SourceDefaults.DefaultRoationFromInwardNormal;
            if (rotationOfPrincipalSourceAxis == null)
                rotationOfPrincipalSourceAxis = SourceDefaults.DefaultRotationOfPrincipalSourceAxis;
        }
                


        #endregion

        //Converging / diveriging or collimated line source
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            //Calculate polar angle           
            var azimuthalAngleEmissionRange = new DoubleRange(0.0, 2 * Math.PI);
            var polarAngle = 0.0;    //for collimated line source

            // sign is negative for diverging and positive positive for converging 
            if (_thetaConvOrDiv != 0.0)            
            {               
                var height = 0.5 * _lineLength / Math.Tan(_thetaConvOrDiv);               
                polarAngle = Math.Atan(finalPosition.X / height);
            }

            //Get final direction
            Direction finalDirection = SourceToolbox.GetDirectionForGiven2DPositionAndPolarAngle(polarAngle, finalPosition);

            return finalDirection;
        }
    }
}
