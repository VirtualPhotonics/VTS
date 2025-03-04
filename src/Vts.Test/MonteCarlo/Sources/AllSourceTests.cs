using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// These tests set up all sources using SourceProvider list and test
    /// SourceInput, CreateSource, and resulting Source 
    /// </summary>
    [TestFixture]
    public class AllSourceTests
    {
        private IEnumerable<ISourceInput> _sourceInputs;
        private List<string> _sourceFiles;

        /// <summary>
        /// setup list of sources to test
        /// </summary>
        [OneTimeSetUp]
        public void Generate_list_of_sources()
        {
            var inputFiles = SourceInputProvider.GenerateAllSourceInputs();
            // generate list of unique detectors from all sample input files
            var sourceInputGrouping = inputFiles.GroupBy(d => d.SourceType).ToList();
            _sourceInputs = sourceInputGrouping.
                Select(x => x.FirstOrDefault());
            _sourceFiles = _sourceInputs.Select(d => d.SourceType).ToList();
        }

        /// <summary>
        /// Tests if source is instantiated using sourceInput.  
        /// </summary>
        [Test]
        public void Verify_source_classes()
        {
            // use factory to instantiate detector with CreateDetector and call Initialize
            var rng = new MersenneTwister(0); 
            foreach (var sourceInput in _sourceInputs)
            {
                // factory generates IDetector using CreateDetector,
                // then calls detector.Initialize method 
                var source = SourceFactory.GetSource(sourceInput, rng);
                Assert.That(source, Is.InstanceOf<ISource>());
            }
        }
    }
}
