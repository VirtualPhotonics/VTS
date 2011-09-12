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
        private OpticalPropertyViewModel _opticalPropertyVM;

        public FemSolverViewModel(Parameters parameters)
        {
            _parameters = parameters;

            OpticalPropertyVM = new OpticalPropertyViewModel() { Title = "Tissue Optical Properties:", G = 0.9, N = 1.0, EnableG = true };
            Commands.FEM_ExecuteFemSolver.Executed += FEM_ExecuteFemSolver_Executed;

            ExecuteFemSolverCommand = new RelayCommand(() => FEM_ExecuteFemSolver_Executed(null, null));    
        }


        public FemSolverViewModel()
            : this(new Parameters
            {
                G = 0.9,
                NTissue = 1.0,
                NExt = 1.0,
                AMeshLevel = 3,
                AMeshLevel0 = 1,
                SMeshLevel = 3,
                SMeshLevel0 = 0,
                ConvTol = 1e-4,
                MgMethod = 6,
                FullMg = 1,
                NPreIteration = 3,
                NPostIteration = 3,
                NMgCycle = 1,
                NIterations = 100,
                Length = 1.0,
            })
        {
        }

        public RelayCommand ExecuteFemSolverCommand { get; private set; }

        public OpticalPropertyViewModel OpticalPropertyVM
        {
            get { return _opticalPropertyVM; }
            set
            {
                _opticalPropertyVM = value;
                OnPropertyChanged("OpticalPropertyVM");
            }
        }

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
            //       starting from "-x" to "+x" with increasing "x" coordinate;
            //       starting from "0" to "+z" with increasing "z" coordinate;            
            Measurement measurement = SolverMGRTE.ExecuteMGRTE(_parameters);       

            var meshData = new MapData(measurement.inten, measurement.xloc, measurement.yloc, measurement.dx, measurement.dy);

            Commands.Mesh_PlotMap.Execute(meshData);
        }
    }
}
