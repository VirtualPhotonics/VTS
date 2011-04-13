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
    public class CustomLineSource : LineSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        #region Constructors
        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with a specified translation, inward normal rotation, and source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="sourceProfile"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
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
            _lineLength = lineLength;
            _sourceProfile = sourceProfile;
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            //_translationFromOrigin = translationFromOrigin.Clone();
            //_rotationFromInwardNormal = rotationFromInwardNormal.Clone();
            //_rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            //_rotationAndTranslationFlags = new SourceFlags(true, true, true);            
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with a specified translation and inward normal rotation, but without source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardnormal"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardnormal)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                rotationFromInwardnormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, true, false);
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with a specified translation and source axis rotation, but without inward normal rotation 
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis
            )
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with a specified translation but without inward normal rotation or source axis rotation 
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with an inward normal rotation and source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationFromInwardnormal"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            PolarAzimuthalAngles rotationFromInwardnormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                rotationFromInwardnormal,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with an inward normal rotation, but without source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationFromInwardnormal"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            PolarAzimuthalAngles rotationFromInwardnormal)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                rotationFromInwardnormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with a source axis rotation, but without inward normal rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Custom Gaussian Line Source with no inward normal rotation or source axis rotation  
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }
        #endregion
        
        // In CustomLineSource
        protected override Direction GetFinalDirection(Position finalPosition)
        {
            return SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
