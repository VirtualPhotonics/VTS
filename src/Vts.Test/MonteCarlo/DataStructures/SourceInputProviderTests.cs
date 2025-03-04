using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SourceInputProviderTests
    {
        IList<ISourceInput> sourceInputList;

        /// <summary>
        /// Run method to generate all sources
        /// </summary>
        [OneTimeSetUp]
        public void generate_all_sources()
        {
            sourceInputList = SourceInputProvider.GenerateAllSourceInputs();
        }
        /// <summary>
        /// Test verifies DirectionalPoint source input generation
        /// </summary>
        [Test]
        public void verify_DirectionalPoint_source_input_is_correct()
        {
            var sourceInput = (DirectionalPointSourceInput)sourceInputList.First(
                    si => si.SourceType == "DirectionalPoint");
            Assert.That(sourceInput != null, Is.True);
            Assert.That(Math.Abs(sourceInput.Direction.Uz - 1) < 0.000001, Is.True);
        }

        /// <summary>
        /// Test verifies CustomLine source input generation
        /// </summary>
        [Test]
        public void verify_CustomLine_source_input_is_correct()
        {
            var sourceInput = (CustomLineSourceInput) sourceInputList.First(
                si => si.SourceType == "CustomLine");
            Assert.That(sourceInput, Is.Not.Null);
            Assert.That(Math.Abs(sourceInput.NewDirectionOfPrincipalSourceAxis.Uz - 1), Is.LessThan(0.000001));
        }
        /// <summary>
        /// Test verifies Gaussian CustomCircular source input generation
        /// </summary>
        [Test]
        public void verify_GaussianCustomCircular_source_input_is_correct()
        {
            var sourceInput = (CustomCircularSourceInput)sourceInputList.First(
                si => si.SourceType == "CustomCircular");
            Assert.That(sourceInput, Is.Not.Null);
            Assert.That(Math.Abs(sourceInput.NewDirectionOfPrincipalSourceAxis.Uz - 1), Is.LessThan(0.000001));
            Assert.That(sourceInput.SourceProfile is GaussianSourceProfile, Is.True);
        }
    }
}
