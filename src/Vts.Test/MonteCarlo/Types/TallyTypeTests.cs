using System.Linq;
using NUnit.Framework;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class TallyTypeTests
    {
        /// <summary>
        /// Validate detector string properties.
        /// Designed test so that wouldn't have to update each time a new detector added.
        /// Typically when new detector added, it is added to some sample input via
        /// SimulationInputProvider so use that to create list.
        /// </summary>
        [Test]
        public void validate_detector_type_strings()
        {
            // use SimulationInputProvider to determine all detectors
            var listOfSimulationInputs = SimulationInputProvider.GenerateAllSimulationInputs();
            var detectorInputs = listOfSimulationInputs.SelectMany(
                si => si.DetectorInputs).ToList();
            var tallyTypeList = TallyType.BuiltInTypes.ToList();
            var cnt = 0;
            foreach (var tallyType in tallyTypeList)
            {
                Assert.That(tallyType, Is.Not.Null);
                // get rid of pMC tallies because won't be SimulationInputProvider
                if (!tallyType.Substring(1, 1).Equals("M"))
                {
                    Assert.That(detectorInputs.Any(d => d.TallyType == tallyType), Is.True);
                    cnt++;
                }
            }
        }
    }
}

