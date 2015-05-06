using System.Linq;
using System.Windows;
using NUnit.Framework;
using Vts.Gui.Silverlight.Model;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.Test.ViewModel
{
    [TestFixture]
    public class InverseSolverViewModelTests
    {
        private InverseSolverViewModel _vm;

        public InverseSolverViewModelTests()
        {
            _vm = new InverseSolverViewModel();
        }

        [Test]
        [Ignore]
        public void Can_Execute_ROfRho_Inverse() { TestCanExecuteInverseSolver(SolutionDomainType.ROfRho); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfRhoAndT_Inverse() { TestCanExecuteInverseSolver(SolutionDomainType.ROfRhoAndTime); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfRhoAndFt_Inverse() { TestCanExecuteInverseSolver(SolutionDomainType.ROfRhoAndFt); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfFx_Inverse() { TestCanExecuteInverseSolver(SolutionDomainType.ROfFx); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfFxAndT_Inverse() { TestCanExecuteInverseSolver(SolutionDomainType.ROfFxAndTime); }
        [Test]
        [Ignore]
        public void Can_Execute_ROfFxAndFt_Inverse() { TestCanExecuteInverseSolver(SolutionDomainType.ROfFxAndFt); }

        private void TestCanExecuteInverseSolver(SolutionDomainType solutionDomainType)
        {
            var possibleSolverCombinations =
                from measuredForwardSolverType in EnumHelper.GetValues<ForwardSolverType>() // for each solver type as the measured solver
                from inverseForwardSolverType in EnumHelper.GetValues<ForwardSolverType>() // for each solver type as the inverse solver
                from independentVariableAxis in solutionDomainType.GetIndependentVariableAxes() // for each independent variable axis (extension method in Extensions folder)
                from fitType in EnumHelper.GetValues<InverseFitType>() // for each kind of fit type
                select 
                    ExecuteInverseSolver(
                        measuredForwardSolverType,
                        inverseForwardSolverType,
                        solutionDomainType, 
                        independentVariableAxis,
                        fitType);

            // test that the execution returns data points (not testing the validity of the data)
            foreach (var points in possibleSolverCombinations)
            {
                Assert.IsTrue(points.Count() >= 1);
            }
        }

        /// <summary>
        /// Sets all necessary values for the InverseSolverViewModel and executes the optimization problem
        /// </summary>
        /// <param name="measuredForwardSolverType"></param>
        /// <param name="inverseForwardSolverType"></param>
        /// <param name="solutionDomainType"></param>
        /// <param name="independentVariableAxis"></param>
        /// <param name="inverseFitType"></param>
        /// <returns></returns>
        private IDataPoint[][] ExecuteInverseSolver(
            ForwardSolverType measuredForwardSolverType, 
            ForwardSolverType inverseForwardSolverType, 
            SolutionDomainType solutionDomainType, 
            IndependentVariableAxis independentVariableAxis, 
            InverseFitType inverseFitType)
        {
            _vm.MeasuredForwardSolverTypeOptionVM.SelectedValue = measuredForwardSolverType;
            _vm.InverseForwardSolverTypeOptionVM.SelectedValue = inverseForwardSolverType;
            _vm.SolutionDomainTypeOptionVM.SelectedValue = solutionDomainType;
            _vm.SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue = independentVariableAxis;
            _vm.InverseFitTypeOptionVM.SelectedValue = inverseFitType;

            var fitResults = _vm.SolveInverse();

            return fitResults.FitDataPoints;
        }
    }
}
