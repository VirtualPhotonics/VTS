using System;
using Vts.Fem.MGRTE._2D;
using GalaSoft.MvvmLight.Command;
using Vts.SiteVisit.Input;

namespace Vts.SiteVisit.ViewModel
{
    public class FemSolverViewModel : BindableObject
    {
        private Parameters _parameters;

        public FemSolverViewModel(Parameters parameters)
        {
            Commands.FEM_ExecuteFemSolver.Executed += FEM_ExecuteFemSolver_Executed;

            ExecuteFemSolverCommand = new RelayCommand(() => FEM_ExecuteFemSolver_Executed(null, null));
    
        }


        public FemSolverViewModel()
            : this(new Parameters
            {
                 //Alevel = ...
            })
        {
        }


        public RelayCommand ExecuteFemSolverCommand { get; private set; }

        public Parameters Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
            }
        }

        private void FEM_ExecuteFemSolver_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            ExecuteSolver();
        }

        private void ExecuteSolver()
        {

            // Purpose: this is the main function for RTE_2D.
            // Note: we assume the spatial domain has "nt" intervals,
            //       starting from "0" to "nt-1" with increasing "x" coordinate;
            //       the top boundary with bigger "x" is labeled as "1" and the bottom as "0";
            //       in each interval, the node with the smaller "x" is labeled as "0" and the node with the bigger "x" is labeled as "1".
            SolverMGRTE.ExecuteMGRTE(_parameters);
        }
    }
}
