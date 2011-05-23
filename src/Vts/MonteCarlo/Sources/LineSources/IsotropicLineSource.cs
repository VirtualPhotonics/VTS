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

        #region Constructors

        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian),
        ///  polar and azimuthal angle range, translation, inward normal rotation, and source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public IsotropicLineSource(
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
        }        

        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, and inward normal rotation.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                lineLength,
                sourceProfile,
                translationFromOrigin,
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
        }

        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation,  and source axis rotation.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,            
            ISourceProfile sourceProfile,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                sourceProfile,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
        }


        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, and translation.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,            
            ISourceProfile sourceProfile,
            Position translationFromOrigin)
            : this(
                lineLength,
                sourceProfile,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
        }

        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range,  inward normal rotation, and source axis rotation.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,           
            ISourceProfile sourceProfile,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                sourceProfile,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                rotationOfPrincipalSourceAxis)
        {
        }

        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range and inward normal rotation.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            PolarAzimuthalAngles rotationFromInwardNormal)
            : this(
                lineLength,
                sourceProfile,
                new Position(0, 0, 0),
                rotationFromInwardNormal,
                new ThreeAxisRotation(0, 0, 0))
        {
        }


        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range and source axis rotation.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,            
            ISourceProfile sourceProfile,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                sourceProfile,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
        }

        /// <summary>
        /// Returns an instance of isotropic Line Source with a specified length, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range.
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        public IsotropicLineSource(
            double lineLength,
            ISourceProfile sourceProfile)
            : this(                
                lineLength,
                sourceProfile,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
        }
        
        
        #endregion

        //Isotropic line source
        protected override Direction GetFinalDirection(Position finalPosition)
        {                   
            var azimuthalAngleEmissionRange = new DoubleRange(0.0, 2 * Math.PI);
            var polarAngleEmissionRange = new DoubleRange(0.0, Math.PI);

            //Sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(polarAngleEmissionRange, azimuthalAngleEmissionRange, Rng);

            return finalDirection;
        }
    }
}