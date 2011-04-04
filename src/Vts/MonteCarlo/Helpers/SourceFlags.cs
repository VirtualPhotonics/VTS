using System.IO;

namespace Vts.MonteCarlo.Helpers
{
    public class SourceFlags
    {
        private bool _translationFromOriginFlag;
        private bool _rotationFromInwardNormalFlag;
        private bool _rotationOfPrincipalSourceAxisFlag;
       
        /// <summary>
        /// Provide on/off flags for translation, rotation from inward normal and rotation of Principal source axis
        /// </summary>
        /// <param name="translationFromOriginFlag">The translation from origin</param>
        /// <param name="rotationFromInwardNormalFlag">The rotation from inward normal</param>
        /// <param name="rotationOfPrincipalSourceAxisFlag">The source axis rotation</param>
        public SourceFlags(bool translationFromOriginFlag, bool rotationFromInwardNormalFlag, bool rotationOfPrincipalSourceAxisFlag)
        {
            _translationFromOriginFlag = translationFromOriginFlag; 
            _rotationFromInwardNormalFlag = rotationFromInwardNormalFlag;
            _rotationOfPrincipalSourceAxisFlag = rotationOfPrincipalSourceAxisFlag;            
        }

        /// <summary>
        /// No translation, no inward normal rotation and no principal axis rotation
        /// </summary>
        public SourceFlags()
            : this(false, false, false)
        {
        }

        public bool TranslationFromOriginFlag { get { return _translationFromOriginFlag; } set { _translationFromOriginFlag = value; } }
        public bool RotationFromInwardNormalFlag { get { return _rotationFromInwardNormalFlag; } set { _rotationFromInwardNormalFlag = value; } }
        public bool RotationOfPrincipalSourceAxisFlag { get { return _rotationOfPrincipalSourceAxisFlag; } set { _rotationOfPrincipalSourceAxisFlag = value; } }
        

        public SourceFlags Clone()
        {
            return new SourceFlags(TranslationFromOriginFlag, RotationFromInwardNormalFlag, RotationOfPrincipalSourceAxisFlag);
        }


    }
}
  