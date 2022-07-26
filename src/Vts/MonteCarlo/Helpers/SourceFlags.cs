namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Implements SourceFlags class
    /// </summary>
    public class SourceFlags
    {

        /// <summary>
        /// Provide on/off flags for translation, rotation from inward normal and rotation of Principal source axis
        /// </summary>
        /// <param name="translationFromOriginFlag">The translation from origin</param>
        /// <param name="beamRotationFromInwardNormalFlag">The rotation from inward normal</param>
        /// <param name="rotationOfPrincipalSourceAxisFlag">The source axis rotation</param>
        public SourceFlags(bool rotationOfPrincipalSourceAxisFlag, bool translationFromOriginFlag, bool beamRotationFromInwardNormalFlag)
        {
            RotationOfPrincipalSourceAxisFlag = rotationOfPrincipalSourceAxisFlag; 
            TranslationFromOriginFlag = translationFromOriginFlag; 
            BeamRotationFromInwardNormalFlag = beamRotationFromInwardNormalFlag;
        }

        /// <summary>
        /// No translation, no inward normal rotation and no principal axis rotation
        /// </summary>
        public SourceFlags()
            : this(false, false, false)
        {
        }
        /// <summary>
        /// boolean flag indicating whether translation from origin required
        /// </summary>
        public bool TranslationFromOriginFlag { get; set; }
        /// <summary>
        /// boolean flag indicating whether beam rotation from inward normal required
        /// </summary>
        public bool BeamRotationFromInwardNormalFlag { get; set; }
        /// <summary>
        /// boolean flag indicating whether rotation of principal source axis required
        /// </summary>
        public bool RotationOfPrincipalSourceAxisFlag { get; set; }

        /// <summary>
        /// method to clone class
        /// </summary>
        /// <returns>SourceFlags clone</returns>
        public SourceFlags Clone()
        {
            return new SourceFlags(RotationOfPrincipalSourceAxisFlag, TranslationFromOriginFlag, BeamRotationFromInwardNormalFlag);
        }


    }
}
  