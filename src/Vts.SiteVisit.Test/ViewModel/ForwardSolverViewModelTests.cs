using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NUnit.Framework;

namespace Vts.SiteVisit.Test.ViewModel
{
    [TestFixture]
    public class ForwardSolverViewModelTests
    {
        private Vts.SiteVisit.ViewModel.ForwardSolverViewModel _vm;

        public ForwardSolverViewModelTests()
        {
            _vm = new Vts.SiteVisit.ViewModel.ForwardSolverViewModel();
        }

        [Test]
        public void Can_Execute_RofRho() { TestCanExecuteForwardSolver(SolutionDomainType.RofRho); }
        [Test]
        public void Can_Execute_RofRhoAndT() { TestCanExecuteForwardSolver(SolutionDomainType.RofRhoAndT); }
        [Test]
        public void Can_Execute_RofRhoAndFt() { TestCanExecuteForwardSolver(SolutionDomainType.RofRhoAndFt); }
        [Test]
        public void Can_Execute_RofFx() { TestCanExecuteForwardSolver(SolutionDomainType.RofFx); }
        [Test]
        public void Can_Execute_RofFxAndT() { TestCanExecuteForwardSolver(SolutionDomainType.RofFxAndT); }
        [Test]
        public void Can_Execute_RofFxAndFt() { TestCanExecuteForwardSolver(SolutionDomainType.RofFxAndFt); }

        private void TestCanExecuteForwardSolver(SolutionDomainType solutionDomainType)
        {
            var possibleSolverCombinations =
                from solverType in EnumHelper.GetValues<ForwardSolverType>() // for each solver type
                from analysisType in EnumHelper.GetValues<ForwardAnalysisType>() // for each analysis type (R, dR/dIV, etc...)
                from independentVariableAxis in solutionDomainType.GetIndependentVariableAxes() // for each independent variable axis (extension method in Extensions folder)
                select ExecuteForwardSolver(solverType, analysisType, solutionDomainType, independentVariableAxis);
            
            // test that the execution returns data points (not testing the validity of the data)
            foreach (var points in possibleSolverCombinations)
            {
                Assert.IsTrue(points.Count() >= 1);
            }
        }

        /// <summary>
        /// Sets all necessary values for the ForwardSolverViewModel and executes forward/analysis query
        /// </summary>
        /// <param name="forwardSolverType"></param>
        /// <param name="forwardAnalysisType"></param>
        /// <param name="solutionDomainType"></param>
        /// <param name="independentVariableAxis"></param>
        /// <returns></returns>
        private IEnumerable<Point> ExecuteForwardSolver(
            ForwardSolverType forwardSolverType, 
            ForwardAnalysisType forwardAnalysisType, 
            SolutionDomainType solutionDomainType,
            IndependentVariableAxis independentVariableAxis)
        {
            _vm.ForwardSolverTypeOptionVM.SelectedValue = forwardSolverType;
            _vm.SolutionDomainTypeOptionVM.SelectedValue = solutionDomainType;
            _vm.SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue = independentVariableAxis;
            _vm.ForwardAnalysisTypeOptionVM.SelectedValue = forwardAnalysisType;

            return _vm.ExecuteForwardSolver();
        }
    }
}
