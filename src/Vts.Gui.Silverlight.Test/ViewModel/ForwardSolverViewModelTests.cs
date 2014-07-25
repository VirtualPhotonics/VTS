using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using Vts.Gui.Silverlight.Model;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.Test.ViewModel
{
    [TestFixture]
    public class ForwardSolverViewModelTests
    {
        private ForwardSolverViewModel _vm;

        public ForwardSolverViewModelTests()
        {
            _vm = new ForwardSolverViewModel();
        }

        [Test]
        [Ignore]
        public void Can_Execute_ROfRho() { TestCanExecuteForwardSolver(SolutionDomainType.ROfRho); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfRhoAndTime() { TestCanExecuteForwardSolver(SolutionDomainType.ROfRhoAndTime); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfRhoAndFt() { TestCanExecuteForwardSolver(SolutionDomainType.ROfRhoAndFt); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfFx() { TestCanExecuteForwardSolver(SolutionDomainType.ROfFx); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfFxAndTime() { TestCanExecuteForwardSolver(SolutionDomainType.ROfFxAndTime); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfFxAndFt() { TestCanExecuteForwardSolver(SolutionDomainType.ROfFxAndFt); }

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
        private IDataPoint[] ExecuteForwardSolver(
            ForwardSolverType forwardSolverType, 
            ForwardAnalysisType forwardAnalysisType, 
            SolutionDomainType solutionDomainType,
            IndependentVariableAxis independentVariableAxis)
        {
            _vm.ForwardSolverTypeOptionVM.SelectedValue = forwardSolverType;
            _vm.SolutionDomainTypeOptionVM.SelectedValue = solutionDomainType;
            _vm.SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue = independentVariableAxis;
            _vm.ForwardAnalysisTypeOptionVM.SelectedValue = forwardAnalysisType;

            return _vm.ExecuteForwardSolver().First();
        }
    }
}
