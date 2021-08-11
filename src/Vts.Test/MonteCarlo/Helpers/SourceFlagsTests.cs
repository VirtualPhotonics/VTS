using System;
using NUnit.Framework;
using Vts.MonteCarlo.Helpers;

namespace Vts.Test.MonteCarlo.Helpers
{
    [TestFixture]
    public class SourceFlagsTests
    {
        /// <summary>
        /// Validate default and fully-defined constructor
        /// </summary>
        [Test]
        public void Validate_constructor_results()
        {
            // default constructor
            var sourceFlags = new SourceFlags();
            Assert.IsInstanceOf<SourceFlags>(sourceFlags);
            // fully defined
            sourceFlags = new SourceFlags()
            {
                RotationOfPrincipalSourceAxisFlag = true,
                TranslationFromOriginFlag = true,
                BeamRotationFromInwardNormalFlag = false
            };
            Assert.IsInstanceOf<SourceFlags>(sourceFlags);
        }
        /// <summary>
        /// Validate Clone method
        /// </summary>
        [Test]
        public void Validate_Clone_results()
        {
            var sourceFlags = new SourceFlags()
            {
                RotationOfPrincipalSourceAxisFlag = true,
                TranslationFromOriginFlag = true,
                BeamRotationFromInwardNormalFlag = false
            };
            var sourceFlagsClone = sourceFlags.Clone();
            Assert.IsTrue(sourceFlagsClone.RotationOfPrincipalSourceAxisFlag);
            Assert.IsTrue(sourceFlagsClone.TranslationFromOriginFlag);
            Assert.IsFalse(sourceFlagsClone.BeamRotationFromInwardNormalFlag);
        }

    }
}

