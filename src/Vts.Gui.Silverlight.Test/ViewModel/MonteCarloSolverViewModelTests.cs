
using NUnit.Framework;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.Test.ViewModel
{
    [TestFixture]
    public class MonteCarloSolverViewModelTests
    {
        private MonteCarloSolverViewModel _vm;

        public MonteCarloSolverViewModelTests()
        {
            _vm = new MonteCarloSolverViewModel();
        }

        [Test]
        public void download_prototype_infiles_works_correctly()
        {
            //_vm.DownloadDefaultSimulationInputCommand.Execute(null);
        }

    }
}
