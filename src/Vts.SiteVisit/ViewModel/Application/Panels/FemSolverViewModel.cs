using System;
using System.Linq;
using Vts.FemModeling.MGRTE._2D;
using GalaSoft.MvvmLight.Command;
using Vts.SiteVisit.Input;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.SiteVisit.Model;

namespace Vts.SiteVisit.ViewModel
{
    public class FemSolverViewModel : BindableObject
    {
        private Parameters _parameters;

        public FemSolverViewModel(Parameters parameters)
        {
            _parameters = parameters;

            Commands.FEM_ExecuteFemSolver.Executed += FEM_ExecuteFemSolver_Executed;

            ExecuteFemSolverCommand = new RelayCommand(() => FEM_ExecuteFemSolver_Executed(null, null));    
        }


        public FemSolverViewModel()
            : this(new Parameters
            {
                G = 0.9,
                Index_i = 1.0,
                Index_o = 1.0,
                Alevel = 4,
                Alevel0 = 1,
                Slevel = 3,
                Slevel0 = 0,
                Tol = 1e-4,
                Whichmg = 6,
                Fmg = 1,
                N1 = 3,
                N2 = 3,
                N3 = 1,
                N_max = 100,
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
                OnPropertyChanged("Parameters");
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
            Measurement measurement = SolverMGRTE.ExecuteMGRTE(_parameters);

            // intensity from FEM solution
            var destinationArray = measurement.inten;

            // todo: replace with measurement members to get 1D arrays of x and y spans
            var xs = new double[measurement.xloc.Distinct().Count()];
            var ys = new double[measurement.yloc.Distinct().Count()];

            var meshData = new MapData(destinationArray, xs, ys);

            Commands.Mesh_PlotMap.Execute(meshData);
        }
    }
}
