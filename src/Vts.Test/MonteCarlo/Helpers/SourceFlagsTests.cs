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
            Assert.That(sourceFlags, Is.InstanceOf<SourceFlags>());
            // fully defined
            sourceFlags = new SourceFlags()
            {
                RotationOfPrincipalSourceAxisFlag = true,
                TranslationFromOriginFlag = true,
                BeamRotationFromInwardNormalFlag = false
            };
            Assert.That(sourceFlags, Is.InstanceOf<SourceFlags>());
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
            Assert.That(sourceFlagsClone.RotationOfPrincipalSourceAxisFlag, Is.True);
            Assert.That(sourceFlagsClone.TranslationFromOriginFlag, Is.True);
            Assert.That(sourceFlagsClone.BeamRotationFromInwardNormalFlag, Is.False);
        }

    }
}

