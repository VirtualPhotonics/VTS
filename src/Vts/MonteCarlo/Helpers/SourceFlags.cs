using System.IO;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Implements SourceFlags class
    /// </summary>
    public class SourceFlags
    {
        private bool _translationFromOriginFlag;
        private bool _beamRotationFromInwardNormalFlag;
        private bool _rotationOfPrincipalSourceAxisFlag;
       
        /// <summary>
        /// Provide on/off flags for translation, rotation from inward normal and rotation of Principal source axis
        /// </summary>
        /// <param name="translationFromOriginFlag">The translation from origin</param>
        /// <param name="beamRotationFromInwardNormalFlag">The rotation from inward normal</param>
        /// <param name="rotationOfPrincipalSourceAxisFlag">The source axis rotation</param>
        public SourceFlags(bool rotationOfPrincipalSourceAxisFlag, bool translationFromOriginFlag, bool beamRotationFromInwardNormalFlag)
        {
            _rotationOfPrincipalSourceAxisFlag = rotationOfPrincipalSourceAxisFlag; 
            _translationFromOriginFlag = translationFromOriginFlag; 
            _beamRotationFromInwardNormalFlag = beamRotationFromInwardNormalFlag;
        }

        /// <summary>
        /// No translation, no inward normal rotation and no principal axis rotation
        /// </summary>
        public SourceFlags()
            : this(false, false, false)
        {
        }

        public bool TranslationFromOriginFlag { get { return _translationFromOriginFlag; } set { _translationFromOriginFlag = value; } }
        public bool BeamRotationFromInwardNormalFlag { get { return _beamRotationFromInwardNormalFlag; } set { _beamRotationFromInwardNormalFlag = value; } }
        public bool RotationOfPrincipalSourceAxisFlag { get { return _rotationOfPrincipalSourceAxisFlag; } set { _rotationOfPrincipalSourceAxisFlag = value; } }
        

        public SourceFlags Clone()
        {
            return new SourceFlags(RotationOfPrincipalSourceAxisFlag, TranslationFromOriginFlag, BeamRotationFromInwardNormalFlag);
        }


    }
}
  