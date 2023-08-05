using NUnit.Framework;
using System.Threading.Tasks;
using Vts.MonteCarlo.CommandLineApplication;

namespace Vts.MonteCarlo.PostProcessor.Test
{
    [TestFixture]
    public class PostProcessorSetupTests
    {
        [Test]
        public void ReadSimulationInputFromFile_returns_null()
        {
            var result = PostProcessorSetup.ReadPostProcessorInputFromFile("");
            Assert.IsNull(result);
        }

        [Test]
        public void ReadSimulationInputFromFile_throws_FileNotFoundException_returns_null()
        {
            var result = PostProcessorSetup.ReadPostProcessorInputFromFile("dummy.txt");
            Assert.IsNull(result);
        }
    }
}
